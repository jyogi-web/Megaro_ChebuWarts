using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// UGS Lobby / Relay を使ったロビー管理シングルトン
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        public event Action OnGameReady;

        private Unity.Services.Lobbies.Models.Lobby _currentLobby;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _pollCoroutine;

        private const string JoinCodeKey = "joinCode";
        private const float HeartbeatInterval = 15f;
        private const float PollInterval = 2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async Task InitializeServicesAsync()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"[LobbyManager] Signed in as {AuthenticationService.Instance.PlayerId}");
        }

        /// <summary>
        /// VR側 Host としてロビーを作成
        /// </summary>
        public async Task CreateLobbyAsHostAsync()
        {
            await InitializeServicesAsync();

            // Relay確保
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Lobby作成（JoinCodeをデータとして埋め込む）
            var options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new System.Collections.Generic.Dictionary<string, DataObject>
                {
                    { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            };
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync("MegaroChebuWarts", 2, options);
            Debug.Log($"[LobbyManager] Lobby created. ID:{_currentLobby.Id}  JoinCode:{joinCode}");
            // TODO: ロビー作成完了

            // UnityTransportにRelayデータをセット
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            _heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine());
            _pollCoroutine = StartCoroutine(PollForGameStartCoroutine());
        }

        /// <summary>
        /// スマホ/PC側 Client としてロビーに参加
        /// </summary>
        public async Task JoinLobbyAsClientAsync(string lobbyId)
        {
            await InitializeServicesAsync();

            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string joinCode = _currentLobby.Data[JoinCodeKey].Value;
            Debug.Log($"[LobbyManager] Joined lobby. JoinCode:{joinCode}");
            // TODO: ロビー参加完了

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }

        /// <summary>
        /// 2人揃うまでポーリングしてOnGameReadyを発火
        /// </summary>
        private IEnumerator PollForGameStartCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(PollInterval);

                var task = LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
                yield return new WaitUntil(() => task.IsCompleted);

                if (task.IsFaulted)
                {
                    Debug.LogWarning("[LobbyManager] Failed to fetch lobby");
                    // TODO: ロビー情報の取得に失敗しました
                    continue;
                }

                _currentLobby = task.Result;
                if (_currentLobby.Players.Count >= 2)
                {
                    Debug.Log("[LobbyManager] 2 players ready! Firing OnGameReady");
                    // TODO: 2人揃いました！
                    OnGameReady?.Invoke();
                    if (_pollCoroutine != null) StopCoroutine(_pollCoroutine);
                    yield break;
                }
            }
        }

        /// <summary>
        /// Lobbyが自動削除されないよう15秒ごとにHeartbeat送信
        /// </summary>
        private IEnumerator HeartbeatLobbyCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(HeartbeatInterval);
                if (_currentLobby == null) yield break;

                var task = LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                yield return new WaitUntil(() => task.IsCompleted);

                if (task.IsFaulted)
                    Debug.LogWarning("[LobbyManager] Heartbeat failed");
                    // TODO: 送信に失敗しました
            }
        }

        public string GetLobbyId() => _currentLobby?.Id ?? string.Empty;

        private void OnDestroy()
        {
            if (_heartbeatCoroutine != null) StopCoroutine(_heartbeatCoroutine);
            if (_pollCoroutine != null) StopCoroutine(_pollCoroutine);
        }
    }
}

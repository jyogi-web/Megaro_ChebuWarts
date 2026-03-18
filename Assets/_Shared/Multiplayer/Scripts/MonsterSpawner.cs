using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// Client からのリクエストで Host 側がモンスターをスポーンするコンポーネント
    /// </summary>
    public class MonsterSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject[] monsterPrefabs;
        [SerializeField] private float spawnCooldownSeconds = 3f;

        private readonly Dictionary<ulong, float> _clientLastSpawnTime = new Dictionary<ulong, float>();

        /// <summary>
        /// ClientがServerに対してモンスタースポーンを要求するServerRpc（正規化座標で受け取り、サーバー側で位置計算）
        /// </summary>
        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestSpawnMonsterServerRpc(float normX, float normY, float mapRange, int prefabIndex, RpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;

            // サーバー側レート制限
            float now = Time.time;
            if (_clientLastSpawnTime.TryGetValue(senderId, out float lastTime) && now - lastTime < spawnCooldownSeconds)
            {
                Debug.LogWarning($"[MonsterSpawner] Rate limit exceeded for client {senderId}");
                return;
            }
            _clientLastSpawnTime[senderId] = now;

            // 正規化座標の妥当性チェック
            if (normX < 0f || normX > 1f || normY < 0f || normY > 1f)
            {
                Debug.LogWarning($"[MonsterSpawner] Invalid normalized coordinates: ({normX}, {normY})");
                return;
            }

            if (monsterPrefabs == null || prefabIndex < 0 || prefabIndex >= monsterPrefabs.Length)
            {
                Debug.LogWarning($"[MonsterSpawner] Invalid prefabIndex: {prefabIndex}");
                return;
            }

            // サーバー側でVRプレイヤー位置を基準にワールド座標を計算
            var vrTracker = VRPlayerTracker.GetFirst();
            Vector3 vrPos = vrTracker != null ? vrTracker.Position.Value : Vector3.zero;
            Vector3 position = new Vector3(
                vrPos.x + (normX - 0.5f) * mapRange,
                0f,
                vrPos.z + (normY - 0.5f) * mapRange
            );

            var instance = Instantiate(monsterPrefabs[prefabIndex], position, Quaternion.identity);
            var networkObject = instance.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("[MonsterSpawner] NetworkObject not found on prefab");
                Destroy(instance);
                return;
            }

            networkObject.Spawn(destroyWithScene: true);
            Debug.Log($"[MonsterSpawner] Monster spawned: {instance.name} @ {position}");
        }
    }
}

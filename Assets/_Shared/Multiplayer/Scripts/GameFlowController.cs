using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// ゲームのタイマー・勝敗・KillCountを管理するネットワークコンポーネント
    /// </summary>
    public class GameFlowController : NetworkBehaviour
    {
        public static GameFlowController Instance { get; private set; }

        [SerializeField] private float gameDuration = 120f;
        [SerializeField] private int killCountToWin = 10;

        [Header("UI")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI killCountText;

        private NetworkVariable<float> _remainingTime;
        private NetworkVariable<int> _killCount;

        private bool _gameEnded;

        private void Awake()
        {
            Instance = this;

            _remainingTime = new NetworkVariable<float>(
                gameDuration,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

            _killCount = new NetworkVariable<int>(
                0,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );
        }

        public float RemainingTime => _remainingTime.Value;
        public int KillCount => _killCount.Value;

        private void Update()
        {
            // タイマー・KillCount UIはクライアント側でも更新
            if (timerText != null)
                timerText.text = $"{Mathf.CeilToInt(_remainingTime.Value)}";
            if (killCountText != null)
                killCountText.text = $"{_killCount.Value} / {killCountToWin}";

            if (!IsServer || _gameEnded) return;

            _remainingTime.Value -= Time.deltaTime;
            if (_remainingTime.Value <= 0f)
            {
                _remainingTime.Value = 0f;
                EndGame(vrWins: false);
            }
        }

        /// <summary>
        /// NetworkMonsterBaseのOnDeath()から呼ばれる
        /// </summary>
        public void OnMonsterDefeated()
        {
            if (!IsServer || _gameEnded) return;

            _killCount.Value++;
            Debug.Log($"[GameFlowController] KillCount: {_killCount.Value}");

            if (_killCount.Value >= killCountToWin)
                EndGame(vrWins: true);
        }

        private void EndGame(bool vrWins)
        {
            if (_gameEnded) return;
            _gameEnded = true;
            Debug.Log($"[GameFlowController] Game over. VR wins: {vrWins}");
            ShowResultClientRpc(vrWins);
        }

        [ClientRpc]
        private void ShowResultClientRpc(bool vrWins)
        {
            string result = vrWins
                ? "VR Side Wins!\nAll enemies defeated!"
                : "Smartphone Side Wins!\nTime's up!";
            // TODO: vrWins: "VR側の勝利！\n敵を全滅させた！" : "スマホ側の勝利！\n時間切れ！"
            Debug.Log($"[GameFlowController] Result: {result}");

            if (resultPanel != null) resultPanel.SetActive(true);
            if (resultText != null) resultText.text = result;
        }

        public override void OnDestroy()
        {
            if (Instance == this) Instance = null;
            base.OnDestroy();
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// LobbyScene の UI を管理するコントローラー
    /// VRPanel（Host）と DisruptorPanel（Client）を切り替えて表示する
    /// </summary>
    public class LobbyUIController : MonoBehaviour
    {
        [Header("VR側 (Host) パネル")]
        [SerializeField] private GameObject vrPanel;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private TextMeshProUGUI lobbyIdText;

        [Header("スマホ/PC側 (Client) パネル")]
        [SerializeField] private GameObject disruptorPanel;
        [SerializeField] private TMP_InputField lobbyIdInput;
        [SerializeField] private Button joinRoomButton;

        [Header("役割選択パネル")]
        [SerializeField] private GameObject roleSelectPanel;

        [Header("共通")]
        [SerializeField] private TextMeshProUGUI statusText;

        private void Start()
        {
            vrPanel.SetActive(false);
            disruptorPanel.SetActive(false);
            if (roleSelectPanel != null) roleSelectPanel.SetActive(true);
        }

        /// <summary>
        /// VR（Host）モードのUIを表示
        /// </summary>
        public void ShowVRPanel()
        {
            if (roleSelectPanel != null) roleSelectPanel.SetActive(false);
            vrPanel.SetActive(true);
            disruptorPanel.SetActive(false);

            createRoomButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        }

        /// <summary>
        /// スマホ/PC（Client）モードのUIを表示
        /// </summary>
        public void ShowDisruptorPanel()
        {
            if (roleSelectPanel != null) roleSelectPanel.SetActive(false);
            vrPanel.SetActive(false);
            disruptorPanel.SetActive(true);

            joinRoomButton.onClick.RemoveAllListeners();
            joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        }

        private async void OnCreateRoomClicked()
        {
            createRoomButton.interactable = false;
            SetStatus("Creating room...");
            // TODO: ルーム作成中...

            try
            {
                LobbyManager.Instance.OnGameReady += OnGameReady;
                await LobbyManager.Instance.CreateLobbyAsHostAsync();
                string lobbyId = LobbyManager.Instance.GetLobbyId();
                lobbyIdText.text = $"Lobby ID: {lobbyId}";
                SetStatus("Waiting for participants...");
                // TODO: 参加者を待っています...
            }
            catch (Exception e)
            {
                Debug.LogError($"[LobbyUI] Failed to create room: {e.Message}");
                SetStatus("Error: Failed to create room");
                // TODO: エラー: ルーム作成に失敗しました
                createRoomButton.interactable = true;
            }
        }

        private async void OnJoinRoomClicked()
        {
            string lobbyId = lobbyIdInput.text.Trim();
            if (string.IsNullOrEmpty(lobbyId))
            {
                SetStatus("Please enter a Lobby ID");
                // TODO: Lobby IDを入力してください
                return;
            }

            joinRoomButton.interactable = false;
            SetStatus("Joining...");
            // TODO: 参加中...

            try
            {
                await LobbyManager.Instance.JoinLobbyAsClientAsync(lobbyId);
                SetStatus("Connected! Waiting for game start...");
                // TODO: 接続完了！ゲーム開始を待っています...
            }
            catch (Exception e)
            {
                Debug.LogError($"[LobbyUI] Failed to join: {e.Message}");
                SetStatus("Error: Failed to join");
                // TODO: エラー: 参加に失敗しました
                joinRoomButton.interactable = true;
            }
        }

        private void OnGameReady()
        {
            SetStatus("Game starting!");
            // TODO: ゲーム開始！
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// スマホ/PC側の妨害UI。
    /// 画面全体に俯瞰マップを表示し、タップした位置にVR空間の敵をスポーンする。
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class DisruptorUIController : MonoBehaviour, IPointerClickHandler
    {
        [Header("スポーン設定")]
        [SerializeField] private MonsterSpawner monsterSpawner;
        [SerializeField] private int monsterPrefabIndex = 0;
        [SerializeField] private MinimapCameraController minimapCamera;

        [Header("クールダウン UI")]
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private float cooldownSeconds = 3f;

        [Header("ミニマップ カメラ設定")]
        [SerializeField] private Camera minimapCam;
        [SerializeField] private RenderTexture minimapRenderTexture;

        private RawImage _minimapImage;
        private bool _onCooldown;

        private void Awake()
        {
            _minimapImage = GetComponent<RawImage>();

            // RenderTextureをRawImageにセット（常に実行）
            if (minimapRenderTexture != null)
                _minimapImage.texture = minimapRenderTexture;

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;
        }

        private void Start()
        {
            // NetworkManager接続後にHost/Client判定（Awake時はSingletonがnullの場合がある）
            var nm = Unity.Netcode.NetworkManager.Singleton;
            if (nm != null && nm.IsHost)
                gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_onCooldown) return;
            if (monsterSpawner == null)
            {
                Debug.LogWarning("[DisruptorUI] MonsterSpawnerが未設定です");
                return;
            }
            if (!monsterSpawner.IsSpawned)
            {
                Debug.LogWarning("[DisruptorUI] MonsterSpawnerがまだSpawnされていません");
                return;
            }

            // ミニマップ上の正規化座標(0~1)を取得
            RectTransform rectTransform = _minimapImage.rectTransform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
                return;

            Vector2 size = rectTransform.rect.size;
            Vector2 normalized = new Vector2(
                (localPoint.x + size.x * 0.5f) / size.x,
                (localPoint.y + size.y * 0.5f) / size.y
            );
            normalized.x = Mathf.Clamp01(normalized.x);
            normalized.y = Mathf.Clamp01(normalized.y);

            // mapRangeはMinimapCameraのorthographicSize*2と一致させる
            float mapRange = minimapCam != null ? minimapCam.orthographicSize * 2f : 30f;

            monsterSpawner.RequestSpawnMonsterServerRpc(normalized.x, normalized.y, mapRange, monsterPrefabIndex);
            Debug.Log($"[DisruptorUI] スポーンリクエスト送信: normalized={normalized}, mapRange={mapRange}");

            StartCoroutine(CooldownCoroutine());
        }

        private IEnumerator CooldownCoroutine()
        {
            _onCooldown = true;
            float elapsed = 0f;

            while (elapsed < cooldownSeconds)
            {
                elapsed += Time.deltaTime;
                if (cooldownOverlay != null)
                    cooldownOverlay.fillAmount = elapsed / cooldownSeconds;
                yield return null;
            }

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;
            _onCooldown = false;
        }
    }
}

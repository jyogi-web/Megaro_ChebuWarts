using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// スマホ/PC側の妨害UI。ミニマップをタップしてVR空間に敵をスポーンする。
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class DisruptorUIController : MonoBehaviour, IPointerClickHandler
    {
        [Header("スポーン設定")]
        [SerializeField] private MonsterSpawner monsterSpawner;
        [SerializeField] private int monsterPrefabIndex = 0;
        [SerializeField] private float mapRange = 20f;

        [Header("クールダウン UI")]
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private float cooldownSeconds = 3f;

        private RawImage _minimapImage;
        private bool _onCooldown;

        private void Awake()
        {
            _minimapImage = GetComponent<RawImage>();

            // Hostの場合はVR側なのでUIを非表示
            if (Unity.Netcode.NetworkManager.Singleton != null &&
                Unity.Netcode.NetworkManager.Singleton.IsHost)
            {
                gameObject.SetActive(false);
                return;
            }

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;
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
            normalized = Vector2.ClampMagnitude(normalized, 1f);
            normalized.x = Mathf.Clamp01(normalized.x);
            normalized.y = Mathf.Clamp01(normalized.y);

            // VRプレイヤー位置を基準にワールド座標を算出
            Vector3 vrPos = VRPlayerTracker.LocalInstance != null
                ? VRPlayerTracker.LocalInstance.Position.Value
                : Vector3.zero;

            Vector3 spawnPos = new Vector3(
                vrPos.x + (normalized.x - 0.5f) * mapRange,
                0f,
                vrPos.z + (normalized.y - 0.5f) * mapRange
            );

            monsterSpawner.RequestSpawnMonsterServerRpc(spawnPos, monsterPrefabIndex);
            Debug.Log($"[DisruptorUI] スポーンリクエスト送信: {spawnPos}");

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

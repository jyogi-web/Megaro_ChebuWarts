using UnityEngine;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// VRプレイヤーの真上に追従する俯瞰カメラ。
    /// RenderTextureに描画してDisruptorUIのミニマップに表示する。
    /// </summary>
    public class MinimapCameraController : MonoBehaviour
    {
        [SerializeField] private float height = 30f;
        [SerializeField] private float orthographicSize = 15f;

        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
            _cam.orthographicSize = orthographicSize;
            _cam.nearClipPlane = 0.1f;
            _cam.farClipPlane = height + 10f;
            _cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void LateUpdate()
        {
            var tracker = VRPlayerTracker.GetFirst();
            if (tracker == null) return;

            Vector3 vrPos = tracker.Position.Value;
            transform.position = new Vector3(vrPos.x, vrPos.y + height, vrPos.z);
        }
    }
}

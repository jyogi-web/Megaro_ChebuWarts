using Unity.Netcode;
using UnityEngine;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// VRプレイヤーの位置をネットワーク越しに同期するコンポーネント
    /// DisruptorUIControllerからLocalInstanceで静的アクセス可能
    /// </summary>
    public class VRPlayerTracker : NetworkBehaviour
    {
        /// <summary>
        /// 自分がOwnerの場合に設定されるローカルインスタンス
        /// </summary>
        public static VRPlayerTracker LocalInstance { get; private set; }

        public NetworkVariable<Vector3> Position { get; } = new NetworkVariable<Vector3>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                LocalInstance = this;
        }

        private void Update()
        {
            if (!IsOwner) return;
            Position.Value = transform.position;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && LocalInstance == this)
                LocalInstance = null;
        }
    }
}

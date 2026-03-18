using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// VRプレイヤーの位置をネットワーク越しに同期するコンポーネント
    /// DisruptorUIControllerからLocalInstanceまたはGetForClientで静的アクセス可能
    /// </summary>
    public class VRPlayerTracker : NetworkBehaviour
    {
        /// <summary>
        /// 自分がOwnerの場合に設定されるローカルインスタンス
        /// </summary>
        public static VRPlayerTracker LocalInstance { get; private set; }

        private static readonly Dictionary<ulong, VRPlayerTracker> _registry = new Dictionary<ulong, VRPlayerTracker>();

        public readonly NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

        public override void OnNetworkSpawn()
        {
            _registry[NetworkObject.OwnerClientId] = this;
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
            _registry.Remove(NetworkObject.OwnerClientId);
            if (IsOwner && LocalInstance == this)
                LocalInstance = null;
        }

        /// <summary>
        /// クライアントIDからVRPlayerTrackerを取得する（全クライアントから利用可能）
        /// </summary>
        public static VRPlayerTracker GetForClient(ulong ownerId)
        {
            _registry.TryGetValue(ownerId, out var tracker);
            return tracker;
        }

        /// <summary>
        /// レジストリ内の最初のVRPlayerTrackerを返す（単一VRプレイヤー想定）
        /// </summary>
        public static VRPlayerTracker GetFirst()
        {
            foreach (var tracker in _registry.Values)
                return tracker;
            return null;
        }
    }
}

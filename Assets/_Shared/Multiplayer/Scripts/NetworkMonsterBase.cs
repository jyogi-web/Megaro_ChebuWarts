using MegaroChebuWarts.Magic;
using MegaroChebuWarts.Monster;
using Unity.Netcode;
using UnityEngine;

namespace MegaroChebuWarts.Multiplayer
{
    /// <summary>
    /// ネットワーク対応モンスター基底クラス
    /// MonsterBaseを継承しつつ、同一GameObjectのMonsterNetworkRpcHelperにServerRpc処理を委譲
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(MonsterNetworkRpcHelper))]
    public class NetworkMonsterBase : MonsterBase
    {
        private MonsterNetworkRpcHelper _rpcHelper;
        private NetworkObject _networkObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            _rpcHelper = GetComponent<MonsterNetworkRpcHelper>();
            _networkObject = GetComponent<NetworkObject>();
        }

        public override void TakeDamage(MagicElement element, float damage)
        {
            // Serverでなければ ServerRpc 経由でダメージを転送
            if (_networkObject != null && _networkObject.IsSpawned && !NetworkManager.Singleton.IsServer)
            {
                _rpcHelper.TakeDamageServerRpc(element, damage);
                return;
            }

            base.TakeDamage(element, damage);
        }

        protected override void OnDeath()
        {
            Debug.Log($"[NetworkMonsterBase] {gameObject.name} defeated!");
            // TODO:__ が倒された！

            GameFlowController.Instance?.OnMonsterDefeated();

            if (_networkObject != null && _networkObject.IsSpawned)
            {
                gameObject.SetActive(false);
                _networkObject.Despawn();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// NetworkBehaviourが必要なRPC処理を担当するヘルパーコンポーネント
    /// NetworkMonsterBaseと同一GameObjectに配置する
    /// </summary>
    public class MonsterNetworkRpcHelper : NetworkBehaviour
    {
        private NetworkMonsterBase _monsterBase;

        private void Awake()
        {
            _monsterBase = GetComponent<NetworkMonsterBase>();
        }

        /// <summary>
        /// Client からダメージ情報を Server に転送する ServerRpc
        /// </summary>
        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void TakeDamageServerRpc(MagicElement element, float damage)
        {
            _monsterBase.TakeDamage(element, damage);
        }
    }
}

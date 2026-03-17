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

        /// <summary>
        /// ClientがServerに対してモンスタースポーンを要求するServerRpc
        /// RequireOwnership=false のため全クライアントから呼び出し可能
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnMonsterServerRpc(Vector3 position, int prefabIndex)
        {
            if (monsterPrefabs == null || prefabIndex < 0 || prefabIndex >= monsterPrefabs.Length)
            {
                Debug.LogWarning($"[MonsterSpawner] Invalid prefabIndex: {prefabIndex}");
                // TODO: 無効なprefabIndex
                return;
            }

            var instance = Instantiate(monsterPrefabs[prefabIndex], position, Quaternion.identity);
            var networkObject = instance.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("[MonsterSpawner] NetworkObject not found on prefab");
                // TODO: prefabにNetworkObjectが見つかりません
                Destroy(instance);
                return;
            }

            networkObject.Spawn(destroyWithScene: true);
            Debug.Log($"[MonsterSpawner] Monster spawned: {instance.name} @ {position}");
            // TODO: モンスタースポーン
        }
    }
}

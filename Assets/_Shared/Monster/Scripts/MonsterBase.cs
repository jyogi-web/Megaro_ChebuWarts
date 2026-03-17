using UnityEngine;
using MegaroChebuWarts.Magic;

namespace MegaroChebuWarts.Monster
{
    /// <summary>
    /// モンスターの基本クラス
    /// HPの管理とダメージの処理を行う
    /// </summary>
    public class MonsterBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float maxHp = 100f;

        protected float currentHp;

        protected virtual void Start()
        {
            currentHp = maxHp;
        }

        public virtual void TakeDamage(MagicElement element, float damage)
        {
            currentHp -= damage;
            Debug.Log($"{gameObject.name} が {element} 属性の魔法で {damage} ダメージを受けた！ 残りHP: {currentHp}");

            if (currentHp <= 0f)
                OnDeath();
        }

        protected virtual void OnDeath()
        {
            Debug.Log($"{gameObject.name} が倒された！");
            gameObject.SetActive(false);
        }
    }
}

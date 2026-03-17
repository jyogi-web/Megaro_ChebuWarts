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

        protected virtual void OnEnable()
        {
            currentHp = Mathf.Max(0f, maxHp);
        }

        public virtual void TakeDamage(MagicElement element, float damage)
        {
            if (damage <= 0f) return;
            currentHp -= damage;
            currentHp = Mathf.Max(0f, currentHp);
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

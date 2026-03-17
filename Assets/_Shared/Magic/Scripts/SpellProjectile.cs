using System.Collections;
using UnityEngine;

namespace MegaroChebuWarts.Magic
{
    // コライダーを必須にする
    [RequireComponent(typeof(Collider))]
    
    /// <summary>
    /// 魔法の弾丸を表すクラス
    /// 魔法の属性とダメージ量を持ち、衝突判定を行う
    /// </summary> 
    public class SpellProjectile : MonoBehaviour
    {
        [SerializeField] private MagicElement element;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float hitCheckRadius = 0.5f;
        [SerializeField] private bool autoSizeFromParticle = true;

        public MagicElement Element => element;
        public float Damage => damage;

        private bool _hasHit;
        private Renderer[] _renderers;

        public void Initialize(MagicElement element, float damage)
        {
            this.element = element;
            this.damage = damage;
            _hasHit = false;
        }

        private void OnEnable()
        {
            _hasHit = false;
            if (autoSizeFromParticle)
                StartCoroutine(AutoSizeRadius());
        }

        private IEnumerator AutoSizeRadius()
        {
            // パーティクルが広がるまで少し待つ
            yield return new WaitForSeconds(0.15f);

            if (_renderers == null)
                _renderers = GetComponentsInChildren<Renderer>();

            var bounds = new Bounds(transform.position, Vector3.zero);
            foreach (var r in _renderers)
            {
                if (r.enabled && r.gameObject.activeInHierarchy)
                    bounds.Encapsulate(r.bounds);
            }

            float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            if (maxExtent > hitCheckRadius)
                hitCheckRadius = maxExtent;
        }

        private void FixedUpdate()
        {
            if (_hasHit) return;
            CheckHit();
        }

        private void CheckHit()
        {
            var colliders = Physics.OverlapSphere(transform.position, hitCheckRadius);
            foreach (var col in colliders)
            {
                if (col.gameObject == gameObject) continue;
                var damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(element, damage);
                    _hasHit = true;
                    gameObject.SetActive(false);
                    return;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasHit) return;
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(element, damage);
                _hasHit = true;
                gameObject.SetActive(false);
            }
        }
    }
}

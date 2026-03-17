using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaroChebuWarts.Magic
{
    /// <summary>
    /// 魔法のエフェクトを管理するマネージャークラス
    /// SpellEffectCatalogからデータを取得し、エフェクトの生成、再生、回収を行う
    /// 同時に発生できるエフェクトの上限を設け、古いものから順に回収する仕組みも実装
    /// </summary>
    public class SpellEffectManager : MonoBehaviour
    {
        [SerializeField] private SpellEffectCatalog catalog;

        private const int MaxConcurrentEffects = 5;

        private readonly Dictionary<MagicElement, Queue<GameObject>> _pool
            = new Dictionary<MagicElement, Queue<GameObject>>();

        private readonly List<(GameObject obj, MagicElement element, float expireAt)> _active
            = new List<(GameObject, MagicElement, float)>();

        private readonly Dictionary<float, WaitForSeconds> _waitCache
            = new Dictionary<float, WaitForSeconds>();

        private AudioSource _audio;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();

            foreach (MagicElement element in Enum.GetValues(typeof(MagicElement)))
            {
                var data = catalog.GetData(element);
                if (data == null || data.EffectPrefab == null) continue;

                var queue = new Queue<GameObject>();
                for (int i = 0; i < data.InitialPoolSize; i++)
                    queue.Enqueue(CreateInstance(data.EffectPrefab));
                _pool[element] = queue;
            }
        }

        public void Fire(SpellRequest request)
        {
            var data = catalog.GetData(request.Element);
            if (data == null || data.EffectPrefab == null)
            {
                Debug.LogWarning($"[SpellEffectManager] No data for element: {request.Element}");
                return;
            }

            // 同時発火上限を超えたら最古を強制回収
            if (_active.Count >= MaxConcurrentEffects)
                ForceRecycleOldest();

            var obj = GetFromPool(request.Element, data.EffectPrefab);
            obj.transform.SetPositionAndRotation(request.Position, request.Rotation);
            obj.SetActive(true);

            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            // ダメージの初期化
            var projectile = obj.GetComponent<SpellProjectile>();
            if (projectile != null)
                projectile.Initialize(request.Element, data.Damage);

            if (data.CastSound != null && _audio != null)
                _audio.PlayOneShot(data.CastSound);

            float expireAt = Time.time + data.Duration;
            _active.Add((obj, request.Element, expireAt));

            StartCoroutine(ReturnToPool(obj, request.Element, data.Duration));
        }

        private IEnumerator ReturnToPool(GameObject obj, MagicElement element, float duration)
        {
            if (!_waitCache.TryGetValue(duration, out var wait))
            {
                wait = new WaitForSeconds(duration);
                _waitCache[duration] = wait;
            }

            yield return wait;

            Recycle(obj, element);
        }

        private void Recycle(GameObject obj, MagicElement element)
        {
            if (obj == null) return;

            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null) ps.Stop();

            obj.SetActive(false);

            if (!_pool.TryGetValue(element, out var queue))
            {
                queue = new Queue<GameObject>();
                _pool[element] = queue;
            }
            queue.Enqueue(obj);

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i].obj == obj)
                {
                    _active.RemoveAt(i);
                    break;
                }
            }
        }

        private void ForceRecycleOldest()
        {
            if (_active.Count == 0) return;

            // expireAt が最も小さいものを回収
            int oldestIndex = 0;
            for (int i = 1; i < _active.Count; i++)
            {
                if (_active[i].expireAt < _active[oldestIndex].expireAt)
                    oldestIndex = i;
            }

            var (obj, element, _) = _active[oldestIndex];
            _active.RemoveAt(oldestIndex);
            Recycle(obj, element);
        }

        private GameObject GetFromPool(MagicElement element, GameObject prefab)
        {
            if (_pool.TryGetValue(element, out var queue) && queue.Count > 0)
                return queue.Dequeue();

            return CreateInstance(prefab);
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            return obj;
        }
    }
}

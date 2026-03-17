using UnityEngine;

namespace MegaroChebuWarts.Magic
{
    [CreateAssetMenu(fileName = "SpellEffect_New", menuName = "Magic/Spell Effect Data")]
    public class SpellEffectData : ScriptableObject
    {
        [Header("属性設定")]
        public MagicElement Element;

        [Header("エフェクト")]
        public GameObject EffectPrefab;

        [Tooltip("自動消滅までの秒数")]
        public float Duration = 2f;

        [Header("プール設定")]
        [Tooltip("事前生成するインスタンス数")]
        public int InitialPoolSize = 3;

        [Header("サウンド")]
        [Tooltip("発動時の効果音（任意）")]
        public AudioClip CastSound;
    }
}

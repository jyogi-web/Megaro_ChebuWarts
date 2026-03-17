using UnityEngine;

namespace MegaroChebuWarts.Magic
{
    /// <summary>
    /// 魔法のエフェクトデータをまとめて管理するカタログ的なScriptableObject
    /// 各属性に対応するSpellEffectDataを配列で保持し、属性からデータを取得できるようにする
    /// </summary>
    [CreateAssetMenu(fileName = "SpellEffectCatalog", menuName = "Magic/Spell Effect Catalog")]
    public class SpellEffectCatalog : ScriptableObject
    {
        [SerializeField] private SpellEffectData[] effects;

        public SpellEffectData GetData(MagicElement element)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i].Element == element)
                    return effects[i];
            }

            Debug.LogWarning($"SpellEffectData not found for element: {element}");
            return null;
        }
    }
}

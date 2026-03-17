using UnityEngine;

namespace MegaroChebuWarts.Magic
{
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

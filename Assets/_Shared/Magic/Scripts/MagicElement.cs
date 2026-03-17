using UnityEngine;

namespace MegaroChebuWarts.Magic
{
    /// <summary>
    /// 魔法の属性を定義するenum型
    /// 五大属性 [地、水、火、風、空] を表す
    /// </summary>
    public enum MagicElement
    {
        Earth = 0,
        Water = 1,
        Fire = 2,
        Wind = 3,
        Sky = 4,
    }

    /// <summary>
    /// 魔法の発動リクエストを表す構造体
    /// 属性、位置、回転、威力などの情報をまとめて保持する
    /// </summary>
    public struct SpellRequest
    {
        public MagicElement Element;
        public Vector3 Position;
        public Quaternion Rotation;
        public float Power;
    }
}

namespace MegaroChebuWarts.Magic
{
    /// <summary>
    /// ダメージを受けることができるオブジェクトのインターフェース
    /// </summary> 
    public interface IDamageable
    {
        void TakeDamage(MagicElement element, float damage);
    }
}

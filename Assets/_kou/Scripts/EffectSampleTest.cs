using UnityEngine;
using UnityEngine.InputSystem;
using MegaroChebuWarts.Magic;

/// <summary>
/// EffectSample.unity シーンでの動作確認用テストスクリプト。
/// キー 1〜5 で各属性のエフェクトを発火する。
/// </summary>
public class EffectSampleTest : MonoBehaviour
{
    [SerializeField] private SpellEffectManager manager;
    [SerializeField] private Transform firePoint;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) Fire(MagicElement.Earth);
        if (keyboard.digit2Key.wasPressedThisFrame) Fire(MagicElement.Water);
        if (keyboard.digit3Key.wasPressedThisFrame) Fire(MagicElement.Fire);
        if (keyboard.digit4Key.wasPressedThisFrame) Fire(MagicElement.Wind);
        if (keyboard.digit5Key.wasPressedThisFrame) Fire(MagicElement.Sky);
    }

    private void Fire(MagicElement element)
    {
        manager.Fire(new SpellRequest
        {
            Element = element,
            Position = firePoint != null ? firePoint.position : Vector3.zero,
            Rotation = Quaternion.identity,
        });
    }
}

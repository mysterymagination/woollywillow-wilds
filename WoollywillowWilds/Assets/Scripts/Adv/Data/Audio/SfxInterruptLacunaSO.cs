using UnityEngine;
using System.Collections;

namespace WildsAdv
{
    [CreateAssetMenu(fileName = "LacunaInterrupt.asset", menuName = "SoundAndEffects/LacunaInterruptSO")]
    public class SfxInterruptLacunaSO : SfxInterruptSO
    {
        [Header("Audio Options")]
        [field: SerializeField] public float Duration { get; set; } = 0.0F;
        override public IEnumerator Interrupt(IInterruptableSfx _interruptableSfx)
        {
            yield return new WaitForSecondsRealtime(Duration);
        }
    }
}

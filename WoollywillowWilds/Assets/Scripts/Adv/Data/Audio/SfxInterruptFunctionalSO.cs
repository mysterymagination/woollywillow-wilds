using UnityEngine;
using System.Collections;

namespace WildsAdv
{
    /// <summary>
    /// Interrupts an sfx stream Component with another ITypeWriterSfx Component playing for the given duration.
    /// todo: how should we handle the case that the interrupting Component would not play long enough to satisfy the duration?
    ///  We could loop him, but I don't think I installed the power to do that. Further, I don't think we have any way to
    ///  know if the interruption ITypeWriterSfx has gone dark. We'd probably need like a lifecycle callback mech
    ///  so we could call play again if the duration isn't up but we hit Teardown(). 
    /// </summary>
    [CreateAssetMenu(fileName = "FunctionalInterrupt.asset", menuName = "SoundAndEffects/FunctionalInterruptSO")]
    public class SfxInterruptFunctionalSO<T> : SfxInterruptSO where T : Component, ITypeWriterSfx
    {
        /// <summary>
        /// The duration of the interruption.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField]
        public float Duration { get; set; } = 0.0F;
        /// <summary>
        /// Configuration data asset to be used in setting up the <see cref="ITypeWriterSfx"/> Component.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField]
        public ScriptableObject SfxData { get; set; }
        override public IEnumerator Interrupt(IInterruptableSfx interruptableSfx)
        {
            yield return interruptableSfx.OnFunctionalInterrupt<T>(SfxData, Duration);
        }
    }
}

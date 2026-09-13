using UnityEngine;
using System.Collections;
using MoodMap = System.Collections.Generic.Dictionary<WildsAdv.Mood, System.Collections.Generic.List<UnityEngine.AudioClip>>;

namespace WildsAdv
{
    /// <summary>
    /// Interrupts an sfx stream Component with another ITypeWriterSfx Component playing for the given duration.
    /// todo: how should we handle the case that the interrupting Component would not play long enough to satisfy the duration?
    ///  We could loop him, but I don't think I installed the power to do that. Further, I don't think we have any way to
    ///  know if the interruption ITypeWriterSfx.
    /// todo: can the Inspector handle a generic param?
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
        override public IEnumerator Interrupt(IInterruptableSfx interruptableSfx)
        {
            yield return interruptableSfx.OnFunctionalInterrupt<T>(Duration);
        }
    }
}

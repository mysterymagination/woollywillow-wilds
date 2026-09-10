using UnityEngine;
using System.Collections;

namespace WildsAdv
{
    [CreateAssetMenu(fileName = "SfxInterrupt.asset", menuName = "SoundAndEffects/SfxInterruptSO")]
    public class SfxInterruptSO : ScriptableObject
    {
        /// <summary>
        /// Initial delay offset into the main stream play time before
        /// the interrupt occurs.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField] public float Delay { get; set; } = 0.0F;
        /// <summary>
        /// Variance allowed in the initial delay offset into the main stream
        /// play time before the interrupt occurs. If 0, the delay will be equal to delay above.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField] public float DelayVariance { get; set; } = 0.0F;
        /// <summary>
        /// Interrupts the SFX currently being played by the input player for the input sentence.
        /// The manner of this interruption depends on the particular <see cref="IInterruptableSfx"/> implementation. 
        /// </summary>
        /// <param name="interruptableSfx">The <see cref="IInterruptableSfx"/> playing the main SFX stream which we wish to interrupt. It implements query functions that inform our interrupt details.</param>
        /// <returns>An <see cref="IEnumerator"/> handle for Coroutine resume after suspend.</returns>
        virtual public IEnumerator Interrupt(IInterruptableSfx _interruptableSfx)
        {
            yield return null;
        }
    }
}

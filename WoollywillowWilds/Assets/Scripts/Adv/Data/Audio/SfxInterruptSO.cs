using UnityEngine;
using System.Collections;

namespace WildsAdv
{
    [CreateAssetMenu(fileName = "SfxInterrupt.asset", menuName = "SoundAndEffects/SfxInterruptSO")]
    public class SfxInterruptSO : ScriptableObject
    {
        [Header("Audio Options")]
        [SerializeField] public float delay = 0.0F;
        [SerializeField] public float variance = 0.0F;
        /// <summary>
        /// Interrupts the SFX currently being played by the input player for the input sentence.
        /// The manner of this interruption depends on the particular <see cref="SfxInterrupt"/> subclass. 
        /// </summary>
        /// <param name="interruptableSfx">The <see cref="IInterruptableSfx"/> playing the main SFX stream which we wish to interrupt. It implements query functions that inform our interrupt details.</param>
        /// <returns>An <see cref="IEnumerator"/> handle for Coroutine resume after suspend.</returns>
        virtual public IEnumerator Interrupt(IInterruptableSfx _interruptableSfx)
        {
            yield return null;
        }
    }
}

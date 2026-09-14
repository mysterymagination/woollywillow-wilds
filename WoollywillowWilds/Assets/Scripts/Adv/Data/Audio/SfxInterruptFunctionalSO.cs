using UnityEngine;
using System.Collections;
using MoodMap = System.Collections.Generic.Dictionary<WildsAdv.Mood, System.Collections.Generic.List<UnityEngine.AudioClip>>;

namespace WildsAdv
{
    /// <summary>
    /// Interrupts an sfx stream Component with another ITypeWriterSfx Component playing for the given duration.
    /// todo: how should we handle the case that the interrupting Component would not play long enough to satisfy the duration?
    ///  We could loop him, but I don't think I installed the power to do that. Further, I don't think we have any way to
    ///  know if the interruption ITypeWriterSfx has gone dark. We'd probably need like a lifecycle callback mech
    ///  so we could call play again if the duration isn't up but we hit Teardown(). 
    /// todo: can the Inspector handle a generic param?
    ///  EDIT: nope. You can sort of work around this by making a subclass that provides a concrete class for T.
    /// todo: we need a way to feed in a preconfigured ITypeWriterSfx Component; otherwise,
    ///  the TypeWriterSfx_PrefabClips we create for a SfxInterruptFunctionalSO_Prefab dynamically won't be able to do anything.
    ///  In the case that we pass in a preconfigured guy, the default setup code run by OnFunctionInterrupt() needs to be skipped or run differently;
    ///  at least the AddComponent() needs to be conditional. Another trick is that the ITypeWriterSfx men are not
    ///  scriptableobjects presently, SO are for assets where as ITypeWriterSfxeses are Components. So we'll need an asset to provide data
    ///  to ITypeWriterSfx Components, primarily TypeWriterSfx_PrefabClips.  
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
        /// The duration of the interruption.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField]
        public System.Type InterruptType { get; set; }
        override public IEnumerator Interrupt(IInterruptableSfx interruptableSfx)
        {
            yield return interruptableSfx.OnFunctionalInterrupt<T>(Duration);
        }
    }
}

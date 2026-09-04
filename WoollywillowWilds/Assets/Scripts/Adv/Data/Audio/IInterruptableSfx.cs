using UnityEngine;
using System.Collections;
using MoodMap = System.Collections.Generic.Dictionary<WildsAdv.Mood, System.Collections.Generic.List<UnityEngine.AudioClip>>;

namespace WildsAdv
{
    /// <summary>
    /// An object that manages playing a sound effect and can provide information
    /// to an <see cref="SfxInterrupt"/> about current state and loaded options for e.g.
    /// mood:track associations. This allows the interruptable sfx manager to simply call
    /// <see cref="SfxInterrupt.Interrupt(this)"/> without needing to know anything about the
    /// details of how the interrupt works.
    /// </summary>
    public interface IInterruptableSfx
    {
        /// <summary>
        /// Tells the interruptable sfx that it should run the input <see cref="ITypeWriterSfx"/>
        /// for the duration of the interrupt. This can be used to e.g. interrupt a stream of prefabricated
        /// chirp trilling with algorithmically clipped chirp trilling to ensure we maximize quality while
        /// avoiding repetitive patterns.
        /// </summary>
        /// <param name="gameObject">
        /// The host <see cref="GameObject"/> to which the interrupting sfx <see cref="Component"/> will be added to by default. 
        /// </param>
        /// <param name="sfxInterruptClass">
        /// The <see cref="System.Type"/> which by default will be a <see cref="Component"/> that implements <see cref="ITypeWriterSfx"/> that we wish to run as an interrupt sfx behavior.
        /// This Component will be added to the host <see cref="GameObject"/> by default, run through the ITypeWriterSfx lifetime, and will then be destroyed. 
        /// </param>
        /// <param name="duration">
        /// The duration of the interrupt in seconds.
        /// </param>
        public IEnumerator OnFunctionalInterrupt(GameObject gameObject, System.Type sfxInterruptClass, float duration)
        {
            Component sfxInterruptComponent = gameObject.AddComponent(sfxInterruptClass);
            ITypeWriterSfx sfxInterrupt = (ITypeWriterSfx)sfxInterruptComponent;

            // todo: what happens if the 'parent' sfx coroutine we're currently running from, presumably AsyncSfx_MainStream(),
            //  gets stopped before we have the chance to stop this sfxInterrupt with 'child' sfx coroutine(s)? Is there a callback Coroutines get when stopped?
            //  EDIT: looks like nothing built in; you can sort of hack it yourself, but that would involve storing the IEnumerator handle we get
            //   here somewhere higher up? Perhaps maintain a list of SFX stuff to kill when a sentence ends?
            if (sfxInterrupt != null)
            {
                sfxInterrupt.Setup();
                sfxInterrupt.Play();
                yield return new WaitForSeconds(duration);
                sfxInterrupt.Stop();
                sfxInterrupt.Teardown();
            }
            Object.Destroy(sfxInterruptComponent);
        }
        /// <summary>
        /// Asks the interruptable sfx manager for its current player.
        /// </summary>
        /// <returns>The <see cref="AudioSource"/> playing the sfx we want to interrupt.</returns>
        public AudioSource QueryPlayer();
        /// <summary>
        /// Asks the interruptable sfx manager for the current mood we're trying to evoke.
        /// </summary>
        /// <returns>The <see cref="Mood"/> our interruption should keep to if possible.</returns>
        public Mood QueryMood();
        /// <summary>
        /// Asks the interruptable sfx manager for the mapping of moods to <see cref="List<AudioClip>"/> for the purposes of switching through potentially multiple premapped mood:track assocations during the interruption.
        /// </summary>
        /// <returns>The <see cref="Mood"/> our interruption should keep to if possible.</returns>
        public MoodMap QueryMoodMap();
    }
}
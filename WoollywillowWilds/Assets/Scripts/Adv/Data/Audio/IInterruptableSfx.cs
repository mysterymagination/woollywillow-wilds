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
        /// Tells the interruptable sfx manager that it should run the code for the input <see cref="SfxMode"/>
        /// for the duration of the interrupt. This can be used to e.g. interrupt a stream of prefabricated
        /// chirp trilling with algorithmically clipped chirp trilling to ensure we maximize quality while
        /// avoiding repetitive patterns.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="SfxMode" we wish to emulate during our interrupt.>.
        /// </param>
        /// <param name="duration">
        /// The duration of the interrupt in seconds.
        /// </param>
        public IEnumerator OnFunctionalInterrupt(SfxMode mode, float duration);
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
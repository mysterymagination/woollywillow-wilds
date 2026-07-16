using System.Collections;

namespace WildsAdv
{
    /// <summary>
    /// Provides data to <see cref="SfxInterrupt"/>s about how SFX interruption should be handled specifically for the implementing object. 
    /// </summary>
    public interface IInterruptableSfx
    {
        /// <summary>
        /// Delegates calling the appropriate async SFX function for the given <see cref="SfxMode"/> to the implementing object.
        /// </summary>
        /// <param name="mode">
        /// An <see cref="SfxMode"/> that determines what async SFX function will run. For example, <see cref="SfxMode.ChirpSentenceAlgoClipped"/> calls through to <see cref="TypeWriter.AsyncSfx_ChirpSentenceAlgoClipped()"/>  
        /// </param>
        IEnumerator OnFunctionalInterrupt(SfxMode mode);
        /// <summary>
        /// Ask the interruptable audio object what <see cref="AudioSource"/> should be interrupted. 
        /// </summary>
        /// <returns>The <see cref="AudioSource"/> that should be interrupted.</returns>
        AudioSource QueryPlayer();
        /// <summary>
        /// Ask the interruptable audio object what mood we're trying to evoke; this can inform the particulars of the interrupt.
        /// </summary>
        /// <returns>The current <see cref="Mood"/>.</returns>
        Mood QueryMood();
        /// <summary>
        /// Ask the interruptable audio object for a mapping of moods to a list of <see cref="AudioClip"/>s appropriate for that mood. 
        /// </summary>
        /// <returns>The current <see cref="Dictionary<Mood, List<AudioClip>>"/> mapping.</returns>
        Dictionary<Mood, List<AudioClip>> QueryMoodMap();
    }
}
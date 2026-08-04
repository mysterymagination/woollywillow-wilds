
namespace WildsAdv
{
    /// <summary>
    /// Abstracts away the specifics of handling a given SFX while the typewriter types to a Component
    /// that implements this interface.
    /// </summary>
    public interface ITypeWriterSfx
    {
        /// <summary>
        /// Choose/Load AudioClip and any other preparation for the SFX.
        /// </summary>
        public void setup();
        /// <summary>
        /// Play the SFX.
        /// </summary>
        public void play();
        /// <summary>
        /// Pause the SFX, stopping playback but keeping the playhead position.
        /// </summary>
        public void pause();
        /// <summary>
        /// Stop the SFX, resetting he playhead.
        /// </summary>
        public void stop();
    }
}
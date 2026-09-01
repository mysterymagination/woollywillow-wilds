using UnityEngine;

namespace WildsAdv
{
    /// <summary>
    /// This mode plays sfx async per write event, with each write event firing off its own Coroutine
    /// with its own AudioSource player so we never cut off existing sounds with new ones; instead, they interleave, mimicking 
    /// the sound of a typewriter hardware key press and hammer stamp on paper. The AudioSource can be destroyed when the
    /// Coroutine exits.
    /// Each sfx clip plays on its own AudioSource spawned at runtime in a Coroutine and despawned when the Coroutine functor e.g. AsyncSfx() exits.
    /// The separate AudioSources is the main point of interest here --  the Coroutine is important mainly because it
    /// gives us a way to both track the useful lifetime of the created AudioSource and suspend execution until the sfx
    /// finished playing so that we don't destroy it prematurely (since AudioSource does not block the main thread while playing
    /// media). If we had nice event callbacks like OnResourceComplete() or something we wouldn't need to build our own
    /// mechanism...
    /// In theory this mode might make the most technically accurate bond between the rendering of the text and
    /// the accompanying sounds, but in practice it's difficult to make this sound 'good' for values of good that
    /// include sounding like indistinct speech.
    /// </summary>
    public class TypeWriterSfx_KeyHammer : MonoBehaviour, ITypeWriterSfx
    {
        /// <summary>
        /// Short sound effects played 1:1 with write events. By default, this will
        /// begin at the 0 index clip and proceed through until the end at which point it will
        /// wrap around. Each clip will play to completion without looping, simulating a typewriter
        /// key-hammer stroke or a single voice tone syllable.
        /// </summary>
        public AudioClip[] typingSfxBlipArray;
        /// <summary>
        /// Tracks the current index into the typingSfxBlipArray.
        /// </summary>
        private int sfxBlipIndex = 0;
        private AudioSource player;
        [Range(0.0F, 1.0F)]
        public float Volume { get; set; } = 0.5F;
        [field: SerializeField]
        public int CharactersWritten { get; set; } = 0.0F;
        [field: SerializeField]
        public float TypingCadence { get; set; } = 0.0F;
        public void Setup()
        {
            Debug.LogError("Unexpected call to Setup of keyhammer sfx; he's designed to set everything up afresh in each Coroutine then fire n forget em.");
        }

        public void Play()
        {
            sfxFunction = AsyncSfx_KeyHammer(CharactersWritten, TypingCadence);
            StartCoroutine(sfxFunction);
        }
        public void Pause()
        {
            Debug.LogError("Unexpected call to Pause of keyhammer sfx; he's designed to rock on until Teardown()");
        }
        public void Teardown()
        {
            sfxBlipIndex = 0;
            Destroy(player);
            StopCoroutine(sfxFunction);
        }
        IEnumerator AsyncSfx_KeyHammer(int charactersWritten, float typingCadence)
        {
            if (sfxBlipIndex >= typingSfxBlipArray.Length)
            {
                sfxBlipIndex = 0;
            }
            AudioClip typingSfx = typingSfxBlipArray[sfxBlipIndex];
            sfxBlipIndex++;
            if (typingSfx)
            {
                /*
                float sfxDurationMs = typingCadence - charactersWritten * keyHammerStrikeTimeMilliseconds;
                sfxDurationMs = Math.Clamp(sfxDurationMs, keyHammerStrikeTimeMilliseconds, keyHammerStrikeTimeMilliseconds + typingCadence);
                yield return new WaitForSeconds(sfxDurationMs / 1000.0F);
                */
                /*
                yield return new WaitForSeconds(typingSfx.clip.length);
                */

                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.resource = typingSfx;
                source.Play();
                yield return new WaitForSeconds(typingSfx.length);

                // remove the host AudioSource Component at the bottom of the Coroutine functor.
                Destroy(source);
            }
        }
    }
}

namespace WildsAdv
{
    public class TypeWriterSfx_KeyHammer : MonoBehaviour, ITypeWriterSfx, IInterruptableSfx
    {
        public AudioClip TypingSfx { get; set; }
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
        IEnumerator AsyncSfx_KeyHammer(int charactersWritten, float typingCadence)
        {
            if (sfxBlipIndex >= typingSfxBlipArray.Length)
            {
                sfxBlipIndex = 0;
            }
            AudioClip typingSfx = typingSfxBlipArray[sfxBlipIndex];
            sfxBlipIndex++;
            if (TypingSfx)
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
                source.resource = TypingSfx;
                source.Play();
                yield return new WaitForSeconds(TypingSfx.length);

                // remove the host AudioSource Component at the bottom of the Coroutine functor.
                Destroy(source);
            }
        }
    }
}
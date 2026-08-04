
namespace WildsAdv
{
    public class TypeWriterSfx_Blips : MonoBehaviour, ITypeWriterSfx, IInterruptableSfx
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
        void Play()
        {
            if (sfxBlipIndex >= typingSfxBlipArray.Length)
            {
                sfxBlipIndex = 0;
            }
            AudioClip typingSfx = typingSfxBlipArray[sfxBlipIndex];
            sfxBlipIndex++;
            if (typingSfx)
            {
                if (sfxBlipIndex >= typingSfxBlipArray.Length)
                {
                    sfxBlipIndex = 0;
                }
                singularSfx.resource = typingSfxBlipArray[sfxBlipIndex];
                singularSfx.Play();
                sfxBlipIndex++;
            }
        }
    }
}
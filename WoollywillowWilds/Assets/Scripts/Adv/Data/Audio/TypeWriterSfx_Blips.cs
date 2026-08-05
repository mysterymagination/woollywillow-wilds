
namespace WildsAdv
{
    /// <summary>
    /// A mix of keyhammer and voice tone, we use a singular AudioSource to play through a large array of short AudioClips
    /// while a sentence types itself out. Each new write event cuts off the previous sfx and replaces it with a new one.
    /// </summary>
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
        /// Whether or not we should set the sfx clip index to a random value around the current one within sfxClipIndexRange after a full stop breath.
        /// </summary>
        public bool randomSfxClipIndex = false;
        /// <summary>
        /// Tracks the current index into the typingSfxBlipArray.
        /// </summary>
        private int sfxBlipIndex = 0;
        private AudioSource player;
        public void Setup()
        {
            player = gameObject.AddComponent<AudioSource>();
            // update field member for IInterruptableSfx queries.
            currentSfxPlayer = singularSfx;
            player.loop = true;
            player.volume = sfxVolume;
        }
        public void Play()
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
        public void Pause()
        {
            player.Pause();
            if (randomSfxClipIndex)
            {
                int cachedBlipIndex = sfxBlipIndex;
                System.Random blipRnd = new System.Random();
                int indexModifier = blipRnd.Next(-sfxClipIndexRange, sfxClipIndexRange);
                sfxBlipIndex += indexModifier;
                sfxBlipIndex = Math.Clamp(sfxBlipIndex, 0, typingSfxBlipArray.Length - 1);
                Debug.Log("Randomizing blip index from " + cachedBlipIndex + " to " + sfxBlipIndex + " based on index mod " + indexModifier);
            }
        }
    }
}
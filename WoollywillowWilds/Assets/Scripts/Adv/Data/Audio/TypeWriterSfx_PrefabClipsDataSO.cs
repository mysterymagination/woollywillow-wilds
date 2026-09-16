using UnityEngine;

namespace WildsAdv
{
    [CreateAssetMenu(fileName = "TypeWriterSfx_PrefabClipsData.asset", menuName = "SoundAndEffects/Typewriter Sfx Clips Data")]
    public class TypeWriterSfx_PrefabClipsDataSO : ScriptableObject
    {
        /// <summary>
        /// An array of AudioClips to play through in the event that we don't have one that matches the current mood.
        /// </summary>
        public AudioClip[] defaultSfxArray;
        /// <summary>
        /// Whether or not we should set the sfx clip index to a random value around the current one within sfxClipIndexRange after a full stop breath.
        /// </summary>
        public bool randomSfxClipIndex = false;
        /// <summary>
        /// True to select randomly from the Interrupts array when an interrupt is called for, false to traverse it sequentially.
        /// </summary>
        public bool randomInterrupt = false;
        /// <summary>
        /// AudioClip to mood associations; these will be massaged into an in-memory Dictionary in Setup().
        /// </summary>
        public MoodTrax sfxVibes;
        [field: SerializeField]
        [Range(0.0F, 1.0F)]
        public float Volume { get; set; } = 0.5F;
        public Mood CurrentMood { get; set; } = Mood.Happy;
        [field: SerializeField]
        public SfxInterruptSO[] Interrupts { get; set; }
        /// <summary>
        /// The fraction of the current AudioClip we should play, for trilling purposes; by default this is 1.0, meaning we play
        /// the entire AudioClip and don't trill at all.
        /// </summary>
        [field: SerializeField]
        [Range(0.0F, 1.0F)]
        public float TrillingClipFraction { get; set; } = 1.0F;
        /// <summary>
        /// Flag determining if the clip fraction we play for possible trilling should be randomized.
        /// </summary>
        [field: SerializeField]
        public bool ClipFractionRandomization { get; set; } = false;

        /// <summary>
        /// Populate the input <see cref="TypeWriterSfx_PrefabClips"/> Component's params using this data asset's config. 
        /// </summary>
        /// <param name="sfxComponent">The Sfx Component that should adopt this configuration.</param>
        public void Populate(TypeWriterSfx_PrefabClips sfxComponent)
        {
            sfxComponent.defaultSfxArray = defaultSfxArray;
            sfxComponent.randomSfxClipIndex = randomSfxClipIndex;
            sfxComponent.randomInterrupt = randomInterrupt;
            sfxComponent.sfxVibes = sfxVibes;
            sfxComponent.Volume = Volume;
            sfxComponent.CurrentMood = CurrentMood;
            sfxComponent.Interrupts = Interrupts;
            sfxComponent.TrillingClipFraction = TrillingClipFraction;
            sfxComponent.ClipFractionRandomization = ClipFractionRandomization;
        }
    }
}

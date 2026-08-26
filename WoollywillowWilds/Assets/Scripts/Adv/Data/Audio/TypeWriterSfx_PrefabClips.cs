
namespace WildsAdv
{
    /// <summary>
    /// Asynchronously loops through a mood-mapped and/or iterative set of prefabricated AudioClips.
    /// Each clip is played in a Coroutine that yield returns a WaitForSecondsRealtime(X seconds) where
    /// X is the length of the AudioClip; this way we can fire off clips and let them play to completion
    /// before moving on to the next one, until an external entity stops us.  
    /// </summary>
    public class TypeWriterSfx_PrefabClips : MonoBehaviour, ITypeWriterSfx, IInterruptableSfx
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
        /// AudioClip to mood associations; these will be massaged into an in-memory Dictionary in Setup().
        /// </summary>
        public MoodTrax sfxVibes;
        public float volume = 0.5F;
        /// <summary>
        /// Tracks the current index into the defaultSfxArray.
        /// </summary>
        private int sfxIndex = 0;
        private AudioClip currentTrack;
        private AudioSource player;
        private Mood mood;
        private Dictionary<Mood, List<AudioClip>> moodTracksMap;
        private IEnumerator sfxFunction;
        public void Setup()
        {
            if (moodTracksMap.Count == 0)
            {
                if (blipSfxVibes != null && blipSfxVibes.Vibes.Count > 0)
                {
                    foreach (VibeTrack vibe in blipSfxVibes.Vibes)
                    {
                        if (!moodTracksMap.ContainsKey(vibe.TrackMood))
                        {
                            moodTracksMap.Add(vibe.TrackMood, new List<AudioClip>());
                        }
                        moodTracksMap[vibe.TrackMood].Add(vibe.TrackClip);
                    }
                }
            }

            player = gameObject.AddComponent<AudioSource>();
            player.loop = true;
            player.volume = volume;
        }
        public void Teardown()
        {
            sfxBlipIndex = 0;
            moodTracksMap.Clear();
            Destroy(player);
        }
        public void Play()
        {
            sfxFunction = AsyncSfx_VoicedSentence(mood);
            StartCoroutine(sfxFunction);
        }
        public void Pause()
        {
            player.Pause();
            StopCoroutine(sfxFunction);
        }

        IEnumerator AsyncSfx_VoicedSentence(Mood mood)
        {
            int iterativeSfxIndex = 0;
            // loop forever, depending on the calling control flow to stop the host coroutine.
            while (true)
            {
                player.Pause();
                AudioClip currentTrack;
                if (VoiceSfxSegmentMap.ContainsKey(mood))
                {
                    List<AudioClip> moodTracks = VoiceSfxSegmentMap[mood];
                    if (randomSfxClipIndex)
                    {
                        System.Random rnd = new System.Random();
                        int clipIndex = rnd.Next(0, moodTracks.Count - 1);
                        currentTrack = moodTracks[clipIndex];
                    }
                    else
                    {
                        if (iterativeSfxIndex < moodTracks.Count - 1)
                        {
                            iterativeSfxIndex++;
                        }
                        else
                        {
                            iterativeSfxIndex = 0;
                        }
                        currentTrack = moodTracks[iterativeSfxIndex];
                    }
                }
                else
                {
                    if (randomSfxClipIndex)
                    {
                        System.Random rnd = new System.Random();
                        int clipIndex = rnd.Next(0, VoiceSfxSegmentArray.Count);
                        currentTrack = VoiceSfxSegmentArray[clipIndex];
                        Debug.Log("Playing " + currentTrack.name + " for " + currentTrack.length + ", from index " + clipIndex);
                    }
                    else
                    {
                        if (iterativeSfxIndex < VoiceSfxSegmentArray.Count - 1)
                        {
                            iterativeSfxIndex++;
                        }
                        else
                        {
                            iterativeSfxIndex = 0;
                        }
                        currentTrack = VoiceSfxSegmentArray[iterativeSfxIndex];
                        Debug.Log("Playing " + currentTrack.name + " for " + currentTrack.length + ", from index " + iterativeSfxIndex);
                    }
                }
                if (currentTrack != null)
                {
                    player.resource = currentTrack;
                }
                player.loop = true;
                player.Play();
                yield return new WaitForSecondsRealtime(currentTrack.length);
            }
        }


        public IEnumerator OnFunctionalInterrupt(SfxMode mode, float duration)
        {
            IEnumerator interruptFunction = null;
            switch (mode)
            {
                case SfxMode.ChirpSentenceAlgoClipped:
                    interruptFunction = AsyncSfx_ChirpSentenceAlgoClipped(currentTrack);
                    break;
                case SfxMode.ChirpSentenceAlgoVariance:
                    interruptFunction = AsyncSfx_ChirpSentenceAlgoVariance(currentMood);
                    break;
                case SfxMode.VoicedSentenceArray:
                    interruptFunction = AsyncSfx_VoicedSentence(currentMood);
                    break;
            }

            // todo: what happens if the 'parent' sfx coroutine we're currently running from, presumably AsyncSfx_ChirpSentencePrefabVariance(),
            //  gets stopped before we have the chance to stop this 'child' sfx coroutine? Is there a callback Coroutines get when stopped?
            //  EDIT: looks like nothing built in; you can sort of hack it yourself, but that would involve storing the IEnumerator handle we get
            //   here somewhere higher up? Perhaps maintain a list of SFX stuff to kill when a sentence ends?
            if (interruptFunction != null)
            {
                StartCoroutine(interruptFunction);
                yield return new WaitForSeconds(duration);
                StopCoroutine(interruptFunction);
            }
        }

        public AudioSource QueryPlayer()
        {
            return player;
        }

        public Mood QueryMood()
        {
            return mood;
        }

        public Dictionary<Mood, List<AudioClip>> QueryMoodMap()
        {
            return moodTracksMap;
        }
    }
}


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
        [Range(0.0F, 1.0F)]
        public float Volume { get; set; } = 0.5F;
        public Mood CurrentMood { get; set; } = Mood.Happy;
        [field: SerializeField]
        public SfxInterruptSO[] Interrupts { get; set; }
        /// <summary>
        /// The fraction of the current AudioClip we should play, for trilling purposes; by default this is 1.0, meaning we play
        /// the entire AudioClip and don't trill at all.
        /// </summary>
        [Range(0.0F, 1.0F)]
        public float TrillingClipFraction { get; set; } = 1.0F;
        /// <summary>
        /// Flag determining if the clip fraction we play for possible trilling should be randomized.
        /// </summary>
        public bool ClipFractionRandomization { get; set; } = false;
        private AudioClip currentTrack;
        private AudioSource player;
        private Dictionary<Mood, List<AudioClip>> moodTracksMap;
        private IEnumerator sfxFunction;
        private IEnumerator interruptFunction;
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
            player.volume = Volume;
        }
        public void Teardown()
        {
            sfxBlipIndex = 0;
            moodTracksMap.Clear();
            Destroy(player);
        }
        public void Play()
        {
            sfxFunction = AsyncSfx_MainStream(mood);
            StartCoroutine(sfxFunction);
            if (Interrupts.Length > 0)
            {
                interruptFunction = Async_Interrupt();
                StartCoroutine(interruptFunction);
            }
        }
        public void Pause()
        {
            player.Pause();
            StopCoroutine(sfxFunction);
            // trill support
            if (TrillingClipFraction < 1.0F)
            {
                // instead of pausing the sfx, we set looping false, ensure we're at the top of the playhead,
                // and allow the last clip to play through.
                singularSfx.Stop();
                singularSfx.loop = false;
                singularSfx.Play();
                currentTrack = (AudioClip)singularSfx.resource;
                // ensure we allow enough time for the chirp to play through.
                yield return new WaitForSeconds(currentTrack.length);
                singularSfx.Pause();
                // reset loop to true now that we've finished the unclipped chirp.
                singularSfx.loop = true;
            }
            // stop interrupt coroutine if relevant.
            StopCoroutine(interruptFunction);
        }

        IEnumerator AsyncSfx_MainStream(Mood mood)
        {
            int iterativeSfxIndex = 0;
            // loop forever, depending on the calling control flow to stop the host coroutine.
            while (true)
            {
                player.Pause();
                if (VoiceSfxSegmentMap.ContainsKey(mood))
                {
                    List<AudioClip> moodTracks = VoiceSfxSegmentMap[mood];
                    if (randomSfxClipIndex)
                    {
                        Random rnd = new Random();
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
                        Random rnd = new Random();
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

                float clipFraction = Math.Clamp(chirpClipFraction, 0.0F, 1.0F);
                if (clipFractionRandomization)
                {
                    float range = chirpClipFraction / 2.0F;
                    clipFraction = UnityEngine.Random.Range(chirpClipFraction - range, chirpClipFraction + range);
                }
                yield return new WaitUntil(() => player.time >= currentTrack.length * clipFraction);
            }
        }

        IEnumerator AsyncSfx_Interrupt()
        {
            // todo: add support for delay sorting alongside iterativeinterrupindex usage and a timer started outside
            //  the loop here so the designer can set specific interrupts to occur at specific absolute times in sequence?

            int iterativeInterruptIndex = 0;
            // loop forever, depending on the calling control flow to stop the host coroutine.
            while (true)
            {
                // figure out what we're injecting.
                int interruptIndex = iterativeInterruptIndex;
                if (randomInterrupt)
                {
                    Random rand = new Random();
                    interruptIndex = rand.Next(0, Interrupts.Length - 1);
                }


                SfxInterruptSO interrupt = Interrupts[interruptIndex];


                // figure out when to inject it.
                float varianceInjectDelay = interrupt.delay;
                if (chirpInjectionRandomization)
                {
                    float delayModifier = UnityEngine.Random.Range(0.0F, interrupt.variance);
                    float plusMinusRoll = UnityEngine.Random.Range(1, 100);
                    delayModifier *= plusMinusRoll <= 50 ? -1 : 1;
                    varianceInjectDelay += delayModifier;
                    varianceInjectDelay = (float)Math.Clamp(varianceInjectDelay, 0.0, interrupt.delay + interrupt.variance);
                }
                Debug.Log("About to wait for " + varianceInjectDelay + " before injecting interrupt into main stream.");
                yield return new WaitForSecondsRealtime(varianceInjectDelay);

                // pause the main stream.
                player.Pause();

                // cache the current main stream track so we can resume it after the interrupt completes.
                UnityEngine.Audio.AudioResource mainTrack = player.resource;
                yield return interrupt.Interrupt(this);
                player.resource = mainTrack;

                // resume playing main stream.
                player.Play();


                // increment or reset interrupt index.
                if (iterativeInterruptIndex < Interrupts.Length - 1)
                {
                    iterativeInterruptIndex++;
                }
                else
                {
                    iterativeInterruptIndex = 0;
                }
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

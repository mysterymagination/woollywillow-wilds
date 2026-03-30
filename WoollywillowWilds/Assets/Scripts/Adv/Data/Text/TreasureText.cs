using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Mono.Cecil.Cil;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WildsAdv
{
    public enum Mood
    {
        Happy,
        Enthusiastic,
        Angry,
        Sad,
        Sultry,
        Serious,
        Neutral
    }

    [Serializable]
    public class TreasureSentence
    {
        public TreasureSentence(Mood mood, string text)
        {
            SentenceText = text;
            SentenceMood = mood;
        }
        [field: SerializeField]
        public string SentenceText { get; }
        [field: SerializeField]
        public Mood SentenceMood { get; } = Mood.Neutral;

        override public string ToString()
        {
            return "{\n  \"text\": \"" + SentenceText + "\",\n  \"mood\": \"" + SentenceMood + "\"\n}";
        }

        public override bool Equals(object obj)
        {
            TreasureSentence otherSentence = (TreasureSentence)obj;
            return otherSentence.SentenceMood == this.SentenceMood
            && otherSentence.SentenceText.Equals(this.SentenceText);
        }
        public override int GetHashCode()
        {
            return JsonUtility.ToJson(this).GetHashCode();
        }
    }
    /// <summary>
    /// A text string in which each sentence can be associated with a mood; these will help inform
    /// which voice sfx track/segment should play for that sentence.
    /// </summary>
    [Serializable]
    public class TreasureText
    {
        /**
         * An ordered array of TreasureSentences, each associated with relevant Mood and presented in the
         * desired sequence for the final text rendering.
         */
        public List<TreasureSentence> Contents = new List<TreasureSentence>();

        public static Mood StringToMood(string moodString)
        {
            if (moodString.Equals(Mood.Happy.ToString().ToLower()))
            {
                return Mood.Happy;
            }
            else if (moodString.Equals(Mood.Enthusiastic.ToString().ToLower()))
            {
                return Mood.Enthusiastic;
            }
            else if (moodString.Equals(Mood.Angry.ToString().ToLower()))
            {
                return Mood.Angry;
            }
            else if (moodString.Equals(Mood.Neutral.ToString().ToLower()))
            {
                return Mood.Neutral;
            }
            else if (moodString.Equals(Mood.Sad.ToString().ToLower()))
            {
                return Mood.Sad;
            }
            else if (moodString.Equals(Mood.Serious.ToString().ToLower()))
            {
                return Mood.Serious;
            }
            else if (moodString.Equals(Mood.Sultry.ToString().ToLower()))
            {
                return Mood.Sultry;
            }
            else
            {
                Debug.LogWarning("Input string " + moodString + " does not match any known moods. Happy by default!");
                return Mood.Happy;
            }
        }

        /// <summary>
        /// Parses the given string for mood annotations and then populates this instance's Contents array with sentence(s) and associated mood in the required sequence to have
        /// the complete text passage make sense.
        /// todo: support multi-mooded sentences?
        /// </summary>
        /// <param name="annotatedText">A text string annotated with mood e.g. [@happy@]</param>
        public void ParseAnnotatedText(string annotatedText)
        {
            Mood currentMood = Mood.Neutral;
            int currentTextPosition = 0;
            int nextAnnotationPosition;
            string annotationTokenOpener = "[@";
            string annotationTokenCloser = "@]";
            string fullstopPattern = "[.!?:;…]";

            nextAnnotationPosition = annotatedText.IndexOf(annotationTokenOpener, currentTextPosition);
            Debug.Log("nextannotationpos is " + nextAnnotationPosition);

            while ((nextAnnotationPosition = annotatedText.IndexOf(annotationTokenOpener, currentTextPosition)) >= 0)
            {
                int textProcessed = 0;
                Match nextFullstop = Regex.Match(annotatedText.Substring(currentTextPosition), fullstopPattern);
                Debug.Log("nextAnnotation occurs at " + nextAnnotationPosition + ", and nextfullstop occurs at " + nextFullstop.Index);
                if (nextFullstop.Index < nextAnnotationPosition)
                {
                    // mood continuation case, append next sentence substring to current mood bucket.
                    string currentMoodSentences = annotatedText.Substring(currentTextPosition, nextAnnotationPosition);
                    string[] currentMoodSentencesArray = Regex.Split(currentMoodSentences, fullstopPattern);
                    foreach (string sentence in currentMoodSentencesArray)
                    {
                        Contents.Add(new TreasureSentence(currentMood, sentence));
                        textProcessed += sentence.Length;
                    }
                }
                else
                {
                    // new mood case; parse it out and reset currentMood.
                    int nextCloseTokenPosition = annotatedText.IndexOf(annotationTokenCloser, currentTextPosition);
                    int firstMoodCharacterPosition = nextAnnotationPosition + annotationTokenOpener.Length;
                    int moodTokenLength = nextCloseTokenPosition - firstMoodCharacterPosition;
                    string moodString = annotatedText.Substring(firstMoodCharacterPosition, moodTokenLength).ToLower();
                    currentMood = StringToMood(moodString);
                    textProcessed += annotationTokenOpener.Length + annotationTokenCloser.Length + moodString.Length;
                }

                // update currentTextPosition
                currentTextPosition += textProcessed;
                Math.Clamp(currentTextPosition, 0, annotatedText.Length - 1);
            }
            // catch the last set of current mood sentence(s), since above loop will exit once we run out of annotations and will therefore leave the last annotated substring.
            if (currentTextPosition < annotatedText.Length - 1)
            {
                string currentMoodSentences = annotatedText.Substring(currentTextPosition, annotatedText.Length - currentTextPosition);
                Debug.Log("currentMoodSentences in the sweep is: " + currentMoodSentences);
                string[] currentMoodSentencesArray = Regex.Split(currentMoodSentences, fullstopPattern);
                // todo: we lose the fullstop pattern itself using this method; might need to use a lookahead instead of split so we don't toss the match out the window.
                foreach (string sentence in currentMoodSentencesArray)
                {
                    Match nextFullstop = Regex.Match(annotatedText.Substring(currentTextPosition), fullstopPattern);
                    string trimmedSentence = sentence.Trim() + nextFullstop;
                    // If there's a fullstop pattern at the end of the text, we get a bogus empty string
                    // as an extra array element.
                    if (trimmedSentence.Length > 0)
                    {
                        Debug.Log("sweep sentence split over fullstop: " + trimmedSentence);
                        Contents.Add(new TreasureSentence(currentMood, trimmedSentence));
                    }
                    currentTextPosition += sentence.Length + nextFullstop.Length;
                }
            }

            foreach (TreasureSentence sentence in Contents)
            {
                Debug.Log("Parsed treasure sentence: " + sentence.ToString());
            }

            // parse a mood annotated string into the Contents array.
            /// Algo:
            /// 1. Assume current mood of neutral and current text position P that begins at 0.
            /// 2. Find next `[@` token position A.
            /// 3. If A != NPOS: working case
            ///     4. Find the next fullstop token position S after P.
            ///     5. If S < A: mood continuation case
            ///         a. Take the text from P..A and split it over fullstop characters.
            ///         b. Add each sentence array element to the current mood bucket.
            ///     6. Else:
            ///         a. Match the text between `[@` token and `]` token to a mood and set that as the current mood.
            ///     7. Loop 2
            /*
                e.g.
                "[@neutral]hello. I am fine. How are you? [@happy]The day, she is warm! I wonder if the chipmunks will return? Godsend it so! [@enthusiastic]whaaaaaat's good, bb? [@sultry]oooooh my~! That's what she said. That's what he said."

                would produce the Contents:
                [
                    {
                        text: "hello."
                        mood: Mood.Neutral
                    },
                    {
                        text: "I am fine."
                        mood: Mood.Neutral
                    },
                    {
                        text: "How are you?"
                        mood: Mood.Neutral
                    },
                    {
                        text: "whaaaaaat's good, bb?"
                        mood: Mood.Enthusiastic
                    },
                    {
                        text: "oooooh my~!"
                        mood: Mood.Sultry
                    },
                    {
                        text: "That's what she said."
                        mood: Mood.Sultry
                    },
                    {
                        text: "That's what he said."
                        mood: Mood.Sultry
                    },
                ]
            */
        }

        /// <summary>
        /// Dumps the raw text of the TreasureText.Contents array into a basic string.
        /// </summary>
        /// <returns>string containing the text data only.</returns>
        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TreasureSentence sentence in Contents)
            {
                sb.Append(sentence);
                sb.Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
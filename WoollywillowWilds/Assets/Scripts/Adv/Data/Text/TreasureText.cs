using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public TreasureSentence(string text, Mood mood)
        {
            SentenceText = text;
            SentenceMood = mood;
        }
        [field: SerializeField]
        public string SentenceText { get; set; }
        [field: SerializeField]
        public Mood SentenceMood { get; set; } = Mood.Neutral;
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
        public List<TreasureSentence> Contents;

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

        public void ParseAnnotatedText(string annotatedText)
        {
            Mood currentMood = Mood.Neutral;
            int currentTextPosition = 0;
            int nextAnnotationPosition = 0;
            string annotationTokenOpener = "[@";
            string annotationTokenCloser = "@]";
            string fullstopPattern = "[.!?:;…]";

            while ((nextAnnotationPosition = annotatedText.IndexOf(annotationTokenOpener, currentTextPosition)) > 0)
            {
                int textProcessed = 0;
                Match nextFullstop = Regex.Match(annotatedText.Substring(currentTextPosition), fullstopPattern);
                if (nextFullstop.Index < nextAnnotationPosition)
                {
                    // mood continuation case, append next sentence substring to current mood bucket.
                    string currentMoodSentences = annotatedText.Substring(currentTextPosition, nextAnnotationPosition);
                    string[] currentMoodSentencesArray = Regex.Split(currentMoodSentences, fullstopPattern);
                    foreach (string sentence in currentMoodSentencesArray)
                    {
                        Contents.Add(new TreasureSentence(sentence, currentMood));
                        textProcessed += sentence.Length;
                    }
                }
                else
                {
                    // new mood case; parse it out and reset currentMood.
                    int nextCloseTokenPosition = annotatedText.IndexOf(annotationTokenCloser, currentTextPosition);
                    int moodTokenLength = nextCloseTokenPosition - currentTextPosition;
                    string moodString = annotatedText.Substring(nextAnnotationPosition + 1, moodTokenLength).ToLower();
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
                string[] currentMoodSentencesArray = Regex.Split(currentMoodSentences, fullstopPattern);
                foreach (string sentence in currentMoodSentencesArray)
                {
                    Contents.Add(new TreasureSentence(sentence, currentMood));
                }
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
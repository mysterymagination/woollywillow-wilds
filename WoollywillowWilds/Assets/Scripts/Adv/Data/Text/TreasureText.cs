using System.Text;

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
    public class TreasureSentence
    {
        public string text = "";
        public Mood mood = Mood.Neutral;
    }
    /// <summary>
    /// A text string in which each sentence can be associated with a mood; these will help inform
    /// which voice sfx track/segment should play for that sentence.
    /// </summary>
    public class TreasureText
    {
        /**
         * An ordered array of TreasureSentences, each associated with relevant Mood and presented in the
         * desired sequence for the final text rendering.
         */
        public TreasureSentence[] Contents;

        public void ParseAnnotatedText(string annotatedText)
        {
            // todo: parse a mood annotated string into the Contents array.
            /*
                e.g.
                "
                @neutral
                hello. I am fine. How are you?

                @enthusiastic
                whaaaaaat's good, bb?

                @sultry
                oooooh my~! That's what she said. That's what he said.
                "

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
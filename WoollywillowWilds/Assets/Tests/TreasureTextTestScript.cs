using NUnit.Framework;
using UnityEngine;
//using WildsAdv;

public class TreasureTextTestScript
{
    /// <summary>
    /// Tests that a well-formed mood annotated text produces the expected Contents array.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationHappyPath()
    {
        string testText = Resources.Load<TextAsset>("LillyLittlebush_TestDesc.txt").text;
        //TreasureText systemUnderText = new TreasureText();
    }

    /// <summary>
    /// Tests that text with no mood annotations produces a Contents in the correct sequence and with each element associated with Mood.Neutral.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationNoAnnotations()
    {

    }

    /// <summary>
    /// Tests that multiple mood annotations contiguously are well-handled, with the last being set as the current mood (multi-mood mode not yet implemented).
    /// </summary>
    [Test]
    public void ParseMoodAnnotationMultipleMoods()
    {

    }
}

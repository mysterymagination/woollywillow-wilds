using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using WildsAdv;

public class TreasureTextTestScript
{
    /// <summary>
    /// Tests that the simplest well-formed mood annotated text produces the expected Contents array.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationSimplestHappyPath()
    {
        string testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/SingleSentence_TestDesc.txt").text;
        TreasureText systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(1));
        TreasureSentence expectedSentence1 = new TreasureSentence(Mood.Enthusiastic, "She's so pretty!");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(expectedSentence1));
    }

    /// <summary>
    /// Tests that the a single mood annotated text with multiple sentences produces the expected Contents array.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationSingleMoodHappyPath()
    {
        string testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/SingleMoodMultiSentence_TestDesc.txt").text;
        TreasureText systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(3));
        TreasureSentence expectedSentence1 = new TreasureSentence(Mood.Enthusiastic, "She's so pretty!");
        TreasureSentence expectedSentence2 = new TreasureSentence(Mood.Enthusiastic, "Here we sit.");
        TreasureSentence expectedSentence3 = new TreasureSentence(Mood.Enthusiastic, "There we stand?");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(expectedSentence3));
    }

    /// <summary>
    /// Tests that a well-formed mood annotated text produces the expected Contents array.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationHappyPath()
    {
        string testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/LillyLittlebush_TestDesc.txt").text;
        TreasureText systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(4));
        TreasureSentence expectedSentence1 = new TreasureSentence(Mood.Enthusiastic, "She's so pretty!");
        TreasureSentence expectedSentence2 = new TreasureSentence(Mood.Sultry, "Her name's Lily Littlebush.");
        TreasureSentence expectedSentence3 = new TreasureSentence(Mood.Happy, "Let's take a look at her story:");
        TreasureSentence expectedSentence4 = new TreasureSentence(Mood.Happy, "she sits in the center of the garden, keeping good and gentle watch, patting puppies as they play.");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(expectedSentence3));
        Assert.That(systemUnderText.Contents[3], Is.EqualTo(expectedSentence4));
    }

    /// <summary>
    /// Tests that text with no mood annotations produces a Contents in the correct sequence and with each element associated with Mood.Neutral.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationNoAnnotationsError()
    {
        string testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/NoAnnotationsError_TestDesc.txt").text;
        TreasureText systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(3));
        TreasureSentence expectedSentence1 = new TreasureSentence(Mood.Neutral, "She's so pretty!");
        TreasureSentence expectedSentence2 = new TreasureSentence(Mood.Neutral, "Ooh look a horse.");
        TreasureSentence expectedSentence3 = new TreasureSentence(Mood.Neutral, "Hmm, indeed indeed?");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(expectedSentence3));
    }

    /// <summary>
    /// Tests that multiple mood annotations contiguously are well-handled, with the last being set as the current mood (multi-mood mode not yet implemented).
    /// </summary>
    [Test]
    public void ParseMoodAnnotationMultipleContiguousMoodsError()
    {
        string testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/MultiMoodError_TestDesc.txt").text;
        TreasureText systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(3));
        TreasureSentence expectedSentence1 = new TreasureSentence(Mood.Happy, "She's so pretty!");
        TreasureSentence expectedSentence2 = new TreasureSentence(Mood.Sultry, "Ooh look a horse.");
        TreasureSentence expectedSentence3 = new TreasureSentence(Mood.Sultry, "Hmm, indeed indeed?");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(expectedSentence3));
    }
}

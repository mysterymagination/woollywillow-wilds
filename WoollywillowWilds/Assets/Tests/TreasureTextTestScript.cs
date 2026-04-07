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
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(7));
        TreasureSentence case1_expectedSentence1 = new TreasureSentence(Mood.Enthusiastic, "She's so pretty!");
        TreasureSentence case1_expectedSentence2 = new TreasureSentence(Mood.Enthusiastic, "And leafy!");
        TreasureSentence case1_expectedSentence3 = new TreasureSentence(Mood.Sultry, "Her name's Lily Littlebush.");
        TreasureSentence case1_expectedSentence4 = new TreasureSentence(Mood.Sultry, "She's leafy!");
        TreasureSentence case1_expectedSentence5 = new TreasureSentence(Mood.Sultry, "Are you?!");
        TreasureSentence case1_expectedSentence6 = new TreasureSentence(Mood.Happy, "Let's take a look at her story:");
        TreasureSentence case1_expectedSentence7 = new TreasureSentence(Mood.Happy, "she sits in the center of the garden, keeping good and gentle watch, patting puppies as they play.");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(case1_expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(case1_expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(case1_expectedSentence3));
        Assert.That(systemUnderText.Contents[3], Is.EqualTo(case1_expectedSentence4));
        Assert.That(systemUnderText.Contents[4], Is.EqualTo(case1_expectedSentence5));
        Assert.That(systemUnderText.Contents[5], Is.EqualTo(case1_expectedSentence6));
        Assert.That(systemUnderText.Contents[6], Is.EqualTo(case1_expectedSentence7));

        testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/HumongousFungus_TestDesc.txt").text;
        systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(10));
        TreasureSentence case2_expectedSentence1 = new TreasureSentence(Mood.Enthusiastic, "Look at the size of 'im!");
        TreasureSentence case2_expectedSentence2 = new TreasureSentence(Mood.Enthusiastic, "Crikey.");
        TreasureSentence case2_expectedSentence3 = new TreasureSentence(Mood.Sad, "If only there were more of him.");
        TreasureSentence case2_expectedSentence4 = new TreasureSentence(Mood.Curious, "Such a beauty.");
        TreasureSentence case2_expectedSentence5 = new TreasureSentence(Mood.Curious, "Mighty and magnificent;");
        TreasureSentence case2_expectedSentence6 = new TreasureSentence(Mood.Curious, "ancient beyond all comprehension, he is.");
        TreasureSentence case2_expectedSentence7 = new TreasureSentence(Mood.Serious, "Danger danger, for sure.");
        TreasureSentence case2_expectedSentence8 = new TreasureSentence(Mood.Happy, "Let's take a look at how he works...");
        TreasureSentence case2_expectedSentence9 = new TreasureSentence(Mood.Happy, "Fruiting bodies, now made from literal human bodies, spread throughout civilizations just like they used to spread through soil before the Spellplague…");
        TreasureSentence case2_expectedSentence10 = new TreasureSentence(Mood.Happy, "it's not ideal, but it sure is interesting!");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(case2_expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(case2_expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(case2_expectedSentence3));
        Assert.That(systemUnderText.Contents[3], Is.EqualTo(case2_expectedSentence4));
        Assert.That(systemUnderText.Contents[4], Is.EqualTo(case2_expectedSentence5));
        Assert.That(systemUnderText.Contents[5], Is.EqualTo(case2_expectedSentence6));
        Assert.That(systemUnderText.Contents[6], Is.EqualTo(case2_expectedSentence7));
        Assert.That(systemUnderText.Contents[7], Is.EqualTo(case2_expectedSentence8));
        Assert.That(systemUnderText.Contents[8], Is.EqualTo(case2_expectedSentence9));
        Assert.That(systemUnderText.Contents[9], Is.EqualTo(case2_expectedSentence10));
    }

    /// <summary>
    /// Tests that well-formed annotated text over multiple lines works as expected.
    /// </summary>
    [Test]
    public void ParseMoodAnnotationMultiline()
    {
        string testText = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/Test_Assets/Resources/Multiline_TestDesc.txt").text;
        TreasureText systemUnderText = new TreasureText();
        systemUnderText.ParseAnnotatedText(testText);
        Assert.That(systemUnderText.Contents.Count, Is.EqualTo(4));
        TreasureSentence expectedSentence1 = new TreasureSentence(Mood.Curious, "Wow, what's bubbling away in that beaker?");
        TreasureSentence expectedSentence2 = new TreasureSentence(Mood.Sultry, "Looks spicy!");
        TreasureSentence expectedSentence3 = new TreasureSentence(Mood.Serious, "Oook ook!");
        TreasureSentence expectedSentence4 = new TreasureSentence(Mood.Serious, "Lemme see.");
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
        TreasureSentence expectedSentence2 = new TreasureSentence(Mood.Curious, "Ooh look a horse.");
        TreasureSentence expectedSentence3 = new TreasureSentence(Mood.Curious, "Who has hooves?");
        Assert.That(systemUnderText.Contents[0], Is.EqualTo(expectedSentence1));
        Assert.That(systemUnderText.Contents[1], Is.EqualTo(expectedSentence2));
        Assert.That(systemUnderText.Contents[2], Is.EqualTo(expectedSentence3));
    }
}

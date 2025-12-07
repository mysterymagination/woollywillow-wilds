using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    public string RoomIntroText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject storyText = GameObject.FindWithTag("TagStoryText");
        Debug.Log("StoryText says " + storyText);
        TMP_Text textComponent = storyText.GetComponent<TMP_Text>();
        textComponent.text = RoomIntroText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

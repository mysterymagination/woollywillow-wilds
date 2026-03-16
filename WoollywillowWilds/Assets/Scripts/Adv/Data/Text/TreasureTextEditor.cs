using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(WildsAdv.TreasureText))]
public class TreasureTextEditor : Editor
{
    const string resourceFilename = "treasuretext-editor-uie";
    public override VisualElement CreateInspectorGUI()
    {
        Debug.Log("should be getting the damnable ui builder crap");
        VisualElement customInspector = new VisualElement();
        var visualTree = Resources.Load(resourceFilename) as VisualTreeAsset;
        visualTree.CloneTree(customInspector);
        customInspector.styleSheets.Add(Resources.Load($"{resourceFilename}-style") as StyleSheet);
        return customInspector;
    }
}

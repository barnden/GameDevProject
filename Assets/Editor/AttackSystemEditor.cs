using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackSystem))]
public class AttackSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        

        if (GUILayout.Button("Generate color"))
        {
            Debug.Log("We pressed the button!");
        }
    }
}

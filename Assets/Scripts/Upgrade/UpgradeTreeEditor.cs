#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(UpgradeTree)), CanEditMultipleObjects]
public class UpgradeTreeEditor : Editor
{
    // Positioning
    static Vector2 nodeSize = new Vector2(184f, 74f);
    static Vector2 nodeTitleSize = new Vector2(184f, 20f);
    static float columnWidth = 120.0f;

    float minTreeHeight = 720f;
    float minTreeWidth = 1000f;

    Vector2 iEdgeVec = new Vector2(186f, nodeSize.y / 2f);
    Vector2 oEdgeVec = new Vector2(-2f, nodeSize.y / 2f);
    Vector2 upArrowVec = new Vector2(-5f, -3f);
    Vector2 dnArrowVec = new Vector2(-5f, 3f);
    Vector2 nextLineVec = new Vector2(0f, EditorGUIUtility.singleLineHeight + 4f);
    Vector2 indentVec = new Vector2(50f, 0f);
    Vector2 nodeContentSize = new Vector2(120f, EditorGUIUtility.singleLineHeight);
    Vector2 nodeLabelSize = new Vector2(184f, EditorGUIUtility.singleLineHeight);

    // Scrolling
    Vector2 mouseSelectionOffset;
    Vector2 scrollPosition = Vector2.zero;
    Vector2 scrollStartPos;

    (UpgradeNode node, Rect? rect, HashSet<int> prereqs, SerializedObject list) active;

    HashSet<KeyCode> keydown;

    UpgradeTree tree;

    private void OnEnable()
    {
        keydown = new HashSet<KeyCode>();

        tree = (UpgradeTree)target;
        EditorSceneManager.sceneSaving += OnSceneSave;
        EditorSceneManager.sceneClosing += OnSceneClose;
        Undo.undoRedoPerformed += OnUndo;
    }

    private void OnSceneClose(UnityEngine.SceneManagement.Scene scene, bool removingScene)
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager.SceneClosingCallback.html
        Save();
    }

    private void OnSceneSave(UnityEngine.SceneManagement.Scene scene, string path)
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager.SceneSavingCallback.html
        Save();
    }

    private void OnUndo()
    {
        if (active.node == null || tree.IndexOf(active.node) == -1)
        {
            SetActive(null);
        }
        Save();
    }

    private void Save()
    {
        EditorUtility.SetDirty(tree);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void UpdateTree(string change = "")
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Undo.RegisterCompleteObjectUndo(tree, change);
    }

    private void DrawArrow(Vector2 begin, Vector2 end, Color color, float width, float opacity = 1f)
    {
        Vector2 startTangent = begin + Vector2.left;
        Vector2 endTangent = end + Vector2.right;

        if (Math.Abs(end.y - begin.y) > 30f && Math.Abs(end.x - begin.x) > 30f)
        {
            startTangent += Vector2.left * 100;
            endTangent += Vector2.right * 100;
        }

        color.a = opacity;
        Handles.color = color;

        Handles.DrawBezier(begin, end, startTangent, endTangent, color, null, width);

        // Arrow head on spline
        Handles.DrawLine(begin, begin + upArrowVec);
        Handles.DrawLine(begin, begin + dnArrowVec);
    }

    private void DrawConnections(UpgradeTree tree, UpgradeNode node, List<int> edges, Color color)
    {
        int i = tree.IndexOf(node);
        foreach (int j in edges)
        {
            Vector2 jposition = tree[j].position - scrollPosition;

            // Highlight all paths that lead to the active 
            bool shouldHighlight = false;

            if (active.node != null)
            {
                shouldHighlight = active.node == node || (active.prereqs != null && active.prereqs.Contains(j) && active.prereqs.Contains(i));
            }
            float opacity = shouldHighlight ? 1.0f : 0.3f;

            // Spline arrow connecting nodes
            DrawArrow(node.position + oEdgeVec - scrollPosition, jposition + iEdgeVec, color, 3f, opacity);
        }
    }

    private void DeleteActiveNode()
    {
        if (active.node == null)
            return;

        string name = active.node.title;
        tree.DeleteNode(active.node);
        UpdateTree($"Deleted node \"{name}\"");

        SetActive(null);
    }

    private void SetActive(UpgradeNode node, Rect? rect = null)
    {
        if (node == null)
        {
            active = (null, null, null, null);
            return;
        }

        int idx = tree.IndexOf(node);
        active = (node, rect, tree.GetAncestors(idx, true), CreateTemporaryMSO());
    }

    private SerializedObject CreateTemporaryMSO()
    {
        // Create a temporary scriptable object to hold List<Modifier> to display properly in EditorGUILayout#PropertyField

        ModifierSO mso = CreateInstance<ModifierSO>();
        mso.Init();
        SerializedObject smso = new SerializedObject(mso);

        return smso;
    }

    private void PropertyLayout<T>(string label, ref T property)
    {
        // Automatically create label/editor horizontal layout based on typeof T

        EditorGUILayout.BeginHorizontal(GUILayout.Width(3f * columnWidth));

        {
            if (typeof(T) != typeof(SerializedProperty))
                EditorGUILayout.LabelField(label, GUILayout.MaxWidth(columnWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));

            switch (property)
            {
                case string s:
                    property = (T)Convert.ChangeType(EditorGUILayout.TextField(s, GUILayout.MaxWidth(2f * columnWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)), typeof(T));
                    break;

                case double d:
                    property = (T)Convert.ChangeType(EditorGUILayout.DoubleField(d, GUILayout.MaxWidth(2f * columnWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)), typeof(T));
                    break;

                case int i:
                    property = (T)Convert.ChangeType(EditorGUILayout.IntField(i, GUILayout.MaxWidth(2f * columnWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)), typeof(T));
                    break;

                case bool b:
                    property = (T)Convert.ChangeType(EditorGUILayout.Toggle(b, GUILayout.MaxWidth(2f * columnWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)), typeof(T));
                    break;

                case SerializedProperty p:
                    EditorGUILayout.PropertyField(p, new GUIContent(label), true);
                    break;

                default:
                    if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)) || property == null)
                    {
                        property = (T)Convert.ChangeType(EditorGUILayout.ObjectField(property as UnityEngine.Object, typeof(T), false, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MaxWidth(2f * columnWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)), typeof(T));
                    }
                    break;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        // Mouse events
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        EventType UIEvent = Event.current.GetTypeForControl(controlID);

        // Cannot receive both mouse and keyboard at the same time, so keep track of keyboard here
        if (Event.current.type == EventType.KeyUp && keydown.Contains(Event.current.keyCode))
        {
            keydown.Remove(Event.current.keyCode);
        }

        if (Event.current.type == EventType.KeyDown)
        {
            keydown.Add(Event.current.keyCode);

            if (Event.current.keyCode == KeyCode.Delete && active.node != null)
            {
                DeleteActiveNode();
            }
        }

        EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.MinHeight(720));

        if (tree.tree == null)
            tree.tree = new List<UpgradeNode>();

        Vector2 mousePosition = Event.current.mousePosition;

        for (int i = 0; i < tree.size; i++)
        {
            UpgradeNode node = tree[i];

            if (node == null || node.title == null)
            {
                tree.DeleteNode(i);
                continue;
            }
            Vector2 position = node.position - scrollPosition;

            // Draw node
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = (active.node == node) ? FontStyle.Bold : FontStyle.Normal;

            Rect outerRect = new Rect(position, nodeSize);
            Rect innerRect = new Rect(position + new Vector2(2f, 2f), nodeSize - new Vector2(4f, 4f));
            Color nodeColor = new Color(.125f, .125f, .125f);

            if (node.bought)
            {
                nodeColor = new Color(.337f, .522f, .349f);
            }
            else if (tree.Buyable(tree.IndexOf(node)))
            {
                nodeColor = new Color(.882f, .882f, .561f);
            }

            EditorGUI.DrawRect(outerRect, nodeColor);
            EditorGUI.DrawRect(innerRect, new Color(.125f, .125f, .125f));
            EditorGUI.LabelField(new Rect(position + Vector2.right * 4f, nodeTitleSize), node.title, style);
            EditorGUI.LabelField(new Rect(position + nextLineVec + Vector2.right * 4f, nodeLabelSize), "Cost");
            node.cost = EditorGUI.DoubleField(new Rect(position + nextLineVec + indentVec, nodeContentSize), node.cost);
            EditorGUI.LabelField(new Rect(position + 2 * nextLineVec + Vector2.right * 4f, nodeLabelSize), "Sprite");

            var parent = tree.GetParentSprite(node);
            var effectiveSprite = tree.GetEffectiveSprite(node);
            GUI.enabled = !node.inheritSprite;
            var sprite = (Sprite)EditorGUI.ObjectField(new Rect(position + 2 * nextLineVec + indentVec, nodeContentSize), effectiveSprite, typeof(Sprite), true);

            if (parent == -1)
            {
                node.sprite = sprite;
            }
            GUI.enabled = true;

            DrawConnections(tree, node, node.prerequisites, Color.white);
            DrawConnections(tree, node, node.exclusion, Color.red);

            if (active.node != null && active.rect == null)
            {
                active.rect = outerRect;
            }

            if (!outerRect.Contains(mousePosition)) continue;

            // Mouse events
            switch (UIEvent)
            {
                case EventType.MouseDown:
                    if (Event.current.button <= 1)
                    {
                        SetActive(node, outerRect);
                        mouseSelectionOffset = node.position - mousePosition;
                    }
                    Repaint();
                    break;
                case EventType.MouseUp:
                    if (Event.current.button != 1 || active.node == null || active.node == node)
                        continue;

                    int s = tree.IndexOf(active.node);

                    if (keydown.Contains(KeyCode.LeftShift))
                    {
                        if (node.exclusion.Contains(s) || active.node.exclusion.Contains(i))
                        {

                            node.exclusion.Remove(s);
                            active.node.exclusion.Remove(i);

                            UpdateTree($"Removed exclusion between \"{active.node.title}\" and \"{node.title}\"");
                        }
                        else
                        {
                            node.exclusion.Add(s);
                            active.node.exclusion.Add(i);

                            UpdateTree($"Added exclusion \"{active.node.title}\" from \"{node.title}\"");
                        }
                    }
                    else
                    {
                        if (node.prerequisites.Contains(s))
                        {
                            node.prerequisites.Remove(s);
                            UpdateTree($"Removed prerequisite \"{active.node.title}\" from \"{node.title}\"");
                        }
                        else if (active.node.prerequisites.Contains(i))
                        {
                            active.node.prerequisites.Remove(i);
                            UpdateTree($"Removed prerequisite \"{node.title}\" from \"{active.node.title}\"");
                        }
                        else if (tree.Connectable(tree.IndexOf(active.node), i))
                        {
                            node.prerequisites.Add(s);

                            for (int k = 0; k < tree.size; k++)
                            {
                                tree.RemoveCycles(k);
                            }

                            UpdateTree($"Added prerequisite \"{active.node.title}\" from \"{node.title}\"");
                        }
                    }

                    break;
            }
        }

        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (Event.current.button == 2)
                    scrollStartPos = (mousePosition + scrollPosition);

                if (active.node != null && active.rect != null && Event.current.button == 0 && !active.rect.Value.Contains(mousePosition))
                {
                    SetActive(null);
                    EditorGUI.FocusTextInControl(null);

                    Repaint();
                }

                if (keydown.Contains(KeyCode.LeftControl))
                {
                    string name = "Upgrade " + (tree.size + 1);

                    if (tree.AddNode(name, mousePosition))
                    {
                        UpdateTree($"Add node \"{name}\"");
                        var node = tree.IndexOf(name);
                        SetActive(tree[node]);
                    }
                }

                break;

            case EventType.MouseDrag:
                if (Event.current.button == 2)
                {
                    scrollPosition = scrollStartPos - mousePosition;
                    Repaint();
                }
                break;
        }

        EditorGUILayout.EndScrollView();
        Rect scrollRect = GUILayoutUtility.GetLastRect();

        // Draw new path
        if (active.node != null && scrollRect.Contains(mousePosition))
        {
            if (UIEvent == EventType.MouseDrag && Event.current.button == 0)
            {
                // Draggable nodes
                active.node.position = mousePosition + mouseSelectionOffset;
                UpdateTree($"Moved {active.node.title}");

                Repaint();
            }
            else if (Event.current.button == 1)
            {
                Vector2 position = active.node.position;

                Color color = keydown.Contains(KeyCode.LeftShift) ? Color.red : Color.white;

                DrawArrow(mousePosition, position - scrollPosition + iEdgeVec, color, 1.5f);

                Repaint();
            }
        }

        // Adjust scrollbar positions
        scrollPosition.x = GUILayout.HorizontalScrollbar(scrollPosition.x, 20f, 0f, minTreeWidth);
        scrollPosition.y = GUI.VerticalScrollbar(new Rect(0, 0, 20, 720), scrollPosition.y, 20f, 0f, minTreeHeight);

        // Build node inspector fields
        EditorGUILayout.BeginHorizontal();
        {
            if (active.node != null)
            {
                // Left most column of node inspector for basic node information
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(3.25f * columnWidth));
                {
                    EditorGUILayout.LabelField("Node Inspector", GUILayout.MaxWidth(3f * columnWidth));
                    {
                        PropertyLayout("Title", ref active.node.title);
                        PropertyLayout("Description", ref active.node.description);
                        PropertyLayout("Cost", ref active.node.cost);
                        PropertyLayout("Is Owned", ref active.node.bought);

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Delete " + active.node.title, GUILayout.MaxWidth(3f * columnWidth)))
                        {
                            DeleteActiveNode();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                // Middle column of node inspector for sprite details
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(3.25f * columnWidth));
                {
                    EditorGUILayout.LabelField("Sprite Options", GUILayout.MaxWidth(3f * columnWidth));

                    PropertyLayout("Inherit Sprite", ref active.node.inheritSprite);

                    var parent = tree.GetParentSprite(active.node);
                    if (active.node.inheritSprite)
                    {
                        GUI.enabled = false;
                        {
                            var parentName = parent < 0 ? "None" : tree[parent].title;

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Inheritance", GUILayout.MaxWidth(columnWidth));
                            EditorGUILayout.LabelField(parentName, GUILayout.MaxWidth(2f * columnWidth));
                            EditorGUILayout.EndHorizontal();

                            if (parent != -1)
                            {
                                PropertyLayout("Effective Sprite", ref tree[parent].sprite);
                            }
                        }
                        GUI.enabled = true;
                    }
                    else
                    {
                        PropertyLayout("Sprite", ref active.node.sprite);
                        PropertyLayout("Z-Index", ref active.node.zindex);

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Auto Z-Index"))
                        {
                            if (parent < 0)
                            {
                                active.node.zindex = 0;
                            }
                            else
                            {
                                active.node.zindex = tree[parent].zindex + 1;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    PropertyLayout("Icon", ref active.node.icon);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                // Right most column of node inspector for modifiers
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(3.25f * columnWidth));
                {
                    SerializedProperty modifiers = active.list.FindProperty("modifiers");
                    active.list.Update();
                    Serializable.SetTargetObjectOfProperty(modifiers, active.node.modifiers);

                    PropertyLayout("Modifiers", ref modifiers);

                    if (active.list.hasModifiedProperties)
                    {
                        active.list.ApplyModifiedProperties();
                        active.node.modifiers = (List<Modifier>)Serializable.GetTargetObjectOfProperty(modifiers);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("No Currently Selected Node", GUILayout.MaxWidth(3f * columnWidth));
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif
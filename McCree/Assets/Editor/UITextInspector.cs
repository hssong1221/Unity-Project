using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(UIText))]
public class UITextInspector : TextEditor
{
    // UIText의 Inspector 무조건 Editor폴더 안에 존재해야한다 아니면 빌드시 실패
    private SerializedProperty m_DisableWordWrap;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_DisableWordWrap = serializedObject.FindProperty("m_DisableWordWrap");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_DisableWordWrap);

        serializedObject.ApplyModifiedProperties();
    }

}
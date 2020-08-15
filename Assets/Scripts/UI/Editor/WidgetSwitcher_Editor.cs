using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UI_Switcher)), CanEditMultipleObjects]
public class WidgetSwitcher_Editor : Editor
{
	#region Public Methods
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		serializedObject.Update();
		if (GUILayout.Button("Reset Switcher"))
		{
			m_IndexSwitcherProperty.intValue = 0;

			m_RectTransformProperty.objectReferenceValue = (target as UI_Switcher).GetComponent<RectTransform>();
			m_SwitcherTarget.ActiveByIndex(0);
		}
		int lastChildIndex = Mathf.Max(0, m_SwitcherTarget.transform.childCount - 1);
		EditorGUILayout.IntSlider(m_IndexSwitcherProperty, 0, lastChildIndex, new GUIContent("Index"));
		m_SwitcherTarget.ActiveByIndex(m_IndexSwitcherProperty.intValue);

		serializedObject.ApplyModifiedProperties();
	}
	#endregion

	#region Private Methods
	private void OnEnable()
	{
		m_RectTransformProperty = serializedObject.FindProperty("m_RectTransform");
		m_IndexSwitcherProperty = serializedObject.FindProperty("m_IndexSwitcher");

		serializedObject.Update();
		m_SwitcherTarget = target as UI_Switcher;
		m_RectTransformProperty.objectReferenceValue = (target as UI_Switcher).GetComponent<RectTransform>();
		serializedObject.ApplyModifiedProperties();
	}
	#endregion

	#region Private Attributes
	private SerializedProperty m_RectTransformProperty = null;
	private SerializedProperty m_IndexSwitcherProperty = null;
	private UI_Switcher m_SwitcherTarget = null;
	#endregion
}

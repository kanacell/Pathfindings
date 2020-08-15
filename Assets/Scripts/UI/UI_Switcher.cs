using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UI_Switcher : MonoBehaviour
{
	#region Public Methods
	public void ActiveByIndex(int _Index)
	{
		if (!m_RectTransform)
			return;

		m_IndexSwitcher = Mathf.Clamp(_Index, 0, m_RectTransform.childCount - 1);
		for(int i = 0; i < m_RectTransform.childCount; i++)
		{
			m_RectTransform.GetChild(i).gameObject.SetActive(i == m_IndexSwitcher);
		}
	}
	#endregion

	#region Private Methods
	private void Start()
	{
		Debug.Log($"is null = {!m_RectTransform}");
		ActiveByIndex(m_IndexSwitcher);
	}
	#endregion

	#region Getters/Setters
	public int IndexSwitcher
	{
		get
		{
			return m_IndexSwitcher;
		}
	}
	#endregion

	#region Private Attributes
	[SerializeField] private int m_IndexSwitcher = 0;
	[SerializeField] private RectTransform m_RectTransform = null;
	#endregion
}

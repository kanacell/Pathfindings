using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGuiButton : MonoBehaviour
{
	#region Public Methods
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	private void OnGUI()
	{
		if (GUILayout.Button("Press Me"))
		{
			if (!m_Instance)
			{
				m_Instance = Instantiate(m_Prefab);
			}

			m_Cpt++;
		}
	}

	private void Update()
	{
		m_Cpt %= 10;
	}
	#endregion

	#region Getters/Setters
	#endregion

	#region Public Attributes
	#endregion

	#region Protected Attributes
	#endregion

	#region Private Attributes
	[SerializeField] private GameObject m_Prefab = null;
	private GameObject m_Instance = null;
	int m_Cpt = 0;
	#endregion
}

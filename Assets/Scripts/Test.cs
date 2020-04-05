using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
	#region Public Methods
	public void DisplayPath()
	{
		if (m_PersistentText)
			m_PersistentText.text = Application.persistentDataPath;

		if (m_ApplicationText)
			m_ApplicationText.text = Application.dataPath;
	}
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	#endregion

	#region Getters/Setters
	#endregion

	#region Public Attributes
	public TextMeshProUGUI m_PersistentText = null;
	public TextMeshProUGUI m_ApplicationText = null;
	#endregion
	
	#region Protected Attributes
	#endregion
	
	#region Private Attributes
	#endregion
}

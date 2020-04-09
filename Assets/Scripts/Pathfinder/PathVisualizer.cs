using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
	#region Public Methods
	public void TracePath(Path _Path)
	{
		if (_Path == null)
			return;

		m_LineRenderer.positionCount = _Path.Steps.Count;
		for(int i = 0; i < m_LineRenderer.positionCount; i++)
		{
			m_LineRenderer.SetPosition(i, _Path.Steps[i].transform.position + Vector3.up);
		}
	}
	#endregion

	#region Private Methods
	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}
	#endregion

	#region Private Attributes
	LineRenderer m_LineRenderer = null;
	#endregion
}

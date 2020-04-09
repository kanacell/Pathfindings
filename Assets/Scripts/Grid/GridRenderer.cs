using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
	#region Public Methods
	public void ApplyTexture(Texture2D _Texture)
	{
		Material tempMat = new Material(m_Renderer.sharedMaterial);
		tempMat.mainTexture = _Texture;
		m_Renderer.sharedMaterial = tempMat;
	}
	#endregion

	#region Private Attributes
	[SerializeField] private MeshRenderer m_Renderer = null;
	#endregion
}

using System.ComponentModel;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
	#region Public Methods
    public void InitRenderer(int _Rows, int _Columns)
    {
        m_GridTexture = new Texture2D(_Columns, _Rows);
        m_GridTexture.filterMode = FilterMode.Point;
        transform.localScale = new Vector3Int(_Columns, 1, _Rows);
        transform.position = new Vector3(_Columns / 2f, 0, _Rows / 2f);
    }

    public void SetTexturePixel(int _PixelX, int _PixelY, Color _PixelColor)
    {
        m_GridTexture.SetPixel(_PixelX, _PixelY, _PixelColor);
    }

    public void ApplyRenderer()
    {
        m_GridTexture.Apply();
        Material tempMat = new Material(m_Renderer.sharedMaterial);
        tempMat.mainTexture = m_GridTexture;
        m_Renderer.sharedMaterial = tempMat;
    }

    [ContextMenu("Clear Renderer")]
    public void ClearRenderer()
    {
        m_GridTexture = new Texture2D(1, 1);
        m_GridTexture.filterMode = FilterMode.Point;
        m_GridTexture.SetPixel(0, 0, Color.white);
        m_GridTexture.Apply();
        Material tempMat = new Material(m_Renderer.sharedMaterial);
        tempMat.mainTexture = m_GridTexture;
        m_Renderer.sharedMaterial = tempMat;
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero;
    }
    #endregion

    #region Private Attributes
    [SerializeField] private MeshRenderer m_Renderer = null;
    private Texture2D m_GridTexture = null;
    #endregion
}

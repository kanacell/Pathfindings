using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    #region Public Methods
    public void TracePath(Path _Path, int _Rows)
    {
        if (_Path == null)
            return;

        m_LineRenderer.positionCount = _Path.Steps.Count;
        for (int i = 0; i < m_LineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(_Path.Steps[i].Column + 0.5f, 0, _Rows - _Path.Steps[i].Row - 0.5f);
            m_LineRenderer.SetPosition(i, pos + Vector3.up);
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

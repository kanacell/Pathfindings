using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class TestCylinder : MonoBehaviour
{
    #region Public Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void Update()
    {
        bool isInside = IsTargetInsideCylinder();
        string message = "";
        message += $"Is target inside cylinder {isInside}\n";
        Debug.Log(message, gameObject);
    }

    private void OnDrawGizmos()
    {
        DrawCylinder();
    }

    private bool IsTargetInsideCylinder()
    {
        Vector3 cylinderAxis = transform.up * m_Length;
        Vector3 toProject = m_Target.position - transform.position;
        Debug.DrawLine(transform.position, m_Target.position, Color.yellow);

        Vector3 unityProjection = Vector3.Project(toProject, cylinderAxis);
        if (m_DebugUnityProjection)
        {
            Debug.DrawLine(transform.position + unityProjection, m_Target.position, Color.red);
        }

        Vector3 customProjection = transform.position + Vector3.Dot(toProject, cylinderAxis) / Vector3.Dot(cylinderAxis, cylinderAxis) * cylinderAxis;
        if (m_DebugCustomProjection)
        {
            Debug.DrawLine(customProjection, m_Target.position, Color.blue);
        }

        bool isInside = false;
        if (m_DisplayCustomProjectionResult)
        {
            isInside = (customProjection - transform.position).magnitude <= m_Length;
            isInside = isInside && (customProjection - m_Target.position).magnitude <= m_Radius;
        }
        else
        {
            isInside = (unityProjection - transform.position).magnitude <= m_Length;
            isInside = isInside && (unityProjection - m_Target.position).magnitude <= m_Radius;
        }
        return isInside;
    }

    private void DrawCylinder()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, m_Radius);
        Handles.DrawWireDisc(CylinderDest, transform.up, m_Radius);

        Vector3 north = transform.position + transform.forward * m_Radius;
        Vector3 south = transform.position - transform.forward * m_Radius;
        Vector3 east = transform.position + transform.right * m_Radius;
        Vector3 west = transform.position - transform.right * m_Radius;

        Handles.DrawLine(north, north + transform.up * m_Length);
        Handles.DrawLine(south, south + transform.up * m_Length);
        Handles.DrawLine(east, east + transform.up * m_Length);
        Handles.DrawLine(west, west + transform.up * m_Length);
        Handles.DrawLine(transform.position, transform.position + transform.up * m_Length);
    }
    #endregion

    #region Getters/Setters
    public Vector3 CylinderDest
    {
        get
        {
            return transform.position + transform.up * m_Length;
        }
    }
    #endregion

    #region Public Attributes
    #endregion

    #region Protected Attributes
    #endregion

    #region Private Attributes
    [SerializeField, Range(1, 5)] private float m_Radius = 1;
	[SerializeField, Range(1, 10)] private float m_Length = 1;
	[SerializeField] private Transform m_Target = null;
    [SerializeField] private bool m_DisplayCustomProjectionResult = false;
    [SerializeField] private bool m_DebugCustomProjection = false;
    [SerializeField] private bool m_DebugUnityProjection = false;
	#endregion
}

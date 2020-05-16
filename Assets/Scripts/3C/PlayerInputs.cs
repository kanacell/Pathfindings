using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
	#region Public Methods
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	private void Update()
	{
		OnHovered?.Invoke(Input.mousePosition);

		if (Input.GetMouseButtonDown(0))
		{
			OnLeftClic?.Invoke(Input.mousePosition);
		}

		if (Input.GetMouseButtonDown(1))
		{
			OnRightClic?.Invoke(Input.mousePosition);
		}

		float horizontalValue = Input.GetAxisRaw("Horizontal");
		OnHorizontalMove?.Invoke(horizontalValue);

		float verticalValue = Input.GetAxisRaw("Vertical");
		OnVerticalMove?.Invoke(verticalValue);

		float zoomValue = Input.GetAxisRaw("Mouse ScrollWheel");
		OnZoom?.Invoke(zoomValue);
	}
	#endregion

	#region Getters/Setters
	#endregion

	#region Public Attributes
	public event System.Action<Vector3> OnHovered;
	public event System.Action<Vector3> OnLeftClic;
	public event System.Action<Vector3> OnRightClic;
	public event System.Action<float> OnHorizontalMove;
	public event System.Action<float> OnVerticalMove;
	public event System.Action<float> OnZoom;
	#endregion
	
	#region Protected Attributes
	#endregion
	
	#region Private Attributes
	#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	#region Private Methods
	private void Start()
	{
		if (m_PlayerInputs)
		{
			m_PlayerInputs.OnHorizontalMove += MoveHorizontal;
			m_PlayerInputs.OnVerticalMove += MoveVertical;
			m_PlayerInputs.OnZoom += Zoom;
		}
	}

	private void MoveHorizontal(float _Direction)
	{
		transform.position += _Direction * Vector3.right * m_MoveSpeed * Time.deltaTime;
	}

	private void MoveVertical(float _Direction)
	{
		transform.position += _Direction * Vector3.forward * m_MoveSpeed * Time.deltaTime;
	}

	private void Zoom(float _Direction)
	{
		Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize -_Direction * m_ZoomSpeed * Time.deltaTime, 5);
	}
	#endregion

	#region Private Attributes
	[Header("Inputs")]
	[SerializeField] private PlayerInputs m_PlayerInputs = null;

	[Header("Speeds settings")]
	[SerializeField, Range(0.1f, 10.0f)] private float m_MoveSpeed = 0.1f;
	[SerializeField, Range(0.1f, 10.0f)] private float m_ZoomSpeed = 0.1f;
	#endregion
}

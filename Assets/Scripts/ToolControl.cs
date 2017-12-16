using System;
using UnityEngine;

public class ToolControl : MonoBehaviour
{
	private Transform _hmd;

	public bool ControlHmd = false;

	public void Start()
	{
		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if (mainCamera == null) throw new NullReferenceException("A Camera with tag MainCamera is required");
		_hmd = mainCamera.transform;
	}

	public void LateUpdate()
	{
		if (ControlHmd)
		{
			_hmd.position = transform.position;
			_hmd.rotation = transform.rotation;
		}
	}
}
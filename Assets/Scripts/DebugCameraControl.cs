using System;
using UnityEngine;
using UnityEngine.VR;

public class DebugCameraControl : MonoBehaviour
{
	private Transform _hmd;

	public bool ControlHmd = false;

	public void Start()
	{
		var mainCamera = Camera.main;
		if (mainCamera == null) throw new NullReferenceException("A main Camera is required");
		_hmd = mainCamera.transform;
	}

	public void LateUpdate()
	{
		if (ControlHmd)
		{
			_hmd.parent.position = -InputTracking.GetLocalPosition(VRNode.CenterEye) + transform.position;
			_hmd.parent.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.CenterEye)) * transform.rotation;
		}
	}
}
using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKLookAt : MonoBehaviour
{
	private Animator _animator;
	private Transform _viewTarget;

	public void Start()
	{
		_animator = GetComponent<Animator>();
		if(_animator == null) throw new NullReferenceException("An Animator is required");

		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if(mainCamera == null) throw new NullReferenceException("A Camera with tag MainCamera is required");
		_viewTarget = mainCamera.transform;
	}

	public void OnAnimatorIK()
	{
		if (!_animator || !_viewTarget) return;

		//TODO: Reduce body weight to 0 when you get near the body... otherwise the model avoids you!
		//TODO: Stop looking when out of reach (e.g. behind or too low)
		_animator.SetLookAtWeight(1f, 0.4f, 0.6f, 1f);
		_animator.SetLookAtPosition(_viewTarget.transform.position);
	}
}

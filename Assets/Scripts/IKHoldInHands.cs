using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKHoldInHands : MonoBehaviour
{
	private Animator _animator;
	private Transform _head;

	public void Start()
	{
		_animator = GetComponent<Animator>();
		if(_animator == null) throw new NullReferenceException("An Animator is required");

		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if(mainCamera == null) throw new NullReferenceException("A Camera with tag MainCamera is required");
		_head = mainCamera.transform;
	}

	public void OnAnimatorIK()
	{
		PositionHand(AvatarIKGoal.RightHand, _head, -1);
		PositionHand(AvatarIKGoal.LeftHand, _head, 1);
	}

	private void PositionHand(AvatarIKGoal goal, Transform target, float side)
	{
		_animator.SetIKPositionWeight(goal, 1);
		_animator.SetIKPosition(goal, target.TransformPoint(new Vector3(.1f * side, -0.02f, 0.03f)));

		_animator.SetIKRotationWeight(goal, 1);
		_animator.SetIKRotation(goal, Quaternion.LookRotation(target.TransformDirection(Vector3.back), target.TransformDirection(Vector3.right * side)));
	}
}

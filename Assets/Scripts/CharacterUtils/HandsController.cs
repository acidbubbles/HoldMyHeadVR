using System;
using UnityEngine;

[Serializable]
public class HandsSettings : ControllerSettings
{
	public Vector3 HandIdlePosition;
	public Vector3 HandIdleRotation;
}

public class HandsController
{
	private const float ReachDistance = 0.5f;
	private const float ReachDuration = 1.2f;
	private const float HeadRadius = .1f;
	private const float VerticalOffset = -0.02f;
	private const float DepthOffset = 0.03f;
	private const float LowerShoulderCenter = 0.2f;

	private readonly HandsSettings _settings;
	private readonly Animator _animator;
	private readonly Transform _head;
	private readonly InverseKinematicsWeightHelper _reach;

	public HandsController(HandsSettings settings, Animator animator, Transform head)
	{
		_settings = settings;
		_animator = animator;
		_head = head;
		_reach = new InverseKinematicsWeightHelper();
	}

	public void Update(FrameContext context)
	{
		//TODO: Instead, try aligning hands against body position
		if (!_settings.Enabled) return;

		var shouldersCenter = (_animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position + _animator.GetBoneTransform(HumanBodyBones.RightShoulder).position) / 2 + Vector3.down * LowerShoulderCenter;

		var withinReach = context.IsLookedAt && Vector3.Distance(shouldersCenter, _head.position) < ReachDistance;
		var weight = _reach.GetWeight(withinReach, ReachDuration);

		PositionHand(AvatarIKGoal.RightHand, _head, weight, -1);
		PositionHand(AvatarIKGoal.LeftHand, _head, weight, 1);
	}

	private void PositionHand(AvatarIKGoal goal, Transform target, float weight, float side)
	{
		var handOnHeadPosition = target.TransformPoint(new Vector3(HeadRadius * side, VerticalOffset, DepthOffset));
		var handOnHeadRotation = Quaternion.LookRotation(target.TransformDirection(Vector3.back), target.TransformDirection(Vector3.right * side));

		var idleHandPosition = _animator.bodyPosition + _animator.bodyRotation * new Vector3(_settings.HandIdlePosition.x * side, _settings.HandIdlePosition.y, _settings.HandIdlePosition.z);
		var idleHandRotation = Quaternion.Euler(_settings.HandIdleRotation * -1);

		var position = Vector3.Slerp(idleHandPosition, handOnHeadPosition, weight);
		var rotation = Quaternion.Slerp(idleHandRotation, handOnHeadRotation, weight);

		_animator.SetIKPositionWeight(goal, 1);
		_animator.SetIKPosition(goal, position);

		_animator.SetIKRotationWeight(goal, 1);
		_animator.SetIKRotation(goal, rotation);
	}
}

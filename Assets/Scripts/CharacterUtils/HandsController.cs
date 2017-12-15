using UnityEngine;

public class HandsController
{
	private const float ReachDistance = 0.5f;
	private const float ReachDuration = 1.2f;
	private const float HeadRadius = .1f;
	private const float VerticalOffset = -0.02f;
	private const float DepthOffset = 0.03f;
	private const float LowerShoulderCenter = 0.2f;

	private readonly ControllerSettings _settings;
	private readonly Animator _animator;
	private readonly InverseKinematicsWeightHelper _reach;

	public HandsController(ControllerSettings settings, Animator animator)
	{
		_settings = settings;
		_animator = animator;
		_reach = new InverseKinematicsWeightHelper();
	}

	public void OnHead(Transform head)
	{
		if (!_settings.enabled) return;

		var shouldersCenter = (_animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position + _animator.GetBoneTransform(HumanBodyBones.RightShoulder).position) / 2 + Vector3.down * LowerShoulderCenter;

		var withinReach = Vector3.Distance(shouldersCenter, head.position) < ReachDistance;
		var weight = _reach.GetWeight(withinReach, ReachDuration);

		PositionHand(AvatarIKGoal.RightHand, head, weight, -1);
		PositionHand(AvatarIKGoal.LeftHand, head, weight, 1);
	}

	private void PositionHand(AvatarIKGoal goal, Transform target, float weight, float side)
	{
		_animator.SetIKPositionWeight(goal, weight);
		_animator.SetIKPosition(goal, target.TransformPoint(new Vector3(HeadRadius * side, VerticalOffset, DepthOffset)));

		_animator.SetIKRotationWeight(goal, weight);
		_animator.SetIKRotation(goal, Quaternion.LookRotation(target.TransformDirection(Vector3.back), target.TransformDirection(Vector3.right * side)));
	}
}

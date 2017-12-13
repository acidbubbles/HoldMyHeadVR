using UnityEngine;

public class HandsController
{
	private const float HeadRadius = .1f;
	private const float VerticalOffset = -0.02f;
	private const float DepthOffset = 0.03f;

	private readonly Animator _animator;

	public HandsController(Animator animator)
	{
		_animator = animator;
	}

	public void OnHead(Transform head)
	{
		PositionHand(AvatarIKGoal.RightHand, head, -1);
		PositionHand(AvatarIKGoal.LeftHand, head, 1);
	}

	private void PositionHand(AvatarIKGoal goal, Transform target, float side)
	{
		_animator.SetIKPositionWeight(goal, 1);
		_animator.SetIKPosition(goal, target.TransformPoint(new Vector3(HeadRadius * side, VerticalOffset, DepthOffset)));

		_animator.SetIKRotationWeight(goal, 1);
		_animator.SetIKRotation(goal, Quaternion.LookRotation(target.TransformDirection(Vector3.back), target.TransformDirection(Vector3.right * side)));
	}
}

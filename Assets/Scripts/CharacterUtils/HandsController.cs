using UnityEngine;

public class HandsController
{
	private const float ReachDistance = 0.5f;
	private const float ReachDuration = 1.2f;
	private const float HeadRadius = .1f;
	private const float VerticalOffset = -0.02f;
	private const float DepthOffset = 0.03f;

	private readonly Animator _animator;

	private bool _snap;
	private float _lastEventTime;
	private float _lastWeight;
	private float _startWeight;

	public HandsController(Animator animator)
	{
		_animator = animator;
	}

	public void OnHead(Transform head)
	{
		var shouldersCenter = (_animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position + _animator.GetBoneTransform(HumanBodyBones.RightShoulder).position) / 2;

		var withinReach = Vector3.Distance(shouldersCenter, head.position) < ReachDistance;
		if (withinReach & !_snap)
		{
			_snap = true;
			_lastEventTime = Time.time;
			_startWeight = _lastWeight;
		} else if (!withinReach & _snap)
		{
			_snap = false;
			_lastEventTime = Time.time;
			_startWeight = _lastWeight;
		}

		_lastWeight = Mathf.SmoothStep(_startWeight, _snap ? 1f : 0f, (Time.time - _lastEventTime) / ReachDuration);

		PositionHand(AvatarIKGoal.RightHand, head, _lastWeight, -1);
		PositionHand(AvatarIKGoal.LeftHand, head, _lastWeight, 1);
	}

	private void PositionHand(AvatarIKGoal goal, Transform target, float weight, float side)
	{
		_animator.SetIKPositionWeight(goal, weight);
		_animator.SetIKPosition(goal, target.TransformPoint(new Vector3(HeadRadius * side, VerticalOffset, DepthOffset)));

		_animator.SetIKRotationWeight(goal, weight);
		_animator.SetIKRotation(goal, Quaternion.LookRotation(target.TransformDirection(Vector3.back), target.TransformDirection(Vector3.right * side)));
	}
}

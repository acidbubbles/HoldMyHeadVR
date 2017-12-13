using UnityEngine;

public class FeetController
{
	private const float LegSpread = 0.1f;
	private const float KneesStraightness = 1f;

	private readonly Animator _animator;

	public FeetController(Animator animator)
	{
		_animator = animator;
	}

	public void OnGround(Vector3 ground, Quaternion forward)
	{
		PositionFoot(
			AvatarIKGoal.LeftFoot,
			AvatarIKHint.LeftKnee,
			ground - new Vector3(-LegSpread, 0, 0),
			forward
		);

		PositionFoot(
			AvatarIKGoal.RightFoot,
			AvatarIKHint.RightKnee,
			ground - new Vector3(LegSpread, 0, 0),
			forward
		);
	}

	private void PositionFoot(AvatarIKGoal foot, AvatarIKHint knee, Vector3 position, Quaternion forward)
	{
		_animator.SetIKHintPositionWeight(knee, 1);
		_animator.SetIKHintPosition(knee, new Vector3(position.x, (_animator.bodyPosition.y - position.y) / 2, position.z + KneesStraightness));

		_animator.SetIKPositionWeight(foot, 1);
		_animator.SetIKPosition(foot, position);

		_animator.SetIKRotationWeight(foot, 1);
		_animator.SetIKRotation(foot, forward);
	}
}

using UnityEngine;

public class FeetController
{
	private const float LegSpread = 0.22f;
	private const float KneesStraightness = 50f;
	private const float GroundOffset = -.15f;

	private readonly ControllerSettings _settings;
	private readonly Animator _animator;
	private readonly Transform _ground;

	public FeetController(ControllerSettings settings, Animator animator, Transform ground)
	{
		_settings = settings;
		_animator = animator;
		_ground = ground;
	}

	public void OnGround(Quaternion forward)
	{
		if (!_settings.Enabled) return;

		PositionFoot(
			AvatarIKGoal.LeftFoot,
			AvatarIKHint.LeftKnee,
			_ground.position - new Vector3(-LegSpread, GroundOffset, 0),
			forward
		);

		PositionFoot(
			AvatarIKGoal.RightFoot,
			AvatarIKHint.RightKnee,
			_ground.position - new Vector3(LegSpread, GroundOffset, 0),
			forward
		);
	}

	private void PositionFoot(AvatarIKGoal foot, AvatarIKHint knee, Vector3 position, Quaternion forward)
	{
		_animator.SetIKHintPositionWeight(knee, 1);
		_animator.SetIKHintPosition(knee, new Vector3(position.x, (_animator.bodyPosition.y - position.y) / 2, _animator.bodyPosition.z - KneesStraightness));

		_animator.SetIKPositionWeight(foot, 1);
		_animator.SetIKPosition(foot, position);

		_animator.SetIKRotationWeight(foot, 1);
		_animator.SetIKRotation(foot, forward);
	}
}

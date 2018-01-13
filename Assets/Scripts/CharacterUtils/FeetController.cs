using UnityEngine;

public class FeetController
{
	private const float LegSpread = 0.22f;
	private const float KneeSpread = 2.7f;
	private const float KneesStraightness = 0.8f;
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

	public void Update()
	{
		if (!_settings.Enabled) return;

		PositionFoot(
			AvatarIKGoal.LeftFoot,
			AvatarIKHint.LeftKnee,
			_animator.rootPosition - _animator.rootRotation * new Vector3(LegSpread, GroundOffset, 0),
			_animator.rootRotation
		);

		PositionFoot(
			AvatarIKGoal.RightFoot,
			AvatarIKHint.RightKnee,
			_animator.rootPosition - _animator.rootRotation * new Vector3(-LegSpread, GroundOffset, 0),
			_animator.rootRotation
		);
	}

	private void PositionFoot(AvatarIKGoal foot, AvatarIKHint knee, Vector3 position, Quaternion forward)
	{
		_animator.SetIKHintPositionWeight(knee, 1);
		var hintPosition = _animator.bodyPosition + _animator.rootRotation * new Vector3((_animator.bodyPosition.x - position.x) / 2 * KneeSpread, (_animator.bodyPosition.y - position.y) / -2, KneesStraightness);
		_animator.SetIKHintPosition(knee, hintPosition);

		_animator.SetIKPositionWeight(foot, 1);
		_animator.SetIKPosition(foot, position);

		_animator.SetIKRotationWeight(foot, 1);
		_animator.SetIKRotation(foot, forward);
	}
}

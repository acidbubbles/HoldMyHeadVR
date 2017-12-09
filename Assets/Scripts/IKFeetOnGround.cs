using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKFeetOnGround : MonoBehaviour
{
	private const float LegSpread = 0.1f;
	private const float KneesStraightness = 1f;

	private Animator _animator;
	private Vector3 _targetLeftFootPosition;
	private Vector3 _targetRightFootPosition;

	public void Start()
	{
		_animator = GetComponent<Animator>();
	}

	public void OnAnimatorIK()
	{
		var ground = new Vector3(transform.position.x, 0, transform.position.z);
		_targetLeftFootPosition = ground - new Vector3(-LegSpread, 0, 0);
		_targetRightFootPosition = ground - new Vector3(LegSpread, 0, 0);

		PositionFoot(AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee, _targetLeftFootPosition);
		PositionFoot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, _targetRightFootPosition);
	}

	private void PositionFoot(AvatarIKGoal foot, AvatarIKHint knee, Vector3 position)
	{
		_animator.SetIKHintPositionWeight(knee, 1);
		_animator.SetIKHintPosition(knee, new Vector3(position.x, (_animator.bodyPosition.y - position.y) / 2, position.z + KneesStraightness));

		_animator.SetIKPositionWeight(foot, 1);
		_animator.SetIKPosition(foot, position);

		_animator.SetIKRotationWeight(foot, 1);
		_animator.SetIKRotation(foot, transform.rotation);
	}
}

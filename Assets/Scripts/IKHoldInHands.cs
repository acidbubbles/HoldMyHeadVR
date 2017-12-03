using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKHoldInHands : MonoBehaviour
{
	protected Animator animator;

	public bool ikActive = false;
	public Transform rightHandObj = null;
	public Transform leftHandObj = null;

	public void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void OnAnimatorIK()
	{
		if (!animator) return;
		if (!ikActive)
		{
			ResetHand(AvatarIKGoal.RightHand);
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
			return;
		}

		if (rightHandObj != null)
			PositionHand(AvatarIKGoal.RightHand, rightHandObj);

		if (rightHandObj != null)
			PositionHand(AvatarIKGoal.LeftHand, leftHandObj);
	}

	private void ResetHand(AvatarIKGoal goal)
	{
		animator.SetIKPositionWeight(goal, 0);
		animator.SetIKRotationWeight(goal, 0);
	}

	private void PositionHand(AvatarIKGoal goal, Transform target)
	{
		animator.SetIKPositionWeight(goal, 1);
		animator.SetIKRotationWeight(goal, 1);
		animator.SetIKPosition(goal, target.position);
		animator.SetIKRotation(goal, target.rotation);
	}
}

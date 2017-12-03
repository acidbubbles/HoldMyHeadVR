using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKLookAt : MonoBehaviour
{
	protected Animator animator;

	public bool ikActive = false;
	public Transform lookObj = null;
	public Transform leftEyeBone = null;
	public Transform rightEyeBone = null;

	public void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void OnAnimatorIK()
	{
		if (!animator) return;

		if (!ikActive)
		{
			animator.SetLookAtWeight(0);
			return;
		}

		if (lookObj != null)
		{
			animator.SetLookAtWeight(0.5f);
			animator.SetLookAtPosition(lookObj.position);

			if (leftEyeBone)
				LookEye(leftEyeBone);

			if (rightEyeBone)
				LookEye(rightEyeBone);
		}
	}

	private void LookEye(Transform eyeBone)
	{
		eyeBone.LookAt(lookObj);
	}
}

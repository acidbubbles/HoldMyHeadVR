using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKLookAt : MonoBehaviour
{
	protected Animator animator;

	public Transform lookObj = null;

	public void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void OnAnimatorIK()
	{
		if (!animator) return;

		if (lookObj != null)
		{
			animator.SetLookAtWeight(1f, 0.4f, 0.6f, 1f);
			animator.SetLookAtPosition(lookObj.position);
		}
	}
}

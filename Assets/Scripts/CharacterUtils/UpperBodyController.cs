using UnityEngine;

public class UpperBodyController
{
	private readonly Animator _animator;

	public UpperBodyController(Animator animator)
	{
		_animator = animator;
	}

	public void Look(Transform viewTarget, float humpUnit)
	{
		//TODO: Reduce body weight to 0 when you get near the body... otherwise the model avoids you!
		//TODO: Stop looking when out of reach (e.g. behind or too low)
		_animator.SetLookAtWeight(1f, 0.3f, 0.5f, 1f);
		_animator.SetLookAtPosition(viewTarget.transform.position);
	}
}

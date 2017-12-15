using UnityEngine;

public class UpperBodyController
{
	private readonly ControllerSettings _settings;
	private readonly Animator _animator;

	public UpperBodyController(ControllerSettings settings, Animator animator)
	{
		_settings = settings;
		_animator = animator;
	}

	public void Look(Transform viewTarget, float breatheUnit)
	{
		if (!_settings.enabled) return;

		//TODO: Reduce body weight to 0 when you get near the body... otherwise the model avoids you!
		//TODO: Stop looking when out of reach (e.g. behind or too low)
		_animator.SetLookAtWeight(1f, 0.2f + 0.1f * breatheUnit, 0.5f, 1f);
		_animator.SetLookAtPosition(viewTarget.transform.position);
	}
}

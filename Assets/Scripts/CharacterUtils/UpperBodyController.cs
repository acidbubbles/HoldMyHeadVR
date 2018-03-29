using UnityEngine;

public class UpperBodyController
{
	private const float FullBodyWeightDistance = 0.4f;
	private const float BaseBodyWeight = 0.35f;
	private const float BreatheBodyWeight = 0.05f;
	private const float HeadWeight = 0.5f;
	private readonly ControllerSettings _settings;
	private readonly Animator _animator;
	private readonly Transform _head;

	public UpperBodyController(ControllerSettings settings, Animator animator, Transform head)
	{
		_settings = settings;
		_animator = animator;
		_head = head;
	}

	public void Update(float breatheUnit)
	{
		if (!_settings.Enabled) return;

		//TODO: Stop looking when out of reach (e.g. behind or too low)
		var distance = Vector3.Distance(_animator.GetBoneTransform(HumanBodyBones.Head).position, _head.position);
		var strength = Mathf.Min(distance, FullBodyWeightDistance) / FullBodyWeightDistance;
		_animator.SetLookAtWeight(1f, BaseBodyWeight * strength + BreatheBodyWeight * breatheUnit, HeadWeight, 1f);
		_animator.SetLookAtPosition(_head.transform.position);
	}

	public void LateUpdate()
	{
	}
}

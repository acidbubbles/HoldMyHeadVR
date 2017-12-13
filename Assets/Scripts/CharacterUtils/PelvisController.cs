using UnityEngine;

public class PelvisController
{
	public float HumpUnit;

	private const float EyesToMouthDistance = 0.1f;
	private const float EyesToBellyMinDistance = 0.15f;
	private const float ReachDistance = 0.15f;
	private const float ReachDuration = 1.2f;
	private const float HumpSpeed = 5f;
	private const float HumpDistance = 0.2f;

	private readonly Animator _animator;
	private readonly Transform _ground;
	private readonly InverseKinematicsWeightHelper _reach;

	private Vector3 _initialBodyPosition;

	public PelvisController(Animator animator, Transform ground)
	{
		_animator = animator;
		_ground = ground;
		_reach = new InverseKinematicsWeightHelper();
	}

	public void OnHead(Transform head)
	{
		//TODO: Accelerate humping based on reach time

		if (_initialBodyPosition == Vector3.zero)
			_initialBodyPosition = _animator.bodyPosition;

		var adjustedInitialBodyPosition = _initialBodyPosition + new Vector3(0, _ground.position.y, 0);

		var target = head.position + Vector3.down * EyesToMouthDistance + Vector3.forward * EyesToBellyMinDistance;

		var withinReach = Vector3.Distance(adjustedInitialBodyPosition, target) < ReachDistance;
		var weight = _reach.GetWeight(withinReach, ReachDuration);

		var humpUnit = (Mathf.Sin(Time.time * HumpSpeed) + 1) / 2f;
		var humpDistance = HumpUnit * HumpDistance;
		target = new Vector3(target.x, Mathf.Min(adjustedInitialBodyPosition.y, target.y), target.z + humpDistance);

		_animator.bodyPosition = Vector3.Lerp(adjustedInitialBodyPosition, target, weight);

		HumpUnit = Mathf.SmoothStep(0f, humpUnit, weight);

		_animator.bodyRotation = _animator.rootRotation * Quaternion.AngleAxis(HumpUnit * 20f, Vector3.right);
	}
}

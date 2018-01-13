using System;
using UnityEngine;

[Serializable]
public class PelvisSettings : ControllerSettings
{
	public Vector3 HipsForward;
	public Vector3 HipsThrustCompensation;
	public float BodyThrustRotation = 20f;
	public float EyesToMouthDistance = 0.03f;
	public float EyesToBellyMinDistance = 0.18f;
}

public class PelvisController
{
	private const float ReachDistance = 0.35f;
	private const float ReachDuration = 1.2f;
	private const float HumpSpeed = 5f;
	private const float HumpDistance = 0.12f;
	private const float BodyUpWiggleRoom = 0.05f;

	private readonly PelvisSettings _settings;
	private readonly Animator _animator;
	private readonly Transform _head;
	private readonly Transform _ground;
	private readonly InverseKinematicsWeightHelper _reach;

	private Vector3 _initialBodyPosition;
	private Quaternion _initialBodyRotation;
	private Quaternion _initialHipsRotation;
	private bool _ready;

	public PelvisController(PelvisSettings settings, Animator animator, Transform head, Transform ground)
	{
		_settings = settings;
		_animator = animator;
		_head = head;
		_ground = ground;
		_reach = new InverseKinematicsWeightHelper();
	}

	public void Update(FrameContext context)
	{
		if (!_settings.Enabled) return;

		//TODO: Accelerate humping based on reach time
		//TODO: Introduce a delay when moving away, but make sure we don't go through when moving forward

		if (!_ready)
		{
			_initialBodyPosition = _animator.bodyPosition - new Vector3(0, BodyUpWiggleRoom, 0);
			_initialBodyRotation = _animator.bodyRotation;
			_initialHipsRotation = Quaternion.Euler(_settings.HipsForward);
			_ready = true;
		}

		var adjustedInitialBodyPosition = _initialBodyPosition + new Vector3(0, _ground.position.y, 0);
		var target = _head.position + _head.TransformDirection(Vector3.down) * _settings.EyesToMouthDistance;
		var withinReach = context.IsLookedAt && adjustedInitialBodyPosition.y + BodyUpWiggleRoom >= target.y && Vector3.Distance(adjustedInitialBodyPosition, target) < ReachDistance;
		var weight = _reach.GetWeight(withinReach, ReachDuration);

		if (weight <= 0)
		{
			_animator.SetBoneLocalRotation(HumanBodyBones.Hips, _initialHipsRotation);
			_animator.bodyPosition = _initialBodyPosition;
			_animator.bodyRotation = _initialBodyRotation;
			return;
		}

		var humpUnit = 1f - (Mathf.Sin(Time.time * HumpSpeed) + 1) / 2f;
		var humpDistance = humpUnit * HumpDistance;
		var weightedHumpUnit = humpUnit * weight;
		var targetBodyPosition = _head.position + _head.TransformDirection(Vector3.forward) * (_settings.EyesToBellyMinDistance + humpDistance) + _head.TransformDirection(Vector3.down) * _settings.EyesToMouthDistance;
		_animator.bodyPosition = Vector3.Slerp(_initialBodyPosition, targetBodyPosition, weight);
		var targetBodyRotation = Quaternion.LookRotation(-(_animator.bodyPosition - target)) * Quaternion.AngleAxis((weightedHumpUnit - 0.5f) * _settings.BodyThrustRotation, Vector3.right);
		_animator.bodyRotation = Quaternion.Slerp(_initialBodyRotation, targetBodyRotation, weight);

		// Compensate for body rotation
		_animator.SetBoneLocalRotation(HumanBodyBones.Hips, _initialHipsRotation * Quaternion.Euler(_settings.HipsThrustCompensation * weightedHumpUnit));
	}
}

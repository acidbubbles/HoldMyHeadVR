﻿using UnityEngine;

public class PelvisController
{
	private const float EyesToMouthDistance = 0.02f;
	private const float EyesToBellyMinDistance = 0.14f;
	private const float ReachDistance = 0.25f;
	private const float ReachDuration = 1.2f;
	private const float HumpSpeed = 4f;
	private const float HumpDistance = 0.12f;
	private const float BodyUpWiggleRoom = 0.1f;

	private readonly ControllerSettings _settings;
	private readonly Animator _animator;
	private readonly Transform _head;
	private readonly Transform _ground;
	private readonly InverseKinematicsWeightHelper _reach;

	private Vector3 _initialBodyPosition;

	public PelvisController(ControllerSettings settings, Animator animator, Transform head, Transform ground)
	{
		_settings = settings;
		_animator = animator;
		_head = head;
		_ground = ground;
		_reach = new InverseKinematicsWeightHelper();
	}

	public void OnHead()
	{
		if (!_settings.Enabled) return;

		//TODO: Broken for Triss
		//TODO: Figure out a better way to "aim" at the player's mouth
		//TODO: Accelerate humping based on reach time

		if (_initialBodyPosition == Vector3.zero)
			_initialBodyPosition = _animator.bodyPosition - new Vector3(0, BodyUpWiggleRoom, 0);

		var adjustedInitialBodyPosition = _initialBodyPosition + new Vector3(0, _ground.position.y, 0);

		var target = _head.position + _head.TransformDirection(Vector3.down) * EyesToMouthDistance;

		var withinReach = adjustedInitialBodyPosition.y + BodyUpWiggleRoom >= target.y && Vector3.Distance(adjustedInitialBodyPosition, target) < ReachDistance;
		var weight = _reach.GetWeight(withinReach, ReachDuration);

		var humpUnit = (Mathf.Sin(Time.time * HumpSpeed) + 1) / 2f;
		var humpDistance = humpUnit * HumpDistance;
		target = new Vector3(target.x, target.y, target.z + humpDistance) + _head.TransformDirection(Vector3.forward) * EyesToBellyMinDistance;
		// Avoid near clipping
		if (target.z < _head.position.z + 0.02f)
			target.z = _head.position.z + 0.02f;

		_animator.bodyPosition = Vector3.Lerp(adjustedInitialBodyPosition, target, weight);

		var weightedHumpUnit = Mathf.SmoothStep(0f, humpUnit, weight);

		_animator.bodyRotation = _animator.rootRotation * Quaternion.AngleAxis(weightedHumpUnit * 20f, Vector3.right);

		// Compensate for body rotation
		_animator.SetBoneLocalRotation(HumanBodyBones.Hips, Quaternion.Euler(0, -90f, -90f + weightedHumpUnit * 10f));
	}
}

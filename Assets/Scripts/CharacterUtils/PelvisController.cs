﻿using System;
using UnityEngine;

[Serializable]
public class PelvisSettings : ControllerSettings
{
	public Vector3 HipsForward;
	public Vector3 HipsThrustCompensation;
	public float BodyThrustRotation;
}

public class PelvisController
{
	private const float EyesToMouthDistance = 0.025f;
	private const float EyesToBellyMinDistance = 0.18f;
	private const float ReachDistance = 0.25f;
	private const float ReachDuration = 1.2f;
	private const float HumpSpeed = 4f;
	private const float HumpDistance = 0.12f;
	private const float BodyUpWiggleRoom = 0.05f;

	private readonly PelvisSettings _settings;
	private readonly Animator _animator;
	private readonly Transform _head;
	private readonly Transform _ground;
	private readonly InverseKinematicsWeightHelper _reach;

	private Vector3 _initialBodyPosition;
	private Quaternion _initialBodyRotation;
	private bool _ready;

	public PelvisController(PelvisSettings settings, Animator animator, Transform head, Transform ground)
	{
		_settings = settings;
		_animator = animator;
		_head = head;
		_ground = ground;
		_reach = new InverseKinematicsWeightHelper();
	}

	public void Update()
	{
		if (!_settings.Enabled) return;

		//TODO: Accelerate humping based on reach time
		//TODO: Introduce a delay when moving away, but make sure we don't go through when moving forward

		if (!_ready)
		{
			_initialBodyPosition = _animator.bodyPosition - new Vector3(0, BodyUpWiggleRoom, 0);
			_initialBodyRotation = _animator.bodyRotation;
			_ready = true;
		}

		var adjustedInitialBodyPosition = _initialBodyPosition + new Vector3(0, _ground.position.y, 0);
		var target = _head.position + _head.TransformDirection(Vector3.down) * EyesToMouthDistance;
		var withinReach = adjustedInitialBodyPosition.y + BodyUpWiggleRoom >= target.y && Vector3.Distance(adjustedInitialBodyPosition, target) < ReachDistance;
		var weight = _reach.GetWeight(withinReach, ReachDuration);

		var humpUnit = (Mathf.Sin(Time.time * HumpSpeed) + 1) / 2f;
		var humpDistance = humpUnit * HumpDistance;
		var weightedHumpUnit = humpUnit * weight;
		var targetBodyPosition = _head.position + _head.TransformDirection(Vector3.forward) * (EyesToBellyMinDistance + humpDistance) + _head.TransformDirection(Vector3.down) * EyesToMouthDistance;
		_animator.bodyPosition = Vector3.Slerp(_initialBodyPosition, targetBodyPosition, weight);
		var targetBodyRotation = Quaternion.LookRotation(-(_animator.bodyPosition - _head.position)) * Quaternion.AngleAxis((weightedHumpUnit - 0.5f) * _settings.BodyThrustRotation, Vector3.right);
		_animator.bodyRotation = Quaternion.Slerp(_initialBodyRotation, targetBodyRotation, weight);

		// Compensate for body rotation
		_animator.SetBoneLocalRotation(HumanBodyBones.Hips, Quaternion.Euler(_settings.HipsForward) * Quaternion.Euler(_settings.HipsThrustCompensation * weightedHumpUnit));
	}
}

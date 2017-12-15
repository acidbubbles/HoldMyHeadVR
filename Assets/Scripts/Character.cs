﻿using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
	private Animator _animator;
	private Transform _viewTarget;

	// Initialization State
	private SkinnedMeshRenderer _skinnedMeshRenderer;
	private float _defaultTopLidValue;
	private float _defaultBottomLidValue;

	// Current State
	private float _nextBlink;

	// Controllers
	private BlinkController _blinkController;
	private FeetController _feetController;
	private HandsController _handsController;
	private PelvisController _pelvisController;
	private UpperBodyController _upperBodyController;

	// ReSharper disable InconsistentNaming
	[Header("Controllers")]
	public ControllerSettings pelvis;
	public ControllerSettings upperBody;
	public ControllerSettings hands;
	public ControllerSettings feet;
	public ControllerSettings breathing;
	public ControllerSettings blink;

	[Header("Eyes Control")]
	public int topLidIndex;
	public int bottomLidIndex;
	public float topLidCloseValue;
	public float bottomLidCloseValue;

	[Header("Breathing")]
	public BlendShapeEntry[] BreathingBlendShapes;

	private BreathingController _breathingController;
	// ReSharper restore InconsistentNaming

	public void Awake()
	{
		_animator = GetComponent<Animator>();
		if(_animator == null) throw new NullReferenceException("An Animator is required");

		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if(mainCamera == null) throw new NullReferenceException("A Camera with tag MainCamera is required");
		_viewTarget = mainCamera.transform;

		_skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>().First(x => x.sharedMesh.blendShapeCount > 0);
		if(_skinnedMeshRenderer == null) throw new NullReferenceException("A child component with a SkinnedMeshRenderer that contains at least one blend shape is required");

		var ground = GameObject.FindGameObjectWithTag("Ground");
		if(ground == null) throw new NullReferenceException("A GameObject with tag Ground is required");

		_pelvisController = new PelvisController(pelvis, _animator, ground.transform);
		_feetController = new FeetController(feet, _animator, ground.transform);
		_handsController = new HandsController(hands, _animator);
		_upperBodyController = new UpperBodyController(upperBody, _animator);
		_breathingController = new BreathingController(breathing, _skinnedMeshRenderer, BreathingBlendShapes);
		_blinkController = new BlinkController(blink);
	}

	public void Start()
	{
		_defaultTopLidValue = _skinnedMeshRenderer.GetBlendShapeWeight(topLidIndex);
		_defaultBottomLidValue = _skinnedMeshRenderer.GetBlendShapeWeight(bottomLidIndex);
	}

	public void OnAnimatorIK()
	{
		// OTHER TODOS
		//TODO: Close top lid when looking down
		//TODO: Tilt head when player's head come close, and close eyes (kissing), potentially blendshapes
		//TODO: Move the podium up or down
		//TODO: Reflections
		//TODO: Switch lighting
		//TODO: Switch character
		//TODO: See hands
		//TODO: * Contact against boobs
		//TODO: * Hold hands

		_pelvisController.OnHead(_viewTarget);
		_handsController.OnHead(_viewTarget);
		_feetController.OnGround(transform.rotation);
		_upperBodyController.Look(_viewTarget, _breathingController.BreatheUnit);
	}

	public void LateUpdate()
	{
		_breathingController.Breathe();

		if (_nextBlink < Time.time)
		{
			_blinkController.Blink();
			_nextBlink = Time.time + UnityEngine.Random.Range(0.8f, 8f);
		}

		var result = _blinkController.Update(_defaultTopLidValue, topLidCloseValue, _defaultBottomLidValue, bottomLidCloseValue);
		if (result.Active)
		{
			_skinnedMeshRenderer.SetBlendShapeWeight(topLidIndex, result.TopLid);
			_skinnedMeshRenderer.SetBlendShapeWeight(bottomLidIndex, result.BottomLid);
		}
	}
}

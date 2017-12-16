﻿using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
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
	public BreathingSettings breathing;
	public BlinkSettings blink;
	private BreathingController _breathingController;
	// ReSharper restore InconsistentNaming

	public void Awake()
	{
		var animator = GetComponent<Animator>();
		if(animator == null) throw new NullReferenceException("An Animator is required");

		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if(mainCamera == null) throw new NullReferenceException("A Camera with tag MainCamera is required");
		var viewTarget = mainCamera.transform;

		var skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>().First(x => x.sharedMesh.blendShapeCount > 0);
		if(skinnedMeshRenderer == null) throw new NullReferenceException("A child component with a SkinnedMeshRenderer that contains at least one blend shape is required");

		var ground = GameObject.FindGameObjectWithTag("Ground");
		if(ground == null) throw new NullReferenceException("A GameObject with tag Ground is required");

		_pelvisController = new PelvisController(pelvis, animator, viewTarget.transform, ground.transform);
		_feetController = new FeetController(feet, animator, ground.transform);
		_handsController = new HandsController(hands, animator, viewTarget);
		_upperBodyController = new UpperBodyController(upperBody, animator, viewTarget);
		_breathingController = new BreathingController(breathing, skinnedMeshRenderer);
		_blinkController = new BlinkController(blink, skinnedMeshRenderer);
	}

	public void Start()
	{
		_blinkController.Start();
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

		_pelvisController.OnHead();
		_handsController.OnHead();
		_feetController.OnGround(transform.rotation);
		_upperBodyController.Look(_breathingController.BreatheUnit);
	}

	public void LateUpdate()
	{
		_breathingController.Update();
		_blinkController.Update();
	}
}

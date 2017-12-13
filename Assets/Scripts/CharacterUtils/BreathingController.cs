﻿using UnityEngine;

public class BreathingController
{
	public float BreatheUnit;

	private readonly SkinnedMeshRenderer _skinnedMeshRenderer;
	private readonly BlendShapeEntry[] _breathingBlendShapes;
	private const float BreathingSpeed = 2f;

	public BreathingController(SkinnedMeshRenderer skinnedMeshRenderer, BlendShapeEntry[] breathingBlendShapes)
	{
		_skinnedMeshRenderer = skinnedMeshRenderer;
		_breathingBlendShapes = breathingBlendShapes;
	}

	public void Breathe()
	{
		var breatheUnit = (Mathf.Sin(Time.time * BreathingSpeed) + 1) / 2f;
		foreach (var blendShape in _breathingBlendShapes)
		{
			_skinnedMeshRenderer.SetBlendShapeWeight(blendShape.index, blendShape.value * breatheUnit);
		}
		BreatheUnit = breatheUnit;
	}
}
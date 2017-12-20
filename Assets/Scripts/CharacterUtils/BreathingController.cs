using System;
using UnityEngine;

[Serializable]
public class BreathingSettings : ControllerSettings
{
	public BlendShapeEntry[] BreathingBlendShapes;
}

public class BreathingController
{
	public float BreatheUnit;

	private readonly BreathingSettings _settings;
	private readonly SkinnedMeshRenderer _skinnedMeshRenderer;
	private readonly BlendShapeEntry[] _breathingBlendShapes;
	private const float BreathingSpeed = 2f;

	public BreathingController(BreathingSettings settings, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		_settings = settings;
		_skinnedMeshRenderer = skinnedMeshRenderer;
	}

	public void LateUpdate()
	{
		if (!_settings.Enabled) return;

		var breatheUnit = (Mathf.Sin(Time.time * BreathingSpeed) + 1) / 2f;
		foreach (var blendShape in _settings.BreathingBlendShapes)
		{
			_skinnedMeshRenderer.SetBlendShapeWeight(blendShape.index, blendShape.value * breatheUnit);
		}
		BreatheUnit = breatheUnit;
	}
}

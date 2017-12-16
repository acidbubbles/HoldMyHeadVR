using System;
using UnityEngine;

[Serializable]
public class BlinkSettings : ControllerSettings
{
	public int TopLidIndex;
	public int BottomLidIndex;
	public float TopLidCloseValue;
	public float BottomLidCloseValue;
}

public class BlinkController
{
	public enum BlinkState
	{
		Inactive,
		Closing,
		Waiting,
		Opening,
	}

	public struct BlinkResult
	{
		public bool Active;
		public float TopLid;
		public float BottomLid;
	}

	private const float BlinkCloseDuration = 0.05f;
	private const float WaitDuration = 0.05f;
	private const float BlinkOpenDuration = 0.1f;

	private readonly BlinkSettings _settings;
	private readonly SkinnedMeshRenderer _skinnedMeshRenderer;

	private float _topLidOpenedValue;
	private float _bottomLidOpenedValue;

	private float _nextBlink;
	private float _lastEventTime;
	private BlinkState _state = BlinkState.Inactive;

	public BlinkController(BlinkSettings settings, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		_settings = settings;
		_skinnedMeshRenderer = skinnedMeshRenderer;
	}

	public void Start()
	{
		_topLidOpenedValue = _skinnedMeshRenderer.GetBlendShapeWeight(_settings.TopLidIndex);
		_bottomLidOpenedValue = _skinnedMeshRenderer.GetBlendShapeWeight(_settings.BottomLidIndex);
	}

	public void Update()
	{
		if (!_settings.Enabled) return;

		if (_nextBlink < Time.time)
		{
			StartBlink();
			_nextBlink = Time.time + UnityEngine.Random.Range(0.8f, 8f);
		}

		var result = UpdateBlink();
		if (result.Active)
		{
			_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, result.TopLid);
			_skinnedMeshRenderer.SetBlendShapeWeight(_settings.BottomLidIndex, result.BottomLid);
		}
	}

	private void StartBlink()
	{
		_lastEventTime = Time.time;
		_state = BlinkState.Closing;
	}

	private BlinkResult UpdateBlink()
	{
		switch (_state)
		{
			case BlinkState.Closing:
				return Closing();
			case BlinkState.Waiting:
				return Waiting();
			case BlinkState.Opening:
				return Opening();
			default:
				return new BlinkResult { Active = false };
		}
	}

	private BlinkResult Closing()
	{
		var topLidValue = Mathf.SmoothStep(_topLidOpenedValue, _settings.TopLidCloseValue, (Time.time - _lastEventTime) / BlinkCloseDuration);
		var bottomLidValue = Mathf.SmoothStep(_bottomLidOpenedValue, _settings.BottomLidCloseValue, (Time.time - _lastEventTime) / BlinkCloseDuration);

		if (Math.Abs(topLidValue - _settings.TopLidCloseValue) < 0.01f)
		{
			_lastEventTime = Time.time;
			_state = BlinkState.Waiting;
		}

		return new BlinkResult
		{
			Active = true,
			TopLid = topLidValue,
			BottomLid = bottomLidValue
		};
	}

	private BlinkResult Waiting()
	{
		if (_lastEventTime + WaitDuration >= Time.time)
		{
			_lastEventTime = Time.time;
			_state = BlinkState.Opening;
		}

		return new BlinkResult
		{
			Active = true,
			TopLid = _settings.TopLidCloseValue,
			BottomLid = _settings.BottomLidCloseValue
		};
	}

	private BlinkResult Opening()
	{
		var topLidValue = Mathf.SmoothStep(_settings.TopLidCloseValue, _topLidOpenedValue, (Time.time - _lastEventTime) / BlinkOpenDuration);
		var bottomLidValue = Mathf.SmoothStep(_settings.BottomLidCloseValue, _bottomLidOpenedValue, (Time.time - _lastEventTime) / BlinkOpenDuration);

		if (Math.Abs(topLidValue - _topLidOpenedValue) < 0.01f)
		{
			_state = BlinkState.Inactive;
		}

		return new BlinkResult
		{
			Active = true,
			TopLid = topLidValue,
			BottomLid = bottomLidValue
		};
	}
}

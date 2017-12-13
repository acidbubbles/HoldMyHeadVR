using System;
using UnityEngine;

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

	private float _lastEventTime;
	private BlinkState _state = BlinkState.Inactive;

	public void Blink()
	{
		_lastEventTime = Time.time;
		_state = BlinkState.Closing;
	}

	public BlinkResult Update(float topLidOpened, float topLidClosed, float bottomLidOpened, float bottomLidClosed)
	{
		switch (_state)
		{
			case BlinkState.Closing:
				return Closing(topLidOpened, topLidClosed, bottomLidOpened, bottomLidClosed);
			case BlinkState.Waiting:
				return Waiting(topLidClosed, bottomLidClosed);
			case BlinkState.Opening:
				return Opening(topLidOpened, topLidClosed, bottomLidOpened, bottomLidClosed);
			default:
				return new BlinkResult { Active = false };
		}
	}

	private BlinkResult Closing(float topLidOpened, float topLidClosed, float bottomLidOpened, float bottomLidClosed)
	{
		var topLidValue = Mathf.SmoothStep(topLidOpened, topLidClosed, (Time.time - _lastEventTime) / BlinkCloseDuration);
		var bottomLidValue = Mathf.SmoothStep(bottomLidOpened, bottomLidClosed, (Time.time - _lastEventTime) / BlinkCloseDuration);

		if (Math.Abs(topLidValue - topLidClosed) < 0.01f)
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

	private BlinkResult Waiting(float topLidClosed, float bottomLidClosed)
	{
		if (_lastEventTime + WaitDuration >= Time.time)
		{
			_lastEventTime = Time.time;
			_state = BlinkState.Opening;
		}

		return new BlinkResult
		{
			Active = true,
			TopLid = topLidClosed,
			BottomLid = bottomLidClosed
		};
	}

	private BlinkResult Opening(float topLidOpened, float topLidClosed, float bottomLidOpened, float bottomLidClosed)
	{
		var topLidValue = Mathf.SmoothStep(topLidClosed, topLidOpened, (Time.time - _lastEventTime) / BlinkOpenDuration);
		var bottomLidValue = Mathf.SmoothStep(bottomLidClosed, bottomLidOpened, (Time.time - _lastEventTime) / BlinkOpenDuration);

		if (Math.Abs(topLidValue - topLidOpened) < 0.01f)
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

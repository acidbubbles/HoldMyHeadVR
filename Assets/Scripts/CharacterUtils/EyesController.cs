using System;
using UnityEngine;

[Serializable]
public class EyesSettings : ControllerSettings
{
	public int TopLidIndex;
	public int BottomLidIndex;
	public float TopLidCloseValue;
	public float BottomLidCloseValue;
	public float LeftEyeForwardEuleurX;
}

public class EyesController
{
	public enum State
	{
		Inactive,
		BlinkClosing,
		BlinkWaiting,
		BlinkOpening,
		LookingDown,
		LookingWait,
		LookingUp
	}

	private const float MinTimeBetweenEvents = 0.8f;
	private const float MaxTimeBetweenEvents = 8f;
	private const float BlinkClosingDuration = 0.05f;
	private const float BlinkWaitingDuration = 0.05f;
	private const float BlinkOpeningDuration = 0.1f;
	private const float LookingDownAngle = 6f;
	private const float LookingDownDuration = 0.3f;
	private const float LookingWaitDuration = 0.5f;
	private const float LookingUpDuration = 0.1f;
	private const int LookDownToBlinkRatio = 4;
	private const float KissDistance = 0.22f;

	private readonly EyesSettings _settings;
	private readonly Transform _head;
	private readonly Animator _animator;
	private readonly SkinnedMeshRenderer _skinnedMeshRenderer;

	private float _topLidOpenedValue;
	private float _bottomLidOpenedValue;
	private float _topLidFullOpenedValue;
	private float _bottomLidFullOpenedValue;

	private float _nextStartTime;
	private float _lastEventTime;
	private State _state = State.Inactive;

	public EyesController(EyesSettings settings, Transform head, Animator animator,
		SkinnedMeshRenderer skinnedMeshRenderer)
	{
		_settings = settings;
		_head = head;
		_animator = animator;
		_skinnedMeshRenderer = skinnedMeshRenderer;
	}

	public void Start()
	{
		_topLidOpenedValue = _topLidFullOpenedValue = _skinnedMeshRenderer.GetBlendShapeWeight(_settings.TopLidIndex);
		_bottomLidOpenedValue = _bottomLidFullOpenedValue = _skinnedMeshRenderer.GetBlendShapeWeight(_settings.BottomLidIndex);
	}

	public void LateUpdate()
	{
		if (!_settings.Enabled) return;

		switch (_state)
		{
			case State.Inactive:
				LidsAdjust();
				Inactive();
				break;
			case State.BlinkClosing:
				LidsAdjust();
				BlinkClosing();
				break;
			case State.BlinkWaiting:
				LidsAdjust();
				BlinkWaiting();
				break;
			case State.BlinkOpening:
				LidsAdjust();
				BlinkOpening();
				break;
			case State.LookingDown:
				LookingDown();
				LidsAdjust();
				break;
			case State.LookingWait:
				LookingWait();
				LidsAdjust();
				break;
			case State.LookingUp:
				LookingUp();
				LidsAdjust();
				break;
		}
	}

	private void Inactive()
	{
		var time = Time.time;
		if (_nextStartTime > time) return;

		_lastEventTime = time;
		_nextStartTime = time + UnityEngine.Random.Range(MinTimeBetweenEvents, MaxTimeBetweenEvents);
		_state = UnityEngine.Random.Range(0, LookDownToBlinkRatio) < 1
			? State.LookingDown
			: State.BlinkClosing;
	}

	private void LidsAdjust()
	{
		var distance = Vector3.Distance(_animator.GetBoneTransform(HumanBodyBones.Head).position, _head.position);
		if (distance <= KissDistance)
		{
			//TODO: Smooth this out
			_topLidOpenedValue = _settings.TopLidCloseValue;
			_bottomLidOpenedValue = _settings.BottomLidCloseValue;
			_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, _settings.TopLidCloseValue);
			_skinnedMeshRenderer.SetBlendShapeWeight(_settings.BottomLidIndex, _settings.BottomLidCloseValue);
		}
		else
		{
			LidsAdjustToLook();
		}
	}

	private void LidsAdjustToLook()
	{
		const float maxAngleDiff = 20f;
		const float maxLidClose = 0.6f;
		var leftEyeLookAngle = _animator.GetBoneTransform(HumanBodyBones.LeftEye).localEulerAngles.x;
		leftEyeLookAngle = (leftEyeLookAngle + _settings.LeftEyeForwardEuleurX) % 360;
		if (leftEyeLookAngle > 180) leftEyeLookAngle = leftEyeLookAngle - 360;

		var ratio = Math.Min(Math.Abs(leftEyeLookAngle), maxAngleDiff) / maxAngleDiff;
		_topLidOpenedValue = _topLidFullOpenedValue + (_settings.TopLidCloseValue - _topLidFullOpenedValue) * (maxLidClose * ratio);
		_bottomLidOpenedValue = _bottomLidFullOpenedValue;

		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, _topLidOpenedValue);
		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, _bottomLidOpenedValue);
	}

	private void BlinkClosing()
	{
		var t = T(BlinkClosingDuration);
		var topLidValue = Mathf.SmoothStep(_topLidOpenedValue, _settings.TopLidCloseValue, t);
		var bottomLidValue = Mathf.SmoothStep(_bottomLidOpenedValue, _settings.BottomLidCloseValue, t);

		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, topLidValue);
		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.BottomLidIndex, bottomLidValue);

		UpdateStateTiming(BlinkClosingDuration, State.BlinkWaiting);
	}

	private void BlinkWaiting()
	{
		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, _settings.TopLidCloseValue);
		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.BottomLidIndex, _settings.BottomLidCloseValue);

		UpdateStateTiming(BlinkWaitingDuration, State.BlinkOpening);
	}

	private void BlinkOpening()
	{
		var t = T(BlinkOpeningDuration);
		var topLidValue = Mathf.SmoothStep(_settings.TopLidCloseValue, _topLidOpenedValue, t);
		var bottomLidValue = Mathf.SmoothStep(_settings.BottomLidCloseValue, _bottomLidOpenedValue, t);

		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.TopLidIndex, topLidValue);
		_skinnedMeshRenderer.SetBlendShapeWeight(_settings.BottomLidIndex, bottomLidValue);

		UpdateStateTiming(BlinkOpeningDuration, State.Inactive);
	}

	private void LookingDown()
	{
		var t = T(LookingDownDuration);
		var lookDownAngle = Mathf.SmoothStep(0, LookingDownAngle, t);

		_animator.GetBoneTransform(HumanBodyBones.LeftEye).Rotate(lookDownAngle, 0, 0);
		_animator.GetBoneTransform(HumanBodyBones.RightEye).Rotate(lookDownAngle, 0, 0);

		UpdateStateTiming(LookingDownDuration, State.LookingWait);
	}

	private void LookingWait()
	{
		_animator.GetBoneTransform(HumanBodyBones.LeftEye).Rotate(LookingDownAngle, 0, 0);
		_animator.GetBoneTransform(HumanBodyBones.RightEye).Rotate(LookingDownAngle, 0, 0);

		UpdateStateTiming(LookingWaitDuration, State.LookingUp);
	}

	private void LookingUp()
	{
		var t = T(LookingUpDuration);
		var lookDownAngle = Mathf.SmoothStep(LookingDownAngle, 0, t);

		_animator.GetBoneTransform(HumanBodyBones.LeftEye).Rotate(lookDownAngle, 0, 0);
		_animator.GetBoneTransform(HumanBodyBones.RightEye).Rotate(lookDownAngle, 0, 0);

		UpdateStateTiming(LookingUpDuration, State.Inactive);
	}

	private float T(float duration)
	{
		return (Time.time - _lastEventTime) / duration;
	}

	private void UpdateStateTiming(float duration, State nextState)
	{
		var time = Time.time;
		if (_lastEventTime + duration >= time) return;

		_lastEventTime = time;
		_state = nextState;
	}
}

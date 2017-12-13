using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
	private const float LegSpread = 0.1f;
	private const float KneesStraightness = 1f;

	private Animator _animator;
	private Transform _viewTarget;

	private SkinnedMeshRenderer _skinnedMeshRenderer;
	private float _defaultTopLidValue;
	private float _defaultBottomLidValue;

	public int topLidIndex;
	public int bottomLidIndex;
	public float topLidCloseValue;
	public float bottomLidCloseValue;

	public void Start()
	{
		// COMMON

		_animator = GetComponent<Animator>();
		if(_animator == null) throw new NullReferenceException("An Animator is required");

		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if(mainCamera == null) throw new NullReferenceException("A Camera with tag MainCamera is required");
		_viewTarget = mainCamera.transform;

		// BLINK

		_skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>().First(x => x.sharedMesh.blendShapeCount > 0);
		if(_skinnedMeshRenderer == null) throw new NullReferenceException("A child component with a SkinnedMeshRenderer that contains at least one blend shape is required");
		_defaultTopLidValue = _skinnedMeshRenderer.GetBlendShapeWeight(topLidIndex);
		_defaultBottomLidValue = _skinnedMeshRenderer.GetBlendShapeWeight(bottomLidIndex);
	}

	public void OnAnimatorIK()
	{
		if (!_animator || !_viewTarget) return;

		// OTHER TODOS
		//TODO: Close top lid when looking down
		//TODO: Tilt head when player's head come close, and close eyes (kissing), potentially blendshapes
		//TODO: Hump player's face when coming close to hips (use bodyPosition)
		//TODO: Move the podium up or down
		//TODO: Reflections
		//TODO: Switch lighting
		//TODO: Switch character
		//TODO: See hands
		//TODO: * Contact against boobs
		//TODO: * Hold hands

		// LOOKING AT PLAYER

		//TODO: Reduce body weight to 0 when you get near the body... otherwise the model avoids you!
		//TODO: Stop looking when out of reach (e.g. behind or too low)
		_animator.SetLookAtWeight(1f, 0.3f, 0.5f, 1f);
		_animator.SetLookAtPosition(_viewTarget.transform.position);

		// HOLD PLAYER HEAD

		PositionHand(AvatarIKGoal.RightHand, _viewTarget, -1);
		PositionHand(AvatarIKGoal.LeftHand, _viewTarget, 1);

		// FEET ON GROUND

		var ground = new Vector3(transform.position.x, 0, transform.position.z);
		var targetLeftFootPosition = ground - new Vector3(-LegSpread, 0, 0);
		var targetRightFootPosition = ground - new Vector3(LegSpread, 0, 0);

		PositionFoot(AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee, targetLeftFootPosition);
		PositionFoot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, targetRightFootPosition);
	}

	private const float BlinkDuration = 0.1f;
	private float _nextBlink;
	private float _currentBlink;
	private bool _closing;
	private bool _opening;

	public void LateUpdate()
	{
		if (_nextBlink < Time.time)
		{
			_currentBlink = Time.time;
			_nextBlink = Time.time + UnityEngine.Random.Range(0.8f, 8f);
			_closing = true;
		}

		if (_closing)
		{
			var topLidValue = Mathf.SmoothStep(_defaultTopLidValue, topLidCloseValue, (Time.time - _currentBlink) / BlinkDuration);
			_skinnedMeshRenderer.SetBlendShapeWeight(topLidIndex, topLidValue);

			var bottomLidValue = Mathf.SmoothStep(_defaultBottomLidValue, bottomLidCloseValue, (Time.time - _currentBlink) / BlinkDuration);
			_skinnedMeshRenderer.SetBlendShapeWeight(bottomLidIndex, bottomLidValue);

			if (Math.Abs(topLidValue - topLidCloseValue) < 0.1f)
			{
				_currentBlink = Time.time;
				_closing = false;
				_opening = true;
			}
		} else if (_opening)
		{
			var topLidValue = Mathf.SmoothStep(topLidCloseValue, _defaultTopLidValue, (Time.time - _currentBlink) / BlinkDuration);
			_skinnedMeshRenderer.SetBlendShapeWeight(topLidIndex, topLidValue);

			var bottomLidValue = Mathf.SmoothStep(bottomLidCloseValue, _defaultBottomLidValue, (Time.time - _currentBlink) / BlinkDuration);
			_skinnedMeshRenderer.SetBlendShapeWeight(bottomLidIndex, bottomLidValue);

			if (Math.Abs(topLidValue - _defaultTopLidValue) < 0.1f)
			{
				_opening = false;
			}
		}
	}

	private void PositionHand(AvatarIKGoal goal, Transform target, float side)
	{
		_animator.SetIKPositionWeight(goal, 1);
		_animator.SetIKPosition(goal, target.TransformPoint(new Vector3(.1f * side, -0.02f, 0.03f)));

		_animator.SetIKRotationWeight(goal, 1);
		_animator.SetIKRotation(goal, Quaternion.LookRotation(target.TransformDirection(Vector3.back), target.TransformDirection(Vector3.right * side)));
	}

	private void PositionFoot(AvatarIKGoal foot, AvatarIKHint knee, Vector3 position)
	{
		_animator.SetIKHintPositionWeight(knee, 1);
		_animator.SetIKHintPosition(knee, new Vector3(position.x, (_animator.bodyPosition.y - position.y) / 2, position.z + KneesStraightness));

		_animator.SetIKPositionWeight(foot, 1);
		_animator.SetIKPosition(foot, position);

		_animator.SetIKRotationWeight(foot, 1);
		_animator.SetIKRotation(foot, transform.rotation);
	}
}

using UnityEngine;

public class InverseKinematicsWeightHelper
{
	private bool _snap;
	private float _lastEventTime;
	private float _lastWeight;
	private float _startWeight;

	public float GetWeight(bool withinReach, float duration)
	{
		if (withinReach & !_snap)
		{
			_snap = true;
			_lastEventTime = Time.time;
			_startWeight = _lastWeight;
		} else if (!withinReach & _snap)
		{
			_snap = false;
			_lastEventTime = Time.time;
			_startWeight = _lastWeight;
		}

		_lastWeight = Mathf.SmoothStep(_startWeight, _snap ? 1f : 0f, (Time.time - _lastEventTime) / duration);

		return _lastWeight;
	}
}
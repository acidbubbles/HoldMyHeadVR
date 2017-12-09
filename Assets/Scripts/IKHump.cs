using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKHump : MonoBehaviour
{
	protected Animator _animator;

	public void Start()
	{
		_animator = GetComponent<Animator>();
	}

	public void OnAnimatorIK()
	{
		if (!_animator) return;

		//TODO: When getting near, aim hips towards face
		//TODO: When getting near enough, start humping slowly, and get faster
		//TODO: If the face gets away, pull out quickly
		//TODO: Never get too near
		//TODO: We want to move the body position, not the hips
		//_animator.SetBoneLocalRotation(
		//	HumanBodyBones.Hips,
		//	rotation
		//);
	}
}

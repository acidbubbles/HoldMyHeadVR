using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
	private const float IsLookedAtAngleRange = 20f;

	// Controllers
	private bool _ready;
	private EyesController _eyesController;
	private FeetController _feetController;
	private HandsController _handsController;
	private PelvisController _pelvisController;
	private UpperBodyController _upperBodyController;
	private BreathingController _breathingController;
	private MouthAudioSource.Ticket _mouthAudioSourceTicket;

	// Reusable objects
	private Animator _animator;
	private Transform _viewTarget;

	// ReSharper disable InconsistentNaming
	[Header("Controllers")]
	public PelvisSettings pelvis;
	public ControllerSettings upperBody;
	public HandsSettings hands;
	public ControllerSettings feet;
	public BreathingSettings breathing;
	public EyesSettings eyes;
	[Header("Targets")]
	public AudioSource mouthAudioSource;
	// ReSharper restore InconsistentNaming

	public void Start()
	{
		_animator = GetComponent<Animator>();
		if(_animator == null) throw new NullReferenceException("An Animator is required");

		var mainCamera = Camera.main;
		if(mainCamera == null) throw new NullReferenceException("A main camera is required");
		_viewTarget = mainCamera.transform;

		var skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(x => x.sharedMesh.blendShapeCount > 0);

		var ground = GameObject.FindGameObjectWithTag("Ground");
		if(ground == null) throw new NullReferenceException("A GameObject with tag Ground is required");

		_pelvisController = new PelvisController(pelvis, _animator, _viewTarget.transform, ground.transform);
		_feetController = new FeetController(feet, _animator, ground.transform);
		_handsController = new HandsController(hands, _animator, _viewTarget);
		_upperBodyController = new UpperBodyController(upperBody, _animator, _viewTarget);
		_breathingController = new BreathingController(breathing, skinnedMeshRenderer);
		_eyesController = new EyesController(eyes, _viewTarget, _animator, skinnedMeshRenderer);

		_eyesController.Start();

		_mouthAudioSourceTicket = mouthAudioSource.GetComponent<MouthAudioSource>().GetTicket();

		_ready = true;
	}

	public void OnAnimatorIK()
	{
		if (!_ready) return;

		// OTHER TODOS
		//TODO: Tilt head when player's head come close, and close eyes (kissing, partially done), potentially blendshapes
		//TODO: Tilt down more when the player is low
		//TODO: Smoother humping and acceleration
		//TODO: Add some variety: Different breathing speed, different body bending, different faces
		//TODO: Move the podium up or down
		//TODO: Reflections
		//TODO: Switch lighting
		//TODO: Switch character
		//TODO: See hands
		//TODO: * Contact against boobs
		//TODO: * Hold hands

		var context = new FrameContext
		{
			IsLookedAt = ComputeIsLookedAt()
		};

		_pelvisController.Update(context);
		_handsController.Update(context);
		_feetController.Update();
		_upperBodyController.Update(_breathingController.BreatheUnit);
	}

	private bool ComputeIsLookedAt()
	{
		var viewtargetGroundPosition = new Vector3(_viewTarget.position.x, 0, _viewTarget.position.z);
		var animatorGroundPosition = new Vector3(_animator.rootPosition.x, 0, _animator.rootPosition.z);
		var desiredDirection = Vector3.Angle(Vector3.forward, animatorGroundPosition - viewtargetGroundPosition);
		desiredDirection = desiredDirection * -Mathf.Sign(Vector3.Cross(animatorGroundPosition, viewtargetGroundPosition).y);
		var cameraDirection = _viewTarget.rotation.eulerAngles.y;
		var difference = Mathf.DeltaAngle(cameraDirection, desiredDirection);
		return Math.Abs(difference) < IsLookedAtAngleRange;
	}

	public void LateUpdate()
	{
		if (!_ready) return;

		_breathingController.LateUpdate();
		_eyesController.LateUpdate();
		_upperBodyController.LateUpdate();

		_mouthAudioSourceTicket.Update(_pelvisController.IsHumping);
	}
}
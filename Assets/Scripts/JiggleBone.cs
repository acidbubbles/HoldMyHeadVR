// ReSharper disable All
//	============================================================
//	Name:		Jiggle Bone v.1.0
//	Author: 	Michael Cook (Fishypants)
//	Date:		9-25-2011
//	License:	Free to use. Any credit would be nice :)
//
//	To Use:
// 		Drag this script onto a bone. (ideally bones at the end)
//		Set the boneForwardAxis to be the front facing axis of the bone.
//		Done! Now you have bones with jiggle dynamics.
//
//	============================================================

using UnityEngine;

public class JiggleBone : MonoBehaviour
{
	public bool debugMode = true;

	// Target and dynamic positions
	Vector3 lastPos = new Vector3();

	// Bone settings
	public bool reverseBone = false;
	public Vector3 boneForwardAxis = new Vector3(0, 0, 1);
	public Vector3 boneUpAxis = new Vector3(0, 1, 0);
	public float targetDistance = 2.0f;

	// Dynamics settings
	public float bStiffness = 0.1f;
	public float bMass = 0.9f;
	public float bDamping = 0.75f;
	public float bGravity = 0.75f;

	// Dynamics variables
	Vector3 force = new Vector3();
	Vector3 acc = new Vector3();
	Vector3 vel = new Vector3();

	// Squash and stretch variables
	public bool SquashAndStretch = true;
	public float sideStretch = 0.15f;
	public float frontStretch = 0.2f;

	void Awake()
	{
		// Set targetPos and lastPos at startup
		lastPos = transform.position + transform.TransformDirection(new Vector3((boneForwardAxis.x * targetDistance), (boneForwardAxis.y * targetDistance), (boneForwardAxis.z * targetDistance)));
	}

	void LateUpdate()
	{
		// Reset the bone rotation so we can recalculate the upVector and forwardVector
		transform.rotation = new Quaternion();

		// Update forwardVector and upVector
		Vector3 forwardVector = transform.TransformDirection(new Vector3((boneForwardAxis.x * targetDistance), (boneForwardAxis.y * targetDistance), (boneForwardAxis.z * targetDistance)));
		Vector3 upVector = transform.TransformDirection(boneUpAxis);

		// Calculate target position
		Vector3 targetPos = transform.position + transform.TransformDirection(new Vector3((boneForwardAxis.x * targetDistance), (boneForwardAxis.y * targetDistance), (boneForwardAxis.z * targetDistance)));

		// Calculate force, acceleration, and velocity per X, Y and Z
		force.x = (targetPos.x - lastPos.x) * bStiffness;
		acc.x = force.x / bMass;
		vel.x += acc.x * (1 - bDamping);

		force.y = (targetPos.y - lastPos.y) * bStiffness;
		force.y -= bGravity / 10; // Add some gravity
		acc.y = force.y / bMass;
		vel.y += acc.y * (1 - bDamping);

		force.z = (targetPos.z - lastPos.z) * bStiffness;
		acc.z = force.z / bMass;
		vel.z += acc.z * (1 - bDamping);

		// Update dynamic postion
		lastPos += vel + force;

		// Set bone rotation to look at lastPos
		if (reverseBone)
			transform.LookAt(2 * transform.position - lastPos, upVector);
		else
			transform.LookAt(lastPos, upVector);

		// ==================================================
		// Squash and Stretch section
		// ==================================================
		if (SquashAndStretch)
		{
			// Create a vector from target position to dynamic position
			// We will measure the magnitude of the vector to determine
			// how much squash and stretch we will apply
			Vector3 dynamicVec = lastPos - targetPos;

			// Get the magnitude of the vector
			float stretchMag = dynamicVec.magnitude;

			// Here we determine the amount of squash and stretch based on stretchMag
			// and the direction the Bone Axis is pointed in. Ideally there should be
			// a vector with two values at 0 and one at 1. Like Vector3(0,0,1)
			// for the 0 values, we assume those are the sides, and 1 is the direction
			// the bone is facing
			float xStretch;
			if (boneForwardAxis.x == 0) xStretch = 1 + (-stretchMag * sideStretch);
			else xStretch = 1 + (stretchMag * frontStretch);

			float yStretch;
			if (boneForwardAxis.y == 0) yStretch = 1 + (-stretchMag * sideStretch);
			else yStretch = 1 + (stretchMag * frontStretch);

			float zStretch;
			if (boneForwardAxis.z == 0) zStretch = 1 + (-stretchMag * sideStretch);
			else zStretch = 1 + (stretchMag * frontStretch);

			// Set the bone scale
			transform.localScale = new Vector3(xStretch, yStretch, zStretch);
		}

		// ==================================================
		// DEBUG VISUALIZATION
		// ==================================================
		// Green line is the bone's local up vector
		// Blue line is the bone's local foward vector
		// Yellow line is the target postion
		// Red line is the dynamic postion
		if (debugMode)
		{
			Debug.DrawRay(transform.position, forwardVector, Color.blue);
			Debug.DrawRay(transform.position, upVector, Color.green);
			Debug.DrawRay(targetPos, Vector3.up * 0.2f, Color.yellow);
			Debug.DrawRay(lastPos, Vector3.up * 0.2f, Color.red);
		}
		// ==================================================
	}
}
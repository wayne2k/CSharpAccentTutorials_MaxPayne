using UnityEngine;

[System.Serializable]
public class IKPositions
{
	public Vector3 handTargetPos1;
	public Vector3 handTargetPos2;
	public Vector3 handTargetRot1;
	public Vector3 elbowTargetPos1;
	public Vector3 elbowTargetPos2;
}

public class IK_ShoulderHandler : MonoBehaviour 
{
	public Transform shoulderToTrack;
	public Transform HandTarget;
	public Transform ElbowTarrget;

	public float lerpRate;
	public IKPositions ikPositions;
	public bool debugMode;

	void Start ()
	{
		HandTarget.position = ikPositions.handTargetPos1;
		ElbowTarrget.position = ikPositions.elbowTargetPos1;
	}

	void Update ()
	{
		Vector3 positionToCopy = shoulderToTrack.TransformPoint(Vector3.zero);
		transform.position = positionToCopy;

		if (debugMode == false)
		{
			HandTarget.localPosition = Vector3.MoveTowards(ikPositions.handTargetPos1, ikPositions.handTargetPos2, lerpRate);
			HandTarget.localRotation = Quaternion.Euler(ikPositions.handTargetRot1);
			ElbowTarrget.localPosition = Vector3.MoveTowards(ikPositions.elbowTargetPos1, ikPositions.elbowTargetPos2, lerpRate);
		}
	}
}

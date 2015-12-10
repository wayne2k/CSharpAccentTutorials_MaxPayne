using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class IK_ShoulderHelper : MonoBehaviour 
{
	public bool storePosition;
	public bool rot1;
	public bool pos1;
	public bool Hand;

	void Update ()
	{
		if (storePosition)
		{
			IK_ShoulderHandler ikHandler = GetComponent<IK_ShoulderHandler>();

			if (Hand)
			{
				Vector3 pos = ikHandler.HandTarget.localPosition;
				Vector3 rot = ikHandler.HandTarget.localEulerAngles;

				if (rot1 == false)
				{
					if (pos1)
					{
						ikHandler.ikPositions.handTargetPos1 = pos;
					}
					else
					{
						ikHandler.ikPositions.handTargetPos2 = pos;
					}
				}
				else
				{
					ikHandler.ikPositions.handTargetRot1 = rot;
				}
			}
			else
			{
				Vector3 pos = ikHandler.ElbowTarrget.localPosition;

				if (pos1)
				{
					ikHandler.ikPositions.elbowTargetPos1 = pos;
				}
				else
				{
					ikHandler.ikPositions.elbowTargetPos2 = pos;
				}
			}
			storePosition = false;
		}
	}
}

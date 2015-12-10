using UnityEngine;

public class IK_Handler_Player : MonoBehaviour 
{
	Animator _anim;
	[SerializeField] IK_ShoulderHandler _rightShoulderIKbase;

	public float bodyWeight = 0.1f;
	public float clampWeight = 0f;
	public float handWeight = 1f;
	public float elbowWeight = 1f;

	Transform _cam;
	Vector3 _targetLookPos;
	Vector3 _curLookPos;
	public float slerpSpeed = 999f;

	Vector3 _shoulderForwardDir = new Vector3(0f,0f,1f);
	float _targetPosLerp;
	float _curPosLerp;

	PlayerController _p1Control;

	float _targetAimWeight;
	float _curAimWeight;

	public Transform debugCube;

	void Start ()
	{
		_anim = GetComponent<Animator>();
		_rightShoulderIKbase = GetComponentInChildren<IK_ShoulderHandler>();
		_cam = Camera.main.transform;
		_p1Control = GetComponent<PlayerController>();
	}

	void FixedUpdate ()
	{
		Ray ray = new Ray(_cam.position, _cam.forward);
		RaycastHit hit;
		LayerMask mask = ~(1<<8);

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
		{
			_targetLookPos = hit.point;
		}
		else
		{
			_targetLookPos = ray.GetPoint(500f);
		}

		Debug.DrawRay(_cam.position,_targetLookPos, Color.red);

//		_curLookPos = Vector3.Lerp(_curLookPos, _targetLookPos, Time.deltaTime * 10f);
		_curLookPos = _targetLookPos;
		debugCube.position = Vector3.Lerp (debugCube.position, _curLookPos, Time.deltaTime * 10);
//		_curLookPos = debugCube.position;

		Quaternion desiredRotation = Quaternion.LookRotation(_curLookPos - _rightShoulderIKbase.transform.position);

		_rightShoulderIKbase.transform.rotation = Quaternion.Slerp(_rightShoulderIKbase.transform.rotation, 
				desiredRotation, Time.deltaTime * slerpSpeed);

		Vector3 dir = transform.InverseTransformPoint(_curLookPos) - _shoulderForwardDir;
		float angle = Vector3.Angle(dir, _shoulderForwardDir);

		if (angle > 30f)
		{
			if (_anim.GetBool("CanMove"))
			{
				_targetPosLerp = 1f;
			}
			else
			{
				_targetPosLerp = 0f;
			}
		}
		else
		{
			_targetPosLerp = 0f;
		}

		_curPosLerp = Mathf.MoveTowards(_curPosLerp, _targetPosLerp, Time.deltaTime * 1f);

		_rightShoulderIKbase.lerpRate = _curPosLerp;
	}

	void OnAnimatorIK ()
	{
		if (_p1Control.prepareToJump)
		{
			_targetAimWeight = 0f;
		}
		else
		{
			_targetAimWeight = 1f;
		}

		_curAimWeight = Mathf.MoveTowards(_curAimWeight, _targetAimWeight, Time.deltaTime * 5f);
//
		_anim.SetIKPositionWeight (AvatarIKGoal.RightHand, _curAimWeight);
		_anim.SetIKPosition (AvatarIKGoal.RightHand, _rightShoulderIKbase.HandTarget.position);

		_anim.SetIKRotationWeight (AvatarIKGoal.RightHand, _curAimWeight);
		_anim.SetIKRotation (AvatarIKGoal.RightHand, _rightShoulderIKbase.HandTarget.rotation);

		_anim.SetIKHintPositionWeight (AvatarIKHint.RightElbow, _curAimWeight);
		_anim.SetIKHintPosition(AvatarIKHint.RightElbow, _rightShoulderIKbase.ElbowTarrget.position);

		_anim.SetLookAtWeight(_curAimWeight, bodyWeight, 1,1, clampWeight);
		_anim.SetLookAtPosition(_curLookPos);
	}
}

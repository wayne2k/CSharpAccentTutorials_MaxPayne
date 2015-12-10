using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(CapsuleCollider))]
[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour 
{
	Rigidbody _rb;
	CapsuleCollider _capCol;
	Animator _anim;
	[SerializeField] PhysicMaterial _zFriction;
	[SerializeField] PhysicMaterial _mFriction;
	Transform _cam;
	Transform _camHolder;

	[SerializeField] float _speed = .8f;
	[SerializeField] float _turnSpeed = 5f;

	public bool prepareToJump;
	bool _jumpOnce;

	bool _rotateToLookCamDirection;
	float _targetTurnAmount;
	float _curTurnAmount;
	bool _canMove;



	Vector3 _directionPos;
	Vector3 _storeDir;

	float _horizontal;
	float _vertical;
	

	void Awake ()
	{
		_rb = GetComponent<Rigidbody>();
		_capCol = GetComponent<CapsuleCollider>();
		_cam = Camera.main.transform;
		_camHolder = _cam.parent.parent;

		SetupAnimator();
	}

	void Update ()
	{
		HandleFriction();

		if (Input.GetMouseButtonUp(1))
		{
			prepareToJump = true;
		}
	}

	void FixedUpdate ()
	{
		_horizontal = Input.GetAxis("Horizontal");
		_vertical = Input.GetAxis("Vertical");

		_storeDir = _camHolder.right;

		_canMove = _anim.GetBool("CanMove");

		Vector3 dirForward = _storeDir * _horizontal;
		Vector3 dirSides = _camHolder.forward * _vertical;

		if (_canMove)
			_rb.AddForce((dirForward + dirSides).normalized * _speed / Time.deltaTime);


		Ray ray = new Ray(_cam.position, _cam.forward);

		_directionPos = ray.GetPoint(100f);

		Vector3 dir = _directionPos - transform.position;
		dir.y = 0f;

		float angle = Quaternion.Angle(transform.rotation,Quaternion.LookRotation(dir));

		if (prepareToJump == false)
		{
			_jumpOnce = false;

			_anim.SetFloat("Forward", _vertical);
			_anim.SetFloat("Sideways", _horizontal);

			RotateWhileAimingOrMoving(angle, dir);
		}
		else
		{
			FindDirectionToJump();
		}



	}

	void FindDirectionToJump ()
	{		
		_anim.SetFloat("Sideways", 0f, 0.1f, Time.deltaTime);
		_anim.SetFloat("Forward",  Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));

		_directionPos = transform.position + (_camHolder.right * _horizontal) + (_camHolder.forward * _vertical);
		Vector3 dir = _directionPos - transform.position;
		dir.y = 0f;

		float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));

		if (angle != 0f && _canMove)
		{
			_rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 2*_turnSpeed * Time.deltaTime);
		}

		if (_jumpOnce == false)
		{
			_anim.SetTrigger("Jump");
			_jumpOnce = true;
		}							
	}

	void RotateWhileAimingOrMoving (float angle, Vector3 dir)
	{
		if (angle > 75f)
		{
			_rotateToLookCamDirection = true;
		}

		if (_rotateToLookCamDirection == false)
		{
			if (_horizontal != 0f || _vertical != 0f)
			{
				if (angle != 0f && _canMove)
				{
					_rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);
				}
			}
		}
		else if (_canMove)
		{
			_targetTurnAmount = 1;

			if (angle != 0)
			{
				_rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);
			}

			if (angle < 1)
			{
				_rotateToLookCamDirection = false;
				_targetTurnAmount = 0;
			}
		}

		_curTurnAmount = Mathf.MoveTowards(_curTurnAmount, _targetTurnAmount, Time.deltaTime * 3f);

		_anim.SetFloat("PivotTurn", _curTurnAmount);
	}


	void SetupAnimator ()
	{
		_anim = GetComponent<Animator> ();

		foreach (var childAnim in GetComponentsInChildren<Animator>()) {

			if (childAnim != _anim)
			{
				_anim.avatar = childAnim.avatar;
				Destroy(childAnim);
				break;
			}
		}
	}

	void HandleFriction ()
	{
		if (_horizontal == 0f && _vertical == 0f)
		{
			_capCol.material = _mFriction;
		}
		else
		{
			_capCol.material = _zFriction;
		}
	}
}

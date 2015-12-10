using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour 
{
	Animator _anim;
	Transform _cam;
	Rigidbody _rb;
	
	Vector3 _targetUpDistance;
	bool _storeTargetUpDis;
	float _airTimer;
	bool _onAir;
	bool _aimingJump;

	bool _addJumpForce;

	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
		_anim = GetComponent<Animator>();
		_cam = Camera.main.transform;
	}

	void FixedUpdate ()
	{
		FindAngles();

		if (_onAir)
		{
			JumpingLogic();
		}
		else
		{
			if (_aimingJump)
			{
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");

				if (Mathf.Abs(h) + Mathf.Abs(v) > 0f)
				{
					_anim.SetTrigger("GetUp");
					_onAir = false;
					_aimingJump = false;
				}
			}
		}

		_anim.SetBool("onAir", _onAir);
	}																			

	void FindAngles ()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		Vector3 dir = ray.GetPoint(100f) - _cam.position;

		Vector3 relativePosition = _cam.InverseTransformDirection(dir).normalized;

		_anim.SetFloat("AimFront", relativePosition.z);
		_anim.SetFloat("AimSides", relativePosition.x);
	}
	
	void JumpingLogic ()
	{
		_airTimer += Time.deltaTime;

		if (_airTimer <	 0.5f)
		{
			_rb.drag = 0f;

			if (_storeTargetUpDis == false)
			{
				_targetUpDistance = transform.position + transform.forward * 1.5f + new Vector3(0f, 1.5f, 0f);
				_storeTargetUpDis = true;
			}

			if (_addJumpForce == false)
			{
				_rb.AddForce((_targetUpDistance - transform.position).normalized * 8f, ForceMode.Impulse);
				_addJumpForce = true;
			}
		}
		else
		{
			RaycastHit hit;

			if (Physics.Raycast(transform.position, -transform.up, out hit, 0.3f))
			{
				_onAir = false;
				_airTimer = 0f;
				_rb.drag = 4f;
				_storeTargetUpDis = false;
				_addJumpForce = false;
			}
		}
	}

	public void GoAirborne ()
	{
		_onAir = true;
		_aimingJump = true;
	}
}























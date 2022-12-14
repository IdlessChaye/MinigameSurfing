using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActiveRagdoll;

public class BoatController : MonoBehaviour
{
	[SerializeField] private bool _isMotoring = false;

	[SerializeField] private List<GameObject> m_motors;

	[SerializeField] public float m_FinalSpeed = 100F;
	[SerializeField] public float m_InertiaFactor = 0.005F;
	[SerializeField] public float m_turningFactor = 2.0F;
	[SerializeField] public float m_accelerationTorqueFactor = 35F;
	[SerializeField] public float m_turningTorqueFactor = 35F;

	[SerializeField] private float m_VeloDamp = 1F;
	[SerializeField] private float m_Force = 1F;

	public Transform boatPlayerPlaceTransform;
	public Transform boatPlayerLeftLegTransform;
	public Transform boatPlayerRightLegTransform;

	public bool isMotoring { get { return _isMotoring; } }

	private float m_verticalInput = 0F;
	private float m_horizontalInput = 0F;
	private Rigidbody m_rigidbody;
	private Vector2 m_androidInputInit;
	private Transform m_transform;

	private float accel = 0;
	private float accelBreak;

	private Buoyancy _buoyancy;
	public Buoyancy buoyancy => _buoyancy;

	public Rigidbody Rigidbody => m_rigidbody;

	private static BoatController _instance;
	public static BoatController Instance => _instance;

	public Vector3 initPos;
	public Quaternion initRot;

	private void Awake()
	{
		_instance = this;
		m_transform = transform;
		m_rigidbody = GetComponent<Rigidbody>();
		_buoyancy = GetComponent<Buoyancy>();

		accelBreak = m_FinalSpeed * 0.3f;

		initPos = transform.position;
		initRot = transform.rotation;
		ResetStatus();
	}

	public void ResetStatus()
	{
		m_rigidbody.isKinematic = true;
		m_rigidbody.isKinematic = false;
		m_rigidbody.velocity = Vector3.zero;
		m_transform.position = initPos + Vector3.up * 1;
		m_transform.rotation = initRot;
	}
	

	void Update()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		Vector2 touchInput = Vector2.zero;
		touchInput.x =  -(Input.acceleration.y - m_androidInputInit.y);
		touchInput.y =  Input.acceleration.x - m_androidInputInit.x;

		if (touchInput.sqrMagnitude > 1)
			touchInput.Normalize();

		setInputs (touchInput.x, touchInput.y);
#else
		setInputs(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
#endif

	}



	public bool SetIsMotoring(bool isMotoring)
	{
		_isMotoring = isMotoring;
		return _isMotoring;
	}

	public void setInputs(float iVerticalInput, float iHorizontalInput)
	{
		if (_isMotoring == false)
		{
			m_verticalInput = 0;
			m_horizontalInput = 0;
			return;
		}
		m_verticalInput = iVerticalInput;
		m_horizontalInput = iHorizontalInput;
	}

	void FixedUpdate()
	{
		if (_isMotoring == false)
			return;

		var motorForwardForce = m_Force * m_transform.forward * m_verticalInput; //(m_verticalInput + 1) / 2;

		var veloDir = m_rigidbody.velocity.normalized;
		var velo = m_rigidbody.velocity.magnitude;
		if (_buoyancy != null && _buoyancy.isInWater == true)
			motorForwardForce -= velo * velo * m_VeloDamp * veloDir;

		m_rigidbody.AddForce(motorForwardForce);


		m_rigidbody.AddRelativeTorque(
			m_verticalInput * -m_accelerationTorqueFactor,
			m_horizontalInput * m_turningFactor,
			m_horizontalInput * -m_turningTorqueFactor
		);

		if (m_motors.Count > 0)
		{

			float motorRotationAngle = 0F;
			float motorMaxRotationAngle = 70;

			motorRotationAngle = -m_horizontalInput * motorMaxRotationAngle;

			for (int i = 0; i < m_motors.Count; i++)
			{
				float currentAngleY = m_motors[i].transform.localEulerAngles.y;
				if (currentAngleY > 180.0f)
					currentAngleY -= 360.0f;

				float localEulerAngleY = Lerp(currentAngleY, motorRotationAngle, Time.deltaTime * 10);
				m_motors[i].transform.localEulerAngles = new Vector3(
					m_motors[i].transform.localEulerAngles.x,
					localEulerAngleY,
					m_motors[i].transform.localEulerAngles.z
				);
			}
		}
	}

	static float Lerp(float from, float to, float value)
	{
		if (value < 0.0f)
			return from;
		else if (value > 1.0f)
			return to;
		return (to - from) * value + from;
	}

}

#pragma warning disable 649

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActiveRagdoll {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class CameraModule : Module {
        [Header("--- GENERAL ---")]
        [Tooltip("Where the camera should point to. Head by default.")]
        public Transform _lookPoint;

		public float lookSensitivity = 1;
        public float scrollSensitivity = 1;
        public bool invertY = false, invertX = false;

		public GameObject Camera;
		public bool isAvoidObstacles = false;

		private Vector2 _cameraRotation;
        private Vector2 _inputDelta;

        [Header("--- SMOOTHING ---")]
        public float smoothSpeed = 20;
        public bool smooth = true;

        private Vector3 _smoothedLookPoint, _startDirection;


        [Header("--- STEEP INCLINATIONS ---")]
        [Tooltip("Allows the camera to make a crane movement over the head when looking down," +
        " increasing visibility downwards.")]
        public bool improveSteepInclinations = true;
        public float inclinationAngle = 30, inclinationDistance = 1.5f;


        [Header("--- DISTANCES ---")]
        public float minDistance = 2;
        public float maxDistance = 10, initialDistance = 5f;

        private float _currentDistance;


        [Header("--- LIMITS ---")]
        [Tooltip("How far can the camera look down.")]
        public float minVerticalAngle = -80;

        [Tooltip("How far can the camera look up.")]
        public float maxVerticalAngle = 80;

        [Tooltip("Which layers don't make the camera reposition. Mainly the ActiveRagdoll one.")]
        public LayerMask dontBlockCamera;

        [Tooltip("How far to reposition the camera from an obstacle.")]
        public float cameraRepositionOffset = 0.15f;


		public Transform targetTF { get; set; }
		private SphericalVector sphericalVector = new SphericalVector(5f, 1f, 0.3f);
		private float viewMoveSpeed;
		private Transform _cam_transform;

		public void Init()
		{
			SetLookPoint();
			targetTF = _lookPoint;
			viewMoveSpeed = 0.03f;
			_cam_transform = Camera.transform;
		}

		public void Look()  // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
		{
			if (targetTF == null)
				return;

			float h = _inputDelta.x;//水平视角移动
			float v = _inputDelta.y;//垂直视角移动
			sphericalVector.azimuth += h * viewMoveSpeed;
			sphericalVector.zenith -= v * viewMoveSpeed;
			sphericalVector.zenith = Mathf.Clamp(sphericalVector.zenith, 0f, 1f);
			sphericalVector.length = _currentDistance;
			Vector3 destTargetPosition = targetTF.position + Vector3.up * 0.5f;
			Vector3 destCameraPosition = destTargetPosition + sphericalVector.Position;//设定摄像机位置
																					   //transform.position = Vector3.Lerp(transform.position,destCameraPosition,Time.fixedDeltaTime * smooth);
			_cam_transform.position = destCameraPosition;
			_cam_transform.forward = GetLookAtVector3(_cam_transform.position, destTargetPosition); //设定摄像机朝向
		}

		Vector3 GetLookAtVector3(Vector3 oriPosition, Vector3 destPosition)
		{
			Vector3 offsetPosition = destPosition - oriPosition;
			return offsetPosition;
		}

		private void OnValidate() {
			SetLookPoint();
		}

		private void SetLookPoint()
		{
			if (_lookPoint == null)
				_lookPoint = _activeRagdoll.GetPlayerBone(HumanBodyBones.Head);
		}

        void Start() {
			PrepareCamera();
		}

		public void PrepareCamera()
		{
			if (Camera == null)
				return;

			SetLookPoint();

			Camera.transform.parent = transform;

			_smoothedLookPoint = _lookPoint.position;
			_currentDistance = initialDistance;

			_startDirection = Vector3.forward;

			Init();
		}

        void Update() {
			Look();
			return;
            UpdateCameraInput();
            UpdateCameraPosRot();
            AvoidObstacles();
        }

        private void UpdateCameraInput() {
            _cameraRotation.x = Mathf.Repeat(_cameraRotation.x + _inputDelta.x * (invertX ? -1 : 1) * lookSensitivity, 360);
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y + _inputDelta.y * (invertY ? 1 : -1) * lookSensitivity,
                                    minVerticalAngle, maxVerticalAngle);
        }

        private void UpdateCameraPosRot() {
            // Improve steep inclinations
            Vector3 movedLookPoint = _lookPoint.position;
            if (improveSteepInclinations) {
                float anglePercent = (_cameraRotation.y - minVerticalAngle) / (maxVerticalAngle - minVerticalAngle);
                float currentDistance = ((anglePercent * inclinationDistance) - inclinationDistance / 2);
                movedLookPoint += (Quaternion.Euler(inclinationAngle, 0, 0)
                    * Auxiliary.GetFloorProjection(Camera.transform.forward)) * currentDistance;
            }
            
            // Smooth
            _smoothedLookPoint = Vector3.Lerp(_smoothedLookPoint, movedLookPoint, smooth ? smoothSpeed * Time.deltaTime : 1);

            Camera.transform.position = _smoothedLookPoint - (_startDirection * _currentDistance);
            Camera.transform.RotateAround(_smoothedLookPoint, Vector3.right, _cameraRotation.y);
            Camera.transform.RotateAround(_smoothedLookPoint, Vector3.up, _cameraRotation.x);
            Camera.transform.LookAt(_smoothedLookPoint);
        }

        private void AvoidObstacles() {
			if (isAvoidObstacles == false)
				return;
            Ray cameraRay = new Ray(_lookPoint.position, Camera.transform.position - _lookPoint.position);
            bool hit = Physics.Raycast(cameraRay, out RaycastHit hitInfo,
                                       Vector3.Distance(Camera.transform.position, _lookPoint.position), ~dontBlockCamera);

            if (hit) {
                Camera.transform.position = hitInfo.point + (hitInfo.normal * cameraRepositionOffset);
                Camera.transform.LookAt(_smoothedLookPoint);
            }
        }

        // ------------- Input Handlers -------------

        public void OnLook(InputValue value) {
            _inputDelta = value.Get<Vector2>() / 10 ;
        }

        public void OnScrollWheel(InputValue value) {
            var scrollValue = value.Get<Vector2>();
            _currentDistance = Mathf.Clamp(_currentDistance + scrollValue.y / 1200 * - scrollSensitivity,
                                    minDistance, maxDistance);
        }
    }

	public struct SphericalVector
	{
		public float length;
		public float azimuth;
		public float zenith;

		public SphericalVector(float l, float a, float z)
		{
			this.length = l;
			this.azimuth = a;
			this.zenith = z;
		}

		public Vector3 Position
		{
			get
			{
				return length * Direction;
			}
		}
		public Vector3 Direction
		{
			get
			{
				float z = zenith * Mathf.PI / 2;
				float a = azimuth * Mathf.PI;
				Vector3 ret = Vector3.zero;
				ret.y = Mathf.Sin(z);
				ret.x = Mathf.Cos(z) * Mathf.Sin(a);
				ret.z = Mathf.Cos(z) * Mathf.Cos(a);
				return ret;
			}
		}
	}
} // namespace ActiveRagdoll
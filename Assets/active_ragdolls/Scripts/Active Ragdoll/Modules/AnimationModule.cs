using System;
using UnityEditor;
using UnityEngine;

namespace ActiveRagdoll {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class AnimationModule : Module {
		public float heightOffset = 0.23f;

		[Header("--- BODY ---")]
        /// <summary> Required to set the target rotations of the joints </summary>
        private Quaternion[] _initialJointsRotation;
        private ConfigurableJoint[] _joints;
        private Transform[] _animatedBones;
        private AnimatorHelper _animatorHelper;
		private Animator _animatedAnimator;
		private Animator _physicalAnimator;
		private Animator _playerAnimator;
		private HumanPoseHandler _phyPoseHandler;
		private HumanPoseHandler _playerPoseHandler;
		private Transform _animatedTorso;
		private Transform _playerTorso;



		[Header("--- INVERSE KINEMATICS ---")]
        public bool _enableIK = true;

        [Tooltip("Those values define the rotation range in which the target direction influences arm movement.")]
        public float minTargetDirAngle = - 30,
                     maxTargetDirAngle = 60;

        [Space(10)]
        [Tooltip("The limits of the arms direction. How far down/up should they be able to point?")]
        public float minArmsAngle = -70;
        public float maxArmsAngle = 100;
        [Tooltip("The limits of the look direction. How far down/up should the character be able to look?")]
        public float minLookAngle = -50, maxLookAngle = 60;

        [Space(10)]
        [Tooltip("The vertical offset of the look direction in reference to the target direction.")]
        public float lookAngleOffset;
        [Tooltip("The vertical offset of the arms direction in reference to the target direction.")]
        public float armsAngleOffset;
        [Tooltip("Defines the orientation of the hands")]
        public float handsRotationOffset = 0;

        [Space(10)]
        [Tooltip("How far apart to place the arms")]
        public float armsHorizontalSeparation = 0.75f;

        [Tooltip("The distance from the body to the hands in relation to how high/low they are. " +
                 "Allows to create more realistic movement patterns.")]
        public AnimationCurve armsDistance;
        
        public Vector3 AimDirection { get; set; }
        private Vector3 _armsDir, _lookDir, _targetDir2D;
        private Transform _chest;
        private float _targetDirVerticalPercent;



        private void Start() {
            _joints = _activeRagdoll.Joints;
            _animatedBones = _activeRagdoll.AnimatedBones;
            _animatorHelper = _activeRagdoll.AnimatorHelper;
			_animatedAnimator = _activeRagdoll.AnimatedAnimator;
			_physicalAnimator = _activeRagdoll.PhysicalAnimator;
			_playerAnimator = _activeRagdoll.PlayerAnimator;
			_phyPoseHandler = new HumanPoseHandler(_physicalAnimator.GetComponent<Animator>().avatar, _physicalAnimator.transform);
			_playerPoseHandler = new HumanPoseHandler(_playerAnimator.GetComponent<Animator>().avatar, _playerAnimator.transform);
			_animatedTorso = _activeRagdoll.AnimatedTorso;
			_playerTorso = _activeRagdoll.PlayerTorso;
			_chest = _animatedAnimator.GetBoneTransform(HumanBodyBones.Spine);

			_initialJointsRotation = new Quaternion[_joints.Length];
            for (int i = 0; i < _joints.Length; i++) {
                _initialJointsRotation[i] = _joints[i].transform.localRotation;
            }
        }

        void Update() {
            UpdateJointTargets();
            UpdateIK();
        }

		/// <summary> Makes the physical bones match the rotation of the animated ones </summary>
		/// Animation Retargeting Helper HumanPoseHandler
		private void UpdateJointTargets() {
			for (int i = 0; i < _joints.Length; i++)
			{
				ConfigurableJointExtensions.SetTargetRotationLocal(_joints[i], _animatedBones[i + 1].localRotation, _initialJointsRotation[i]);
			}
			HumanPose humanPose = new HumanPose();

			_phyPoseHandler.GetHumanPose(ref humanPose);
			_playerPoseHandler.SetHumanPose(ref humanPose);

			_playerTorso.rotation = _animatedTorso.rotation;
			_playerTorso.position = _animatedTorso.position + Vector3.up * heightOffset;
			
		}

        private void UpdateIK() {
            if (!_enableIK) {
                _animatorHelper.LeftArmIKWeight = 0;
                _animatorHelper.RightArmIKWeight = 0;
                _animatorHelper.LookIKWeight = 0;
                return;
            }
            _animatorHelper.LookIKWeight = 1;
			//_animatorHelper.LeftLegIKWeight = 1;
			//_animatorHelper.RightLegIKWeight = 1;

            ReflectBackwards();
            _targetDir2D = Auxiliary.GetFloorProjection(AimDirection);
            CalculateVerticalPercent();
			UpdateLookIK();
            UpdateArmsIK();
		}

        /// <summary> Reflect the direction when looking backwards, avoids neck-breaking twists </summary>
        /// <param name=""></param>
        private void ReflectBackwards() {
            bool lookingBackwards = Vector3.Angle(AimDirection, _animatedTorso.forward) > 90;
            if (lookingBackwards) AimDirection = Vector3.Reflect(AimDirection, _animatedTorso.forward);
        }

        /// <summary> Calculate the vertical inlinacion percentage of the target direction
        /// (how much it is looking up) </summary>
        private void CalculateVerticalPercent() {
            float directionAngle = Vector3.Angle(AimDirection, Vector3.up);
            directionAngle -= 90;
            _targetDirVerticalPercent = 1 - Mathf.Clamp01((directionAngle - minTargetDirAngle) / Mathf.Abs(maxTargetDirAngle - minTargetDirAngle));
        }

        private void UpdateLookIK() {
            float lookVerticalAngle = _targetDirVerticalPercent * Mathf.Abs(maxLookAngle - minLookAngle) + minLookAngle;
            lookVerticalAngle += lookAngleOffset;
            _lookDir = Quaternion.AngleAxis(-lookVerticalAngle, _animatedTorso.right) * _targetDir2D;

            Vector3 lookPoint = _animatedAnimator.GetBoneTransform(HumanBodyBones.Head).position + _lookDir;
            _animatorHelper.LookAtPoint(lookPoint);
        }

        private void UpdateArmsIK() {
            float armsVerticalAngle = _targetDirVerticalPercent * Mathf.Abs(maxArmsAngle - minArmsAngle) + minArmsAngle;
            armsVerticalAngle += armsAngleOffset;
            _armsDir = Quaternion.AngleAxis(-armsVerticalAngle, _animatedTorso.right) * _targetDir2D;

            float currentArmsDistance = armsDistance.Evaluate(_targetDirVerticalPercent);

            Vector3 armsMiddleTarget = _chest.position + _armsDir * currentArmsDistance;
            Vector3 upRef = Vector3.Cross(_armsDir, _animatedTorso.right).normalized;
            Vector3 armsHorizontalVec = Vector3.Cross(_armsDir, upRef).normalized;
            Quaternion handsRot = _armsDir != Vector3.zero? Quaternion.LookRotation(_armsDir, upRef)
                                                            : Quaternion.identity;

            _animatorHelper.LeftHandTarget.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation / 2;
            _animatorHelper.RightHandTarget.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation / 2;
            _animatorHelper.LeftHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, 90 - handsRotationOffset);
            _animatorHelper.RightHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, -90 + handsRotationOffset);

            var armsUpVec = Vector3.Cross(_armsDir, _animatedTorso.right).normalized;
            _animatorHelper.LeftHandHint.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation - armsUpVec;
            _animatorHelper.RightHandHint.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation - armsUpVec;
        }

        /// <summary> Plays an animation using the animator. The speed doesn't change the actual
        /// speed of the animator, but a parameter of the same name that can be used to multiply
        /// the speed of certain animations. </summary>
        /// <param name="animation">The name of the animation state to be played</param>
        /// <param name="speed">The speed to be set</param>
        public void PlayAnimation(string animation, float speed = 1) {
			_animatedAnimator.Play(animation);
			_animatedAnimator.SetFloat("speed", speed);
        }
        
        public void UseLeftArm(float weight) {
            if (!_enableIK)
                return;

            _animatorHelper.LeftArmIKWeight = weight;
        }

        public void UseRightArm(float weight) {
            if (!_enableIK)
                return;

            _animatorHelper.RightArmIKWeight = weight;
        }
    }
} // namespace ActiveRagdoll
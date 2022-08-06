using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveRagdoll {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class Gripper : MonoBehaviour {
        public GripModule GripMod { get; set; }

        /// <summary> If the component is activated after colliding with something, it won't grip
        /// to it unless the collision enters again. This variable hold the last collision to avoid
        /// skipping it. </summary>
        private Rigidbody _lastCollision;

        private ConfigurableJoint _joint;
        private Grippable _gripped;

		public static bool IsHoldBoat = false;
        
        public void Start() {
            // Start disabled is useful to avoid fake gripping something at the start
            enabled = false;
        }

        private void Grip(Rigidbody whatToGrip) {
            if (!enabled) {
                _lastCollision = whatToGrip;
                return;
            }

			if (PersonBoatMananger.Instance.PersonBoatStatus == PersonBoatStatus.PersonSurfing)
				return;

            if (_joint != null)
                return;

            if (!GripMod.canGripYourself
                    && whatToGrip.transform.IsChildOf(GripMod.ActiveRagdoll.transform))
                return;

            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = whatToGrip;
            _joint.xMotion = ConfigurableJointMotion.Locked;
            _joint.yMotion = ConfigurableJointMotion.Locked;
            _joint.zMotion = ConfigurableJointMotion.Locked;

            if (whatToGrip.TryGetComponent(out _gripped))
                _gripped.jointMotionsConfig.ApplyTo(ref _joint);
            else
                GripMod.defaultMotionsConfig.ApplyTo(ref _joint);

			if (whatToGrip.GetComponent<BoatController>() != null)
			{
				IsHoldBoat = true;
			}
        }

        public void UnGrip() {
            if (_joint == null)
                return;

            Destroy(_joint);
            _joint = null;
            _gripped = null;
			IsHoldBoat = false;
			enabled = false;

			var rig = GetComponent<Rigidbody>();
			if (rig != null)
				Destroy(rig);
		}

        private void OnCollisionEnter(Collision collision) {
            if (GripMod.onlyUseTriggers)
                return;

            if (collision.rigidbody != null && collision.transform.tag.Equals(Const.TagGrippable))
                Grip(collision.rigidbody);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.attachedRigidbody != null && other.tag.Equals(Const.TagGrippable))
                Grip(other.attachedRigidbody);
        }

        private void OnCollisionExit(Collision collision) {
            if (collision.rigidbody == _lastCollision && collision.transform.tag.Equals(Const.TagGrippable))
                _lastCollision = null;
        }

        private void OnTriggerExit(Collider other) {
            if (other.attachedRigidbody == _lastCollision && other.tag.Equals(Const.TagGrippable))
                _lastCollision = null;
        }



        private void OnEnable() {
            if (_lastCollision != null)
                Grip(_lastCollision);
        }

        private void OnDisable() {
            UnGrip();
        }
    }
} // namespace ActiveRagdoll

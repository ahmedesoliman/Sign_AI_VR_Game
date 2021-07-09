namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.AI;
    using UnityEngine.SceneManagement;
    using GameCreator.Core;
    using System;

    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("Game Creator/Characters/Character", 100)]
    public class Character : GlobalID, IGameSave
    {
        [System.Serializable]
        public class State
        {
            public Vector3 forwardSpeed;
            public float sidesSpeed;
            public float pivotSpeed;
            public bool targetLock;
            public float isGrounded;
            public float isSliding;
            public float isDashing;
            public float verticalSpeed;
            public Vector3 normal;

            public State()
            {
                this.forwardSpeed = Vector3.zero;
                this.sidesSpeed = 0f;
                this.targetLock = false;
                this.isGrounded = 1.0f;
                this.isSliding = 0.0f;
                this.isDashing = 0.0f;
                this.verticalSpeed = 0f;
                this.normal = Vector3.zero;
            }
        }

        [Serializable]
        public class SaveData
        {
            public Vector3 position = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
        }

        [Serializable]
        public class OnLoadSceneData
        {
            public bool active { get; private set; }
            public Vector3 position { get; private set; }
            public Quaternion rotation { get; private set; }

            public OnLoadSceneData(Vector3 position, Quaternion rotation)
            {
                this.active = true;
                this.position = position;
                this.rotation = rotation;
            }

            public void Consume()
            {
                this.active = false;
            }
        }

        public class LandEvent : UnityEvent<float> { }
        public class JumpEvent : UnityEvent<int> { }
        public class DashEvent : UnityEvent { }
        public class StepEvent : UnityEvent<CharacterLocomotion.STEP> { }
        public class IsControllableEvent : UnityEvent<bool> { }

        protected const string ERR_NOCAM = "No Main Camera found.";

        // PROPERTIES: ----------------------------------------------------------------------------

        public CharacterLocomotion characterLocomotion;

        public State characterState = new State();
        private CharacterAnimator animator;
        private CharacterRagdoll ragdoll;

        public JumpEvent onJump = new JumpEvent();
        public LandEvent onLand = new LandEvent();
        public DashEvent onDash = new DashEvent();
        public StepEvent onStep = new StepEvent();

        public IsControllableEvent onIsControllable = new IsControllableEvent();

        public bool save;
        protected SaveData initSaveData = new SaveData();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            this.CharacterAwake();

            this.initSaveData = new SaveData()
            {
                position = transform.position,
                rotation = transform.rotation,
            };

            if (this.save)
            {
                SaveLoadManager.Instance.Initialize(this);
            }
        }

        protected void CharacterAwake()
        {
            if (!Application.isPlaying) return;
            this.animator = GetComponent<CharacterAnimator>();
            this.characterLocomotion.Setup(this);

            if (this.animator != null && this.animator.autoInitializeRagdoll)
            {
                this.InitializeRagdoll();
            }
        }

        protected void OnDestroy()
        {
            this.OnDestroyGID();
            if (!Application.isPlaying) return;

            if (this.save && !this.exitingApplication)
            {
                SaveLoadManager.Instance.OnDestroyIGameSave(this);
            }
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
        {
            if (!Application.isPlaying) return;
            this.CharacterUpdate();
        }

        protected void CharacterUpdate()
        {
            if (this.ragdoll != null && this.ragdoll.GetState() != CharacterRagdoll.State.Normal) return;

            this.characterLocomotion.Update();
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying) return;
            if (this.ragdoll != null && this.ragdoll.GetState() != CharacterRagdoll.State.Normal)
            {
                this.ragdoll.Update();
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public State GetCharacterState()
        {
            return this.characterState;
        }

        public void SetRagdoll(bool active, bool autoStand = false)
        {
            if (active && this.ragdoll.GetState() != CharacterRagdoll.State.Normal) return;
            if (!active && this.ragdoll.GetState() == CharacterRagdoll.State.Normal) return;

            this.characterLocomotion.characterController.enabled = !active;
            this.animator.animator.enabled = !active;

            Transform model = this.animator.animator.transform;
            switch (active)
            {
                case true:
                    this.ragdoll.Ragdoll(true, autoStand);
                    model.SetParent(null, true);
                    break;

                case false:
                    model.SetParent(transform, true);
                    this.ragdoll.Ragdoll(false, autoStand);
                    break;
            }
        }

        public void InitializeRagdoll()
        {
            this.ragdoll = new CharacterRagdoll(this);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public bool IsControllable()
        {
            if (this.characterLocomotion == null) return false;
            return this.characterLocomotion.isControllable;
        }

        public bool IsRagdoll()
        {
            return (this.ragdoll != null && this.ragdoll.GetState() != CharacterRagdoll.State.Normal);
        }

        public int GetCharacterMotion()
        {
            if (this.characterState == null) return 0;
            if (this.characterLocomotion == null) return 0;

            float speed = Mathf.Abs(this.characterState.forwardSpeed.magnitude);
            if (Mathf.Approximately(speed, 0.0f)) return 0;
            else if (this.characterLocomotion.canRun && speed > this.characterLocomotion.runSpeed/2.0f)
            {
                return 2;
            }

            return 1;
        }

        public bool IsGrounded()
        {
            if (this.characterState == null) return true;
            return Mathf.Approximately(this.characterState.isGrounded, 1.0f);
        }

        public CharacterAnimator GetCharacterAnimator()
        {
            return this.animator;
        }

        // JUMP: ----------------------------------------------------------------------------------

        public bool Dash(Vector3 direction, float impulse, float duration, float drag = 10f)
        {
            if (this.characterLocomotion.isBusy) return false;

            this.characterLocomotion.Dash(direction, impulse, duration, drag);
            if (this.animator != null) this.animator.Dash();
            if (this.onDash != null) this.onDash.Invoke();
            return true;
        }

        public void RootMovement(float impulse, float duration, float gravityInfluence,
            AnimationCurve acForward, AnimationCurve acSides, AnimationCurve acVertical)
        {
            this.characterLocomotion.RootMovement(
                impulse, duration, gravityInfluence,
                acForward, acSides, acVertical
            );
        }

        public void Jump(float force)
        {
            int jumpChain = this.characterLocomotion.Jump(force);
            if (jumpChain >= 0 && this.animator != null)
            {
                this.animator.Jump();
            }
        }

        public void Jump()
        {
            int jumpChain = this.characterLocomotion.Jump();
            if (jumpChain >= 0 && this.animator != null)
            {
                this.animator.Jump(jumpChain);
            }
        }

        // HEAD TRACKER: --------------------------------------------------------------------------

        public CharacterHeadTrack GetHeadTracker()
        {
            if (this.animator == null) return null;
            return this.animator.GetHeadTracker();
        }

        // FLOOR COLLISION: -----------------------------------------------------------------------

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!Application.isPlaying) return;

            float coefficient = this.characterLocomotion.pushForce;
            if (coefficient < float.Epsilon) return;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < 90f) this.characterLocomotion.terrainNormal = hit.normal;

            Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
            if (angle <= 90f && angle >= 5f && hitRigidbody != null && !hitRigidbody.isKinematic)
            {
                Vector3 force = hit.controller.velocity * coefficient / Time.fixedDeltaTime;
                hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Force);
            }
        }

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            if (this.ragdoll != null) this.ragdoll.OnDrawGizmos();
        }

        // GAME SAVE: -----------------------------------------------------------------------------

        public string GetUniqueName()
        {
            string uniqueName = string.Format(
                "character:{0}",
                this.GetUniqueCharacterID()
            );

            return uniqueName;
        }

        protected virtual string GetUniqueCharacterID()
        {
            return this.GetID();
        }

        public Type GetSaveDataType()
        {
            return typeof(SaveData);
        }

        public object GetSaveData()
        {
            return new SaveData()
            {
                position = transform.position,
                rotation = transform.rotation
            };
        }

        public void ResetData()
        {
            transform.position = this.initSaveData.position;
            transform.rotation = this.initSaveData.rotation;
        }

        public void OnLoad(object generic)
        {
            SaveData container = generic as SaveData;
            if (container == null) return;

            transform.position = container.position;
            transform.rotation = container.rotation;
        }
    }
}

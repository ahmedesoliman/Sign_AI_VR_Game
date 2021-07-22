namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Variables;

    [System.Serializable]
    public class CharacterLocomotion
    {
        public enum ANIM_CONSTRAINT
        {
            NONE,
            KEEP_MOVEMENT,
            KEEP_POSITION
        }

        public enum LOCOMOTION_SYSTEM
        {
            CharacterController,
            NavigationMeshAgent
        }

        public enum FACE_DIRECTION
        {
            MovementDirection,
            CameraDirection,
            Target,
            GroundPlaneCursor,
            GroundPlaneCursorDelta
        }

        public enum OVERRIDE_FACE_DIRECTION
        {
            None = -1,
            MovementDirection = FACE_DIRECTION.MovementDirection,
            CameraDirection = FACE_DIRECTION.CameraDirection,
            Target = FACE_DIRECTION.Target,
            GroundPlaneCursor = FACE_DIRECTION.GroundPlaneCursor,
            GroundPlaneCursorDelta = FACE_DIRECTION.GroundPlaneCursorDelta
        }

        public enum STEP
        {
            Any,
            Left,
            Right
        }

        // CONSTANTS: -----------------------------------------------------------------------------

        private const float JUMP_COYOTE_TIME = 0.3f;
        private const float MAX_GROUND_VSPEED = -9.8f;
        private const float GROUND_TIME_OFFSET = 0.1f;

        private const float ACCELERATION = 25f;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool isControllable = true;
        public bool isBusy = false;

        public float runSpeed = 4.0f;
        [Range(0, 720f)]
        public float angularSpeed = 540f;
        public float gravity = -9.81f;
        public float maxFallSpeed = -100f;

        public bool canRun = true;
        public bool canJump = true;
        public float jumpForce = 6.0f;
        public int jumpTimes = 1;
        public float timeBetweenJumps = 0.5f;
        public float pushForce = 1.0f;

        [HideInInspector] public Vector3 terrainNormal = Vector3.up;
        [HideInInspector] public float verticalSpeed = 0.0f;

        // ADVANCED PROPERTIES: -------------------------------------------------------------------

        public OVERRIDE_FACE_DIRECTION overrideFaceDirection = OVERRIDE_FACE_DIRECTION.None;
        public TargetPosition overrideFaceDirectionTarget = new TargetPosition();

        public FACE_DIRECTION faceDirection = FACE_DIRECTION.MovementDirection;
        public TargetPosition faceDirectionTarget = new TargetPosition();

        [Tooltip("Check this if you want to use Unity's NavMesh and have a map baked")]
        public bool canUseNavigationMesh = false;
        
        // INNER PROPERTIES: ----------------------------------------------------------------------

        private float lastGroundTime = 0f;
        private float lastJumpTime = 0f;
        private int jumpChain = 0;

        [HideInInspector] public Character character;

        [HideInInspector] public ANIM_CONSTRAINT animatorConstraint = ANIM_CONSTRAINT.NONE;
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public NavMeshAgent navmeshAgent;

        public LOCOMOTION_SYSTEM currentLocomotionType { get; private set; }
        public ILocomotionSystem currentLocomotionSystem { get; private set; }

        private bool useGravity = true;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Character character)
        {
            this.lastGroundTime = Time.time;
            this.lastJumpTime = Time.time;

            this.character = character;
            this.characterController = this.character.GetComponent<CharacterController>();

            this.currentLocomotionType = LOCOMOTION_SYSTEM.CharacterController;

            this.GenerateNavmeshAgent();
            this.SetDirectionalDirection(Vector3.zero);
        }

        // UPDATE: --------------------------------------------------------------------------------

        public void Update()
        {
            this.currentLocomotionType = LOCOMOTION_SYSTEM.CharacterController;
            if (this.currentLocomotionSystem != null)
            {
                this.currentLocomotionType = this.currentLocomotionSystem.Update();
            }

            switch (this.currentLocomotionType)
            {
                case LOCOMOTION_SYSTEM.CharacterController:
                    this.UpdateVerticalSpeed(this.characterController.isGrounded);
                    break;

                case LOCOMOTION_SYSTEM.NavigationMeshAgent:
                    this.UpdateVerticalSpeed(!this.navmeshAgent.isOnOffMeshLink);
                    break;
            }

            this.UpdateCharacterState(this.currentLocomotionType);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Dash(Vector3 direction, float impulse, float duration, float drag)
        {
            this.SetDirectionalDirection(Vector3.zero);
            this.currentLocomotionSystem.Dash(direction, impulse, duration, drag);
        }

        public void RootMovement(float impulse, float duration, float gravityInfluence,
            AnimationCurve acForward, AnimationCurve acSides, AnimationCurve acVertical)
        {
            this.SetDirectionalDirection(Vector3.zero);
            this.currentLocomotionSystem.RootMovement(
                impulse, duration, gravityInfluence,
                acForward, acSides, acVertical
            );
        }

        public int Jump()
        {
            return this.Jump(this.jumpForce);
        }

        public int Jump(float jumpForce)
        {
            bool isGrounded = (
                this.characterController.isGrounded ||
                Time.time < this.lastGroundTime + JUMP_COYOTE_TIME
            );

            bool jumpDelay = this.lastJumpTime + this.timeBetweenJumps < Time.time;
            bool jumpNumber = isGrounded || this.jumpChain < this.jumpTimes;
            if (this.canJump && jumpNumber && jumpDelay)
            {
                this.verticalSpeed = jumpForce;
                this.lastJumpTime = Time.time;
                if (this.character.onJump != null)
                {
                    this.character.onJump.Invoke(this.jumpChain);
                }

                this.jumpChain++;

                return this.jumpChain;
            }

            return -1;
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            this.Teleport(position);
            this.character.transform.rotation = rotation;

            Vector3 direction = rotation * Vector3.forward;

            this.SetDirectionalDirection(direction);
            this.currentLocomotionSystem.movementDirection = direction;
        }

        public void Teleport(Vector3 position)
        {
            switch (this.currentLocomotionType)
            {
                case CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController:
                    this.character.transform.position = position;
                    break;

                case CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent:
                    this.character.transform.position = position;
                    this.character.characterLocomotion.navmeshAgent.Warp(position);
                    break;
            }
        }

        public void SetAnimatorConstraint(ANIM_CONSTRAINT constraint)
        {
            this.animatorConstraint = constraint;
        }

        public void ChangeHeight(float height)
        {
            if (this.characterController != null)
            {
                this.characterController.height = height;
                this.characterController.center = Vector3.up * (height / 2.0f);
            }

            if (this.navmeshAgent != null)
            {
                this.navmeshAgent.height = height;
            }
        }

        public void SetIsControllable(bool isControllable)
        {
            if (isControllable == this.isControllable) return;
            this.isControllable = isControllable;

            if (!isControllable) this.SetDirectionalDirection(Vector3.zero);
            if (this.character.onIsControllable != null)
            {
                this.character.onIsControllable.Invoke(this.isControllable);
            }
        }

        public Vector3 GetAimDirection()
        {
            return this.currentLocomotionSystem.aimDirection;
        }

        public Vector3 GetMovementDirection()
        {
            return this.currentLocomotionSystem.movementDirection;
        }

        // PUBLIC LOCOMOTION METHODS: -------------------------------------------------------------

        public void SetDirectionalDirection(Vector3 direction, ILocomotionSystem.TargetRotation rotation = null)
        {
            this.ChangeLocomotionSystem<LocomotionSystemDirectional>();
            ((LocomotionSystemDirectional)this.currentLocomotionSystem).SetDirection(direction, rotation);
        }

        public void SetTankDirection(Vector3 direction, float rotationY)
        {
            this.ChangeLocomotionSystem<LocomotionSystemTank>();
            ((LocomotionSystemTank)this.currentLocomotionSystem).SetDirection(
                direction,
                rotationY
            );
        }

        public void SetTarget(Ray ray, LayerMask layerMask, ILocomotionSystem.TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            this.ChangeLocomotionSystem<LocomotionSystemTarget>();
            ((LocomotionSystemTarget)this.currentLocomotionSystem)
                .SetTarget(ray, layerMask, rotation, stopThreshold, callback);
        }

        public void SetTarget(Vector3 position, ILocomotionSystem.TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            this.ChangeLocomotionSystem<LocomotionSystemTarget>();
            ((LocomotionSystemTarget)this.currentLocomotionSystem)
                .SetTarget(position, rotation, stopThreshold, callback);
        }

        public void FollowTarget(Transform target, float minRadius, float maxRadius)
        {
            this.ChangeLocomotionSystem<LocomotionSystemFollow>();
            ((LocomotionSystemFollow)this.currentLocomotionSystem).SetFollow(target, minRadius, maxRadius);
        }

        public void Stop(ILocomotionSystem.TargetRotation rotation = null, UnityAction callback = null)
        {
            this.ChangeLocomotionSystem<LocomotionSystemTarget>();
            ((LocomotionSystemTarget)this.currentLocomotionSystem).Stop(rotation, callback);
        }

        public void SetRotation(Vector3 direction)
        {
            this.ChangeLocomotionSystem<LocomotionSystemRotation>();
            ((LocomotionSystemRotation)this.currentLocomotionSystem).SetDirection(direction);
        }

        public void UseGravity(bool useGravity)
        {
            this.useGravity = useGravity;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void GenerateNavmeshAgent()
        {
            if (!this.canUseNavigationMesh) return;

            if (this.navmeshAgent == null) this.navmeshAgent = this.character.gameObject.GetComponent<NavMeshAgent>();
            if (this.navmeshAgent == null) this.navmeshAgent = this.character.gameObject.AddComponent<NavMeshAgent>();

            this.navmeshAgent.updatePosition = false;
            this.navmeshAgent.updateRotation = false;
            this.navmeshAgent.updateUpAxis = false;
            this.navmeshAgent.radius = this.characterController.radius;
            this.navmeshAgent.height = this.characterController.height;
            this.navmeshAgent.acceleration = ACCELERATION;
        }

        private void ChangeLocomotionSystem<TLS>() where TLS : ILocomotionSystem, new()
        {
            if (this.currentLocomotionSystem != null && typeof(TLS) == this.currentLocomotionSystem.GetType()) return;
            if (this.currentLocomotionSystem != null) this.currentLocomotionSystem.OnDestroy();

            this.currentLocomotionSystem = new TLS();
            this.currentLocomotionSystem.Setup(this);
        }

        private void UpdateVerticalSpeed(bool isGrounded)
        {
            this.verticalSpeed += this.useGravity ? this.gravity * Time.deltaTime : 0f;
            if (isGrounded)
            {
                if (Time.time - this.lastGroundTime > JUMP_COYOTE_TIME + Time.deltaTime &&
                    this.character.onLand != null)
                {
                    this.character.onLand.Invoke(this.verticalSpeed);
                }

                this.jumpChain = 0;
                this.lastGroundTime = Time.time;
                this.verticalSpeed = Mathf.Max(this.verticalSpeed, MAX_GROUND_VSPEED);
            }

            this.verticalSpeed = this.gravity <= 0 
                ? Mathf.Max(this.verticalSpeed, -Mathf.Abs(this.maxFallSpeed)) 
                : Mathf.Min(this.verticalSpeed, Mathf.Abs(this.maxFallSpeed));
        }

        private void UpdateCharacterState(LOCOMOTION_SYSTEM locomotionSystem)
        {
            Vector3 worldVelocity = Vector3.zero;
            bool isSliding = this.currentLocomotionSystem.isSliding;
            bool isGrounded = true;

            switch (locomotionSystem)
            {
                case LOCOMOTION_SYSTEM.CharacterController:
                    worldVelocity = this.characterController.velocity;
                    isGrounded = (
                        this.characterController.isGrounded ||
                        Time.time - this.lastGroundTime < GROUND_TIME_OFFSET
                    );
                    break;

                case LOCOMOTION_SYSTEM.NavigationMeshAgent:
                    worldVelocity = (this.navmeshAgent.velocity == Vector3.zero
                        ? this.characterController.velocity
                        : this.navmeshAgent.velocity
                    );
                    isGrounded = (
                        !this.navmeshAgent.isOnOffMeshLink ||
                        Time.time - this.lastGroundTime < GROUND_TIME_OFFSET
                    );
                    break;
            }

            Vector3 localVelocity = this.character.transform.InverseTransformDirection(worldVelocity);
            this.character.characterState.forwardSpeed = localVelocity;
            this.character.characterState.sidesSpeed = Mathf.Atan2(localVelocity.x, localVelocity.z);
            this.character.characterState.verticalSpeed = worldVelocity.y;

            this.character.characterState.pivotSpeed = this.currentLocomotionSystem.pivotSpeed;

            this.character.characterState.isGrounded = isGrounded ? 1f : 0f;
            this.character.characterState.isSliding = isSliding ? 1f : 0f;
            this.character.characterState.isDashing = this.currentLocomotionSystem.isDashing ? 1f : 0f;
            this.character.characterState.normal = this.terrainNormal;
        }
    }
}
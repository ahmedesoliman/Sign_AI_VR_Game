namespace GameCreator.Characters
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core;
    using System.Runtime.CompilerServices;

    [AddComponentMenu("Game Creator/Characters/Character Animator", 100)]
    public class CharacterAnimator : MonoBehaviour
    {
        private const float NORMAL_SMOOTH = 0.1f;
        private const float MAX_LAND_FORCE_SPEED = -10.0f;
        private const float FLOAT_ERROR_MARGIN = 0.01f;

        public const string LAYER_BASE = "Base";

        private const string EXC_NO_CHARACTER = "No CharacterNavigatorController found on gameObject";
        private const string EXC_NO_ANIMATOR = "No Animator attached to CharacterNavigationAnimator";

        private class AnimFloat
        {
            private bool setup;
            private float value;
            private float velocity;

            public float Get(float target, float smooth)
            {
                if (!this.setup)
                {
                    this.value = target;
                    this.velocity = 0.0f;
                    this.setup = true;
                }

                this.value = Mathf.SmoothDamp(
                    this.value,
                    target,
                    ref velocity,
                    smooth
                );

                if (this.value < FLOAT_ERROR_MARGIN && this.value > -FLOAT_ERROR_MARGIN)
                {
                    this.value = 0f;
                }

                return this.value;
            }

            public void Set(float value)
            {
                this.value = value;
                this.velocity = 0.0f;
            }
        }

        public class EventIK : UnityEvent<int> { }

		// PROPERTIES: ----------------------------------------------------------------------------

		public Animator animator;
		private Character character;

        [SerializeField]
        protected CharacterState defaultState;

        private CharacterAnimatorEvents animEvents;
        private CharacterAnimation characterAnimation;
        private CharacterAttachments characterAttachments;
        private CharacterAnimatorRotation characterRotation;

		private CharacterHeadTrack headTrack;
        private CharacterFootIK footIK;
        private CharacterHandIK handIK;

        private bool stiffBody = false;

        private static readonly int HASH_MOVE_FORWARD_SPEED = Animator.StringToHash("MoveForward");
        private static readonly int HASH_MOVE_SIDES_SPEED = Animator.StringToHash("MoveSides");
        private static readonly int HASH_MOVE_TURN_SPEED = Animator.StringToHash("TurnSpeed");
        private static readonly int HASH_MOVEMENT_SPEED = Animator.StringToHash("Movement");
        private static readonly int HASH_IS_GROUNDED = Animator.StringToHash("IsGrounded");
        private static readonly int HASH_IS_SLIDING = Animator.StringToHash("IsSliding");
        private static readonly int HASH_IS_DASHING = Animator.StringToHash("IsDashing");
        private static readonly int HASH_VERTICAL_SPEED = Animator.StringToHash("VerticalSpeed");
        private static readonly int HASH_NORMAL_X = Animator.StringToHash("NormalX");
        private static readonly int HASH_NORMAL_Y = Animator.StringToHash("NormalY");
        private static readonly int HASH_NORMAL_Z = Animator.StringToHash("NormalZ");
        private static readonly int HASH_JUMP = Animator.StringToHash("Jump");
        private static readonly int HASH_JUMP_CHAIN = Animator.StringToHash("JumpChain");
        private static readonly int HASH_LAND = Animator.StringToHash("Land");
        private static readonly int HASH_LAND_FORCE = Animator.StringToHash("LandForce");
        private static readonly int HASH_TIME_SCALE = Animator.StringToHash("TimeScale");

        private Dictionary<int, AnimFloat> paramValues = new Dictionary<int, AnimFloat>();
        private float rotationVelocity;

        public bool useFootIK = true;
        public LayerMask footLayerMask = Physics.DefaultRaycastLayers;
        public bool useHandIK = true;
        public bool useSmartHeadIK = true;
        public bool useProceduralLanding = true;

        public bool autoInitializeRagdoll = true;
        [Tooltip("Total amount of mass of the character")]
        public float ragdollMass = 80f;

        [Tooltip("Time needed to confirm the ragdoll is stable before getting up"), Range(0.1f, 5.0f)]
        public float stableTimeout = 0.5f;

        public AnimationClip standFaceDown;
        public AnimationClip standFaceUp;

        public Action overrideLateUpdate;

        [Range(0f, 1f)]
        public float timeScaleCoefficient = 1f;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void Awake()
		{
            if (this.animator != null) this.animator.applyRootMotion = false;

            this.character = gameObject.GetComponent<Character>();
            this.characterAnimation = new CharacterAnimation(this, this.defaultState);
            this.characterRotation = new CharacterAnimatorRotation();

            this.paramValues.Add(HASH_MOVE_FORWARD_SPEED, new AnimFloat());
            this.paramValues.Add(HASH_MOVE_SIDES_SPEED, new AnimFloat());
            this.paramValues.Add(HASH_MOVE_TURN_SPEED, new AnimFloat());
            this.paramValues.Add(HASH_MOVEMENT_SPEED, new AnimFloat());
            this.paramValues.Add(HASH_VERTICAL_SPEED, new AnimFloat());
            this.paramValues.Add(HASH_NORMAL_X, new AnimFloat());
            this.paramValues.Add(HASH_NORMAL_Y, new AnimFloat());
            this.paramValues.Add(HASH_NORMAL_Z, new AnimFloat());
            this.paramValues.Add(HASH_LAND_FORCE, new AnimFloat());
            this.paramValues.Add(HASH_IS_GROUNDED, new AnimFloat());
            this.paramValues.Add(HASH_IS_SLIDING, new AnimFloat());
            this.paramValues.Add(HASH_IS_DASHING, new AnimFloat());
        }

        private void Start()
        {
            this.character.onLand.AddListener(this.OnLand);
        }

        private void OnDestroy()
        {
            if (this.characterAnimation != null) this.characterAnimation.OnDestroy();
            if (this.animator != null) Destroy(this.animator.gameObject);
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
		{
            if (!this.animator.gameObject.activeInHierarchy) return;

			if (this.character  == null) throw new UnityException(EXC_NO_CHARACTER);
			if (this.animator   == null) throw new UnityException(EXC_NO_ANIMATOR);
			if (this.animEvents == null) this.GenerateAnimatorEvents();

            if (this.characterAttachments == null) this.GenerateCharacterAttachments();
            if (this.characterAnimation != null) this.characterAnimation.Update();

            if (this.useFootIK && this.footIK == null) this.GenerateFootIK();
            if (this.useHandIK && this.handIK == null) this.GenerateHandIK();
            if (this.useSmartHeadIK && this.headTrack == null)
            {
                if (this.GetHeadTracker() != null) this.headTrack.Untrack();
            }

            Quaternion rotation = this.characterRotation.Update();

			Character.State state = this.character.GetCharacterState();
            Vector3 direction = (!this.character.enabled || state.forwardSpeed.magnitude < 0.01f
                ? Vector3.zero
                : state.forwardSpeed
            );

            direction = Quaternion.Euler(0f, -rotation.eulerAngles.y, 0f) * direction;

            switch (this.character.IsRagdoll())
            {
                case true:
                    rotation = this.animator.transform.localRotation;
                    break;

                case false:
                    rotation.eulerAngles = new Vector3(
                        rotation.eulerAngles.x,
                        Mathf.SmoothDampAngle(
                            this.animator.transform.localRotation.eulerAngles.y,
                            this.character.IsRagdoll()
                                ? this.animator.transform.localRotation.eulerAngles.y
                                : rotation.eulerAngles.y,
                            ref this.rotationVelocity, 1f
                        ),
                        rotation.eulerAngles.z
                    );
                    break;
            }

            this.animator.transform.localRotation = rotation;
            direction = Vector3.Scale(direction, Vector3.one * (1.0f / this.character.characterLocomotion.runSpeed));

            float paramMoveForwardSpeed = this.paramValues[HASH_MOVE_FORWARD_SPEED].Get(direction.z, 0.1f);
            float paramMoveSidesSpeed = this.paramValues[HASH_MOVE_SIDES_SPEED].Get(direction.x, 0.2f);
            float paramMovementSpeed = this.paramValues[HASH_MOVEMENT_SPEED].Get(
                Vector3.Scale(direction, new Vector3(1,0,1)).magnitude,
                0.1f
            );

            float paramMoveTurnSpeed = this.paramValues[HASH_MOVE_TURN_SPEED].Get(state.pivotSpeed, 0.1f);
            float paramVerticalSpeed = this.paramValues[HASH_VERTICAL_SPEED].Get(state.verticalSpeed, 0.2f);
            float paramIsGrounded = this.paramValues[HASH_IS_GROUNDED].Get(state.isGrounded, 0.1f);
            float paramIsSliding = this.paramValues[HASH_IS_SLIDING].Get(state.isSliding, 0.1f);
            float paramIsDashing = this.paramValues[HASH_IS_DASHING].Get(state.isDashing, 0.05f);
            float paramLandForce = this.paramValues[HASH_LAND_FORCE].Get(0f, 2f);

            this.animator.SetFloat(HASH_MOVE_FORWARD_SPEED, paramMoveForwardSpeed);
            this.animator.SetFloat(HASH_MOVE_SIDES_SPEED, paramMoveSidesSpeed);
            this.animator.SetFloat(HASH_MOVE_TURN_SPEED, paramMoveTurnSpeed);
            this.animator.SetFloat(HASH_MOVEMENT_SPEED, paramMovementSpeed);
            this.animator.SetFloat(HASH_IS_GROUNDED, paramIsGrounded);
            this.animator.SetFloat(HASH_IS_SLIDING, paramIsSliding);
            this.animator.SetFloat(HASH_IS_DASHING, paramIsDashing);
            this.animator.SetFloat(HASH_VERTICAL_SPEED, paramVerticalSpeed);
            this.animator.SetFloat(HASH_TIME_SCALE, Time.timeScale * this.timeScaleCoefficient);
            this.animator.SetFloat(HASH_LAND_FORCE, paramLandForce);

            this.Normals(state);
		}

        private void Normals(Character.State state)
        {
            Vector3 normal = Vector3.up;
            if (Mathf.Approximately(state.isGrounded, 1.0f))
            {
                normal = this.character.transform.InverseTransformDirection(state.normal);
            }

            float paramNormalX = this.paramValues[HASH_NORMAL_X].Get(normal.x, NORMAL_SMOOTH);
            float paramNormalY = this.paramValues[HASH_NORMAL_Y].Get(normal.y, NORMAL_SMOOTH);
            float paramNormalZ = this.paramValues[HASH_NORMAL_Z].Get(normal.z, NORMAL_SMOOTH);

            this.animator.SetFloat(HASH_NORMAL_X, paramNormalX);
            this.animator.SetFloat(HASH_NORMAL_Y, paramNormalY);
            this.animator.SetFloat(HASH_NORMAL_Z, paramNormalZ);
        }

        private void LateUpdate()
        {
            if (this.overrideLateUpdate != null)
            {
                this.overrideLateUpdate.Invoke();
                return;
            }

            if (this.stiffBody)
            {
                Transform spine = this.animator.GetBoneTransform(HumanBodyBones.Spine);
                Transform hips = this.animator.GetBoneTransform(HumanBodyBones.Hips);

                spine.localRotation = (
                    spine.localRotation *
                    Quaternion.Inverse(hips.localRotation)
                );

                this.animator.GetBoneTransform(HumanBodyBones.Chest).localRotation = Quaternion.identity;
                this.animator.GetBoneTransform(HumanBodyBones.UpperChest).localRotation = Quaternion.identity;
                this.animator.GetBoneTransform(HumanBodyBones.Neck).localRotation = Quaternion.identity;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public CharacterHeadTrack GetHeadTracker()
		{
			if (this.headTrack == null)
			{
				this.headTrack = gameObject.GetComponentInChildren<CharacterHeadTrack>();
				if (this.headTrack == null && this.animator != null && this.animator.isHuman)
				{
					this.headTrack = this.animator.gameObject.AddComponent<CharacterHeadTrack>();
				}
			}

			return this.headTrack;
		}

		public void Jump(int jumpChain = 0)
		{
            this.animator.SetInteger(HASH_JUMP_CHAIN, jumpChain);
			this.animator.SetTrigger(HASH_JUMP);
		}

        public void Dash()
        {
            this.paramValues[HASH_IS_DASHING].Set(1f);
        }

        public Transform GetHeadTransform()
        {
            if (!this.animator.isHuman) return transform;
            Transform head = this.animator.GetBoneTransform(HumanBodyBones.Head);
            return head ?? transform;
        }

        public void PlayGesture(AnimationClip clip, float speed, AvatarMask avatarMask = null,
            float transitionIn = 0.15f, float transitionOut = 0.15f)
        {
            this.characterAnimation.PlayGesture(clip, avatarMask, transitionIn, transitionOut, speed);
        }

        public void CrossFadeGesture(AnimationClip clip, float speed, AvatarMask avatarMask = null,
            float transitionIn = 0.15f, float transitionOut = 0.15f)
        {
            this.characterAnimation.CrossFadeGesture(
                clip, avatarMask,
                transitionIn, transitionOut, speed
            );
        }
        
        public void CrossFadeGesture(RuntimeAnimatorController rtc, float speed, AvatarMask avatarMask = null,
            float transitionIn = 0.15f, float transitionOut = 0.15f, 
            params PlayableGesture.Parameter[] parameters)
        {
            this.characterAnimation.CrossFadeGesture(
                rtc, avatarMask, 
                transitionIn, transitionOut, speed,
                parameters
            );
        }

        public void StopGesture(float transitionOut = 0.0f)
        {
            this.characterAnimation.StopGesture(transitionOut);
        }

        public void SetState(CharacterState state, AvatarMask avatarMask,
                             float weight, float time, float speed, CharacterAnimation.Layer layer,
                             params PlayableGesture.Parameter[] parameters)
        {
            this.characterAnimation.SetState(
                state, avatarMask, weight, time, speed, (int)layer, parameters
            );
        }

        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask,
                             float weight, float time, float speed,
                             CharacterAnimation.Layer layer, bool syncTime = false,
                             params PlayableGesture.Parameter[] parameters)
        {
            this.characterAnimation.SetState(
                rtc, avatarMask, weight, time, speed, (int)layer, syncTime, parameters
            );
        }

        public void SetState(AnimationClip clip, AvatarMask avatarMask,
                             float weight, float time, float speed, CharacterAnimation.Layer layer)
        {
            this.characterAnimation.SetState(clip, avatarMask, weight, time, speed, (int)layer);
        }

        public void ResetState(float time, CharacterAnimation.Layer layer)
        {
            this.characterAnimation.ResetState(time, (int)layer);
        }

        public void ChangeStateWeight(CharacterAnimation.Layer layer, float weight)
        {
            this.characterAnimation.ChangeStateWeight((int)layer, weight);
        }

        public void ResetControllerTopology(RuntimeAnimatorController runtimeController)
        {
            this.characterAnimation.ChangeRuntimeController(runtimeController);
        }

        public CharacterAttachments GetCharacterAttachments()
        {
            return this.characterAttachments;
        }

        public void SetCharacterAttachments(CharacterAttachments attachments)
        {
            this.characterAttachments = attachments;
        }

        public CharacterHandIK GetCharacterHandIK()
        {
            return this.handIK;
        }

        public CharacterState GetState(CharacterAnimation.Layer layer)
        {
            return this.characterAnimation.GetState((int)layer);
        }

        public void ChangeModel(GameObject prefabModel)
        {
            RuntimeAnimatorController runtimeController = null;
            Dictionary<HumanBodyBones, List<CharacterAttachments.Attachment>> attachments =
                new Dictionary<HumanBodyBones, List<CharacterAttachments.Attachment>>();

            if (this.characterAttachments != null)
            {
                attachments = this.characterAttachments.attachments;
            }

            if (this.animator != null)
            {
                runtimeController = this.animator.runtimeAnimatorController;
                Destroy(this.animator.gameObject);
            }

            GameObject instance = Instantiate<GameObject>(prefabModel, transform);
            instance.name = prefabModel.name;

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            Animator instanceAnimator = instance.GetComponent<Animator>();
            if (instanceAnimator != null)
            {
                this.animator = instanceAnimator;
                this.animator.applyRootMotion = false;
                this.ResetControllerTopology(runtimeController);
            }

            if (this.autoInitializeRagdoll)
            {
                this.character.InitializeRagdoll();
            }

            this.GenerateCharacterAttachments();
            foreach (KeyValuePair<HumanBodyBones, List<CharacterAttachments.Attachment>> item in attachments)
            {
                List<CharacterAttachments.Attachment> list = new List<CharacterAttachments.Attachment>(item.Value);
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].prefab == null) continue;
                    this.characterAttachments.Attach(
                        item.Key, list[i].prefab,
                        list[i].locPosition,
                        list[i].locRotation
                    );
                }
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            this.characterRotation.SetQuaternion(rotation);
        }

        public void SetRotationPitch(float value)
        {
            this.characterRotation.SetPitch(value);
        }

        public void SetRotationYaw(float value)
        {
            this.characterRotation.SetYaw(value);
        }

        public void SetRotationRoll(float value)
        {
            this.characterRotation.SetRoll(value);
        }

        public Quaternion GetCurrentRotation()
        {
            return this.characterRotation.GetCurrentRotation();
        }

        public Quaternion GetTargetRotation()
        {
            return this.characterRotation.GetTargetRotation();
        }

        public void SetVisibility(bool visible)
        {
            this.animator.gameObject.SetActive(visible);
        }

        public void SetStiffBody(bool stiffBody)
        {
            this.stiffBody = stiffBody;
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnLand(float verticalSpeed)
        {
            if (!this.useProceduralLanding) return;

            float force = Mathf.InverseLerp(0f, MAX_LAND_FORCE_SPEED, verticalSpeed);
            this.paramValues[HASH_LAND_FORCE].Set(force);
            this.animator.SetTrigger(HASH_LAND);
        }

		private void GenerateAnimatorEvents()
		{
			this.animEvents = this.animator.gameObject.AddComponent<CharacterAnimatorEvents>();
			this.animEvents.Setup(this.character);
		}

        private void GenerateCharacterAttachments()
        {
            this.characterAttachments = this.animator.gameObject.AddComponent<CharacterAttachments>();
            this.characterAttachments.Setup(this.animator);
        }

        private void GenerateFootIK()
        {
            if (this.animator != null && this.animator.isHuman)
            {
                this.footIK = this.animator.gameObject.AddComponent<CharacterFootIK>();
                this.footIK.Setup(this.character);
            }
        }

        private void GenerateHandIK()
        {
            if (this.animator != null && this.animator.isHuman)
            {
                this.handIK = this.animator.gameObject.AddComponent<CharacterHandIK>();
                this.handIK.Setup(this.character);
            }
        }
	}
}

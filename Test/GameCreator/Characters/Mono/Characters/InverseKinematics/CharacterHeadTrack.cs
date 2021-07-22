namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class CharacterHeadTrack : MonoBehaviour 
	{
        private const float AIM_DISTANCE = 10f;
        private const float VISION_ANGLE = 220f;

		private const float MOVE_HEAD_SPEED = 0.15f;

        private const float MIN_WEIGHT = 0.0f;
		private const float MAX_WEIGHT = 0.8f;

		private class TrackInfo
		{
			public enum TrackState
			{
				TRACKING_POSITION,
				NOT_TRACKING
			}

			public TrackState currentTrackState;
			public Vector3 currentPosition;
			public float currentWeight;
            private float moveHeadSpeed = MOVE_HEAD_SPEED;

            private Vector3 _positionSpeed = Vector3.zero;
            private float _weightSpeed = 0.0f;

            public Transform headTransform;
            public Character character;
            public CharacterAnimator characterAnimator;

            public TrackInfo(Character character)
			{
                this.character = character;
                this.characterAnimator = this.character.GetCharacterAnimator();

                if (this.characterAnimator != null)
                {
                    this.headTransform = this.characterAnimator.GetHeadTransform();
                }

				this.currentTrackState = TrackState.NOT_TRACKING;
				this.currentPosition = Vector3.zero;
				this.currentWeight = 0.0f;

                this._positionSpeed = Vector3.zero;
                this._weightSpeed = 0f;
			}

			public void UpdateInfo(TrackState nextTrackState, Vector3 targetPosition = default(Vector3))
			{
                float targetWeight;

                if (nextTrackState == TrackState.NOT_TRACKING)
                {
                    Vector3 direction = this.character.transform.TransformDirection(Vector3.forward);
                    if (this.headTransform != null && this.characterAnimator.useSmartHeadIK)
                    {
                        Vector3 aimDirection = this.character.characterLocomotion.GetAimDirection();
                        if (aimDirection != Vector3.zero &&
                            Vector3.Angle(aimDirection, direction) < VISION_ANGLE/2f)
                        {
                            direction = aimDirection;
                        }
                    }

                    targetWeight = MAX_WEIGHT;
                    targetPosition = this.headTransform.position + (direction * AIM_DISTANCE);
                }
                else
                {
                    targetWeight = MAX_WEIGHT;
                }

                this.currentPosition = Vector3.SmoothDamp(
                    this.currentPosition,
                    targetPosition,
                    ref this._positionSpeed,
                    this.moveHeadSpeed
                );

                this.currentWeight = Mathf.SmoothDamp(
                    this.currentWeight,
                    targetWeight,
                    ref this._weightSpeed,
                    this.moveHeadSpeed
                );

                this.currentTrackState = nextTrackState;
            }

			public void ChangeTrackTarget(float moveHeadSpeed = MOVE_HEAD_SPEED)
			{
                this.moveHeadSpeed = moveHeadSpeed;
                this._weightSpeed = 0.0f;
                this._positionSpeed = Vector3.zero;
			}
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		private Animator animator;
		private TrackInfo trackInfo;

		private Target headTarget;

        public CharacterAnimator.EventIK eventBeforeIK = new CharacterAnimator.EventIK();
        public CharacterAnimator.EventIK eventAfterIK = new CharacterAnimator.EventIK();

        // MAIN METHODS: --------------------------------------------------------------------------

        private void Awake()
		{
			this.animator = gameObject.GetComponentInChildren<Animator>();
			if (this.animator == null || !this.animator.isHuman) return;

            Character character = gameObject.GetComponentInParent<Character>();
            this.trackInfo = new TrackInfo(character);
			this.headTarget = new Target();
		}

		private void OnAnimatorIK (int layerIndex) 
		{
            if (this.animator == null || !this.animator.isHuman) return;
            this.eventBeforeIK.Invoke(layerIndex);

			if (this.headTarget == null || !this.headTarget.HasTarget()) 
			{
                this.trackInfo.UpdateInfo(TrackInfo.TrackState.NOT_TRACKING);
			}
			else
			{
				this.trackInfo.UpdateInfo(TrackInfo.TrackState.TRACKING_POSITION, this.headTarget.GetPosition());
			}

			this.animator.SetLookAtPosition(this.trackInfo.currentPosition);
			this.animator.SetLookAtWeight(this.trackInfo.currentWeight);

            this.eventAfterIK.Invoke(layerIndex);
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void Track(Vector3 position, float headSpeed = MOVE_HEAD_SPEED)
		{
			this.headTarget = new TargetPosition(position);
			this.trackInfo.ChangeTrackTarget(headSpeed);
		}

		public void Track(Transform transform, float headSpeed = MOVE_HEAD_SPEED)
		{
			this.headTarget = new TargetTransform(transform);
			this.trackInfo.ChangeTrackTarget(headSpeed);
		}

		public void TrackPlayer(float headSpeed = MOVE_HEAD_SPEED)
		{
			this.headTarget = new TargetPlayer();
			this.trackInfo.ChangeTrackTarget(headSpeed);
		}

		public void TrackCamera(float headSpeed = MOVE_HEAD_SPEED)
		{
			this.headTarget = new TargetCamera();
			this.trackInfo.ChangeTrackTarget(headSpeed);
		}

		public void Untrack(float headSpeed = MOVE_HEAD_SPEED)
		{
			this.headTarget = new Target();
			this.trackInfo.ChangeTrackTarget(headSpeed);
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// TARGET CLASSES: ------------------------------------------------------------------------

		private class Target
		{
			public virtual Vector3 GetPosition() { return Vector3.zero; }
			public virtual bool HasTarget() { return false; }
		}

		private class TargetPosition : Target
		{
			public Vector3 position = Vector3.zero;

			public TargetPosition(Vector3 position)
			{
				this.position = position;
			}

			public override bool HasTarget ()
			{
				return true;
			}

			public override Vector3 GetPosition()
			{
				return this.position;
			}
		}

		private class TargetTransform : Target
		{
			public Transform transform = null;

			public TargetTransform(Transform transform = null)
			{
				this.transform = transform;

                if (transform != null)
                {
                    CharacterAnimator charAnimator = transform.GetComponent<CharacterAnimator>();
                    if (charAnimator != null) this.transform = charAnimator.GetHeadTransform();
                }
			}

			public override bool HasTarget ()
			{
				return (this.transform != null);
			}

			public override Vector3 GetPosition()
			{
				if (this.transform == null) return Vector3.zero;
				return this.transform.position;
			}
		}

		private class TargetPlayer : TargetTransform
		{
			public TargetPlayer() : base()
			{
				if (HookPlayer.Instance == null) return;

                Transform target = HookPlayer.Instance.transform;
                if (HookPlayer.Instance.Get<CharacterAnimator>() != null)
                {
                    target = HookPlayer.Instance.Get<CharacterAnimator>().GetHeadTransform();
                }

				this.transform = target;
			}
		}

		private class TargetCamera : TargetTransform
		{
			public TargetCamera() : base()
			{
				if (HookCamera.Instance == null) return;
				this.transform = HookCamera.Instance.transform;
			}
		}

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GIZMOS: --------------------------------------------------------------------------------

        void OnDrawGizmosSelected()
        {
            if (this.trackInfo.characterAnimator != null && this.trackInfo.characterAnimator.useSmartHeadIK)
            {
                float radius = 0.3f;
                Vector3 position = this.trackInfo.headTransform.position;
                Vector3 direction = this.trackInfo.character.characterLocomotion.GetAimDirection();

                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f);
                Gizmos.DrawWireSphere(position, radius);

                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.75f);
                Gizmos.DrawCube(position + (direction * radius), Vector3.one * 0.02f);
            }
        }
    }
}
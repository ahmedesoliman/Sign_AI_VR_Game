namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;
    using GameCreator.Characters;
    using GameCreator.Variables;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeFirstPerson : ICameraMotorType 
	{
        public enum RotateInput
        {
            MouseMove,
            HoldLeftMouse,
            HoldRightMouse,
            HoldMiddleMouse
        }

        public enum ModelManipulator
        {
            None,
            Hide3DModel,
            StiffSpineAnimation
        }

        private const string INPUT_MOUSE_X = "Mouse X";
        private const string INPUT_MOUSE_Y = "Mouse Y";
        private const string INPUT_MOUSE_W = "Mouse ScrollWheel";

        public static Rect MOBILE_RECT = new Rect(0.5f, 0.0f, 0.5f, 1.0f);

        public static new string NAME = "First Person Camera";

		// PROPERTIES: ----------------------------------------------------------------------------

		public Vector3 positionOffset = new Vector3(0f, 2f, 0f);

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;

        private float targetRotationX = 0.0f;
		private float targetRotationY = 0.0f;

		private float rotationXVelocity = 0.0f;
		private float rotationYVelocity = 0.0f;
		private PlayerCharacter player;
        private CursorLockMode cursorLock = CursorLockMode.None;

        public RotateInput rotateInput = RotateInput.MouseMove;
        public Vector2Property sensitivity = new Vector2Property(Vector2.one * 2f);

		[Range(0.0f, 180f)] 
		public float maxPitch = 120f;

		[Range(0f, 1f)]
		public float smoothRotation = 0.1f;

        public float headbobPeriod = 0.25f;
        public Vector3 headbobAmount = new Vector3(0.1f, 0.05f, 0.01f);

        private float headbobLerpAmount;
        private float headbobLerpVelocity;

        public ModelManipulator modelManipulator = ModelManipulator.Hide3DModel;

		// OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void EnableMotor()
        {
            Transform target = this.GetTarget();

            if (target != null)
            {
                this.targetRotationX = target.transform.rotation.eulerAngles.y;
                this.targetRotationY = target.transform.rotation.eulerAngles.x;

                this.rotationX = this.targetRotationX;
                this.rotationY = this.targetRotationY;
            }

            CharacterAnimator animator = HookPlayer.Instance.Get<CharacterAnimator>();
            switch (this.modelManipulator)
            {
                case ModelManipulator.Hide3DModel:
                    animator.SetVisibility(false);
                    break;

                case ModelManipulator.StiffSpineAnimation:
                    animator.SetStiffBody(true);
                    break;
            }

            this.cursorLock = Cursor.lockState;
        }

        public override void DisableMotor()
        {
            CharacterAnimator animator = HookPlayer.Instance.Get<CharacterAnimator>();
            switch (this.modelManipulator)
            {
                case ModelManipulator.Hide3DModel:
                    animator.SetVisibility(true);
                    break;

                case ModelManipulator.StiffSpineAnimation:
                    animator.SetStiffBody(false);
                    break;
            }
        }

        public override Vector3 GetPosition(CameraController camera, bool withoutSmoothing = false)
		{
			Transform target = this.GetTarget();
			if (target == null) return base.GetPosition(camera);

            Vector3 position = target.position + target.TransformDirection(this.positionOffset);
            return position + target.TransformDirection(this.Headbob());
		}

		public override Vector3 GetDirection (CameraController camera, bool withoutSmoothing = false)
		{
			if (HookPlayer.Instance != null)
			{
				if (this.player == null) this.player = HookPlayer.Instance.Get<PlayerCharacter>();
				if (!this.player.IsControllable()) return camera.transform.TransformDirection(Vector3.forward);
			}

            if (withoutSmoothing)
            {
                Transform target = this.GetTarget();
                if (target != null)
                {
                    return (
                        Quaternion.AngleAxis(target.transform.rotation.eulerAngles.y, Vector3.up) *
                        Quaternion.AngleAxis(target.transform.rotation.eulerAngles.x, Vector3.left) *
                        Vector3.forward
                    );
                }
            }

            this.rotationX = Mathf.SmoothDampAngle(
                this.rotationX, this.targetRotationX,
                ref this.rotationXVelocity, this.smoothRotation
            );

            this.rotationY = Mathf.SmoothDampAngle(
                this.rotationY, this.targetRotationY,
                ref this.rotationYVelocity, this.smoothRotation
            );

            this.RotationInput();

            if (float.IsNaN(this.rotationX)) this.rotationX = this.targetRotationX;
            if (float.IsNaN(this.rotationY)) this.rotationY = this.targetRotationY;

			Quaternion quaternionX = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion quaternionY = Quaternion.AngleAxis(this.rotationY, Vector3.left);

			return (quaternionX * quaternionY) * Vector3.forward;
		}

        public override bool UseSmoothRotation()
        {
            return false;
        }

        public override bool UseSmoothPosition()
        {
            return false;
        }

        public void AddRotation(float pitch, float yaw)
        {
            this.targetRotationY += yaw;
            this.targetRotationX += pitch;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Transform GetTarget()
		{
			if (HookPlayer.Instance != null) return HookPlayer.Instance.transform;
			return null;
		}

		private static float ClampAngle(float angle, float min, float max)
		{
            return Mathf.Clamp(angle % 360f, min, max);
		}

        private Vector3 Headbob()
        {
            if (HookPlayer.Instance == null) return Vector3.zero;

            PlayerCharacter playerCharacter = HookPlayer.Instance.Get<PlayerCharacter>();
            if (playerCharacter == null) return Vector3.zero;

            if (Mathf.Approximately(this.headbobPeriod, 0f)) return Vector3.zero;

            Character.State state = playerCharacter.GetCharacterState();
            if (!playerCharacter.IsGrounded()) return Vector3.zero;

            float speed = Mathf.Abs(state.forwardSpeed.magnitude);
            speed = (
                speed > playerCharacter.characterLocomotion.runSpeed - 0.1f ? 1f :
                speed > (playerCharacter.characterLocomotion.runSpeed / 2f) - 0.1f ? 0.5f :
                0f
            );

            this.headbobLerpAmount = Mathf.SmoothDamp(
                this.headbobLerpAmount, speed,
                ref this.headbobLerpVelocity, 0.1f
            );

            Vector3 headbobVector = new Vector3(
                Mathf.Sin((speed / this.headbobPeriod) * 2.0f * Time.time) * this.headbobAmount.x,
                Mathf.Sin((speed / this.headbobPeriod) * 1.0f * Time.time) * this.headbobAmount.y,
                Mathf.Sin((speed / this.headbobPeriod) * 1.0f * Time.time) * this.headbobAmount.z
            );

            return Vector3.Slerp(Vector3.zero, headbobVector, this.headbobLerpAmount);
        }

        private void RotationInput()
        {
            if (Application.isMobilePlatform)
            {
                Rect screenRect = new Rect(
                    Screen.width * MOBILE_RECT.x,
                    Screen.height * MOBILE_RECT.y,
                    Screen.width * MOBILE_RECT.width,
                    Screen.height * MOBILE_RECT.height
                );

                for (int i = 0; i < Input.touchCount; ++i)
                {
                    Touch touch = Input.touches[i];
                    if (touch.phase == TouchPhase.Moved && screenRect.Contains(touch.position))
                    {
                        Vector2 sensitivityValue = this.sensitivity.GetValue(gameObject);

                        this.targetRotationX += touch.deltaPosition.x / Screen.width * sensitivityValue.x * 10f * Time.timeScale;
                        this.targetRotationY += touch.deltaPosition.y / Screen.height * sensitivityValue.y * 10f * Time.timeScale;
                        break;
                    }
                }
            }
            else
            {
                bool inputConditions;
                if (this.rotateInput == RotateInput.MouseMove) inputConditions = true;
                else if (this.rotateInput == RotateInput.HoldLeftMouse && Input.GetMouseButton(0)) inputConditions = true;
                else if (this.rotateInput == RotateInput.HoldRightMouse && Input.GetMouseButton(1)) inputConditions = true;
                else if (this.rotateInput == RotateInput.HoldMiddleMouse && Input.GetMouseButton(2)) inputConditions = true;
                else inputConditions = false;

                if (inputConditions)
                {
                    float axisX = Input.GetAxisRaw(INPUT_MOUSE_X);
                    float axisY = Input.GetAxisRaw(INPUT_MOUSE_Y);

                    if (this.cursorLock != Cursor.lockState && !Mathf.Approximately(axisX + axisY, 0f))
                    {
                        this.cursorLock = Cursor.lockState;
                        return;
                    }

                    Vector2 sensitivityValue = this.sensitivity.GetValue(gameObject);
                    this.targetRotationX += axisX * sensitivityValue.x * Time.timeScale;
                    this.targetRotationY += axisY * sensitivityValue.y * Time.timeScale;
                }
            }

            this.targetRotationX %= 360f;
            this.targetRotationY %= 360f;

            this.targetRotationY = Mathf.Clamp(
                this.targetRotationY,
                -this.maxPitch / 2.0f,
                this.maxPitch / 2.0f
            );
        }
    }
}
 
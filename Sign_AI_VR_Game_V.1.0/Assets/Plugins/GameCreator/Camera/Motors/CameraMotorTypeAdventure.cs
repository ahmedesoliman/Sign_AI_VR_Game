namespace GameCreator.Camera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;
    using GameCreator.Characters;
    using System.Runtime.CompilerServices;
    using GameCreator.Variables;

    [AddComponentMenu("")]
    [System.Serializable]
    public class CameraMotorTypeAdventure : ICameraMotorType
    {
        public enum OrbitInput
        {
            MouseMove,
            HoldLeftMouse,
            HoldRightMouse,
            HoldMiddleMouse
        }

        public enum InitDirection
        {
            TargetDirection,
            MotorDirection,
            CameraDirection
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private const string INPUT_MOUSE_X = "Mouse X";
        private const string INPUT_MOUSE_Y = "Mouse Y";
        private const string INPUT_MOUSE_W = "Mouse ScrollWheel";

        public static new string NAME = "Adventure Camera";
        public static Rect MOBILE_RECT = new Rect(0.5f, 0.0f, 0.5f, 1.0f);

        // PROPERTIES: ----------------------------------------------------------------------------

        private GameObject pivot;
        private bool motorEnabled;
        private float targetRotationX;
        private float targetRotationY;
        private Vector3 _velocityPosition = Vector3.zero;

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);
        public Vector3 targetOffset = Vector3.up;
        public Vector3 pivotOffset = Vector3.zero;

        public InitDirection initialDirection = InitDirection.TargetDirection;

        public bool allowOrbitInput = true;
        public OrbitInput orbitInput = OrbitInput.MouseMove;
        public float orbitSpeed = 25.0f;
        private float orbitInputTime = -1000f;

        [Range(0.0f, 180f)] public float maxPitch = 120f;
        public Vector2Property sensitivity = new Vector2Property(Vector2.one * 10f);

        public bool allowZoom = true;
        public float zoomSpeed = 25.0f;
        public float initialZoom = 3.0f;
        [Range(1f, 20f)]
        public float zoomSensitivity = 5.0f;
        public Vector2 zoomLimits = new Vector2(1f, 10f);

        private float wallConstrainZoom;
        private float desiredZoom;
        private float currentZoom;
        private float targetZoom;

        public bool avoidWallClip = true;
        public float wallClipRadius = 0.4f;
        public LayerMask wallClipLayerMask = ~4;

        public bool autoRepositionBehind = true;
        public float autoRepositionTimeout = 1.5f;
        public float autoRepositionSpeed = 2.5f;
        private float recoverSpeedX;
        private float recoverSpeedY;

        private RaycastHit[] hitsBuffer = new RaycastHit[50];
        private CursorLockMode cursorLock = CursorLockMode.None;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.pivot = new GameObject(gameObject.name + " Pivot");
            this.pivot.transform.SetParent(transform);
            
            Vector3 pivotPosition = (Vector3.forward * this.initialZoom) + this.pivotOffset;
            
            this.pivot.transform.localRotation = Quaternion.identity;
            this.pivot.transform.localPosition = pivotPosition;
        }

        public override void EnableMotor()
        {
            GameObject targetGo = this.target.GetGameObject(gameObject);
            if (targetGo != null)
            {
                Transform targetTransform = targetGo.transform;

                switch (this.initialDirection)
                {
                    case InitDirection.TargetDirection:
                        this.targetRotationX = targetTransform.rotation.eulerAngles.y + 180f;
                        this.targetRotationY = targetTransform.rotation.eulerAngles.x;
                        break;

                    case InitDirection.MotorDirection:
                        this.targetRotationX = transform.rotation.eulerAngles.y + 180f;
                        this.targetRotationY = transform.rotation.eulerAngles.x;
                        break;

                    case InitDirection.CameraDirection:
                        Transform cam = HookCamera.Instance.transform;
                        this.targetRotationX = cam.rotation.eulerAngles.y + 180f;
                        this.targetRotationY = cam.rotation.eulerAngles.x;
                        break;

                }

                transform.position = targetTransform.position + this.targetOffset;
                transform.rotation = Quaternion.Euler(
                    this.targetRotationY,
                    this.targetRotationX,
                    0f
                );
            }

            this.desiredZoom = this.initialZoom;
            this.currentZoom = this.initialZoom;
            this.wallConstrainZoom = this.zoomLimits.y;

            this.motorEnabled = true;
            this.cursorLock = Cursor.lockState;
        }

        public override void DisableMotor()
        {
            this.motorEnabled = false;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void UpdateMotor()
        {
            float rotationX = 0.0f;
            float rotationY = 0.0f;

            if (this.allowOrbitInput) this.RotationInput(ref rotationX, ref rotationY);

            this.targetRotationX += rotationX;
            this.targetRotationY += rotationY;

            if (this.autoRepositionBehind &&
                this.orbitInputTime + this.autoRepositionTimeout < Time.time)
            {
                this.RotationRecover();
            }

            this.targetRotationX %= 360f;
            this.targetRotationY %= 360f;

            this.targetRotationY = Mathf.Clamp(
                this.targetRotationY,
                -this.maxPitch / 2.0f,
                this.maxPitch / 2.0f
            );

            float smoothTime = (HookCamera.Instance != null
                ? HookCamera.Instance.Get<CameraController>().cameraSmoothTime.positionDuration
                : 0.1f
            );

            Transform targetTransform = this.target.GetTransform(gameObject);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetTransform.TransformPoint(this.targetOffset),
                ref this._velocityPosition,
                smoothTime
            );

            Quaternion targetRotation = Quaternion.Euler(
                this.targetRotationY,
                this.targetRotationX,
                0f
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * this.orbitSpeed
            );

            if (this.allowZoom)
            {
                this.desiredZoom = Mathf.Clamp(
                    this.desiredZoom - Input.GetAxis(INPUT_MOUSE_W) * this.zoomSensitivity,
                    this.zoomLimits.x, this.zoomLimits.y
                );
            }

            this.currentZoom = Mathf.Max(this.zoomLimits.x, this.desiredZoom);
            this.currentZoom = Mathf.Min(this.currentZoom, this.wallConstrainZoom);
            this.targetZoom = Mathf.Lerp(this.targetZoom, this.currentZoom, Time.deltaTime * this.zoomSpeed);

            Vector3 pivotPosition = (Vector3.forward * this.targetZoom) + this.pivotOffset;
            this.pivot.transform.localPosition = pivotPosition;
        }

        public override Vector3 GetPosition(CameraController camera, bool withoutSmoothing = false)
        {
            return this.pivot.transform.position;
        }

        public override Vector3 GetDirection(CameraController camera, bool withoutSmoothing = false)
        {
            return transform.TransformDirection(-Vector3.forward);
        }

        public override bool UseSmoothPosition()
        {
            return false;
        }

        public override bool UseSmoothRotation()
        {
            return false;
        }

        public void AddRotation(float pitch, float yaw)
        {
            this.targetRotationY += yaw;
            this.targetRotationX += pitch;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void FixedUpdate()
        {
            this.wallConstrainZoom = this.zoomLimits.y;
            if (!this.motorEnabled || !this.avoidWallClip) return;

            if (this.avoidWallClip && HookCamera.Instance != null)
            {
                Vector3 direction = this.pivot.transform.TransformDirection(Vector3.forward);
                QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

                int hitCount = Physics.SphereCastNonAlloc(
                    transform.position + (direction * this.wallClipRadius),
                    this.wallClipRadius, direction, this.hitsBuffer, this.zoomLimits.y,
                    this.wallClipLayerMask, queryTrigger
                );

                float minDistance = this.zoomLimits.y;
                Transform targetTransform = this.target.GetTransform(gameObject);
                
                for (int i = 0; i < hitCount; ++i)
                {
                    float hitDistance = this.hitsBuffer[i].distance + this.wallClipRadius;
                    Transform bufferTransform = this.hitsBuffer[i].collider.transform;
                
                    bool childOfTarget = (
                        bufferTransform == targetTransform ||
                        bufferTransform.IsChildOf(targetTransform)
                    );
                
                    if (hitDistance <= minDistance && !childOfTarget)
                    {
                        minDistance = hitDistance;
                        this.wallConstrainZoom = Mathf.Clamp(
                            minDistance,
                            this.zoomLimits.x,
                            this.zoomLimits.y
                        );
                    }
                }
            }
        }

        private void RotationInput(ref float rotationX, ref float rotationY)
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

                        rotationX = (touch.deltaPosition.x / Screen.width) * sensitivityValue.x * 10f * Time.timeScale;
                        rotationY = (touch.deltaPosition.y / Screen.height) * sensitivityValue.y * 10f * Time.timeScale;
                        orbitInputTime = Time.time;
                        this.recoverSpeedX = 0f;
                        this.recoverSpeedY = 0f;
                        break;
                    }
                }
            }
            else
            {
                bool inputConditions;
                if (this.orbitInput == OrbitInput.MouseMove) inputConditions = true;
                else if (this.orbitInput == OrbitInput.HoldLeftMouse && Input.GetMouseButton(0)) inputConditions = true;
                else if (this.orbitInput == OrbitInput.HoldRightMouse && Input.GetMouseButton(1)) inputConditions = true;
                else if (this.orbitInput == OrbitInput.HoldMiddleMouse && Input.GetMouseButton(2)) inputConditions = true;
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

                    rotationX = axisX * sensitivityValue.x * Time.timeScale;
                    rotationY = axisY * sensitivityValue.y * Time.timeScale;

                    if (!Mathf.Approximately(axisX, 0f) || !Mathf.Approximately(axisY, 0f))
                    {
                        orbitInputTime = Time.time;
                        this.recoverSpeedX = 0f;
                        this.recoverSpeedY = 0f;
                    }
                }
            }
        }

        private void RotationRecover()
        {
            Transform targetTransform = this.target.GetTransform(gameObject);
            if (targetTransform == null) return;

            float targetRecoverX = targetTransform.eulerAngles.y + 180f;
            float targetRecoverY = targetTransform.eulerAngles.x;

            this.targetRotationX = Mathf.SmoothDampAngle(
                this.targetRotationX, targetRecoverX,
                ref this.recoverSpeedX, this.autoRepositionSpeed
            );

            this.targetRotationY = Mathf.SmoothDampAngle(
                this.targetRotationY, targetRecoverY,
                ref this.recoverSpeedY, this.autoRepositionSpeed
            );
        }
    }
}
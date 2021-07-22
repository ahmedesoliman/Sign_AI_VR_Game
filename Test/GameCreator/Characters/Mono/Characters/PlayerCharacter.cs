namespace GameCreator.Characters
{
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("Game Creator/Characters/Player Character", 100)]
    public class PlayerCharacter : Character
    {
        public enum INPUT_TYPE
        {
            PointAndClick,
            Directional,
            FollowPointer,
            SideScrollX,
            SideScrollZ,
            TankControl
        }

        public enum MOUSE_BUTTON
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }

        protected const string AXIS_H = "Horizontal";
        protected const string AXIS_V = "Vertical";

        protected static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        protected const string PLAYER_ID = "player";
        public static OnLoadSceneData ON_LOAD_SCENE_DATA;

        // PROPERTIES: ----------------------------------------------------------------------------

        public INPUT_TYPE inputType = INPUT_TYPE.Directional;
        public MOUSE_BUTTON mouseButtonMove = MOUSE_BUTTON.LeftClick;
        public LayerMask mouseLayerMask = ~0;
        public bool invertAxis;

        public KeyCode jumpKey = KeyCode.Space;

        protected bool uiConstrained;
        protected Camera cacheCamera;

        private Vector3 direction = Vector3.zero;
        private Vector3 directionVelocity = Vector3.zero;

        public bool useAcceleration = true;
        public float acceleration = 4f;
        public float deceleration = 2f;

        private bool forceDisplayTouchstick = false;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            this.CharacterAwake();

            this.initSaveData = new SaveData()
            {
                position = transform.position,
                rotation = transform.rotation,
            };

            if (this.save)
            {
                SaveLoadManager.Instance.Initialize(
                    this, (int)SaveLoadManager.Priority.Normal, true
                );
            }

            HookPlayer hookPlayer = gameObject.GetComponent<HookPlayer>();
            if (hookPlayer == null) gameObject.AddComponent<HookPlayer>();

            if (ON_LOAD_SCENE_DATA != null && ON_LOAD_SCENE_DATA.active)
            {
                transform.position = ON_LOAD_SCENE_DATA.position;
                transform.rotation = ON_LOAD_SCENE_DATA.rotation;
                ON_LOAD_SCENE_DATA.Consume();
            }

            #if UNITY_EDITOR
            DatabaseGeneral general = DatabaseGeneral.Load(); 
            if (general.forceDisplayInEditor)
            {
                this.forceDisplayTouchstick = general.forceDisplayInEditor;
            }
            #endif
        }

        // UPDATE: --------------------------------------------------------------------------------

        protected virtual void Update()
        {
            if (!Application.isPlaying) return;

            switch (this.inputType)
            {
                case INPUT_TYPE.Directional: this.UpdateInputDirectional(); break;
                case INPUT_TYPE.PointAndClick: this.UpdateInputPointClick(); break;
                case INPUT_TYPE.FollowPointer: this.UpdateInputFollowPointer(); break;
                case INPUT_TYPE.SideScrollX: this.UpdateInputSideScroll(Vector3.right); break;
                case INPUT_TYPE.SideScrollZ: this.UpdateInputSideScroll(Vector3.forward); break;
                case INPUT_TYPE.TankControl: this.UpdateInputTank(); break;
            }

            if (this.IsControllable())
            {
                if (Input.GetKeyDown(this.jumpKey)) this.Jump();
            }
            else
            {
                this.direction = Vector3.zero;
                this.directionVelocity = Vector3.zero;
            }

            this.CharacterUpdate();
        }

        protected virtual void UpdateInputDirectional()
        {
            Vector3 targetDirection = Vector3.zero;
            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform || 
                TouchStickManager.FORCE_USAGE ||
                this.forceDisplayTouchstick)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                targetDirection = new Vector3(touchDirection.x, 0.0f, touchDirection.y);
            }
            else
            {
                targetDirection = new Vector3(
                    Input.GetAxisRaw(AXIS_H),
                    0.0f,
                    Input.GetAxisRaw(AXIS_V)
                );
            }

            this.ComputeMovement(targetDirection);

            Camera maincam = this.GetMainCamera();
            if (maincam == null) return;

            Vector3 moveDirection = (
                Quaternion.Euler(0, maincam.transform.rotation.eulerAngles.y, 0) * 
                this.direction
            );

            this.characterLocomotion.SetDirectionalDirection(moveDirection);
        }

        protected virtual void UpdateInputTank()
        {
            Vector3 movement = Vector3.zero;
            float rotationY = 0f;

            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform || 
                TouchStickManager.FORCE_USAGE ||
                this.forceDisplayTouchstick)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                movement = new Vector3(0f, 0.0f, touchDirection.y);
                rotationY = touchDirection.x;
            }
            else
            {
                movement = transform.TransformDirection(new Vector3(
                    0f,
                    0f, 
                    Input.GetAxisRaw(AXIS_V)
                ));

                rotationY = Input.GetAxis(AXIS_H);
            }

            this.ComputeMovement(movement);
            this.characterLocomotion.SetTankDirection(this.direction, rotationY);
        }

        protected virtual void UpdateInputPointClick()
        {
            if (!this.IsControllable()) return;
            this.UpdateUIConstraints();

            if (Input.GetMouseButtonDown((int)this.mouseButtonMove) && !this.uiConstrained)
            {
                Camera maincam = this.GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);
                this.characterLocomotion.SetTarget(cameraRay, this.mouseLayerMask, null, 0f, null);
            }
        }

        protected virtual void UpdateInputFollowPointer()
        {
            if (!this.IsControllable()) return;
            this.UpdateUIConstraints();

            if (Input.GetMouseButton((int)this.mouseButtonMove) && !this.uiConstrained)
            {
                if (HookPlayer.Instance == null) return;

                Camera maincam = this.GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);

                Transform player = HookPlayer.Instance.transform;
                Plane groundPlane = new Plane(Vector3.up, player.position);

                float rayDistance = 0f;
                if (groundPlane.Raycast(cameraRay, out rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    if (Vector3.Distance(player.position, cursor) >= 0.05f)
                    {
                        Vector3 target = Vector3.MoveTowards(player.position, cursor, 1f);
                        this.characterLocomotion.SetTarget(target, null, 0f, null);
                    }
                }
            }
        }

        protected virtual void UpdateInputSideScroll(Vector3 axis)
        {
            Vector3 targetDirection = Vector3.zero;
            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform || 
                TouchStickManager.FORCE_USAGE ||
                this.forceDisplayTouchstick)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                targetDirection = axis * touchDirection.x;
            }
            else
            {
                targetDirection = axis * Input.GetAxis(AXIS_H);
            }

            Camera maincam = this.GetMainCamera();
            if (maincam == null) return;

            this.ComputeMovement(targetDirection);

            float invertValue = (this.invertAxis ? -1 : 1);
            Vector3 moveDirection = Vector3.Scale(this.direction, axis * invertValue);

            moveDirection.Normalize();
            moveDirection *= this.direction.magnitude;

            this.characterLocomotion.SetDirectionalDirection(moveDirection);
        }

        // OTHER METHODS: -------------------------------------------------------------------------

        protected Camera GetMainCamera()
        {
            if (HookCamera.Instance != null) return HookCamera.Instance.Get<Camera>();
            if (this.cacheCamera != null) return this.cacheCamera;

            this.cacheCamera = Camera.main;
            if (this.cacheCamera != null)
            {
                return this.cacheCamera;
            }

            this.cacheCamera = GameObject.FindObjectOfType<Camera>();
            if (this.cacheCamera != null)
            {
                return this.cacheCamera;
            }

            Debug.LogError(ERR_NOCAM, gameObject);
            return null;
        }

        protected void UpdateUIConstraints()
        {
            EventSystemManager.Instance.Wakeup();
            this.uiConstrained = EventSystemManager.Instance.IsPointerOverUI();

            #if UNITY_IOS || UNITY_ANDROID
            for (int i = 0; i < Input.touches.Length; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                int fingerID = Input.GetTouch(i).fingerId;
                bool pointerOverUI = EventSystemManager.Instance.IsPointerOverUI(fingerID);
                if (pointerOverUI) this.uiConstrained = true;
            }
            #endif
        }

        protected void ComputeMovement(Vector3 target)
        {
            switch (this.useAcceleration)
            {
                case true:
                    float acceleration = Mathf.Approximately(target.sqrMagnitude, 0f)
                        ? this.deceleration
                        : this.acceleration;

                    this.direction = Vector3.SmoothDamp(
                        this.direction, target,
                        ref this.directionVelocity,
                        1f / acceleration,
                        acceleration
                    );

                    if (Mathf.Abs(target.sqrMagnitude) < 0.05f &&
                        Mathf.Abs(this.direction.sqrMagnitude) < 0.05f)
                    {
                        this.direction = Vector3.zero;
                    }

                    break;

                case false:
                    this.direction = target;
                    break;
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ForceDirection(Vector3 forceDirection)
        {
            this.direction = forceDirection;
            this.directionVelocity = Vector3.zero;
        }

        // GAME SAVE: -----------------------------------------------------------------------------

        protected override string GetUniqueCharacterID()
        {
            return PLAYER_ID;
        }
    }
}
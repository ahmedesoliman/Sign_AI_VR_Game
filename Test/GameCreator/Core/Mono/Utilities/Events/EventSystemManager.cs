namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.SceneManagement;
    using GameCreator.Core.Hooks;

	[AddComponentMenu("Game Creator/Managers/EventSystemManager", 100)]
	public class EventSystemManager : Singleton<EventSystemManager>
	{
        protected override void OnCreate ()
		{
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            StandaloneInputModule input = GameObject.FindObjectOfType<StandaloneInputModule>();

            if (input != null) Destroy(input);
            if (eventSystem != null) Destroy(eventSystem);

            gameObject.AddComponent<EventSystem>();
            this.inputModule = gameObject.AddComponent<GameCreatorStandaloneInputModule>();

			SceneManager.sceneLoaded += this.OnSceneLoad;
        }

		public void Wakeup()
		{
			return;
		}

        // PROPERTIES: ----------------------------------------------------------------------------

        private GameCreatorStandaloneInputModule inputModule;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject GetPointerGameObject(int pointerID = -1)
        {
            return this.inputModule.GameObjectUnderPointer(pointerID);
        }

        public bool IsPointerOverUI(int pointerID = -1)
        {
            GameObject pointer = this.GetPointerGameObject(pointerID);
            if (pointer == null) return false;

            return (pointer.transform as RectTransform != null);
        }

		public void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
		{
            this.RequireInit();
		}

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RequireInit()
        {
            if (HookCamera.Instance == null)
            {
                this.RequireCamera();
            }

            if (HookCamera.Instance != null)
            {
                PhysicsRaycaster raycaster3D = HookCamera.Instance.Get<PhysicsRaycaster>();
                Physics2DRaycaster raycaster2D = HookCamera.Instance.Get<Physics2DRaycaster>();

                if (raycaster3D == null) HookCamera.Instance.gameObject.AddComponent<PhysicsRaycaster>();
                if (raycaster2D == null) HookCamera.Instance.gameObject.AddComponent<Physics2DRaycaster>();
            }
        }

        private void RequireCamera()
        {
            GameObject cameraTag = GameObject.FindWithTag("MainCamera");
            if (cameraTag != null)
            {
                cameraTag.AddComponent<HookCamera>();
                return;
            }

            Camera cameraComp = FindObjectOfType<Camera>();
            if (cameraComp != null)
            {
                cameraComp.gameObject.AddComponent<HookCamera>();
                return;
            }
        }
    }
}
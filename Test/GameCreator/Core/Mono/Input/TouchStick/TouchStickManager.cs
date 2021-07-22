namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using GameCreator.Characters;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
    public class TouchStickManager : Singleton<TouchStickManager>
    {
        protected const string RESOURCE_PATH = "GameCreator/Input/TouchStick";

        public static bool FORCE_USAGE = false;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected TouchStick touchStick;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnCreate()
        {
            DontDestroyOnLoad(gameObject);

            GameObject prefab = DatabaseGeneral.Load().prefabTouchstick;
            if (prefab == null) prefab = Resources.Load<GameObject>(RESOURCE_PATH);

            GameObject instance = Instantiate<GameObject>(prefab, transform);
            this.touchStick = instance.GetComponentInChildren<TouchStick>();

            SceneManager.sceneLoaded += this.OnSceneLoad;
            this.UpdatePlayerEvents();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            bool visible = (HookPlayer.Instance != null);
            this.SetVisibility(visible);
            this.UpdatePlayerEvents();
        }

        private void UpdatePlayerEvents()
        {
            if (HookPlayer.Instance != null)
            {
                PlayerCharacter player = HookPlayer.Instance.Get<PlayerCharacter>();

                player.onIsControllable.RemoveListener(this.OnChangeIsControllable);
                player.onIsControllable.AddListener(this.OnChangeIsControllable);
            }
        }

        private void OnChangeIsControllable(bool isControllable)
        {
            this.SetVisibility(isControllable);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual Vector2 GetDirection(PlayerCharacter player)
        {
            if (!player.IsControllable())
            {
                this.touchStick.gameObject.SetActive(false);
                return Vector2.zero;
            }

            this.touchStick.gameObject.SetActive(true);
            return this.touchStick.GetDirection();
        }

        public virtual void SetVisibility(bool visible)
        {
            this.touchStick.gameObject.SetActive(visible);
        }
    }
}
namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core.Hooks;

    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("")]
    public class CanvasRenderMode : MonoBehaviour
    {
        private void Awake()
        {
            Canvas canvas = GetComponent<Canvas>();
            DatabaseGeneral general = DatabaseGeneral.Load();

            Camera mainCamera = (HookCamera.Instance != null 
                ? HookCamera.Instance.Get<Camera>() 
                : Camera.main
            );

            if (canvas != null && general != null)
            {
                switch (general.generalRenderMode)
                {
                    case DatabaseGeneral.GENERAL_SCREEN_SPACE.ScreenSpaceOverlay:
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        break;

                    case DatabaseGeneral.GENERAL_SCREEN_SPACE.ScreenSpaceCamera:
                        canvas.renderMode = RenderMode.ScreenSpaceCamera;
                        canvas.worldCamera = mainCamera;
                        break;

                    case DatabaseGeneral.GENERAL_SCREEN_SPACE.WorldSpaceCamera:
                        canvas.renderMode = RenderMode.WorldSpace;
                        canvas.worldCamera = mainCamera;
                        break;
                }
            }
        }
    }
}
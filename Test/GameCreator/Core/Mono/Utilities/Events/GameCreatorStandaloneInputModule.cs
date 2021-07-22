namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [AddComponentMenu("")]
    public class GameCreatorStandaloneInputModule : StandaloneInputModule
    {
        public GameObject GameObjectUnderPointer(int pointerId)
        {
            var lastPointer = GetLastPointerEventData(pointerId);
            if (lastPointer != null)
            {
                return lastPointer.pointerCurrentRaycast.gameObject;
            }

            return null;
        }

        public GameObject GameObjectUnderPointer()
        {
            return GameObjectUnderPointer(PointerInputModule.kMouseLeftId);
        }
    }
}
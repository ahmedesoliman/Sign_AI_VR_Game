namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [Serializable]
    public class HelperListVariable
    {
        public enum Target
        {
            Player,
            Camera,
            Invoker,
            GameObject
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target targetType = Target.GameObject;
        public GameObject targetObject;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public ListVariables GetListVariables(GameObject invoker)
        {
            switch (this.targetType)
            {
                case Target.Player:
                    if (HookPlayer.Instance == null) return null;
                    return HookPlayer.Instance.Get<ListVariables>();

                case Target.Camera:
                    if (HookCamera.Instance == null) return null;
                    return HookCamera.Instance.Get<ListVariables>();

                case Target.Invoker:
                    return invoker == null
                        ? null
                        : invoker.GetComponent<ListVariables>();

                case Target.GameObject:
                    return this.targetObject == null
                        ? null
                        : this.targetObject.GetComponent<ListVariables>();
            }

            return null;
        }

        public Variable.DataType GetDataType(GameObject invoker)
        {
            ListVariables list = this.GetListVariables(invoker);
            return list != null ? list.type : Variable.DataType.Null;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override string ToString()
        {
            if (this.targetType == Target.GameObject)
            {
                return (this.targetObject != null
                    ? this.targetObject.name
                    : "(null)"
                );
            }

            return this.targetType.ToString();
        }
    }
}
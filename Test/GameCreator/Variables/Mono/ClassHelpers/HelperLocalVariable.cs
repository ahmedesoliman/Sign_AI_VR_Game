namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [Serializable]
    public class HelperLocalVariable : BaseHelperVariable
    {
        public enum Target
        {
            Player,
            Invoker,
            GameObject,
            GameObjectPath
        }

        public string name = "";
        public Target targetType = Target.GameObject;
        public GameObject targetObject;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override object Get(GameObject invoker = null)
        {
            return VariablesManager.GetLocal(
                this.GetGameObject(invoker),
                this.name,
                true
            );
        }

        public override void Set(object value, GameObject invoker = null)
        {
            VariablesManager.SetLocal(
                this.GetGameObject(invoker),
                this.name,
                value,
                true
            );
        }

        public GameObject GetGameObject(GameObject invoker)
        {
            switch (this.targetType)
            {
                case Target.Player:
                    if (HookPlayer.Instance == null) return null;
                    return HookPlayer.Instance.gameObject;

                case Target.Invoker: return invoker;
                case Target.GameObject: return this.targetObject;
                case Target.GameObjectPath: return this.targetObject;
            }

            return null;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.name;
        }

		public override string ToStringValue(GameObject invoker = null)
		{
            object value = VariablesManager.GetLocal(
                this.GetGameObject(invoker),
                this.name,
                true
            );

            return (value != null ? value.ToString() : "null");
		}

        public override Variable.DataType GetDataType(GameObject invoker = null)
        {
            return VariablesManager.GetLocalType(invoker, this.name, true);
        }
    }
}
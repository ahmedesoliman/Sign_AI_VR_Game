namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using GameCreator.Variables;

    [CustomPropertyDrawer(typeof(TargetGameObject))]
    public class TargetGameObjectPD : TargetGenericPD
	{
        public const string PROP_GAMEOBJECT = "gameObject";
        public const string PROP_GLOBAL = "global";
        public const string PROP_LOCAL = "local";
        public const string PROP_LIST = "list";

        // PAINT METHODS: -------------------------------------------------------------------------

        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            TargetGameObject.Target optionTyped = (TargetGameObject.Target)option;
            switch (optionTyped)
            {
                case TargetGameObject.Target.GameObject:
                    return property.FindPropertyRelative(PROP_GAMEOBJECT);

                case TargetGameObject.Target.LocalVariable:
                    return property.FindPropertyRelative(PROP_LOCAL);

                case TargetGameObject.Target.ListVariable:
                    return property.FindPropertyRelative(PROP_LIST);

                case TargetGameObject.Target.GlobalVariable:
                    return property.FindPropertyRelative(PROP_GLOBAL);
            }

            return null;
        }

        protected override void Initialize(SerializedProperty property)
        {
            int allowTypesMask = (1 << (int)Variable.DataType.GameObject);

            property
                .FindPropertyRelative(PROP_GLOBAL)
                .FindPropertyRelative(HelperGenericVariablePD.PROP_ALLOW_TYPES_MASK)
                .intValue = allowTypesMask;

            property
                .FindPropertyRelative(PROP_LOCAL)
                .FindPropertyRelative(HelperGenericVariablePD.PROP_ALLOW_TYPES_MASK)
                .intValue = allowTypesMask;
        }
    }
}
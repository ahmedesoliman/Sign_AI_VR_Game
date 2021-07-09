namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using GameCreator.Variables;

    [CustomPropertyDrawer(typeof(TargetPosition))]
	public class TargetPositionPD : TargetGenericPD
    {
        private const string PROP_OFFSET = "offset";
        private const string PROP_TRANSFORM = "targetTransform";
        private const string PROP_POSITION = "targetPosition";
        private const string PROP_LOCAL = "local";
        private const string PROP_GLOBAL = "global";
        private const string PROP_LIST = "list";

        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            TargetPosition.Target optionTyped = (TargetPosition.Target)option;
            switch (optionTyped)
            {
                case TargetPosition.Target.Transform:
                    return property.FindPropertyRelative(PROP_TRANSFORM);

                case TargetPosition.Target.Position:
                    return property.FindPropertyRelative(PROP_POSITION);

                case TargetPosition.Target.LocalVariable:
                    return property.FindPropertyRelative(PROP_LOCAL);

                case TargetPosition.Target.GlobalVariable:
                    return property.FindPropertyRelative(PROP_GLOBAL);

                case TargetPosition.Target.ListVariable:
                    return property.FindPropertyRelative(PROP_LIST);
            }

            return null;
        }

        protected override SerializedProperty GetExtraProperty(int option, SerializedProperty property)
        {
            TargetPosition.Target optionTyped = (TargetPosition.Target)option;
            switch (optionTyped)
            {
                case TargetPosition.Target.Player:
                case TargetPosition.Target.Camera:
                case TargetPosition.Target.Invoker:
                case TargetPosition.Target.Transform:
                    return property.FindPropertyRelative(PROP_OFFSET);
            }

            return null;
        }

        protected override void Initialize(SerializedProperty property)
        {
            int allowTypesMask = (
                (1 << (int)Variable.DataType.Vector2) |
                (1 << (int)Variable.DataType.Vector3) |
                (1 << (int)Variable.DataType.GameObject)
            );

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
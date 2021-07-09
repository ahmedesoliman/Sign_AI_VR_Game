namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using GameCreator.Variables;

    [CustomPropertyDrawer(typeof(TargetDirection))]
	public class TargetDirectionPD : TargetGenericPD
    {
        private const string PROP_OFFSET = "offset";
        private const string PROP_TRANSFORM = "targetTransform";
        private const string PROP_POINT = "targetPoint";
        private const string PROP_LOCAL = "local";
        private const string PROP_GLOBAL = "global";

        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            TargetDirection.Target optionTyped = (TargetDirection.Target)option;
            switch (optionTyped)
            {
                case TargetDirection.Target.Transform:
                    return property.FindPropertyRelative(PROP_TRANSFORM);

                case TargetDirection.Target.Point:
                    return property.FindPropertyRelative(PROP_POINT);

                case TargetDirection.Target.LocalVariable:
                    return property.FindPropertyRelative(PROP_LOCAL);

                case TargetDirection.Target.GlobalVariable:
                    return property.FindPropertyRelative(PROP_GLOBAL);
            }

            return null;
        }

        protected override SerializedProperty GetExtraProperty(int option, SerializedProperty property)
        {
            TargetDirection.Target optionTyped = (TargetDirection.Target)option;
            switch (optionTyped)
            {
                case TargetDirection.Target.Player:
                case TargetDirection.Target.Camera:
                case TargetDirection.Target.Transform:
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
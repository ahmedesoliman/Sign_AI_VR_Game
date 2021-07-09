namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using GameCreator.Variables;

    [CustomPropertyDrawer(typeof(TargetCharacter))]
    public class TargetCharacterPD : TargetGenericPD
	{
        public const string PROP_CHARACTER = "character";
        public const string PROP_GLOBAL = "global";
        public const string PROP_LOCAL = "local";
        public const string PROP_LIST = "list";

        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            TargetCharacter.Target optionTyped = (TargetCharacter.Target)option;
            switch (optionTyped)
            {
                case TargetCharacter.Target.Character:
                    return property.FindPropertyRelative(PROP_CHARACTER);

                case TargetCharacter.Target.LocalVariable:
                    return property.FindPropertyRelative(PROP_LOCAL);

                case TargetCharacter.Target.GlobalVariable:
                    return property.FindPropertyRelative(PROP_GLOBAL);

                case TargetCharacter.Target.ListVariable:
                    return property.FindPropertyRelative(PROP_LIST);
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
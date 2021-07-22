namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(UniqueInstanceID))]
    public class UniqueInstanceIDPD : PropertyDrawer
    {
        private const string PROP_VALUE = "value";
        private const float BTN_UUID_WIDTH = 25f;
        private const float BTN_EDIT_WIDTH = 40f;
        private const float BTN_REGN_WIDTH = 30f;

        private const string NAME_UUID = "UID";
        private const string NAME_EDIT = "Edit";
        private const string NAME_REGN = "R";

        private const string MSG_REGEN1 = "Are you sure you want to generate a new unique ID for this component?";
        private const string MSG_REGEN2 = "We recommend saving a copy of the previous ID in case you want to revert changes";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool isEditing = false;

        // PAINT METHODS: -------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            SerializedProperty spValue = property.FindPropertyRelative(PROP_VALUE);

            if (string.IsNullOrEmpty(spValue.stringValue))
            {
                this.GenerateUniqueID(spValue);
            }

            Rect rectUUID = new Rect(
                position.x,
                position.y,
                BTN_UUID_WIDTH,
                position.height
            );
            Rect rectText = new Rect(
                rectUUID.x + rectUUID.width,
                position.y,
                position.width - BTN_UUID_WIDTH - BTN_EDIT_WIDTH - BTN_REGN_WIDTH,
                position.height
            );
            Rect rectEdit = new Rect(
                rectText.x + rectText.width,
                position.y,
                BTN_EDIT_WIDTH,
                position.height
            );
            Rect rectRegen = new Rect(
                rectEdit.x + rectEdit.width,
                position.y,
                BTN_REGN_WIDTH,
                position.height
            );

            EditorGUI.LabelField(
                rectUUID,
                NAME_UUID
            );

            EditorGUI.BeginDisabledGroup(!this.isEditing);
            EditorGUI.PropertyField(
                rectText, 
                spValue,
                GUIContent.none,
                false
            );
            EditorGUI.EndDisabledGroup();

            if (GUI.Button(rectEdit, NAME_EDIT, EditorStyles.miniButtonMid))
            {
                this.isEditing = !this.isEditing;
            }

            if (GUI.Button(rectRegen, NAME_REGN, EditorStyles.miniButtonRight))
            {
                if (EditorUtility.DisplayDialog(MSG_REGEN1, MSG_REGEN2, "Yes", "Cancel"))
                {
                    this.GenerateUniqueID(spValue);
                }
            }
		}

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void GenerateUniqueID(SerializedProperty property)
        {
            property.stringValue = Guid.NewGuid().ToString("N");
        }
	}
}
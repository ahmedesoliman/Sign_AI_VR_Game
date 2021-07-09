using System.Runtime.CompilerServices;
namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class GlobalEditorID
    {
        private const string REGEN_1 = "Are you sure you want to regenerate this ID?";
        private const string REGEN_2 = "This operation can't be undone";

        private const float INFO_WIDTH = 50f;
        private const float EDIT_WIDTH = 50f;
        private const float HEIGHT = 18f;
        private const float PADDING = 2f;

        private static bool IS_EDITING = false;

        // PAINT METHOD: --------------------------------------------------------------------------

        public static void Paint(GlobalID globalID)
		{
            switch (IS_EDITING)
            {
                case true: PaintEditing(globalID);  break;
                case false: PaintNormal(globalID); break;
            }
        }

        private static void PaintEditing(GlobalID globalID)
        {
            Rect rect = GUILayoutUtility.GetRect(
                0f, 9999f,
                HEIGHT + PADDING,
                HEIGHT + PADDING
            );

            Rect rectInfo = new Rect(
                rect.x,
                rect.y,
                INFO_WIDTH,
                HEIGHT
            );

            Rect rectContent = new Rect(
                rectInfo.x + rectInfo.width,
                rectInfo.y,
                rect.width - INFO_WIDTH - EDIT_WIDTH,
                HEIGHT
            );

            Rect rectEdit = new Rect(
                rectContent.x + rectContent.width,
                rectContent.y,
                EDIT_WIDTH,
                HEIGHT
            );
                
            if (GUI.Button(rectInfo, "Regen", CoreGUIStyles.GetButtonLeft()))
            {
                if (EditorUtility.DisplayDialog(REGEN_1, REGEN_2, "Yes", "Cancel"))
                {
                    string gid = Guid.NewGuid().ToString("D");
                    SetGID(globalID, gid);
                }
            }

            string prvGid = globalID.GetID();
            string newGid = EditorGUI.TextField(
                rectContent, 
                prvGid, 
                CoreGUIStyles.GlobalIDText()
            );

            if (prvGid != newGid)
            {
                SetGID(globalID, newGid);
            }

            if (GUI.Button(rectEdit, "Back", CoreGUIStyles.GetButtonRight()))
            {
                IS_EDITING = false;
            }
        }

        private static void PaintNormal(GlobalID globalID)
        {
            Rect rect = GUILayoutUtility.GetRect(
                0f, 9999f, 
                HEIGHT + PADDING,
                HEIGHT + PADDING
            );

            Rect rectInfo = new Rect(
                rect.x,
                rect.y,
                INFO_WIDTH,
                HEIGHT
            );

            Rect rectContent = new Rect(
                rectInfo.x + rectInfo.width,
                rectInfo.y,
                rect.width - INFO_WIDTH - EDIT_WIDTH,
                HEIGHT
            );

            Rect rectEdit = new Rect(
                rectContent.x + rectContent.width,
                rectContent.y,
                EDIT_WIDTH,
                HEIGHT
            );

            GUI.enabled = false;
            EditorGUI.LabelField(rectInfo, "ID", CoreGUIStyles.GetButtonLeft());
            GUI.enabled = true;


            GUI.enabled = false;
            EditorGUI.LabelField(rectContent, globalID.GetID(), CoreGUIStyles.GetButtonMid());
            GUI.enabled = true;

            if (GUI.Button(rectEdit, "Edit", CoreGUIStyles.GetButtonRight()))
            {
                IS_EDITING = true;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void SetGID(GlobalID globalID, string gid)
        {
            SerializedObject soGID = new SerializedObject(globalID);
            soGID.ApplyModifiedProperties();
            soGID.Update();

            gid = ProcessGID(gid);

            soGID.FindProperty("gid").stringValue = gid;
            soGID.ApplyModifiedProperties();
            soGID.Update();
        }

        private static string ProcessGID(string gid)
        {
            return gid.Trim().Replace(' ', '-');
        }
    }
}
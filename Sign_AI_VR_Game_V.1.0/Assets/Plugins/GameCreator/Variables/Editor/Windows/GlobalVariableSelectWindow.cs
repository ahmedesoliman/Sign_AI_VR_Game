namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using GameCreator.Core;

    public class GlobalVariableSelectWindow : GenericVariableSelectWindow
    {
        private string TITLE_MANAGE_VARIABLES = "Manage Global Variables";

        // INITIALIZERS: --------------------------------------------------------------------------

        public GlobalVariableSelectWindow(Rect ctaRect, Action<string> callback, int allowTypesMask) 
            : base(ctaRect, callback, allowTypesMask)
        {
            
        }

		// OVERRIDERS: ----------------------------------------------------------------------------

		protected override GUIContent[] GetVariables(int allowTypesMask)
		{
            DatabaseVariables database = DatabaseVariables.Load();
            if (database == null || database.GetGlobalVariables() == null)
            {
                PreferencesWindow.OpenWindowTab("Variables");
                return new GUIContent[0];
            }

            GlobalVariables globalVariables = database.GetGlobalVariables();
            List<GUIContent> variables = new List<GUIContent>();
            for (int i = 0; i < globalVariables.references.Length; ++i)
            {
                Variable.DataType type = (Variable.DataType)globalVariables.references[i].variable.type;
                if ((allowTypesMask & 1 << (int)type) == 0) continue;

                variables.Add(new GUIContent(
                    " " + globalVariables.references[i].variable.name,
                    VariableEditorUtils.GetIcon(type)
                ));
            }

            return variables.ToArray();
		}

		protected override void PaintFooter()
		{
            GUILayout.BeginVertical(CoreGUIStyles.GetSearchBox());

            if (GUILayout.Button(TITLE_MANAGE_VARIABLES))
            {
                PreferencesWindow.OpenWindowTab("Variables");
                this.editorWindow.Close();
            }

            GUILayout.EndVertical();
		}
	}
}
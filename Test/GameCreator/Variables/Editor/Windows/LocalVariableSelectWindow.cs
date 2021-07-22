namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using GameCreator.Core;

    public class LocalVariableSelectWindow : GenericVariableSelectWindow
    {
        private GameObject target;
        private bool inChildren;

        // INITIALIZERS: --------------------------------------------------------------------------

        public LocalVariableSelectWindow(Rect ctaRect, GameObject target, bool inChildren, Action<string> callback, int allowTypesMask) 
            : base(ctaRect, callback, allowTypesMask)
        {
            this.target = target;
            this.inChildren = inChildren;
        }

		// OVERRIDERS: ----------------------------------------------------------------------------

		protected override GUIContent[] GetVariables(int allowTypesMask)
		{
            if (this.target == null) return new GUIContent[0];
            LocalVariables[] localVariables = LocalVariablesUtilities.GatherLocals(
                this.target, this.inChildren
            );

            List<GUIContent> variables = new List<GUIContent>();
            for (int i = 0; i < localVariables.Length; ++i)
            {
                LocalVariables local = localVariables[i];
                for (int j = 0; j < local.references.Length; ++j)
                {
                    Variable.DataType type = (Variable.DataType)local.references[j].variable.type;
                    if ((allowTypesMask & 1 << (int)type) == 0) continue;

                    variables.Add(new GUIContent(
                        local.references[j].variable.name,
                        VariableEditorUtils.GetIcon(type)
                    ));
                }
            }

            return variables.ToArray();
		}

		protected override void PaintFooter()
		{
            return;
		}
	}
}
namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignTexture2D : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Texture2D)]
        public VariableProperty variable;

        public Texture2D value = null;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            this.variable.Set(this.value, target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Texture2D";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Texture2D", this.variable);
        }

        public override bool PaintInspectorTarget()
        {
            return false;
        }

        #endif
    }
}

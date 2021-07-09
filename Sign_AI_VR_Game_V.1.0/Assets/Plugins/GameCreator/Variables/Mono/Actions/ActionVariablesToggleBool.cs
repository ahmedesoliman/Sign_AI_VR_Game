namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	[AddComponentMenu("")]
	public class ActionVariablesToggleBool : IAction
	{
        [VariableFilter(Variable.DataType.Bool)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.GlobalVariable);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            bool value = (bool)this.variable.Get(target);
            this.variable.Set(!value, target);

            return true;
        }

		#if UNITY_EDITOR
        public static new string NAME = "Variables/Variable Bool Toggle";
        private const string NODE_NAME = "Toggle variable {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_NAME, this.variable);
        }
        #endif
    }
}

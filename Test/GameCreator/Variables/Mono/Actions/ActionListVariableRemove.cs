namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	public class ActionListVariableRemove : IAction
	{
        public HelperGetListVariable listVariables = new HelperGetListVariable();

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = this.listVariables.GetListVariables(target);
            if (list == null || list.variables.Count == 0) return true;

            list.Remove(this.listVariables.select, this.listVariables.index);
            return true;
        }

        #if UNITY_EDITOR

        private const string NODE_TITLE = "Remove {0}";
        public static new string NAME = "Variables/Remove from List Variables";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.listVariables
            );
        }

        #endif
    }
}

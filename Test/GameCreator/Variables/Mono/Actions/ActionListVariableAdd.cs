namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	public class ActionListVariableAdd : IAction
	{
        public HelperGetListVariable listVariables = new HelperGetListVariable();
        [Space] public TargetGameObject item = new TargetGameObject();

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = this.listVariables.GetListVariables(target);
            if (list == null) return true;

            GameObject elementGo = this.item.GetGameObject(target);
            if (elementGo == null) return true;

            list.Push(elementGo, this.listVariables.select, this.listVariables.index);
            return true;
        }

        #if UNITY_EDITOR

        private const string NODE_TITLE = "Add {0} to {1}";
        public static new string NAME = "Variables/Add to List Variables";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.item,
                this.listVariables
            );
        }

        #endif
    }
}

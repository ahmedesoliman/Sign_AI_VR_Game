namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	public class ActionListVariableSelect : IAction
	{
        public enum Select
        {
            Nearest,
            Farthest
        }

        public HelperListVariable fromListVariables = new HelperListVariable();

        [Space]
        public Select select = Select.Nearest;
        public TargetGameObject to = new TargetGameObject(TargetGameObject.Target.Invoker);

        [Space, VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty assignToVariable = new VariableProperty();

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = this.fromListVariables.GetListVariables(target);
            if (list == null || list.variables.Count == 0) return true;

            Transform other = this.to.GetTransform(target);
            if (other == null) return true;

            GameObject selectedGO = null;
            float selectedDistance = 0;
            switch (this.select)
            {
                case Select.Nearest: selectedDistance = Mathf.Infinity; break;
                case Select.Farthest: selectedDistance = Mathf.NegativeInfinity; break;
            }

            for (int i = 0; i < list.variables.Count; ++i)
            {
                GameObject item = list.variables[i].Get() as GameObject;
                if (item == null) continue;

                float distance = Vector3.Distance(other.position, item.transform.position);
                switch (this.select)
                {
                    case Select.Nearest:
                        if (distance < selectedDistance)
                        {
                            selectedDistance = distance;
                            selectedGO = item;
                        }
                        break;

                    case Select.Farthest:
                        if (distance > selectedDistance)
                        {
                            selectedDistance = distance;
                            selectedGO = item;
                        }
                        break;
                }
            }

            if (selectedGO == null) return true;
            this.assignToVariable.Set(selectedGO);
            return true;
        }

        #if UNITY_EDITOR

        private const string NODE_TITLE = "Select {0} from List Variables to {1}";
        public static new string NAME = "Variables/List Variables Select";


        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.select,
                this.to
            );
        }

        #endif
    }
}

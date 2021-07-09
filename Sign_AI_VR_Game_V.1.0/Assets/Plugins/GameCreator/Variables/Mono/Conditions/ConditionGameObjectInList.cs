namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;

	[AddComponentMenu("")]
	public class ConditionGameObjectInList : ICondition
	{
        public HelperListVariable listVariables = new HelperListVariable();

        [Space]
        public TargetGameObject containsObject = new TargetGameObject(TargetGameObject.Target.GameObject);

		public override bool Check(GameObject target)
		{
            GameObject go = this.containsObject.GetGameObject(target);
            if (go == null) return false;

            ListVariables list = this.listVariables.GetListVariables(target);
            if (list == null) return false;
            if (list.type != Variable.DataType.GameObject) return false;

            for (int i = 0; i < list.variables.Count; ++i)
            {
                GameObject item = list.variables[i].Get<GameObject>();
                if (item == null) continue;
                if (item.GetInstanceID() == go.GetInstanceID()) return true;
            }

            return false;
        }
        
		#if UNITY_EDITOR

        public static new string NAME = "Variables/List Variable contains Object";

        private const string NODE_TITLE = "{0} List contains {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.listVariables,
                this.containsObject
            );
        }

        #endif
    }
}

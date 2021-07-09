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
    public class ActionVariablesAssignGameObject : IActionVariablesAssign
	{
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable;

        public GameObject value;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override void ExecuteAssignement(GameObject target)
		{
            switch (this.valueFrom)
            {
                case ValueFrom.Invoker:
                    this.variable.Set(target);
                    break;

                case ValueFrom.Player :
                    this.variable.Set(HookPlayer.Instance.gameObject);
                    break;

                case ValueFrom.Constant :
                    this.variable.Set(this.value, target);
                    break;

                case ValueFrom.GlobalVariable:
                    this.variable.Set(this.global.Get(target), target);
                    break;

                case ValueFrom.LocalVariable:
                    this.variable.Set(this.local.Get(target), target);
                    break;

                case ValueFrom.ListVariable:
                    this.variable.Set(this.list.Get(target), target);
                    break;
            }
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Variables/Variable GameObject";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, "GameObject", this.variable);
		}

		public override bool PaintInspectorTarget()
		{
            return true;
		}

        #endif
	}
}

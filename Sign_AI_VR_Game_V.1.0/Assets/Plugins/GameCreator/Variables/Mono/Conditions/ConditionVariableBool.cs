namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
    public class ConditionVariableBool : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Bool)]
        public VariableProperty variable = new VariableProperty();
        public BoolProperty compareTo = new BoolProperty();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
		{
			return (bool)this.variable.Get(target) == this.compareTo.GetValue(target);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Bool";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.variable,
                this.compareTo
            );
        }

        #endif
    }
}
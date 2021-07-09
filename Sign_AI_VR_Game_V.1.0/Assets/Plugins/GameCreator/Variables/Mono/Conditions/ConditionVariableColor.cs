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
    public class ConditionVariableColor : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Color)]
        public VariableProperty variable = new VariableProperty();
        public ColorProperty compareTo = new ColorProperty();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
		{
			return (Color)this.variable.Get(target) == this.compareTo.GetValue(target);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Color";

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
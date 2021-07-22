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
    public class ConditionVariableVector3 : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Vector3)]
        public VariableProperty variable = new VariableProperty();
        public Vector3Property compareTo = new Vector3Property();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
		{
			return (Vector3)this.variable.Get(target) == this.compareTo.GetValue(target);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Vector3";

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
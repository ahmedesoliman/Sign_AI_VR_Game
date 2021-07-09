namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
    public class ConditionVariableNumber : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable = new VariableProperty();

        [Space]
        public NumberProperty compareTo = new NumberProperty();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
		{
			float var1 = Convert.ToSingle(this.variable.Get(target));
            float var2 = this.compareTo.GetValue(target);

            switch (this.comparison)
            {
                case Comparison.Equal: return Mathf.Approximately(var1, var2);
                case Comparison.EqualInteger: return Mathf.RoundToInt(var1) == Mathf.RoundToInt(var2);
                case Comparison.Less: return var1 < var2;
                case Comparison.LessOrEqual: return var1 <= var2;
                case Comparison.Greater: return var1 > var2;
                case Comparison.GreaterOrEqual: return var1 >= var2;
            }

            return false;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Number";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.variable,
                this.compareTo
            );
        }

		protected override bool ShowComparison()
		{
            return true;
		}

        #endif
	}
}
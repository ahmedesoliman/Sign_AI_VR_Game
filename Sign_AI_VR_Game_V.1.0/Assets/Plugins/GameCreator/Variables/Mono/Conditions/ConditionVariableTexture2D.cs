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
    public class ConditionVariableTexture2D : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Texture2D)]
        public VariableProperty variable = new VariableProperty();
        public Texture2DProperty compareTo = new Texture2DProperty();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
		{
			return (Texture2D)this.variable.Get(target) == this.compareTo.GetValue(target);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Texture2D";

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
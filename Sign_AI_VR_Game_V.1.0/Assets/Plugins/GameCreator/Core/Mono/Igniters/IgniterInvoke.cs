namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

	[AddComponentMenu("")]
	public class IgniterInvoke : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "General/On Invoke";
		public new static string COMMENT = "To invoke this Trigger call [Trigger instance].Execute()";
        #endif

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeInvoker = new VariableProperty(Variable.VarType.GlobalVariable);

        protected override void ExecuteTrigger(GameObject target = null)
        {
            if (target != null) storeInvoker.Set(target);
            base.ExecuteTrigger(target);
        }
    }
}
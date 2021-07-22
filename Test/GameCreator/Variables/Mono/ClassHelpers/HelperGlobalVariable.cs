namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class HelperGlobalVariable : BaseHelperVariable
    {
        public string name;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override object Get(GameObject invoker = null)
        {
            return VariablesManager.GetGlobal(this.name);
        }

        public override void Set(object value, GameObject invoker = null)
        {
            VariablesManager.SetGlobal(this.name, value);
        }

		// OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
		{
            return this.name;
		}

        public override string ToStringValue(GameObject invoker = null)
		{
            object value = VariablesManager.GetGlobal(this.name);
            return (value != null ? value.ToString() : "null");
		}

        public override Variable.DataType GetDataType(GameObject invoker = null)
        {
            return VariablesManager.GetGlobalType(this.name, true);
        }
    }
}
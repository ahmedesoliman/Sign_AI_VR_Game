namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class VariableGlobalProperty
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public HelperGlobalVariable globalVariable = new HelperGlobalVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableGlobalProperty()
        {
            this.globalVariable = this.globalVariable ?? new HelperGlobalVariable();
		}

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get()
        {
            return this.globalVariable.Get(null);
        }

        public void Set(object value)
        {
            this.globalVariable.Set(value, null);
        }

        public string GetVariableID()
        {
            return this.globalVariable.name;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

		public override string ToString()
		{
            return this.globalVariable.ToString();
		}

        public string ToStringValue()
        {
            return this.globalVariable.ToStringValue(null);
        }
	}
}
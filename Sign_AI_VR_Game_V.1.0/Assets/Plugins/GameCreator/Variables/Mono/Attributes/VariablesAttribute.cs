namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    public class VariableFilterAttribute : PropertyAttribute
    {
        public Variable.DataType[] types = new Variable.DataType[]
        {
            Variable.DataType.Number
        };

        // INITIALIZER: ---------------------------------------------------------------------------

        public VariableFilterAttribute(params Variable.DataType[] types)
        {
            this.types = types;
        }
    }
}
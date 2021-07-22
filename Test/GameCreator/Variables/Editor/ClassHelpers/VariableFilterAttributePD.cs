namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(VariableFilterAttribute))]
    public class VariableFilterAttributePD : VariablePropertyPD
    {
		protected override int GetAllowTypesMask()
		{
            int mask = 0;
            VariableFilterAttribute filter = this.attribute as VariableFilterAttribute;

            for (int i = 0; i < filter.types.Length; ++i)
            {
                mask |= (1 << (int)filter.types[i]);
            }

            return mask;
		}
	}
}
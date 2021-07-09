namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableBol : VariableGeneric<bool>
    {
        public new const string NAME = "Bool";
        
        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableBol()
        { }

        public VariableBol(bool value) : base(value)
        { }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return VariableBase.CanSaveType(Variable.DataType.Bool);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Bool;
        }
    }
}
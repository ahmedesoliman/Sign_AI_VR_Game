namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableVc2 : VariableGeneric<Vector2>
    {
        public new const string NAME = "Vector2";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableVc2()
        { }

        public VariableVc2(Vector2 value) : base(value)
        { }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return VariableBase.CanSaveType(Variable.DataType.Vector2);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Vector2;
        }
    }
}
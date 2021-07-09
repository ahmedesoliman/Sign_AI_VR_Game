namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableObj : VariableGeneric<GameObject>
    {
        public new const string NAME = "Game Object";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableObj()
        { }

        public VariableObj(GameObject value) : base(value)
        { }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return VariableBase.CanSaveType(Variable.DataType.GameObject);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.GameObject;
        }
    }
}
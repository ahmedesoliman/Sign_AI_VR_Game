namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public abstract class VariableBase
    {
        public const string NAME = "Null";

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual bool CanSave()
        {
            return true;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static bool CanSaveType(Variable.DataType dataType)
        {
            switch (dataType)
            {
                case Variable.DataType.Null: return false;
                case Variable.DataType.String: return true;
                case Variable.DataType.Number: return true;
                case Variable.DataType.Bool: return true;
                case Variable.DataType.Color: return true;
                case Variable.DataType.Vector2: return true;
                case Variable.DataType.Vector3: return true;
                case Variable.DataType.Texture2D: return true;
                case Variable.DataType.Sprite: return true;
                case Variable.DataType.GameObject: return false;
            }

            return false;
        }

        public static Variable.DataType GetDataType()
        {
            return Variable.DataType.Null;
        }
    }
}
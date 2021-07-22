namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public abstract class VariableGeneric<T> : VariableBase
    {
        public new const string NAME = "Generic";

        [SerializeField] private T value;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected VariableGeneric(T value = default(T))
        {
            this.value = value;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Get()
        {
            return this.value;
        }

        public void Set(T value)
        {
            this.value = value;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Null;
        }
    }
}
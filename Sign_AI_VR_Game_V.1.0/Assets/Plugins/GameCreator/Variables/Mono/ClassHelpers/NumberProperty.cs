namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class NumberProperty : BaseProperty<float>
    {
        public NumberProperty() : base() { }
        public NumberProperty(float value) : base(value) { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int GetInt(GameObject invoker)
        {
            return Mathf.FloorToInt(this.GetValue(invoker));
        }
    }
}
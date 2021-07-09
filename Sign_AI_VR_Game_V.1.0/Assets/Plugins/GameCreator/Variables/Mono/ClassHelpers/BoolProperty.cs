namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class BoolProperty : BaseProperty<bool>
    {
        public BoolProperty() : base() { }
        public BoolProperty(bool value) : base(value) { }
    }
}
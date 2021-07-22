namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class StringProperty : BaseProperty<string>
    {
        public StringProperty() : base() { }
        public StringProperty(string value) : base(value) { }
    }
}
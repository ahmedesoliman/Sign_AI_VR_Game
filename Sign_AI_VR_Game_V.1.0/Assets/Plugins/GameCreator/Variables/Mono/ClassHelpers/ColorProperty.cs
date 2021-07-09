namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class ColorProperty : BaseProperty<Color>
    {
        public ColorProperty() : base() { }
        public ColorProperty(Color value) : base(value) { }
    }
}
namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Texture2DProperty : BaseProperty<Texture2D>
    {
        public Texture2DProperty() : base() { }
        public Texture2DProperty(Texture2D value) : base(value) { }
    }
}
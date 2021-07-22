namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Vector2Property : BaseProperty<Vector2>
    {
        public Vector2Property() : base() { }
        public Vector2Property(Vector2 value) : base(value) { }
    }
}
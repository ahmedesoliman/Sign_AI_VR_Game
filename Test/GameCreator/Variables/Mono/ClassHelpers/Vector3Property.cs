namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Vector3Property : BaseProperty<Vector3>
    {
        public Vector3Property() : base() { }
        public Vector3Property(Vector3 value) : base(value) { }
    }
}
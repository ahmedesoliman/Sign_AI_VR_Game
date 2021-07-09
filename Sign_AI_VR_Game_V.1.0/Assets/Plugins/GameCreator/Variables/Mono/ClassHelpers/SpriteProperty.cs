namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class SpriteProperty : BaseProperty<Sprite>
    {
        public SpriteProperty() : base() { }
        public SpriteProperty(Sprite value) : base(value) { }
    }
}
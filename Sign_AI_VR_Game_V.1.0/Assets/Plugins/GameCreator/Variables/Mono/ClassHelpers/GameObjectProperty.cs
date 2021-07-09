namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameObjectProperty : BaseProperty<GameObject>
    {
        public GameObjectProperty() : base() { }
        public GameObjectProperty(GameObject value) : base(value) { }

        protected override string GetValueName()
        {
            GameObject instance = this.value as GameObject;
            return instance == null ? "(none)" : this.value.name;
        }
    }
}
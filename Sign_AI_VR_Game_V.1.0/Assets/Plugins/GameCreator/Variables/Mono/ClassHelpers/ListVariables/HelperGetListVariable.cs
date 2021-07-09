namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class HelperGetListVariable : HelperListVariable
    {
        public ListVariables.Position select = ListVariables.Position.First;
        public int index = 0;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(GameObject invoker)
        {
            ListVariables list = this.GetListVariables(invoker);
            Variable result = VariablesManager.GetListItem(list, this.select, this.index);
            return result != null ? result.Get() : null;
        }

        public void Set(object value, GameObject invoker = null)
        {
            ListVariables list = this.GetListVariables(invoker);
            list.Push(value, this.select, this.index);
        }

        public GameObject GetGameObject(GameObject invoker)
        {
            ListVariables list = this.GetListVariables(invoker);
            return (list != null ? list.gameObject : null);
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            return string.Format("list[{0}]", this.select.ToString());
        }

        public string ToStringValue(GameObject invoker)
        {
            object value = this.Get(invoker);
            return (value != null ? value.ToString() : "null");
        }
    }
}
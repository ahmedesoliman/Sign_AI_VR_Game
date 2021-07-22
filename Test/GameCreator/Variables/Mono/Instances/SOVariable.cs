namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SOVariable : ScriptableObject
    {
        public Variable variable = new Variable();

        // EDITOR METHODS: ------------------------------------------------------------------------

        #if UNITY_EDITOR

        public bool isExpanded = false;

        public bool CanSave()
        {
            return this.variable.CanSave();
        }

        #endif
    }
}
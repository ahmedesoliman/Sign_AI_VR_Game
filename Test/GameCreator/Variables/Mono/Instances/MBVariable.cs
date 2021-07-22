namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("")]
    public class MBVariable : MonoBehaviour
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
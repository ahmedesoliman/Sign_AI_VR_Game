namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public class DatabaseVariables : IDatabase
    {
        [Serializable]
        public class Container
        {
            public List<Variable> variables;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        [SerializeField] protected GlobalTags tags;
        [SerializeField] protected GlobalVariables variables;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GlobalVariables GetGlobalVariables()
        {
            return this.variables;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseVariables Load()
        {
            return IDatabase.LoadDatabase<DatabaseVariables>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseVariables>();
        }

        public override int GetSidebarPriority()
        {
            return 3;
        }

        #endif
    }
}
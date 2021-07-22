namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

	public class DatabaseQuickstart : IDatabase 
	{
		// PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseQuickstart Load()
        {
            return IDatabase.LoadDatabase<DatabaseQuickstart>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseQuickstart>();
        }

        public override int GetSidebarPriority()
        {
            return 0;
        }

        #endif
    }
}
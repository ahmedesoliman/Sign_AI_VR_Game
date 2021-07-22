namespace GameCreator.Feedback
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public class DatabaseFeedback : IDatabase
	{
        public enum FeedbackType
		{
			General,
			BugReport,
			FeatureRequest,
			Other
		}

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseFeedback Load()
        {
            return IDatabase.LoadDatabase<DatabaseFeedback>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseFeedback>();
        }

        public override int GetSidebarPriority()
        {
            return 101;
        }

        #endif
    }
}
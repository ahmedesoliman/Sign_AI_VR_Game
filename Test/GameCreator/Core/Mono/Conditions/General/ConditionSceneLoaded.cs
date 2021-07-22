namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.SceneManagement;

	[AddComponentMenu("")]
	public class ConditionSceneLoaded : ICondition
	{
        public string sceneName = "";

		public override bool Check(GameObject target)
		{
            Scene scene = SceneManager.GetSceneByName(this.sceneName);
            return scene.IsValid() && scene.isLoaded;
		}
        
		#if UNITY_EDITOR

        public static new string NAME = "General/Scene Loaded";
        private const string NODE_TITLE = "Is scene {0} loaded";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.sceneName);
        }

        #endif
    }
}

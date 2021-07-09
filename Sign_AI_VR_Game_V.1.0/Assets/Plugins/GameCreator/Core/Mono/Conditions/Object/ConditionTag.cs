namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ConditionTag : ICondition
	{
        public TargetGameObject targetGameObject = new TargetGameObject();

        [TagSelector] public string conditionTag = "";

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
		{
            GameObject result = this.targetGameObject.GetGameObject(target);
            if (result == null) return false;

            return result.CompareTag(this.conditionTag);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Tag";
		private const string NODE_TITLE = "Has {0} tag {1}";

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                this.targetGameObject,
                (string.IsNullOrEmpty(this.conditionTag) ? "none" : this.conditionTag)
            );
		}

		#endif
	}
}

namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ConditionEnabledComponent : ICondition
	{
        public enum CheckStatus
        {
            IsEnabled,
            IsDisabled
        }

		public Behaviour component;
		public CheckStatus state = CheckStatus.IsEnabled;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
		{
			if (!component) return false;

			if (component.isActiveAndEnabled && this.state == CheckStatus.IsEnabled) return true;
            if (!component.isActiveAndEnabled && this.state == CheckStatus.IsDisabled) return true;
			return false;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Component Enabled";
		private const string NODE_TITLE = "Component {0} {1}";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
                NODE_TITLE,
                this.component ? this.component.name : "(none)",
				ObjectNames.NicifyVariableName(this.state.ToString())
            );
		}

		#endif
	}
}

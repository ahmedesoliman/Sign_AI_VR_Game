namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionChangeColor : IAction
	{
        public TargetGameObject target = new TargetGameObject();
        [Space] public ColorProperty color = new ColorProperty(Color.white);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = this.target.GetGameObject(target);
            if (go != null)
            {
                Renderer render = go.GetComponent<Renderer>();
                if (render != null) render.material.color = this.color.GetValue(target);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Change Color";
        private const string NODE_TITLE = "Change {0} color";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                this.target
            );
		}

		#endif
	}
}

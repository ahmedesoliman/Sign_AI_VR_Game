namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;
    using UnityEngine.Serialization;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class ActionChangeMaterial : IAction
	{
        public Material material;

        [FormerlySerializedAs("target")]
        [Space] public TargetGameObject targetRenderer = new TargetGameObject();
        [Space] public NumberProperty materialIndex = new NumberProperty(0);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = this.targetRenderer.GetGameObject(target);
            if (go != null)
            {
                Renderer render = go.GetComponent<Renderer>();
                int matIndex = this.materialIndex.GetInt(target);

                if (render != null)
                {
                    if (matIndex >= 0 && matIndex < render.materials.Length)
                    {
                        Material[] materials = render.materials;
                        materials[matIndex] = this.material;

                        render.materials = materials;
                    }
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Change Material";
        private const string NODE_TITLE = "Change material {0}";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, (
                this.material == null
                ? "(none)"
                : this.material.name
            ));
		}

		#endif
	}
}

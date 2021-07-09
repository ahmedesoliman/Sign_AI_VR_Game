namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(AssetManifest))]
    public class AssetManifestEditor : Editor
    {
        private const string MODULE = "{0} - {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private AssetManifest manifest;

        // INITIALIZERS: --------------------------------------------------------------------------
        
		private void OnEnable()
		{
            this.manifest = (AssetManifest)target;
		}

        // PAINT METHODS: -------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            for (int i = 0; i < this.manifest.manifests.Length; ++i)
            {
                string module = string.Format(
                    MODULE,
                    this.manifest.manifests[i].module.moduleID,
                    this.manifest.manifests[i].module.version
                );

                EditorGUILayout.HelpBox(module, MessageType.None);
            }
		}
	}
}
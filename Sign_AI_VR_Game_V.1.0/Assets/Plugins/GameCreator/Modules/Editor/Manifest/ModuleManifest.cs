namespace GameCreator.ModuleManager
{
    using UnityEngine;
    using UnityEditor;

    [System.Serializable]
    public class ModuleManifest
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public Module module;

        // INITIALIZERS: --------------------------------------------------------------------------

        public ModuleManifest()
        {
            this.module = new Module();
        }

        public ModuleManifest(Module module)
        {
            this.module = module;
        }
    }
}
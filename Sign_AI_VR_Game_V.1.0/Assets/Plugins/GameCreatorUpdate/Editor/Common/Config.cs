namespace GameCreator.Update
{
    using System;
    using System.IO;
    using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [Serializable]
    public class Config : ScriptableObject
    {
        private const string CUR_PATH = "Assets/Plugins/GameCreatorData/Update/Editor/Config.asset";
        private const string UPD_PATH = "Assets/Plugins/GameCreatorUpdate/Data/Config.asset";

        private static Config CURRENT = null;
        private static Config UPDATE = null;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Version version = Version.NONE;
        public bool interactiveInstall = false;

        public string[] removeDirectories = new string[0];
        public string[] buildDirectories = new string[0];

        // METHODS: -------------------------------------------------------------------------------

        #if UNITY_EDITOR

        public static Config GetCurrent()
        {
            if (CURRENT != null) return CURRENT;

            CURRENT = AssetDatabase.LoadAssetAtPath<Config>(CUR_PATH);
            if (CURRENT == null)
            {
                CreatePaths(CUR_PATH);

                CURRENT = CreateInstance<Config>();
                AssetDatabase.CreateAsset(CURRENT, CUR_PATH);

                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(CUR_PATH);
            }

            return CURRENT;
        }

        public static Config GetUpdate()
        {
            if (UPDATE != null) return UPDATE;

            UPDATE = AssetDatabase.LoadMainAssetAtPath(UPD_PATH) as Config;
            if (UPDATE == null)
            {
                CreatePaths(UPD_PATH);

                UPDATE = CreateInstance<Config>();
                AssetDatabase.CreateAsset(UPDATE, UPD_PATH);

                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(UPD_PATH);
            }

            return UPDATE;
        }

        private static void CreatePaths(string directoryChain)
        {
            string[] folders = directoryChain.Split('/');
            string path = folders[0];
            for (int i = 1; i < folders.Length - 1; ++i)
            {
                string fullPath = Path.Combine(path, folders[i]);
                if (!AssetDatabase.IsValidFolder(fullPath))
                {
                    AssetDatabase.CreateFolder(path, folders[i]);
                }

                path = fullPath;
            }
        }

        #endif
    }
}
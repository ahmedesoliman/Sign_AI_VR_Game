namespace GameCreator.Core
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	using System.Linq;
	using System.Reflection;
	#endif

    public abstract class IDatabase : ScriptableObject
	{
		private const string DATABASE_RESOURCE_PATH = "GameCreator/Databases";

        // MAIN METHODS: --------------------------------------------------------------------------

        public static T LoadDatabaseCopy<T>() where T : IDatabase
		{
			T database = IDatabase.LoadDatabase<T>();
			return Instantiate(database);
		}

        public static T LoadDatabase<T>(bool onlyLoad = false) where T : IDatabase
		{
            string path = Path.Combine(
                DATABASE_RESOURCE_PATH, 
                IDatabase.GetAssetFilename(typeof(T), false)
            );

			T database = Resources.Load<T>(path);
            if (database == null && !onlyLoad) database = ScriptableObject.CreateInstance<T>();

			return database;
		}

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static string GetAssetFilename(Type type, bool withExtension)
        {
            string[] names = type.Name.Split(new char[] { '.' });

            string name = names[names.Length - 1];
            if (withExtension)
            {
                name = string.Format("{0}.asset", name);
            }

            return name;
        }

        // EDITOR METHODS: ------------------------------------------------------------------------

        #if UNITY_EDITOR

        protected static void Setup<T>() where T : IDatabase
        {
            EditorApplication.update += SetupDeferred<T>;
        }

        private static void SetupDeferred<T>() where T : IDatabase
        {
            EditorApplication.update -= SetupDeferred<T>;

            T database = ScriptableObject.CreateInstance<T>();
            string assetPath = database.GetAssetPath();
            IDatabase asset = AssetDatabase.LoadAssetAtPath<IDatabase>(assetPath);

            if (asset == null)
            {
                GameCreatorUtilities.CreateFolderStructure(assetPath);
                AssetDatabase.Refresh();

                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        // VIRTUAL & ABSTRACT METHODS: ------------------------------------------------------------

        protected virtual string GetProjectPath()
        {
            return "Assets/Plugins/GameCreatorData/Core/Resources";
        }

        protected virtual string GetResourcePath()
        {
            return "GameCreator/Databases";
        }

        protected virtual string GetAssetPath()
        {
            string assetPath = Path.Combine(
                this.GetProjectPath(),
                this.GetResourcePath()
            );

            return Path.Combine(
                assetPath, 
                IDatabase.GetAssetFilename(this.GetType(), true)
            );
        }

        public virtual int GetSidebarPriority()
        {
            return 50;
        }

        #endif
    }
}

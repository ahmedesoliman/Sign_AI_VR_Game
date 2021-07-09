namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    #if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    #endif

    public static class GameCreatorUtilities
	{
        #if UNITY_EDITOR

		private const string CHARACTERS = "abcdefghijklmnopqrstuvwxyz0123456789";
		private static int RANDOM_SEED = 0;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static string RandomHash(int count)
		{
			string hash = "";
			for (int i = 0; i < count; ++i)
			{
				int charPosition = GameCreatorUtilities.RandomValue(0, CHARACTERS.Length);
				hash += CHARACTERS[charPosition];
			}

			return hash;
		}

		public static int RandomValue(int min, int max)
		{
			if (RANDOM_SEED == 0) RANDOM_SEED = Guid.NewGuid().GetHashCode();
			return UnityEngine.Random.Range(min, max);
		}

        public static void CreateFolderStructure(string path)
        {
            string[] pathSplit = path.Split(new char[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
            string stackPath = pathSplit[0];

            for (int i = 1; i < pathSplit.Length; ++i)
            {
                string thisFolder = pathSplit[i];
                string indexPath = Path.Combine(stackPath, thisFolder);
                if (!AssetDatabase.IsValidFolder(indexPath))
                {
                    string guid = AssetDatabase.CreateFolder(stackPath, thisFolder);
                    stackPath = AssetDatabase.GUIDToAssetPath(guid);
                }
                else
                {
                    stackPath = Path.Combine(stackPath, thisFolder);
                }
            }
        }

        public static List<T> FindAssetsByType<T>() where T : ScriptableObject
        {
            List<T> assets = new List<T>();

            string search = string.Format("t:{0}", typeof(T).Name);
            string[] guids = AssetDatabase.FindAssets(search, null);

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset) assets.Add(asset);
            }

            return assets;
        }

        #endif
	}
}

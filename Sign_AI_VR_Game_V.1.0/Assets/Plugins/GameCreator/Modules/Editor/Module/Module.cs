namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [System.Serializable]
    public class Module
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public string moduleID;
        public Version version;

        public string displayName;
        public string description;
        public string category;

        public Dependency[] dependencies = new Dependency[0];
        public string[] tags = new string[0];

        public string authorName;
        public string authorMail;
        public string authorSite;

        public bool includesData  = false;
        public string[] codePaths = new string[0];
        public string[] dataPaths = new string[0];

        // INITIALIZERS: --------------------------------------------------------------------------

        public Module()
        {

        }

        public static void UpdateModule(SerializedProperty spModule, Module other)
        {
            spModule.FindPropertyRelative("moduleID").stringValue = other.moduleID;

            SerializedProperty spVersion = spModule.FindPropertyRelative("version");
            spVersion.FindPropertyRelative("major").intValue = other.version.major;
            spVersion.FindPropertyRelative("minor").intValue = other.version.minor;
            spVersion.FindPropertyRelative("patch").intValue = other.version.patch;

            spModule.FindPropertyRelative("displayName").stringValue = other.displayName;
            spModule.FindPropertyRelative("description").stringValue = other.description;
            spModule.FindPropertyRelative("category").stringValue = other.category;

            SerializedProperty spDependencies = spModule.FindPropertyRelative("dependencies");
            spDependencies.arraySize = other.dependencies.Length;
            for (int i = 0; i < other.dependencies.Length; ++i)
            {
                SerializedProperty spDependency = spDependencies.GetArrayElementAtIndex(i);
                spDependency.FindPropertyRelative("moduleID").stringValue = other.dependencies[i].moduleID;

                SerializedProperty spDependencyVersion = spDependency.FindPropertyRelative("version");
                spDependencyVersion.FindPropertyRelative("major").intValue = other.dependencies[i].version.major;
                spDependencyVersion.FindPropertyRelative("minor").intValue = other.dependencies[i].version.minor;
                spDependencyVersion.FindPropertyRelative("patch").intValue = other.dependencies[i].version.patch;
            }

            SerializedProperty spTags = spModule.FindPropertyRelative("tags");
            spTags.arraySize = other.tags.Length;
            for (int i = 0; i < other.tags.Length; ++i)
            {
                spTags.GetArrayElementAtIndex(i).stringValue = other.tags[i];
            }

            spModule.FindPropertyRelative("authorName").stringValue = other.authorName;
            spModule.FindPropertyRelative("authorMail").stringValue = other.authorMail;
            spModule.FindPropertyRelative("authorSite").stringValue = other.authorSite;

            spModule.FindPropertyRelative("includesData").boolValue = other.includesData;

            SerializedProperty spCodePaths = spModule.FindPropertyRelative("codePaths");
            spCodePaths.arraySize = other.codePaths.Length;
            for (int i = 0; i < other.codePaths.Length; ++i)
            {
                spCodePaths.GetArrayElementAtIndex(i).stringValue = other.codePaths[i];
            }

            SerializedProperty spDataPaths = spModule.FindPropertyRelative("dataPaths");
            spDataPaths.arraySize = other.dataPaths.Length;
            for (int i = 0; i < other.dataPaths.Length; ++i)
            {
                spDataPaths.GetArrayElementAtIndex(i).stringValue = other.dataPaths[i];
            }
        }
    }
}
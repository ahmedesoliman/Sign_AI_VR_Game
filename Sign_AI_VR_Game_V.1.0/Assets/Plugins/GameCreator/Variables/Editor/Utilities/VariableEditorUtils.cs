namespace GameCreator.Variables
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class VariableEditorUtils
    {
        private const string VARIABLE_ICONS_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Variables/";
        private const string VARIABLE_ICON_DEFAULT_NAME = "Default.png";
        private static Dictionary<string, Texture2D> VARIABLE_ICONS;
        private static readonly string[] VARIABLE_ICON_NAMES = new string[]
        {
            "Null.png",
            "String.png",
            "Number.png",
            "Bool.png",
            "Color.png",
            "Vector2.png",
            "Vector3.png",
            "Texture2D.png",
            "Sprite.png",
            "Game Object.png",
            "Transform.png",
            "Rigidbody.png",
        };

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static Texture2D GetIcon(Variable.DataType type)
        {
            string name = VARIABLE_ICON_NAMES[(int)type];

            if (VARIABLE_ICONS == null) VARIABLE_ICONS = new Dictionary<string, Texture2D>();
            if (!VARIABLE_ICONS.ContainsKey(name))
            {
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    Path.Combine(VARIABLE_ICONS_PATH, name)
                );

                if (icon == null)
                {
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        Path.Combine(VARIABLE_ICONS_PATH, VARIABLE_ICON_DEFAULT_NAME)
                    );
                }

                VARIABLE_ICONS.Add(name, icon);
            }

            return VARIABLE_ICONS[name];
        }
    }
}
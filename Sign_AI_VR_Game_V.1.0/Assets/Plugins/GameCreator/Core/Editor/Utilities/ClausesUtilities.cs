namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class ClausesUtilities
    {
        public enum Icon
        {
            Copy,
            Duplicate,
            Paste,
            Delete,
            Play,
            If,
            Then,
            Else,
            Collapse,
            Expand,
            EyeOpen,
            EyeClosed
        }

        private const string ICONS_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Utilities/{0}";
        private static readonly string[] ICONS = new string[]
        {
            "Copy{0}.png",
            "Duplicate{0}.png",
            "Paste{0}.png",
            "Delete{0}.png",
            "Play{0}.png",
            "If{0}.png",
            "Then{0}.png",
            "Else{0}.png",
            "Collapse{0}.png",
            "Expand{0}.png",
            "EyeOpen{0}.png",
            "EyeClosed{0}.png",
        };

        private static readonly string[] TEXTS = new string[]
        {
            "",
            "",
            "",
            "",
            "",
            " If",
            " Then",
            " Else",
            "",
            "",
            "",
            ""
        };

        private static readonly string[] TOOLTIPS = new string[]
        {
            "Copy",
            "Duplicate",
            "Paste",
            "Delete",
            "Play",
            "If all conditions are true",
            "Execute the following Action",
            "If no conditions were true, execute this",
            "Collapse All",
            "Expand All",
            "Visible",
            "Hidden"
        };

        private static GUIContent[] GUICONTENTS = new GUIContent[12];

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static GUIContent Get(Icon icon)
        {
            int iconIndex = (int)icon;
            if (GUICONTENTS[iconIndex] == null)
            {
                string iconName = string.Format(ICONS[iconIndex], EditorGUIUtility.isProSkin
                    ? "@pro"
                    : "@personal"
                );

                string path = string.Format(ICONS_PATH, iconName);
                Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                string iconText = TEXTS[iconIndex];
                string iconTooltip = TOOLTIPS[iconIndex];
                GUICONTENTS[iconIndex] = new GUIContent(iconText, iconTexture, iconTooltip);
            }

            return GUICONTENTS[iconIndex];
        }
    }
}
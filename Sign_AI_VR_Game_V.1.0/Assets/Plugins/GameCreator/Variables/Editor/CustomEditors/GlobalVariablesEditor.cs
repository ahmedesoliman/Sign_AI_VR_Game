namespace GameCreator.Variables
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using GameCreator.Core;

    [CustomEditor(typeof(GlobalVariables))]
    public class GlobalVariablesEditor : GenericVariablesEditor<SOVariableEditor, SOVariable>
    {
        private const string LABEL_TITLE = "Global Variables";

        public const string PATH_ASSET = "Assets/Plugins/GameCreatorData/Core/Variables/";
        public const string NAME_ASSET = "Variables.asset";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SearchField searchField;

        private Rect editTagsRect = Rect.zero;
        private GlobalVariables instance;

		// INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            this.searchField = new SearchField();
            this.instance = this.target as GlobalVariables;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

		// PAINT METHODS: -------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            this.PaintSearch();

            EditorGUILayout.LabelField(LABEL_TITLE, EditorStyles.boldLabel);
            base.OnInspectorGUI(this.search, this.tagsMask);
		}

        private void PaintSearch()
        {
            EditorGUILayout.BeginHorizontal();

            this.search = this.searchField.OnGUI(this.search);
            GUILayoutOption[] options = new GUILayoutOption[]
            {
                GUILayout.Width(80f),
                GUILayout.Height(18f)
            };

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.search));
            string[] tags = GlobalTagsEditor.GetTagNames();
            this.tagsMask = EditorGUILayout.MaskField(
                GUIContent.none,
                this.tagsMask,
                tags,
                EditorStyles.miniButtonLeft, 
                options
            );
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Edit Tags", EditorStyles.miniButtonRight, options))
            {
                PopupWindow.Show(this.editTagsRect, new TagsEditorWindow());
            }

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                this.editTagsRect = GUILayoutUtility.GetLastRect();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		protected override SOVariable[] GetReferences()
        {
            return this.instance.references;
        }
   
        protected override string GetReferenceName(int index)
        {
            if (index < 0 || index >= this.instance.references.Length)
            {
                return "<i>Unbound Variable</i>";
            }

            if (this.instance.references[index] == null)
            {
                return "<i>Undefined Variable</i>";
            }

            return this.subEditors[index].GetName();
        }

        protected override Variable.DataType GetReferenceType(int index)
        {
            if (index >= this.instance.references.Length) return Variable.DataType.Null;

            SOVariable reference = this.instance.references[index];
            if (reference == null) return Variable.DataType.Null;

            Variable variable = reference.variable;
            return (Variable.DataType)variable.type;
        }

        protected override bool MatchSearch(int index, string search, int tagsMask)
        {
            if (index >= this.subEditors.Length) return false;
            if (this.subEditors[index] == null) return false;
            return this.subEditors[index].MatchSearch(search, tagsMask);
        }

		protected override SOVariable CreateReferenceInstance(string name)
        {
            SOVariable variable = ScriptableObject.CreateInstance<SOVariable>();
            variable.name = GameCreatorUtilities.RandomHash(8);
            variable.variable.name = name;

            AssetDatabase.AddObjectToAsset(variable, this.instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(Path.Combine(PATH_ASSET, NAME_ASSET));

            return variable;
        }

        protected override void DeleteReferenceInstance(int index)
        {
            SOVariable source = (SOVariable)this.spReferences
                .GetArrayElementAtIndex(index)
                .objectReferenceValue;

            this.spReferences.RemoveFromObjectArrayAt(index);
            this.RemoveSubEditorsElement(index);

            DestroyImmediate(source, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(Path.Combine(PATH_ASSET, NAME_ASSET));
            Undo.ClearAll();
        }

		protected override Tag[] GetReferenceTags(int index)
		{
            List<Tag> result = new List<Tag>();

            if (index >= this.instance.references.Length) return result.ToArray();

            SOVariable reference = this.instance.references[index];
            if (reference == null) return result.ToArray();

            int mask = reference.variable.tags;
            Tag[] tags = GlobalTagsEditor.GetTags();

            for (int i = 0; i < tags.Length; ++i)
            {
                if ((mask & 1) != 0)
                {
                    result.Add(tags[i]);
                }

                mask >>= 1;
            }

            return result.ToArray();
		}
	}
}
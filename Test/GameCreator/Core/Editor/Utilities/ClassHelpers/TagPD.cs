namespace GameCreator.Core
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        private const string UNTAGGED = "Untagged";
        public List<string> tags = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (tags.Count == 0) this.BuildTags();

                EditorGUI.BeginProperty(position, label, property);

                string propertyString = property.stringValue;
                int index = -1;
                if (propertyString == "")
                {
                    index = 0;
                }
                else
                {
                    for (int i = 1; i < this.tags.Count; i++)
                    {
                        if (this.tags[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                index = EditorGUI.Popup(position, label.text, index, this.tags.ToArray());
                if (index >= 1)
                {
                    property.stringValue = this.tags[index];
                }
                else
                {
                    property.stringValue = "";
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void BuildTags()
        {
            this.tags = new List<string>();

            this.tags.Add(UNTAGGED);
            this.tags.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
        }
    }
}
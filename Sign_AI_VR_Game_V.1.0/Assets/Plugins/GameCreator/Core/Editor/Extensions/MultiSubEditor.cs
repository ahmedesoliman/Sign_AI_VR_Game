namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;

    public abstract class MultiSubEditor<TEditor, TTarget> : Editor
        where TEditor : Editor
        where TTarget : Object
    {
        private const string PROP_IS_EXPANDED = "isExpanded";
		private const float ANIM_BOOL_SPEED = 3.0f;

		public TEditor[] subEditors { get; private set; }

        protected AnimBool[] isExpanded;
        protected SerializedProperty[] expandProp;

		protected Rect[] handleRect;
		protected Rect[] objectRect;

        public bool forceInitialize;

        public void UpdateSubEditors(TTarget[] subInstances)
		{
            if (serializedObject == null) return;

            if (!this.forceInitialize && this.subEditors != null &&
                this.subEditors.Length == subInstances.Length)
            {
                bool difference = false;
                for (int i = 0; i < this.subEditors.Length; ++i)
                {
                    if (this.subEditors[i] == null) continue;
                    if (subInstances[i] == null) continue;

                    if (this.subEditors[i].target == subInstances[i]) continue;
                    difference = true;
                    break;
                }

                if (!difference) return;
            }

            this.forceInitialize = false;
            this.CleanSubEditors();

			this.subEditors = new TEditor[subInstances.Length];
			this.isExpanded = new AnimBool[subInstances.Length];
			this.handleRect = new Rect[subInstances.Length];
			this.objectRect = new Rect[subInstances.Length];
            this.expandProp = new SerializedProperty[subInstances.Length];

            int length = subInstances.Length;
			for (int i = 0; i < length; i++)
			{
                if (subInstances[i] != null)
				{
					this.subEditors[i] = Editor.CreateEditor(subInstances[i]) as TEditor;
					this.Setup(this.subEditors[i], i);

                    this.expandProp[i] = this.subEditors[i]
                        .serializedObject
                        .FindProperty(PROP_IS_EXPANDED);
				}

                bool expand = (this.expandProp[i] != null && this.expandProp[i].boolValue);

				this.handleRect[i] = Rect.zero;
				this.objectRect[i] = Rect.zero;
                this.isExpanded[i] = new AnimBool(expand) { speed = ANIM_BOOL_SPEED };
                this.isExpanded[i].valueChanged.AddListener(this.Repaint);
			}
		}

		public void CleanSubEditors()
		{
			if (this.subEditors == null) return;

			for (int i = 0; i < subEditors.Length; i++)
			{
				if (this.subEditors[i] == null) continue;
				DestroyImmediate(this.subEditors[i]);
			}

			this.subEditors = null;
			this.isExpanded = null;
			this.handleRect = null;
			this.objectRect = null;
            this.expandProp = null;
		}

        public void ToggleExpand(int index)
        {
            this.SetExpand(index, !this.isExpanded[index].target);
        }

        public void SetExpand(int index, bool state)
        {
            this.isExpanded[index].target = state;
            if (this.expandProp[index] != null)
            {
                this.subEditors[index].serializedObject.Update();
                this.expandProp[index].boolValue = state;

                this.subEditors[index].serializedObject.ApplyModifiedPropertiesWithoutUndo();
                this.subEditors[index].serializedObject.Update();
            }
        }

		protected void AddSubEditorElement(TTarget target, int index, bool openItem)
		{
			List<TEditor> tmpSubEditor = new List<TEditor>(this.subEditors);
			List<AnimBool> tmpIsExpanded = new List<AnimBool>(this.isExpanded);
			List<Rect> tmpHandleRect = new List<Rect>(this.handleRect);
			List<Rect> tmpActionRect = new List<Rect>(this.objectRect);
            List<SerializedProperty> tmpExpandProp = new List<SerializedProperty>(this.expandProp);

			if (index < 0) index = this.subEditors.Length;

			if (index >= this.subEditors.Length) tmpSubEditor.Add(Editor.CreateEditor(target) as TEditor);
			else tmpSubEditor.Insert(index, Editor.CreateEditor(target) as TEditor);

			this.Setup(tmpSubEditor[index], index);


			AnimBool element = new AnimBool(false);
			element.target = openItem;
			element.speed = ANIM_BOOL_SPEED;
			element.valueChanged.AddListener(this.Repaint);

			if (index >= this.subEditors.Length) tmpHandleRect.Add(Rect.zero);
			else tmpHandleRect.Insert(index, Rect.zero);

			if (index >= this.subEditors.Length) tmpActionRect.Add(Rect.zero);
			else tmpActionRect.Insert(index, Rect.zero);

			if (index >= this.subEditors.Length) tmpIsExpanded.Add(element);
			else tmpIsExpanded.Insert(index, element);

            SerializedProperty expandProperty = tmpSubEditor[index].serializedObject.FindProperty(PROP_IS_EXPANDED);
            if (expandProperty != null)
            {
                tmpSubEditor[index].serializedObject.Update();
                expandProperty.boolValue = openItem;
                tmpSubEditor[index].serializedObject.ApplyModifiedPropertiesWithoutUndo();
                tmpSubEditor[index].serializedObject.Update();
            }

            if (index >= this.subEditors.Length) tmpExpandProp.Add(expandProperty);
            else tmpExpandProp.Insert(index, expandProperty);

			this.subEditors = tmpSubEditor.ToArray();
			this.isExpanded = tmpIsExpanded.ToArray();
			this.handleRect = tmpHandleRect.ToArray();
			this.objectRect = tmpActionRect.ToArray();
            this.expandProp = tmpExpandProp.ToArray();
		}

		protected void MoveSubEditorsElement(int srcIndex, int dstIndex)
		{
			if (srcIndex == dstIndex) return;

			TEditor tmpSubEditor = this.subEditors[srcIndex];
			AnimBool tmpIsExpanded = this.isExpanded[srcIndex];
			Rect tmpHandleRect = this.handleRect[srcIndex];
			Rect tmpActionRect = this.objectRect[srcIndex];
            SerializedProperty tmpExpandProp = this.expandProp[srcIndex];

			if (dstIndex < srcIndex)
			{
				System.Array.Copy(this.subEditors, dstIndex, this.subEditors, dstIndex + 1, srcIndex - dstIndex);
				System.Array.Copy(this.isExpanded, dstIndex, this.isExpanded, dstIndex + 1, srcIndex - dstIndex);
				System.Array.Copy(this.handleRect, dstIndex, this.handleRect, dstIndex + 1, srcIndex - dstIndex);
				System.Array.Copy(this.objectRect, dstIndex, this.objectRect, dstIndex + 1, srcIndex - dstIndex);
                System.Array.Copy(this.expandProp, dstIndex, this.expandProp, dstIndex + 1, srcIndex - dstIndex);
			}
			else
			{
				System.Array.Copy(this.subEditors, srcIndex + 1, this.subEditors, srcIndex, dstIndex - srcIndex);
				System.Array.Copy(this.isExpanded, srcIndex + 1, this.isExpanded, srcIndex, dstIndex - srcIndex);
				System.Array.Copy(this.handleRect, srcIndex + 1, this.handleRect, srcIndex, dstIndex - srcIndex);
				System.Array.Copy(this.objectRect, srcIndex + 1, this.objectRect, srcIndex, dstIndex - srcIndex);
                System.Array.Copy(this.expandProp, srcIndex + 1, this.expandProp, srcIndex, dstIndex - srcIndex);
			}

			this.subEditors[dstIndex] = tmpSubEditor;
			this.isExpanded[dstIndex] = tmpIsExpanded;
			this.handleRect[dstIndex] = tmpHandleRect;
			this.objectRect[dstIndex] = tmpActionRect;
            this.expandProp[dstIndex] = tmpExpandProp;
		}

		protected void RemoveSubEditorsElement(int removeIndex)
		{
			List<TEditor> tmpSubEditor = new List<TEditor>(this.subEditors);
			List<AnimBool> tmpIsExpanded = new List<AnimBool>(this.isExpanded);
			List<Rect> tmpHandleRect = new List<Rect>(this.handleRect);
			List<Rect> tmpActionRect = new List<Rect>(this.objectRect);
            List<SerializedProperty> tmpExpandProp = new List<SerializedProperty>(this.expandProp);

			tmpSubEditor.RemoveAt(removeIndex);
			tmpIsExpanded.RemoveAt(removeIndex);
			tmpHandleRect.RemoveAt(removeIndex);
			tmpActionRect.RemoveAt(removeIndex);
            tmpExpandProp.RemoveAt(removeIndex);

			this.subEditors = tmpSubEditor.ToArray();
			this.isExpanded = tmpIsExpanded.ToArray();
			this.handleRect = tmpHandleRect.ToArray();
			this.objectRect = tmpActionRect.ToArray();
            this.expandProp = tmpExpandProp.ToArray();
		}

        protected virtual void Setup(TEditor editor, int editorIndex) { }
	}
}

namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomEditor(typeof(AnimationClipGroup))]
    public class AnimationClipGroupEditor : Editor
    {
        private AnimationClipGroup group;
        private bool isDragginClip = false;

        // INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
        {
            this.group = this.target as AnimationClipGroup;
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (this.target == null || this.group == null) return;
            if (!string.IsNullOrEmpty(this.group.message))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(this.group.message, MessageType.Info);
                EditorGUILayout.Space();
            }

            this.PaintDropClip();
            this.PaintAssets();
        }

        private void PaintDropClip()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Event evt = Event.current;
            Rect dropRect = GUILayoutUtility.GetRect(
                0.0f,
                50.0f,
                GUILayout.ExpandWidth(true)
            );

            GUIStyle styleDropZone = (this.isDragginClip
                ? CoreGUIStyles.GetDropZoneActive()
                : CoreGUIStyles.GetDropZoneNormal()
            );
                
            GUI.Box(dropRect, "Drop animation clip", styleDropZone);

            Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            buttonRect = new Rect(
                buttonRect.x + EditorGUIUtility.labelWidth,
                buttonRect.y,
                buttonRect.width - EditorGUIUtility.labelWidth,
                buttonRect.height
            );

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    this.isDragginClip = false;
                    if (!dropRect.Contains(evt.mousePosition)) break;
                    if (DragAndDrop.objectReferences.Length == 0) break;


                    AnimationClip draggedClip = DragAndDrop.objectReferences[0] as AnimationClip;
                    if (draggedClip == null) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragUpdated)
                    {
                        this.isDragginClip = true;
                    }
                    else if (evt.type == EventType.DragPerform)
                    {
                        this.isDragginClip = false;

                        DragAndDrop.AcceptDrag();

                        AnimationClip clip = Instantiate(draggedClip);
                        clip.name = draggedClip.name;

                        AssetDatabase.AddObjectToAsset(clip, this.group);
                        string path = AssetDatabase.GetAssetPath(this.group);
                        AssetDatabase.ImportAsset(path);

                        Event.current.Use();
                    }
                    break;
            }
        }

        private const float DELETE_WIDTH = 25f;

        private void PaintAssets()
        {
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(this.target.GetInstanceID())
            );

            int removeIndex = -1;
            for (int i = 0; i < subAssets.Length; ++i)
            {
                if (subAssets[i] as AnimationClip == null) continue;

                Rect rectTotal = GUILayoutUtility.GetRect(
                    GUIContent.none, 
                    CoreGUIStyles.GetButtonMid()
                );

                Rect rectLabel = new Rect(
                    rectTotal.x,
                    rectTotal.y,
                    rectTotal.width - DELETE_WIDTH,
                    rectTotal.height
                );

                Rect rectButon = new Rect(
                    rectLabel.x + rectLabel.width,
                    rectLabel.y,
                    DELETE_WIDTH,
                    rectLabel.height
                );

                EditorGUI.LabelField(
                    rectLabel, 
                    subAssets[i].name, 
                    CoreGUIStyles.GetToggleButtonLeftOn()
                );

                if (GUI.Button(rectButon, "X", CoreGUIStyles.GetButtonRight()))
                {
                    removeIndex = i;
                }

            }

            if (removeIndex >= 0)
            {
                AssetDatabase.RemoveObjectFromAsset(subAssets[removeIndex]);

                string path = AssetDatabase.GetAssetPath(this.target);
                AssetDatabase.ImportAsset(path);
            }
        }
    }
}
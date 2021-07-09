namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using UnityEngine.UI;
    using UnityEditor.UI;

    #if UNITY_EDITOR
    using UnityEditor.SceneManagement;
    #endif


    public abstract class CreateSceneObject
	{
        private const string NAME_FMT = "{0} ({1})";

        private static DefaultControls.Resources standardResources;
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";

        // PUBLIC METHODS: ------------------------------------------------------------------------

		public static GameObject Create(string name, bool autoSelect = true)
		{
            if (GameObject.Find(name) != null)
            {
                int index = 1;
                string uniqueName = "";

                do 
                {
                    uniqueName = string.Format(NAME_FMT, name, index);
                    ++index;
                } while (GameObject.Find(uniqueName) != null);
                name = uniqueName;
            }

			GameObject newGameObject = new GameObject(name);
            return CreateSceneObject.Create(newGameObject, autoSelect);
		}

		public static GameObject Create(GameObject newGameObject, bool autoSelect = true)
		{
            if (Selection.activeTransform != null && Selection.activeGameObject != newGameObject)
            {
                newGameObject.transform.SetParent(Selection.activeTransform);
                newGameObject.transform.localPosition = Vector3.zero;
            }
            else if (SceneView.currentDrawingSceneView != null)
            {
                Transform camTrans = SceneView.currentDrawingSceneView.camera.transform;
                Ray ray = new Ray(camTrans.position, camTrans.TransformDirection(Vector3.forward));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    newGameObject.transform.position = hit.point;
                }
            }

            #if UNITY_EDITOR
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(newGameObject.scene);
            #endif

            if (autoSelect) Selection.activeGameObject = newGameObject;
            return newGameObject;
		}

        // PUBLIC UI METHODS: ---------------------------------------------------------------------

        public static DefaultControls.Resources GetStandardResources()
        {
            if (standardResources.standard == null)
            {
                standardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                standardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                standardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                standardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                standardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                standardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                standardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
            }

            return standardResources;
        }

        public static GameObject GetCanvasGameObject()
        {
            GameObject selection = Selection.activeGameObject;

            Canvas canvas = (selection != null) ? selection.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }
            
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }

            GameObject newObject = new GameObject("Canvas");
            newObject.AddComponent<Canvas>();
            newObject.AddComponent<CanvasScaler>();
            newObject.AddComponent<GraphicRaycaster>();
            return newObject;
        }
	}
}
namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public abstract class CoreGUIStyles
	{
        private const string TAG_BG_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Variables/Tag.png";
        private const string DROPZONE_NORMAL_PATH = "Assets/Plugins/GameCreator/Characters/Icons/DropZoneNormal.png";
        private const string DROPZONE_ACTIVE_PATH = "Assets/Plugins/GameCreator/Characters/Icons/DropZoneActive.png";
        private const string DROP_MARK_PATH = "Assets/Plugins/GameCreator/Extra/Icons/EditorUI/DropMark.png";

        private static GUIStyle btnLeft;
		private static GUIStyle btnRight;
		private static GUIStyle btnMid;
		private static GUIStyle btnToggleLeftAdd;
		private static GUIStyle btnToggleLeftOn;
		private static GUIStyle btnToggleLeftOff;
		private static GUIStyle btnToggleMidOn;
		private static GUIStyle btnToggleMidOff;
		private static GUIStyle btnToggleRightOn;
		private static GUIStyle btnToggleRightOff;
		private static GUIStyle btnToggleNormalOn;
		private static GUIStyle btnToggleNormalOff;
		private static GUIStyle btnToggleOn;
		private static GUIStyle btnToggleOff;
		private static GUIStyle highlight;
		private static GUIStyle dropMark;
		private static GUIStyle boxExpanded;
		private static GUIStyle box;
		private static GUIStyle helpBox;
        private static GUIStyle searchbox;
		private static GUIStyle btnToolbar;
		private static GUIStyle btnGridLeftOn;
		private static GUIStyle btnGridLeftOff;
		private static GUIStyle btnGridMidOn;
		private static GUIStyle btnGridMidOff;
		private static GUIStyle btnGridRightOn;
		private static GUIStyle btnGridRightOff;
		private static GUIStyle btnRigArrow;
		private static GUIStyle btnLftArrow;
		private static GUIStyle itemPreferencesSidebar;
        private static GUIStyle labelTag;
        private static GUIStyle textarea;
        private static GUIStyle dropZoneNormal;
        private static GUIStyle dropZoneActive;
        private static GUIStyle globalIDText;

		public static GUIStyle GetButtonLeft()
		{
			if (CoreGUIStyles.btnLeft == null) 
			{
				CoreGUIStyles.btnLeft = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
				CoreGUIStyles.btnLeft.richText = true;
			}

			return CoreGUIStyles.btnLeft;
		}

		public static GUIStyle GetButtonRight()
		{
			if (CoreGUIStyles.btnRight == null) 
			{
				CoreGUIStyles.btnRight = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
				CoreGUIStyles.btnRight.richText = true;
			}
			return CoreGUIStyles.btnRight;
		}

		public static GUIStyle GetButtonMid()
		{
			if (CoreGUIStyles.btnMid == null) 
			{
				CoreGUIStyles.btnMid = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
				CoreGUIStyles.btnMid.richText = true;
			}
			return CoreGUIStyles.btnMid;
		}

		public static GUIStyle GetToggleButtonLeftOn()
		{
			if (CoreGUIStyles.btnToggleLeftOn == null) 
			{
				CoreGUIStyles.btnToggleLeftOn = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
				CoreGUIStyles.btnToggleLeftOn.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleLeftOn.normal = CoreGUIStyles.btnToggleLeftOn.onNormal;
				CoreGUIStyles.btnToggleLeftOn.hover = CoreGUIStyles.btnToggleLeftOn.onHover;
				CoreGUIStyles.btnToggleLeftOn.active = CoreGUIStyles.btnToggleLeftOn.onActive;
				CoreGUIStyles.btnToggleLeftOn.focused = CoreGUIStyles.btnToggleLeftOn.onFocused;
				CoreGUIStyles.btnToggleLeftOn.richText = true;
				CoreGUIStyles.btnToggleLeftOn.margin.bottom = 0;
			}

			return CoreGUIStyles.btnToggleLeftOn;
		}

		public static GUIStyle GetToggleButtonLeftOff()
		{
			if (CoreGUIStyles.btnToggleLeftOff == null) 
			{
				CoreGUIStyles.btnToggleLeftOff = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
				CoreGUIStyles.btnToggleLeftOff.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleLeftOff.richText = true;
				CoreGUIStyles.btnToggleLeftOff.margin.bottom = 0;
			}
			return CoreGUIStyles.btnToggleLeftOff;
		}

		public static GUIStyle GetToggleButtonLeftAdd()
		{
			if (CoreGUIStyles.btnToggleLeftAdd == null)
			{
				CoreGUIStyles.btnToggleLeftAdd = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
				CoreGUIStyles.btnToggleLeftAdd.alignment = TextAnchor.MiddleCenter;
				CoreGUIStyles.btnToggleLeftAdd.richText = true;
			}
			return CoreGUIStyles.btnToggleLeftAdd;
		}

		public static GUIStyle GetToggleButtonMidOn()
		{
			if (CoreGUIStyles.btnToggleMidOn == null) 
			{
				CoreGUIStyles.btnToggleMidOn = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
				CoreGUIStyles.btnToggleMidOn.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleMidOn.normal = CoreGUIStyles.btnToggleMidOn.onNormal;
				CoreGUIStyles.btnToggleMidOn.hover = CoreGUIStyles.btnToggleMidOn.onHover;
				CoreGUIStyles.btnToggleMidOn.active = CoreGUIStyles.btnToggleMidOn.onActive;
				CoreGUIStyles.btnToggleMidOn.focused = CoreGUIStyles.btnToggleMidOn.onFocused;
				CoreGUIStyles.btnToggleMidOn.richText = true;
				CoreGUIStyles.btnToggleMidOn.margin.bottom = 0;
			}

			return CoreGUIStyles.btnToggleMidOn;
		}

		public static GUIStyle GetToggleButtonMidOff()
		{
			if (CoreGUIStyles.btnToggleMidOff == null) 
			{
				CoreGUIStyles.btnToggleMidOff = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
				CoreGUIStyles.btnToggleMidOff.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleMidOff.richText = true;
				CoreGUIStyles.btnToggleMidOff.margin.bottom = 0;
			}
			return CoreGUIStyles.btnToggleMidOff;
		}

		public static GUIStyle GetToggleButtonRightOn()
		{
			if (CoreGUIStyles.btnToggleRightOn == null) 
			{
				CoreGUIStyles.btnToggleRightOn = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
				CoreGUIStyles.btnToggleRightOn.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleRightOn.normal = CoreGUIStyles.btnToggleRightOn.onNormal;
				CoreGUIStyles.btnToggleRightOn.hover = CoreGUIStyles.btnToggleRightOn.onHover;
				CoreGUIStyles.btnToggleRightOn.active = CoreGUIStyles.btnToggleRightOn.onActive;
				CoreGUIStyles.btnToggleRightOn.focused = CoreGUIStyles.btnToggleRightOn.onFocused;
				CoreGUIStyles.btnToggleRightOn.richText = true;
				CoreGUIStyles.btnToggleRightOn.margin.bottom = 0;
			}

			return CoreGUIStyles.btnToggleRightOn;
		}

		public static GUIStyle GetToggleButtonRightOff()
		{
			if (CoreGUIStyles.btnToggleRightOff == null) 
			{
				CoreGUIStyles.btnToggleRightOff = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
				CoreGUIStyles.btnToggleRightOff.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleRightOff.richText = true;
				CoreGUIStyles.btnToggleRightOff.margin.bottom = 0;
			}
			return CoreGUIStyles.btnToggleRightOff;
		}

		public static GUIStyle GetToggleButtonNormalOn()
		{
			if (CoreGUIStyles.btnToggleNormalOn == null) 
			{
				CoreGUIStyles.btnToggleNormalOn = new GUIStyle(CoreGUIStyles.GetToggleButtonNormalOff());
				CoreGUIStyles.btnToggleNormalOn.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleNormalOn.normal = CoreGUIStyles.btnToggleNormalOn.onNormal;
				CoreGUIStyles.btnToggleNormalOn.hover = CoreGUIStyles.btnToggleNormalOn.onHover;
				CoreGUIStyles.btnToggleNormalOn.active = CoreGUIStyles.btnToggleNormalOn.onActive;
				CoreGUIStyles.btnToggleNormalOn.focused = CoreGUIStyles.btnToggleNormalOn.onFocused;
				CoreGUIStyles.btnToggleNormalOn.richText = true;
				CoreGUIStyles.btnToggleNormalOn.margin.bottom = 0;
			}
			return CoreGUIStyles.btnToggleNormalOn;
		}

		public static GUIStyle GetToggleButtonNormalOff()
		{
			if (CoreGUIStyles.btnToggleNormalOff == null) 
			{
				CoreGUIStyles.btnToggleNormalOff = new GUIStyle(GUI.skin.GetStyle("Button"));
				CoreGUIStyles.btnToggleNormalOff.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleNormalOff.richText = true;
				CoreGUIStyles.btnToggleNormalOff.fixedHeight = 20f;
				CoreGUIStyles.btnToggleNormalOff.margin.bottom = 0;
			}
			return CoreGUIStyles.btnToggleNormalOff;
		}

		public static GUIStyle GetToggleButtonOn()
		{
			if (CoreGUIStyles.btnToggleOn == null) 
			{
				CoreGUIStyles.btnToggleOn = new GUIStyle(CoreGUIStyles.GetToggleButtonOff());
				CoreGUIStyles.btnToggleOn.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleOn.normal = CoreGUIStyles.btnToggleOn.onNormal;
				CoreGUIStyles.btnToggleOn.hover = CoreGUIStyles.btnToggleOn.onHover;
				CoreGUIStyles.btnToggleOn.active = CoreGUIStyles.btnToggleOn.onActive;
				CoreGUIStyles.btnToggleOn.focused = CoreGUIStyles.btnToggleOn.onFocused;
				CoreGUIStyles.btnToggleOn.richText = true;
				CoreGUIStyles.btnToggleOn.margin.bottom = 0;
			}
			return CoreGUIStyles.btnToggleOn;
		}

		public static GUIStyle GetToggleButtonOff()
		{
			if (CoreGUIStyles.btnToggleOff == null) 
			{
				CoreGUIStyles.btnToggleOff = new GUIStyle(GUI.skin.GetStyle("LargeButton"));
				CoreGUIStyles.btnToggleOff.alignment = TextAnchor.MiddleLeft;
				CoreGUIStyles.btnToggleOff.richText = true;
				CoreGUIStyles.btnToggleOff.margin.bottom = 0;
				CoreGUIStyles.btnToggleOff.padding = new RectOffset(
					30, CoreGUIStyles.btnToggleOff.padding.right,
					CoreGUIStyles.btnToggleOff.padding.top,
					CoreGUIStyles.btnToggleOff.padding.bottom
				);
			}
			return CoreGUIStyles.btnToggleOff;
		}

        /*
		public static GUIStyle GetHighlight()
		{
			if (CoreGUIStyles.highlight == null)
			{
				CoreGUIStyles.highlight = new GUIStyle(GUI.skin.GetStyle("LightmapEditorSelectedHighlight"));
			}
			
			return CoreGUIStyles.highlight;
		}*/

		public static GUIStyle GetDropMarker()
		{
			if (CoreGUIStyles.dropMark == null)
			{
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(DROP_MARK_PATH);

                CoreGUIStyles.dropMark = new GUIStyle();
                CoreGUIStyles.dropMark.normal.background = texture;
                CoreGUIStyles.dropMark.border = new RectOffset(7, 7, 0, 0);
                CoreGUIStyles.dropMark.overflow = new RectOffset(4, -4, 6, -6);
                CoreGUIStyles.dropMark.active = CoreGUIStyles.dropMark.normal;
                CoreGUIStyles.dropMark.hover = CoreGUIStyles.dropMark.normal;
                CoreGUIStyles.dropMark.focused = CoreGUIStyles.dropMark.normal;
                CoreGUIStyles.dropMark.onNormal = CoreGUIStyles.dropMark.normal;
                CoreGUIStyles.dropMark.onActive = CoreGUIStyles.dropMark.normal;
                CoreGUIStyles.dropMark.onFocused = CoreGUIStyles.dropMark.normal;
            }

			return CoreGUIStyles.dropMark;
		}

		public static GUIStyle GetBoxExpanded()
		{
			if (CoreGUIStyles.boxExpanded == null)
			{
				CoreGUIStyles.boxExpanded = new GUIStyle(EditorStyles.helpBox);
				CoreGUIStyles.boxExpanded.padding = new RectOffset(1,1,3,3);
				CoreGUIStyles.boxExpanded.margin = new RectOffset(
					CoreGUIStyles.boxExpanded.margin.left,
					CoreGUIStyles.boxExpanded.margin.right,
					0,0
				);
			}

			return CoreGUIStyles.boxExpanded;
		}

		public static GUIStyle GetBox()
		{
			if (CoreGUIStyles.box == null)
			{
				CoreGUIStyles.box = new GUIStyle(EditorStyles.helpBox);
				CoreGUIStyles.box.margin = new RectOffset(0,0,0,0);
				CoreGUIStyles.box.padding = new RectOffset(5,5,5,5);
            }

			return CoreGUIStyles.box;
		}

		public static GUIStyle GetHelpBox()
		{
			if (CoreGUIStyles.helpBox == null)
			{
				CoreGUIStyles.helpBox = new GUIStyle(EditorStyles.helpBox);
				CoreGUIStyles.helpBox.margin = new RectOffset(0,0,0,0);
				CoreGUIStyles.helpBox.padding = new RectOffset(0,0,0,0);
			}

			return CoreGUIStyles.helpBox;
		}

		public static GUIStyle GetToolbarButton()
		{
			if (CoreGUIStyles.btnToolbar == null)
			{
				CoreGUIStyles.btnToolbar = new GUIStyle(EditorStyles.toolbarButton);
			}

			return btnToolbar;
		}

        public static GUIStyle GetSearchBox()
        {
            if (CoreGUIStyles.searchbox == null)
            {
                CoreGUIStyles.searchbox = new GUIStyle(GUI.skin.box);
                CoreGUIStyles.searchbox.margin = new RectOffset(0, 0, 0, 0);
                CoreGUIStyles.searchbox.padding = new RectOffset(5,5,5,5);
            }

            return CoreGUIStyles.searchbox;
        }

		// GRID BUTTONS: --------------------------------------------------------------------------

		public static GUIStyle GetGridButtonLeftOn()
		{
			if (CoreGUIStyles.btnGridLeftOn == null) 
			{
				CoreGUIStyles.btnGridLeftOn = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
				CoreGUIStyles.btnGridLeftOn.normal = CoreGUIStyles.btnGridLeftOn.onNormal;
				CoreGUIStyles.btnGridLeftOn.hover = CoreGUIStyles.btnGridLeftOn.onHover;
				CoreGUIStyles.btnGridLeftOn.active = CoreGUIStyles.btnGridLeftOn.onActive;
				CoreGUIStyles.btnGridLeftOn.focused = CoreGUIStyles.btnGridLeftOn.onFocused;
			}

			return CoreGUIStyles.btnGridLeftOn;
		}

		public static GUIStyle GetGridButtonLeftOff()
		{
			if (CoreGUIStyles.btnGridLeftOff == null) 
			{
				CoreGUIStyles.btnGridLeftOff = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
			}
			return CoreGUIStyles.btnGridLeftOff;
		}

		public static GUIStyle GetGridButtonMidOn()
		{
			if (CoreGUIStyles.btnGridMidOn == null) 
			{
				CoreGUIStyles.btnGridMidOn = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
				CoreGUIStyles.btnGridMidOn.normal = CoreGUIStyles.btnGridMidOn.onNormal;
				CoreGUIStyles.btnGridMidOn.hover = CoreGUIStyles.btnGridMidOn.onHover;
				CoreGUIStyles.btnGridMidOn.active = CoreGUIStyles.btnGridMidOn.onActive;
				CoreGUIStyles.btnGridMidOn.focused = CoreGUIStyles.btnGridMidOn.onFocused;
			}

			return CoreGUIStyles.btnGridMidOn;
		}

		public static GUIStyle GetGridButtonMidOff()
		{
			if (CoreGUIStyles.btnGridMidOff == null) 
			{
				CoreGUIStyles.btnGridMidOff = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
			}
			return CoreGUIStyles.btnGridMidOff;
		}

		public static GUIStyle GetGridButtonRightOn()
		{
			if (CoreGUIStyles.btnGridRightOn == null) 
			{
				CoreGUIStyles.btnGridRightOn = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
				CoreGUIStyles.btnGridRightOn.normal = CoreGUIStyles.btnGridRightOn.onNormal;
				CoreGUIStyles.btnGridRightOn.hover = CoreGUIStyles.btnGridRightOn.onHover;
				CoreGUIStyles.btnGridRightOn.active = CoreGUIStyles.btnGridRightOn.onActive;
				CoreGUIStyles.btnGridRightOn.focused = CoreGUIStyles.btnGridRightOn.onFocused;
			}

			return CoreGUIStyles.btnGridRightOn;
		}

		public static GUIStyle GetGridButtonRightOff()
		{
			if (CoreGUIStyles.btnGridRightOff == null) 
			{
				CoreGUIStyles.btnGridRightOff = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
			}

			return CoreGUIStyles.btnGridRightOff;
		}

		public static GUIStyle GetButtonLeftArrow()
		{
			if (CoreGUIStyles.btnLftArrow == null)
			{
				CoreGUIStyles.btnLftArrow = new GUIStyle(GUI.skin.GetStyle("AC LeftArrow"));
			}

			return CoreGUIStyles.btnLftArrow;
		}

		public static GUIStyle GetButtonRightArrow()
		{
			if (CoreGUIStyles.btnRigArrow == null)
			{
				CoreGUIStyles.btnRigArrow = new GUIStyle(GUI.skin.GetStyle("AC RightArrow"));
			}

			return CoreGUIStyles.btnRigArrow;
		}

		public static GUIStyle GetItemPreferencesSidebar()
		{
			if (CoreGUIStyles.itemPreferencesSidebar == null)
			{
				CoreGUIStyles.itemPreferencesSidebar = new GUIStyle(GUI.skin.GetStyle("MenuItem"));
				CoreGUIStyles.itemPreferencesSidebar.alignment = TextAnchor.MiddleRight;
				CoreGUIStyles.itemPreferencesSidebar.active.background = null;
				CoreGUIStyles.itemPreferencesSidebar.fixedHeight = 30f;
			}

			return CoreGUIStyles.itemPreferencesSidebar;
		}

        public static GUIStyle GetLabelTag()
        {
            if (CoreGUIStyles.labelTag == null)
            {
                CoreGUIStyles.labelTag = new GUIStyle();
                Texture2D bg = AssetDatabase.LoadAssetAtPath<Texture2D>(TAG_BG_PATH);

                CoreGUIStyles.labelTag.normal.background = bg;
                CoreGUIStyles.labelTag.padding = new RectOffset(0, 0, 0, 0);
                CoreGUIStyles.labelTag.margin = new RectOffset(0, 0, 0, 0);
                CoreGUIStyles.labelTag.fontSize = 10;
                CoreGUIStyles.labelTag.fontStyle = FontStyle.Normal;
                CoreGUIStyles.labelTag.alignment = TextAnchor.MiddleCenter;
                CoreGUIStyles.labelTag.normal.textColor = Color.white;
            }

            return CoreGUIStyles.labelTag;
        }

        public static GUIStyle GetTextarea()
        {
            if (CoreGUIStyles.textarea == null)
            {
                CoreGUIStyles.textarea = new GUIStyle(EditorStyles.textArea);
                CoreGUIStyles.textarea.wordWrap = true;
            }

            return CoreGUIStyles.labelTag;
        }

        public static GUIStyle GetDropZoneNormal()
        {
            if (CoreGUIStyles.dropZoneNormal == null)
            {
                CoreGUIStyles.dropZoneNormal = new GUIStyle(EditorStyles.helpBox);
                Texture2D bgNormal = AssetDatabase.LoadAssetAtPath<Texture2D>(DROPZONE_NORMAL_PATH);

                CoreGUIStyles.dropZoneNormal.normal.background = bgNormal;
                CoreGUIStyles.dropZoneNormal.focused.background = bgNormal;
                CoreGUIStyles.dropZoneNormal.active.background = bgNormal;
                CoreGUIStyles.dropZoneNormal.hover.background = bgNormal;

                CoreGUIStyles.dropZoneNormal.border = new RectOffset(4,4,4,4);
                CoreGUIStyles.dropZoneNormal.padding = new RectOffset(0, 0, 0, 0);
                CoreGUIStyles.dropZoneNormal.margin  = new RectOffset(0, 0, 0, 0);
                CoreGUIStyles.dropZoneNormal.alignment = TextAnchor.MiddleCenter;
            }

            return CoreGUIStyles.dropZoneNormal;
        }

        public static GUIStyle GetDropZoneActive()
        {
            if (CoreGUIStyles.dropZoneActive == null)
            {
                CoreGUIStyles.dropZoneActive = new GUIStyle(CoreGUIStyles.GetDropZoneNormal());
                Texture2D bgActive = AssetDatabase.LoadAssetAtPath<Texture2D>(DROPZONE_ACTIVE_PATH);

                CoreGUIStyles.dropZoneActive.normal.background = bgActive;
                CoreGUIStyles.dropZoneActive.focused.background = bgActive;
                CoreGUIStyles.dropZoneActive.active.background = bgActive;
                CoreGUIStyles.dropZoneActive.hover.background = bgActive;

                CoreGUIStyles.dropZoneActive.normal.textColor = Color.black;
                CoreGUIStyles.dropZoneActive.focused.textColor = Color.black;
                CoreGUIStyles.dropZoneActive.active.textColor = Color.black;
                CoreGUIStyles.dropZoneActive.hover.textColor = Color.black;
            }

            return CoreGUIStyles.dropZoneActive;
        }

        public static GUIStyle GlobalIDText()
        {
            if (CoreGUIStyles.globalIDText == null)
            {
                CoreGUIStyles.globalIDText = new GUIStyle(EditorStyles.textField);
                CoreGUIStyles.globalIDText.margin = CoreGUIStyles.GetButtonMid().margin;
                CoreGUIStyles.globalIDText.padding = CoreGUIStyles.GetButtonMid().padding;
                CoreGUIStyles.globalIDText.contentOffset = CoreGUIStyles.GetButtonMid().contentOffset;
                CoreGUIStyles.globalIDText.alignment = TextAnchor.MiddleCenter;
            }

            return CoreGUIStyles.globalIDText;
        }
    }
}
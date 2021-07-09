namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using GameCreator.Core;

    public abstract class GenericVariableSelectWindow : PopupWindowContent
    {
        private const float SEARCH_HEIGHT = 28f;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private SearchField searchField;
        private string searchText = "";
        private bool searchFocus = true;

        private float windowWidth = 0f;
        private Vector2 scroll = Vector2.zero;
        private bool keyPressedEnter = false;
        private bool keyPressedUp = false;
        private bool keyPressedDown = false;

        private int varIndex = 0;
        private Rect varRect = Rect.zero;

        private Action<string> callback;
        private GUIContent[] variables;
        private int allowTypesMask = 0;

        private GUIStyle styleItem;
        private GUIStyle styleBackground;

        // INITIALIZERS: --------------------------------------------------------------------------

        public GenericVariableSelectWindow(Rect ctaRect, Action<string> callback, int allowTypesMask)
        {
            this.windowWidth = ctaRect.width;
            this.callback = callback;
            this.allowTypesMask = allowTypesMask;
        }

        public override void OnOpen()
        {
            this.variables = this.GetVariables(this.allowTypesMask);

            this.searchField = new SearchField();
            this.searchFocus = true;
            this.InitializeStyles();
        }

        private void InitializeStyles()
        {
            this.styleItem = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
            this.styleItem.fixedHeight = 20f;
            this.styleItem.padding = new RectOffset(
                5,
                5,
                this.styleItem.padding.top,
                this.styleItem.padding.bottom
            );

            this.styleItem.margin = new RectOffset(0, 0, 0, 0);
            this.styleItem.imagePosition = ImagePosition.ImageLeft;

            this.styleBackground = new GUIStyle();
            this.styleBackground.margin = new RectOffset(0, 0, 0, 0);
            this.styleBackground.padding = new RectOffset(0, 0, 0, 0);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(Mathf.Max(150, this.windowWidth), 300);
        }

        // VIRTUAL & ABSTRACT METHODS: ------------------------------------------------------------

        protected abstract GUIContent[] GetVariables(int allowTypesMask);
        protected abstract void PaintFooter();

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnGUI(Rect rect)
        {
            this.HandleKeyboardInput();
            this.PaintSearch(rect);

            this.scroll = GUILayout.BeginScrollView(
                this.scroll,
                GUIStyle.none,
                GUI.skin.verticalScrollbar
            );

            for (int i = 0; i < this.variables.Length; ++i)
            {
                if (this.variables[i].text.Contains(this.searchText))
                {
                    this.PaintVariable(i);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            float scrollHeight = GUILayoutUtility.GetLastRect().height;

            if (this.keyPressedDown && this.varIndex < this.variables.Length - 1)
            {
                this.varIndex++;
                UnityEngine.Event.current.Use();
            }
            else if (this.keyPressedUp && this.varIndex > 0)
            {
                this.varIndex--;
                UnityEngine.Event.current.Use();
            }

            if (UnityEngine.Event.current.type == EventType.Repaint && 
                (this.keyPressedUp || this.keyPressedDown))
            {
                if (this.varRect != Rect.zero)
                {
                    if (this.scroll.y > this.varRect.y)
                    {
                        this.scroll = Vector2.up * (this.varRect.position.y);
                        this.editorWindow.Repaint();
                    }
                    else if (this.scroll.y + scrollHeight < this.varRect.position.y + this.varRect.size.y)
                    {
                        float positionY = this.varRect.y + this.varRect.height - scrollHeight;
                        this.scroll = Vector2.up * positionY;
                        this.editorWindow.Repaint();
                    }
                }
            }

            this.PaintFooter();

            if (UnityEngine.Event.current.type == EventType.MouseMove ||
                UnityEngine.Event.current.type == EventType.MouseDown)
            {
                this.editorWindow.Repaint();
            }
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void PaintSearch(Rect rect)
        {
            Rect rectWrap = GUILayoutUtility.GetRect(rect.width, SEARCH_HEIGHT);
            Rect rectSearch = new Rect(
                rectWrap.x + 5f,
                rectWrap.y + 5f,
                rectWrap.width - 10f,
                rectWrap.height - 10f
            );

            GUI.BeginGroup(rectWrap, CoreGUIStyles.GetSearchBox());
            this.searchText = this.searchField.OnGUI(rectSearch, this.searchText);

            if (this.searchFocus)
            {
                this.searchField.SetFocus();
                this.searchFocus = false;
            }

            GUI.EndGroup();
        }

        private void PaintVariable(int index)
        {
            bool mouseEnter = this.varIndex == index && UnityEngine.Event.current.type == EventType.MouseDown;
            Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);

            bool buttonHasFocus = this.varIndex == index;
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                if (this.varIndex == index) this.varRect = buttonRect;

                this.styleItem.Draw(
                    buttonRect,
                    this.variables[index],
                    buttonHasFocus,
                    buttonHasFocus,
                    false,
                    false
                );
            }

            if (buttonHasFocus && (mouseEnter || this.keyPressedEnter))
            {
                if (this.keyPressedEnter) UnityEngine.Event.current.Use();
                this.Callback(this.variables[index].text);
                this.editorWindow.Close();
            }

            if (UnityEngine.Event.current.type == EventType.MouseMove &&
                GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition))
            {
                this.varIndex = index;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void HandleKeyboardInput()
        {
            this.keyPressedUp = false;
            this.keyPressedDown = false;
            this.keyPressedEnter = false;

            if (UnityEngine.Event.current.type != EventType.KeyDown) return;

            this.keyPressedUp = (UnityEngine.Event.current.keyCode == KeyCode.UpArrow);
            this.keyPressedDown = (UnityEngine.Event.current.keyCode == KeyCode.DownArrow);

            this.keyPressedEnter = (
                UnityEngine.Event.current.keyCode == KeyCode.KeypadEnter ||
                UnityEngine.Event.current.keyCode == KeyCode.Return
            );
        }

        private void Callback(string name)
        {
            name = VariableEditor.ProcessName(name);
            if (this.callback != null) this.callback(name);
        }
    }
}
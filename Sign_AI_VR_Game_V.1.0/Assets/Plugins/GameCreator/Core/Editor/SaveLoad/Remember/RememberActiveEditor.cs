namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomEditor(typeof(RememberActive))]
    public class RememberActiveEditor : RememberEditor
    {
        protected override string Comment()
        {
            return "Automatically restores the state (active, inactive or destroyed) when loading a game";
        }

        protected override void OnPaint()
        { }
    }
}
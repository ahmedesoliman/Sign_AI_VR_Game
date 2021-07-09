namespace GameCreator.Localization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

    [CustomPropertyDrawer(typeof(LocStringNoPostProcessAttribute))]
    public class LocStringNoPostProcessPropertyDrawer : LocStringPropertyDrawer
	{
        protected override bool PaintPostProcess()
        {
            return false;
        }
	}
}

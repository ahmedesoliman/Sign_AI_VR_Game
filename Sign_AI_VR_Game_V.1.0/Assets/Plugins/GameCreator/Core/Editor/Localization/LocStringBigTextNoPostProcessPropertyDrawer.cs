namespace GameCreator.Localization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

    [CustomPropertyDrawer (typeof (LocStringBigTextNoPostProcessAttribute))]
    public class LocStringBigTextNoPostProcessPropertyDrawer : LocStringBigTextPropertyDrawer
	{
        protected override bool PaintPostProcess()
        {
            return false;
        }
	}
}

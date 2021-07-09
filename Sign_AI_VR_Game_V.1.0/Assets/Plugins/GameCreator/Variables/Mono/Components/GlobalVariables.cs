namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [Serializable]
    public class GlobalVariables : ScriptableObject
    {
        public SOVariable[] references = new SOVariable[0];
	}
}
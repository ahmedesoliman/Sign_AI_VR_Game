namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    public static class GlobalVariablesUtilities
    {
        public static Variable Get(string name)
        {
            return GlobalVariablesManager.Instance.Get(name);
        }
    }
}
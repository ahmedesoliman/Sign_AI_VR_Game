namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Variables;

    [AddComponentMenu("")]
    public class ConditionHasSaveGame : ICondition
    {
        public NumberProperty profile = new NumberProperty(0);

        public override bool Check(GameObject target)
        {
            int number = this.profile.GetInt(target);
            return SaveLoadManager.Instance.GetProfileInfo(number) != null;
        }

#if UNITY_EDITOR
        public static new string NAME = "Custom/ConditionHasSaveGame";
#endif
    }
}

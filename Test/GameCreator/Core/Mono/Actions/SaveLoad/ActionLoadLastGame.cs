namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class ActionLoadLastGame : IAction
	{
        private bool complete = false;

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            SaveLoadManager.Instance.LoadLast(this.OnLoad);
            this.complete = false;

            WaitUntil waitUntil = new WaitUntil(() => this.complete);
            yield return waitUntil;

            yield return 0;
        }

        private void OnLoad()
        {
            this.complete = true;
        }

        #if UNITY_EDITOR
        public static new string NAME = "Save & Load/Load Last Game";
        private const string NODE_TITLE = "Load Last Game";
        private const string MSG = "Loads the last saved game";

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(MSG, MessageType.Info);
        }
        #endif
    }
}

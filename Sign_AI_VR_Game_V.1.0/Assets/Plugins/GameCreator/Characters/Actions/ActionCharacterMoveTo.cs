namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCharacterMoveTo : IAction 
	{
		public enum MOVE_TO
		{
			Position,
			Transform,
			Marker,
            Variable
		}

        public TargetCharacter target = new TargetCharacter();

		public MOVE_TO moveTo = MOVE_TO.Position;
		public bool waitUntilArrives = true;

		public Vector3 position;
		public new Transform transform;
		public NavigationMarker marker;

        [VariableFilter(Variable.DataType.GameObject, Variable.DataType.Vector3)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        public bool cancelable = false;
        public float cancelDelay = 1.0f;

        [Range(0.0f, 5.0f)]
        [Tooltip("Threshold distance from the target that is considered as reached")]
        public float stopThreshold = 0.0f;

        private Character character = null;
        private bool forceStop = false;

        private bool wasControllable = false;
		private bool isCharacterMoving = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.waitUntilArrives) return false;
            this.character = this.target.GetCharacter(target);

            Vector3 cPosition = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            float cStopThresh = 0f;

            this.GetTarget(this.character, target, ref cPosition, ref cRotation, ref cStopThresh);
            cStopThresh = Mathf.Max(cStopThresh, this.stopThreshold);

            this.character.characterLocomotion.SetTarget(cPosition, cRotation, cStopThresh);
            return true;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            this.forceStop = false;
            this.character = this.target.GetCharacter(target);

            Vector3 cPosition  = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            float cStopThresh = this.stopThreshold;

            this.GetTarget(character, target, ref cPosition, ref cRotation, ref cStopThresh);
            cStopThresh = Mathf.Max(cStopThresh, this.stopThreshold);

            this.isCharacterMoving = true;
            this.wasControllable = this.character.characterLocomotion.isControllable;
            this.character.characterLocomotion.SetIsControllable(false);

            this.character.characterLocomotion.SetTarget(cPosition, cRotation, cStopThresh, this.CharacterArrivedCallback);

            bool canceled = false;
            float initTime = Time.time;

            while (this.isCharacterMoving && !canceled && !forceStop)
            {
	            if (this.cancelable && (Time.time - initTime) >= this.cancelDelay)
                {
                    canceled = Input.anyKey;
                }

                yield return null;
            }

            this.character.characterLocomotion.SetIsControllable(this.wasControllable);

            if (canceled) yield return 999999;
			else yield return 0;
		}

        public override void Stop()
        {
	        this.forceStop = true;
            if (this.character == null) return;

            this.character.characterLocomotion.SetIsControllable(this.wasControllable);
            this.character.characterLocomotion.Stop();
        }

        public void CharacterArrivedCallback()
		{
			this.isCharacterMoving = false;
		}

        private void GetTarget(Character targetCharacter, GameObject invoker, 
            ref Vector3 cPosition, ref ILocomotionSystem.TargetRotation cRotation, ref float cStopThresh)
        {
            cStopThresh = 0.0f;
            switch (this.moveTo)
            {
                case MOVE_TO.Position: cPosition = this.position; break;
                case MOVE_TO.Transform: cPosition = this.transform.position; break;
                case MOVE_TO.Marker:
                    cPosition = this.marker.transform.position;
                    cRotation = new ILocomotionSystem.TargetRotation(true, this.marker.transform.forward);
                    cStopThresh = this.marker.stopThreshold;
                    break;

                case MOVE_TO.Variable:
                    object valueVariable = this.variable.Get(invoker);
                    switch (this.variable.GetVariableDataType(invoker))
                    {
                        case Variable.DataType.GameObject:
                            GameObject variableGo = valueVariable as GameObject;
                            if (variableGo == null)
                            {
                                if (targetCharacter != null) cPosition = targetCharacter.transform.position;
                                return;
                            }

                            NavigationMarker varMarker = variableGo.GetComponent<NavigationMarker>();
                            if (varMarker != null)
                            {
                                cPosition = varMarker.transform.position;
                                cRotation = new ILocomotionSystem.TargetRotation(true, varMarker.transform.forward);
                                cStopThresh = varMarker.stopThreshold;
                            }
                            else cPosition = variableGo.transform.position;
                            break;

                        case Variable.DataType.Vector3:
                            cPosition = (Vector3)valueVariable;
                            break;
                    }
                    break;
            }
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Move Character";
		private const string NODE_TITLE = "Move {0} to {1} {2}";

        private static readonly GUIContent GC_CANCEL = new GUIContent("Cancelable");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spMoveTo;
		private SerializedProperty spWaitUntilArrives;
		private SerializedProperty spPosition;
		private SerializedProperty spTransform;
		private SerializedProperty spMarker;
        private SerializedProperty spVariable;

        private SerializedProperty spStopThreshold;
        private SerializedProperty spCancelable;
        private SerializedProperty spCancelDelay;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			string value = "none";
			switch (this.moveTo)
			{
			case MOVE_TO.Position : 
				value = string.Format("({0},{1},{2})", this.position.x, this.position.y, this.position.z); 
				break;

			case MOVE_TO.Transform :
				value = (this.transform == null ? "nothing" : this.transform.gameObject.name);
				break;
			case MOVE_TO.Marker :
				value = (this.marker == null ? "nothing" : this.marker.gameObject.name);
				break;

            case MOVE_TO.Variable:
                value = this.variable.GetVariableID();
                break;
			}

			return string.Format(
				NODE_TITLE, 
                this.target,
				this.moveTo, 
				value
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spMoveTo = this.serializedObject.FindProperty("moveTo");
			this.spWaitUntilArrives = this.serializedObject.FindProperty("waitUntilArrives");
			this.spPosition = this.serializedObject.FindProperty("position");
			this.spTransform = this.serializedObject.FindProperty("transform");
			this.spMarker = this.serializedObject.FindProperty("marker");
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spStopThreshold = this.serializedObject.FindProperty("stopThreshold");
            this.spCancelable = this.serializedObject.FindProperty("cancelable");
            this.spCancelDelay = this.serializedObject.FindProperty("cancelDelay");
		}

		protected override void OnDisableEditorChild ()
		{
			return;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spMoveTo);

			switch ((MOVE_TO)this.spMoveTo.intValue)
			{
			case MOVE_TO.Position  : EditorGUILayout.PropertyField(this.spPosition);  break;
			case MOVE_TO.Transform : EditorGUILayout.PropertyField(this.spTransform); break;
			case MOVE_TO.Marker    : EditorGUILayout.PropertyField(this.spMarker);    break;
            case MOVE_TO.Variable  : EditorGUILayout.PropertyField(this.spVariable); break;
            }

			EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spStopThreshold);
            EditorGUILayout.PropertyField(this.spWaitUntilArrives);
            if (this.spWaitUntilArrives.boolValue)
            {
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
                Rect rectLabel = new Rect(
                    rect.x,
                    rect.y,
                    EditorGUIUtility.labelWidth,
                    rect.height
                );
                Rect rectCancenable = new Rect(
                    rectLabel.x + rectLabel.width,
                    rectLabel.y,
                    20f,
                    rectLabel.height
                );
                Rect rectDelay = new Rect(
                    rectCancenable.x + rectCancenable.width,
                    rectCancenable.y,
                    rect.width - (rectLabel.width + rectCancenable.width),
                    rectCancenable.height
                );

                EditorGUI.LabelField(rectLabel, GC_CANCEL);
                EditorGUI.PropertyField(rectCancenable, this.spCancelable, GUIContent.none);

                EditorGUI.BeginDisabledGroup(!this.spCancelable.boolValue);
                EditorGUI.PropertyField(rectDelay, this.spCancelDelay, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
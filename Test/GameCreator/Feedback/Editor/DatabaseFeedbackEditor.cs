namespace GameCreator.Feedback
{
	using System;
	using System.Net;
	using System.Net.Mail;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Text.RegularExpressions;
	using GameCreator.Core;

	[CustomEditor(typeof(DatabaseFeedback))]
	public class DatabaseFeedbackEditor :  IDatabaseEditor
	{
		private const string MSG_TITLE = "A Letter to the Developers.";
		private const string MSG_SUBTL = "We would really like to know what you think of Game Creator.";
		private const string DIALOG_ERROR_SEND_TITLE = "Please, make sure to fill at least your name and provide some content";
		private const string DIALOG_ERROR_SEND_SUBTL = "The email field is optional. We don't collect any personal information that you don't type.";
		private const string DIALOG_ERROR_SERVER_TITLE = "Error while connecting to our server";
		private const string DIALOG_ERROR_SERVER_SUBTL = "Retry in a couple of minutes. If the problem persists, send us a notice at hello@catsoft-studios.com";

		private const string FEEDBACK_SUBJECT = "Game Creator Feedback from {0}";
		private const string FEEDBACK_BODY = "Name:{0}\nMail:{1}\nContent:{2}";
		private const string FEEDBACK_MAIL = "incoming+gamecreator/issues@gitlab.com";

		private const string MSG_COMPLETE_TITLE = "Thank you for the feedback!";
		private const string MSG_COMPLETE_SUBTL = "It is really important for us to know what you think of Game Creator. Don't hesitate to send us as many messages as you want :-D";

		private const string KEY_EDITORPREF_NAME = "gamecreator-feedback-name";
		private const string KEY_EDITORPREF_MAIL = "gamecreator-feedback-mail";
		private const string KEY_EDITORPREF_CONT = "gamecreator-feedback-cont";
		private const string KEY_EDITORPREF_FEED = "gamecreator-feedback-feed";

		private const float CONT_HEIGHT = 300f;
		private static readonly GUIContent FIELD_NAME = new GUIContent("Name");
		private static readonly GUIContent FIELD_MAIL = new GUIContent("Email (optional)");
		private static readonly GUIContent FIELD_TYPE = new GUIContent("Feedback Category");

		private enum State
		{
			Writing,
			Sending,
			Sent
		}

		private State state = State.Writing;
		private DatabaseFeedback.FeedbackType userFeedbackType = DatabaseFeedback.FeedbackType.General;
		private string userName = "";
		private string userMail = "";
		private string userCont = "";

		private bool blurFocus = false;
		private bool stylesInitialize = false;
		private GUIStyle styleBox;
		private GUIStyle styleWrapLabel;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;

			this.state = State.Writing;
			this.stylesInitialize = false;

			this.userName = EditorPrefs.GetString(KEY_EDITORPREF_NAME);
			this.userMail = EditorPrefs.GetString(KEY_EDITORPREF_MAIL);
			this.userCont = EditorPrefs.GetString(KEY_EDITORPREF_CONT);
			this.userFeedbackType = (DatabaseFeedback.FeedbackType)EditorPrefs.GetInt(KEY_EDITORPREF_FEED, 0);
		}

		// OVERRIDE METHODS: -------------------------------------------------------------------------------------------

		public override string GetDocumentationURL ()
		{
			return "http://docs.gamecreator.io";
		}

		public override string GetName ()
		{
			return "Feedback";
		}

		public override int GetPanelWeight ()
		{
			return 0;
		}

		// GUI METHODS: ------------------------------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
			this.InitializeStyles();
			if (this.blurFocus)
			{
				GUI.SetNextControlName("__dummy_element");
				GUI.TextField(new Rect(-100, -100, 1, 1), "");
				GUI.FocusControl("__dummy_element");
				this.blurFocus = false;
			}

			GUILayout.Space(40f);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginVertical(this.styleBox, GUILayout.Width(400f), GUILayout.MaxWidth(400f));

			switch (this.state)
			{
			case State.Writing : this.PaintWriting(); break;
			case State.Sending : this.PaintWriting(); break;
			case State.Sent : this.PaintSent(); break;
			}

			EditorGUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void PaintWriting()
		{
			EditorGUILayout.BeginVertical(this.styleBox);
			EditorGUILayout.LabelField(MSG_TITLE, EditorStyles.boldLabel);
			EditorGUILayout.LabelField(MSG_SUBTL);
			EditorGUILayout.EndVertical();

			EditorGUI.BeginDisabledGroup(this.state == State.Sending);
			EditorGUILayout.Space();
			this.userName = EditorGUILayout.TextField(FIELD_NAME, this.userName);
			this.userMail = EditorGUILayout.TextField(FIELD_MAIL, this.userMail);

			EditorGUILayout.Space();

			this.userFeedbackType = (DatabaseFeedback.FeedbackType)EditorGUILayout.EnumPopup(
				FIELD_TYPE, this.userFeedbackType
			);

			this.userCont = EditorGUILayout.TextArea(this.userCont, GUILayout.Height(CONT_HEIGHT));

			EditorPrefs.SetString(KEY_EDITORPREF_NAME, this.userName);
			EditorPrefs.SetString(KEY_EDITORPREF_MAIL, this.userMail);
			EditorPrefs.SetString(KEY_EDITORPREF_CONT, this.userCont);
			EditorPrefs.SetInt(KEY_EDITORPREF_FEED, (int)this.userFeedbackType);

			if (GUILayout.Button(this.state == State.Writing ? "Send" : "Sending..."))
			{
				if (string.IsNullOrEmpty(this.userName) || string.IsNullOrEmpty(this.userCont))
				{
					EditorUtility.DisplayDialog(DIALOG_ERROR_SEND_TITLE, DIALOG_ERROR_SEND_SUBTL, "Ok");
				}
				else
				{
					this.state = State.Sending;
					this.SendMessage();
				}
			}



			EditorGUI.EndDisabledGroup();
		}

		private void PaintSent()
		{
			EditorGUILayout.LabelField(MSG_COMPLETE_TITLE, EditorStyles.boldLabel);
			EditorGUILayout.LabelField(MSG_COMPLETE_SUBTL, this.styleWrapLabel);

			if (GUILayout.Button("Ok"))
			{
				this.state = State.Writing;
			}
		}

		private void SendMessage()
		{
			FeedbackHttpRequest.Request(
				new FeedbackHttpRequest.Data(
					this.userName, 
					this.userMail, 
					Regex.Replace(this.userFeedbackType.ToString(), "([a-z])_?([A-Z])", "$1 $2"), 
					this.userCont
				),
				this.CallbackMessage
			);
		}

		private void CallbackMessage(bool isError, string content)
		{
			this.blurFocus = true;

			if (isError)
			{
				Debug.LogError(content);
				EditorUtility.DisplayDialog(DIALOG_ERROR_SERVER_TITLE, DIALOG_ERROR_SERVER_SUBTL, "Ok");

				this.state = State.Writing;
				return;
			}

			EditorPrefs.SetString(KEY_EDITORPREF_NAME, "");
			EditorPrefs.SetString(KEY_EDITORPREF_MAIL, "");
			EditorPrefs.SetString(KEY_EDITORPREF_CONT, "");
			EditorPrefs.SetInt(KEY_EDITORPREF_FEED, 0);

			this.userFeedbackType = DatabaseFeedback.FeedbackType.General;
			this.userName = "";
			this.userMail = "";
			this.userCont = "";

			this.state = State.Sent;
		}

		private void InitializeStyles()
		{
			if (this.stylesInitialize) return;
			this.stylesInitialize = true;

			this.styleBox = new GUIStyle(EditorStyles.helpBox);
			this.styleBox.padding = new RectOffset(10,10,10,10);

			this.styleWrapLabel = new GUIStyle(EditorStyles.label);
			this.styleWrapLabel.wordWrap = true;
		}
	}
}
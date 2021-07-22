namespace GameCreator.Feedback
{
	using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Networking;
	using UnityEditor;

	public abstract class FeedbackHttpRequest
	{
		private class RequestData
		{
			public UnityWebRequest request;
			public UnityAction<bool, string> callback;

			public RequestData(UnityWebRequest request, UnityAction<bool, string> callback)
			{
				this.request = request;
				this.callback = callback;
			}
		}

		[System.Serializable]
		public class Data
		{
            public string n = ""; // name
            public string e = ""; // email
            public string f = ""; // feedback
            public string c = ""; // content

			public Data(string name, string email, string feedback, string content)
			{
                this.n = name;
                this.e = email;
                this.f = feedback;
                this.c = content;
			}
		}

		private const string ERR_NO_INTERNET = "No internet connection";
        private const string URL = "https://us-central1-unity-game-creator.cloudfunctions.net/feedback/";
        private const string FMT = "{{\"data\": {0}}}";

		private static RequestData REQUEST_DATA;

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public static void Request(Data data, UnityAction<bool, string> callback)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable) 
			{
				callback.Invoke(false, ERR_NO_INTERNET);
				return;
			}

            string post = string.Format(FMT, EditorJsonUtility.ToJson(data));

            UnityWebRequest request = UnityWebRequest.Post(URL, string.Empty);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(post));
            request.uploadHandler.contentType = "application/json";

			REQUEST_DATA = new RequestData(request, callback);
			EditorApplication.update += FeedbackHttpRequest.EditorUpdate;
            REQUEST_DATA.request.SendWebRequest();
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private static void EditorUpdate()
		{
			if (!REQUEST_DATA.request.isDone) return;

			if (REQUEST_DATA.request.responseCode == 200) 
			{
				REQUEST_DATA.callback.Invoke(false, REQUEST_DATA.request.downloadHandler.text);
			}
			else 
			{
				Debug.LogWarning(REQUEST_DATA.request.error);
				REQUEST_DATA.callback.Invoke(true, REQUEST_DATA.request.error);
			}
			
			EditorApplication.update -= FeedbackHttpRequest.EditorUpdate;
		}
	}
}
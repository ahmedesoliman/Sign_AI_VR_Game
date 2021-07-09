namespace GameCreator.Update
{
    using System;
    using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Networking;
	using UnityEditor;

	public abstract class UpdateHttpRequest
    {
		private class RequestData
		{
			public UnityWebRequest request;
			public UnityAction<bool, OutputData> callback;

			public RequestData(UnityWebRequest request, UnityAction<bool, OutputData> callback)
			{
				this.request = request;
				this.callback = callback;
			}
		}

        [Serializable]
        public class InputData
        {
            public string data = "";

            public InputData(string package)
            {
                this.data = package;
            }
        }

        [Serializable]
        public class OutputData
        {
            [Serializable]
            public class Result
            {
                public string result;
            }

            [Serializable]
            public class Data
            {
                public Version version;
                public string type;
                public string date;
                public string changelog;
            }

            public bool error;
            public Data data;
        }

        // STATIC & CONST PROPERTIES: -------------------------------------------------------------

        private const string URL = "https://us-central1-unity-game-creator.cloudfunctions.net/version";
        private const string FMT = "{{\"data\": {0}}}";

		private static RequestData REQUEST_DATA;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static void Request(string data, UnityAction<bool, OutputData> callback)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable) return;
            GameCreatorUpdate.CHECKING_UPDATES = true;

            string post = EditorJsonUtility.ToJson(new InputData(data));
            UnityWebRequest request = UnityWebRequest.Post(URL, string.Empty);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(post));
            request.uploadHandler.contentType = "application/json";

			REQUEST_DATA = new RequestData(request, callback);
			EditorApplication.update += UpdateHttpRequest.EditorUpdate;
            REQUEST_DATA.request.SendWebRequest();
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private static void EditorUpdate()
		{
            if (!REQUEST_DATA.request.isDone) return;
            EditorApplication.update -= UpdateHttpRequest.EditorUpdate;

            if (REQUEST_DATA.request.responseCode == 200) 
			{
                OutputData.Result result = JsonUtility.FromJson<OutputData.Result>(
                    REQUEST_DATA.request.downloadHandler.text
                );
                    
                OutputData outputData = JsonUtility.FromJson<OutputData>(result.result);
                REQUEST_DATA.callback.Invoke(outputData.error, outputData);
			}
			else 
			{
				Debug.LogWarning(REQUEST_DATA.request.error);
				REQUEST_DATA.callback.Invoke(true, null);
			}
		}
	}
}
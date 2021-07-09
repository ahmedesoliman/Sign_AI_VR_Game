namespace GameCreator.Core
{
	using UnityEngine;
	using System.Collections;

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private const string MSG_INSANCE_ALREADY = "[Singleton] Instance {0} already destroyed on application exit";
		private const string MSG_MULTI_INSTANCE = "[Singleton] Multiple instances of a singleton gameObject '{0}'";

		private static T instance;
		private static bool SHOW_DEBUG = false;

        public static bool IS_EXITING = false;

		// PROPERTIES: ----------------------------------------------------------------------------

		protected bool isExiting => IS_EXITING;

		// CONSTRUCTOR: ---------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected static void OnRuntimeStartSingleton()
        {
			IS_EXITING = false;
		}

		public static T Instance
		{
			get
			{
				if (instance == null && !IS_EXITING)
				{
					instance = (T) FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						DebugLogFormat(MSG_MULTI_INSTANCE, instance.gameObject.name);

						return instance;
					}

					if (instance == null)
					{
						GameObject singleton = new GameObject();
						instance = singleton.AddComponent<T>();
						singleton.name = string.Format("{0}(singleton)", typeof(T).ToString());

                        Singleton<T> component = instance.GetComponent<Singleton<T>>();
                        component.OnCreate();

                        if (component.ShouldNotDestroyOnLoad()) DontDestroyOnLoad(singleton);
						DebugLogFormat("[Singleton] Creating an instance of {0} with DontDestroyOnLoad", typeof(T));
					} 
					else 
					{
						DebugLogFormat("[Singleton] Using instance already created '{0}'", instance.gameObject.name);
					}
				}

				return instance;
			}
		}

		// VIRTUAL METHODS: -----------------------------------------------------------------------

		protected virtual void OnCreate() 
        { 
            return; 
        }

        protected void WakeUp()
        {
            return;
        }

        protected virtual bool ShouldNotDestroyOnLoad()
        {
            return true;
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnApplicationQuit()
        {
			IS_EXITING = true;
        }

		private void OnDestroy () 
		{
            instance = null;
		}

		private static void DebugLogFormat(string content, params object[] parameters)
		{
			if (!SHOW_DEBUG) return;
			Debug.LogFormat(content, parameters);
		}
	}
}
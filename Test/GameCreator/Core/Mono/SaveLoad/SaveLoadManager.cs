namespace GameCreator.Core
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

	[AddComponentMenu("Game Creator/Managers/SaveLoadManager", 100)]
	public class SaveLoadManager : Singleton<SaveLoadManager>
	{
        public enum Priority
        {
            High = 0,
            Normal = 50,
            Low = 100
        }

        [Serializable]
        public class Storage
        {
            public IGameSave target;
            public int priority;

            public Storage(IGameSave target, int priority = (int)Priority.Normal)
            {
                this.target = target;
                this.priority = priority;
            }
        }

        public class ProfileEvent : UnityEvent<int, int> { }

        // CONST & STATIC PROPERTIES: -------------------------------------------------------------

        private const string STORE_KEYFMT = "gamedata:{0:D2}:{1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool IsLoading { get; private set; }
        public static bool IsProfileLoaded { get; private set; }

        public static int ActiveProfile { get; private set; }
        public static int LoadedProfile { get; private set; }

        public SavesData savesData {get; private set;}

        private ScenesData scenesData;
        private KeysData keysData;

        private List<Storage> storage;
        private Dictionary<string, object> data;

        public UnityAction<int> onSave;
        public UnityAction<int> onLoad;

        public ProfileEvent eventOnChangeProfile = new ProfileEvent();

        // INITIALIZE: ----------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            SaveLoadManager.Instance.WakeUp();
        }

		protected override void OnCreate()
        {
            this.savesData = new SavesData(this);
            this.scenesData = new ScenesData(SceneManager.GetActiveScene().name);
            this.keysData = new KeysData();

            SceneManager.sceneLoaded += this.OnLoadScene;
            SceneManager.sceneUnloaded += this.OnUnloadScene;
        }

        private void OnLoadScene(Scene scene, LoadSceneMode mode)
        {
            this.scenesData.Add(scene.name, mode);
        }

        private void OnUnloadScene(Scene scene)
        {
            this.scenesData.Remove(scene.name);
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void SetCurrentProfile(int profile)
		{
            int prevProfile = SaveLoadManager.ActiveProfile;
			SaveLoadManager.ActiveProfile = profile;

            if (prevProfile != profile && this.eventOnChangeProfile != null)
            {
                this.eventOnChangeProfile.Invoke(prevProfile, profile);
            }
		}

		public int GetCurrentProfile()
		{
			return SaveLoadManager.ActiveProfile;
		}

        public void DeleteProfile(int profile)
        {
            this.keysData.Delete(profile);
            this.savesData.profiles.Remove(profile);

            if (ActiveProfile == profile)
            {
                this.data = new Dictionary<string, object>();
                this.storage = new List<Storage>();
            }
        }

        public void Initialize(IGameSave gameSave, int priority = (int)Priority.Normal,
            bool limitOnLoad = false)
		{
            if (this.storage == null) this.storage = new List<Storage>();
            if (this.data == null) this.data = new Dictionary<string, object>();

			string key = gameSave.GetUniqueName();
            int index = -1;

            for (int i = this.storage.Count - 1; i >= 0; --i)
            {
                if (this.storage[i] == null || this.storage[i].target == null)
                {
                    this.storage.RemoveAt(i);
                    continue;
                }

                if (this.storage[i].target.GetUniqueName() == key)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                this.storage[index].target = gameSave;
                this.storage[index].priority = priority;
            }
            else
            {
                this.storage.Add(new Storage(gameSave, priority));
            }

            if (!limitOnLoad)
            {
                if (this.data.ContainsKey(key))
                {
                    gameSave.OnLoad(this.data[key]);
                }
                else if (SaveLoadManager.IsProfileLoaded)
                {
                    this.LoadItem(gameSave, SaveLoadManager.ActiveProfile);
                }
            }
		}

		public void Save(int profile)
		{
            if (this.onSave != null) this.onSave.Invoke(profile);
            if (this.storage == null) this.storage = new List<Storage>();
            if (this.data == null) this.data = new Dictionary<string, object>();

            this.SetCurrentProfile(profile);

            if (DatabaseGeneral.Load().saveScenes)
            {
                object saveData = this.scenesData.GetSaveData();
                this.data[this.scenesData.GetUniqueName()] = saveData;
            }

            for (int i = this.storage.Count - 1; i >= 0; --i)
            {
                IGameSave item = this.storage[i].target;
                if (item == null)
                {
                    this.storage.RemoveAt(i);
                    continue;
                }

                object saveData = item.GetSaveData();
                if (saveData == null) continue;

                if (!saveData.GetType().IsSerializable)
                {
                    throw new NonSerializableException(saveData.GetType().ToString());
                }

                this.data[item.GetUniqueName()] = saveData;
            }

            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, object> item in this.data)
            {
                string serializedSaveData = JsonUtility.ToJson(item.Value, false);
                string key = this.GetKeyName(profile, item.Key);

                keys.Add(key);
                DatabaseGeneral.Load().GetDataProvider().SetString(key, serializedSaveData);
            }

            this.keysData.Update(profile, keys);
		}

		public void Load(int profile, Action callback = null)
		{
            this.SetCurrentProfile(profile);
            SaveLoadManager.IsLoading = true;
            SaveLoadManager.LoadedProfile = profile;

            if (this.storage == null) this.storage = new List<Storage>();
            this.data = new Dictionary<string, object>();

            this.storage.Sort((Storage a, Storage b) =>
            {
                if (a.priority < b.priority) return  1;
                if (a.priority > b.priority) return -1;
                return 0;
            });

            StartCoroutine(this.CoroutineLoad(profile, callback));
		}

        private IEnumerator CoroutineLoad(int profile, Action callback)
        {
            string key = this.GetKeyName(profile, this.scenesData.GetUniqueName());
            string serializedData = DatabaseGeneral.Load().GetDataProvider().GetString(key);

            if (DatabaseGeneral.Load().saveScenes)
            {
                object genericData = JsonUtility.FromJson(
                    serializedData,
                    this.scenesData.GetSaveDataType()
                );

                yield return this.scenesData.OnLoad(genericData);
            }

            for (int i = this.storage.Count - 1; i >= 0; --i)
            {
                IGameSave item = this.storage[i].target;

                if (item == null)
                {
                    this.storage.RemoveAt(i);
                    continue;
                }

                item.ResetData();
                this.LoadItem(item, profile);
            }

            SaveLoadManager.IsLoading = false;
            SaveLoadManager.IsProfileLoaded = true;

            if (this.onLoad != null) this.onLoad.Invoke(profile);
            if (callback != null) callback.Invoke();
        }

        public void LoadLast(Action callback = null)
        {
            int profile = this.savesData.GetLastSave();
            if (profile >= 0) this.Load(profile, callback);
        }

        public int GetSavesCount()
        {
            return this.savesData.GetSavesCount();
        }

        public SavesData.Profile GetProfileInfo(int profile)
        {
            return this.savesData.GetProfileInfo(profile);
        }

        public void OnDestroyIGameSave(IGameSave gameSave)
        {
            if (this.isExiting) return;
            if (gameSave == null || this.data == null) return;
            if (IsLoading && (!IsProfileLoaded || ActiveProfile != LoadedProfile)) return;

            object saveData = gameSave.GetSaveData();
            this.data[gameSave.GetUniqueName()] = saveData;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void LoadItem(IGameSave gameSave, int profile)
		{
			if (gameSave == null) return;

			string key = this.GetKeyName(profile, gameSave.GetUniqueName());
            string serializedData = DatabaseGeneral.Load().GetDataProvider().GetString(key);

			if (!string.IsNullOrEmpty(serializedData))
			{
				Type type = gameSave.GetSaveDataType();
                object genericData = JsonUtility.FromJson(serializedData, type);

				gameSave.OnLoad(genericData);
			}
		}

		private string GetKeyName(int profile, string key)
		{
			return string.Format(STORE_KEYFMT, profile, key);
		}

		// EXCEPTIONS: ----------------------------------------------------------------------------

		[Serializable]
		private class NonSerializableException : Exception
		{
            private const string MESSAGE = "Unable to serialize: {0}. Add [System.Serializable]";
			public NonSerializableException(string key) : base(string.Format(MESSAGE, key)) {}
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////
	// INTERFACE ISAVEGAME: -----------------------------------------------------------------------
	///////////////////////////////////////////////////////////////////////////////////////////////

	public interface IGameSave
	{
		string GetUniqueName();

		Type GetSaveDataType();
		object GetSaveData();

		void ResetData();
        void OnLoad(object generic);
	}
}

namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;

    public class SavesData
    {
        private const string STORE_KEYFMT = "gamedata:profiles";

        // CLASSES: -------------------------------------------------------------------------------

        [Serializable]
        public class Profile
        {
            public string date;
        }

        [Serializable]
        public class Profiles : SerializableDictionaryBase<int, Profile> { }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Profiles profiles = new Profiles();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public SavesData(SaveLoadManager manager)
        {
            string data = DatabaseGeneral
                .Load()
                .GetDataProvider()
                .GetString(STORE_KEYFMT, String.Empty);

            if (!string.IsNullOrEmpty(data))
            {
                this.profiles = JsonUtility.FromJson<Profiles>(data);
            }

            manager.onSave += this.OnSave;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnSave(int profile)
        {
            if (!this.profiles.ContainsKey(profile))
            {
                this.profiles.Add(profile, new Profile());
            }

            this.profiles[profile].date = DateTime.Now.ToString();
            DatabaseGeneral
                .Load()
                .GetDataProvider()
                .SetString(STORE_KEYFMT, JsonUtility.ToJson(this.profiles));
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int GetLastSave()
        {
            int profile = -1;
            DateTime maxDate = DateTime.MinValue;

            foreach (KeyValuePair<int, Profile> item in this.profiles)
            {
                DateTime itemDate = DateTime.Parse(item.Value.date);
                if (DateTime.Compare(itemDate, maxDate) > 0)
                {
                    profile = item.Key;
                    maxDate = itemDate;
                }
            }

            return profile;
        }

        public int GetSavesCount()
        {
            return this.profiles.Count;
        }

        public Profile GetProfileInfo(int profile)
        {
            if (this.profiles.ContainsKey(profile))
            {
                return this.profiles[profile];
            }

            return null;
        }
    }
}

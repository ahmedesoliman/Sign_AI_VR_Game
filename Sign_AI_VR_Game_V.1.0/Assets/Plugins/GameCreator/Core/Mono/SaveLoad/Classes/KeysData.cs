namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class KeysData
    {
        private const string STORE_KEYFMT = "gamedata:{0:D2}:key-references";

        // CLASSES: -------------------------------------------------------------------------------

        [Serializable]
        private class Data
        {
            public List<string> keys = new List<string>();

            public Data()
            { }

            public Data(HashSet<string> hashset)
            {
                this.keys = new List<string>(hashset);
            }
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public void Update(int profile, List<string> keys)
        {
            Data data = this.GetCurrentKeys(profile);
            HashSet<string> hash = new HashSet<string>(data.keys);

            foreach (string key in keys) if (!hash.Contains(key)) hash.Add(key);

            DatabaseGeneral.Load().GetDataProvider().SetString(
                this.GetKey(profile),
                JsonUtility.ToJson(new Data(hash))
            );
        }

        public void Delete(int profile)
        {
            Data data = this.GetCurrentKeys(profile);
            IDataProvider provider = DatabaseGeneral.Load().GetDataProvider();

            foreach (string key in data.keys) provider.DeleteKey(key);
            provider.DeleteKey(this.GetKey(profile));
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Data GetCurrentKeys(int profile)
        {
            IDataProvider provider = DatabaseGeneral.Load().GetDataProvider();
            if (provider == null) return new Data();

            string strKeys = provider.GetString(this.GetKey(profile));
            return string.IsNullOrEmpty(strKeys)
                ? new Data()
                : JsonUtility.FromJson<Data>(strKeys);
        }

        private string GetKey(int profile)
        {
            return string.Format(STORE_KEYFMT, profile);
        }
    }
}
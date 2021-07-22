namespace GameCreator.Core
{
    using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public class DataProviderDefault : IDataProvider
    {
        // GETTER METHODS: ------------------------------------------------------------------------

        public override float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public override int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public override string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        // SETTER METHODS: ------------------------------------------------------------------------

        public override void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public override void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public override void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        // OTHER METHODS: -------------------------------------------------------------------------

        public override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public override void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}
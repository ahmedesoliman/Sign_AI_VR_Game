namespace GameCreator.Core
{
    using UnityEngine;

    [System.Serializable]
    public abstract class IDataProvider : ScriptableObject
    {
        public string title = "";
        public string description = "";

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract string GetString(string key, string defaultValue = "");
        public abstract void SetString(string key, string value);

        public abstract void DeleteAll();
        public abstract void DeleteKey(string key);
        public abstract bool HasKey(string key);

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual float GetFloat(string key, float defaultValue = 0.0f)
        {
            float result = defaultValue;
            float.TryParse(this.GetString(key), out result);

            return result;
        }

        public virtual int GetInt(string key, int defaultValue = 0)
        {
            int result = defaultValue;
            int.TryParse(this.GetString(key), out result);

            return result;
        }

        public virtual void SetFloat(string key, float value)
        {
            this.SetString(key, value.ToString());
        }

        public virtual void SetInt(string key, int value)
        {
            this.SetString(key, value.ToString());
        }
    }
}
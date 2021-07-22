namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class ScenesData
    {
        public string mainScene;
        public List<string> additiveScenes;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        public ScenesData(string mainScene)
        {
            this.mainScene = mainScene;
            this.additiveScenes = new List<string>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Add(string name, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                this.mainScene = name;
                this.additiveScenes = new List<string>();
            }
            else if (mode == LoadSceneMode.Additive)
            {
                this.additiveScenes.Add(name);
            }
        }

        public void Remove(string name)
        {
            this.additiveScenes.Remove(name);
        }

        // IGAMESAVE INTERFACE: -------------------------------------------------------------------

        public object GetSaveData()
        {
            return this;
        }

        public Type GetSaveDataType()
        {
            return typeof(ScenesData);
        }

        public string GetUniqueName()
        {
            return "scenes-data";
        }

        public IEnumerator OnLoad(object generic)
        {
            ScenesData data = (ScenesData)generic;

            SceneManager.LoadScene(data.mainScene, LoadSceneMode.Single);
            yield return null;

            for (int i = 0; i < data.additiveScenes.Count; ++i)
            {
                SceneManager.LoadScene(data.additiveScenes[i], LoadSceneMode.Additive);
                yield return null;
            }

            this.mainScene = data.mainScene;
            this.additiveScenes = data.additiveScenes;
        }

        public void ResetData()
        {
            this.mainScene = SceneManager.GetActiveScene().name;
            this.additiveScenes = new List<string>();
        }
    }
}
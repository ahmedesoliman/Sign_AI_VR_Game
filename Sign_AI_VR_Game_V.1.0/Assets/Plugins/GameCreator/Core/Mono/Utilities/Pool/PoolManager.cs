namespace GameCreator.Pool
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [AddComponentMenu("")]
    public class PoolManager : Singleton<PoolManager>
    {
        [Serializable]
        public class PoolData
        {
            public PoolObject prefab;
            public Transform container;
            public List<GameObject> instances;

            public PoolData(PoolObject prefab)
            {
                this.container = new GameObject(prefab.gameObject.name).transform;
                this.container.SetParent(PoolManager.Instance.transform);
                this.container.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                this.prefab = prefab;
                this.Rebuild();
            }

            private void Rebuild()
            {
                this.instances = new List<GameObject>();
                prefab.gameObject.SetActive(false);
                for (int i = 0; i < prefab.initCount; ++i)
                {
                    GameObject instance = Instantiate(prefab.gameObject);
                    instance.SetActive(false);
                    instance.transform.SetParent(this.container);
                    this.instances.Add(instance);
                }
            }

            public GameObject Get()
            {
                int count = this.instances.Count;
                if (count == 0) this.Rebuild();

                for (int i = count - 1; i >= 0; --i)
                {
                    if (this.instances[i] == null)
                    {
                        this.instances.RemoveAt(i);
                        continue;
                    }

                    if (!this.instances[i].activeSelf)
                    {
                        this.instances[i].SetActive(true);
                        this.instances[i].transform.SetParent(this.container);
                        return this.instances[i];
                    }
                }

                prefab.gameObject.SetActive(false);
                GameObject instance = Instantiate(prefab.gameObject);
                instance.transform.SetParent(this.container);

                this.instances.Add(instance);
                return instance;
            }
        }


        // PROPERTIES: ---------------------------------------------------------

        private Dictionary<int, PoolData> pool;

        // INITIALIZERS: -------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            this.pool = new Dictionary<int, PoolData>();
        }

        // PUBLIC METHODS: -----------------------------------------------------

        public GameObject Pick(GameObject prefab)
        {
            if (prefab == null) return null;
            PoolObject component = prefab.GetComponent<PoolObject>();
            if (component == null) component = prefab.AddComponent<PoolObject>();

            return this.Pick(component);
        }

        public GameObject Pick(PoolObject prefab)
        {
            if (prefab == null) return null;
            int instanceID = prefab.GetInstanceID();

            if (!this.pool.ContainsKey(instanceID)) this.BuildPool(prefab);
            return this.pool[instanceID].Get();
        }

        // PRIVATE METHODS: ----------------------------------------------------

        private void BuildPool(PoolObject prefab)
        {
            int instanceID = prefab.GetInstanceID();
            this.pool.Add(instanceID, new PoolData(prefab));
        }
    }
}
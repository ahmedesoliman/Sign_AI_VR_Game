namespace GameCreator.Characters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("")]
    public class CharacterAttachments : MonoBehaviour
    {
        [Serializable]
        public class EventData
        {
            public GameObject attachment;
            public HumanBodyBones bone;
            public bool isDestroy;
        }

        [Serializable]
        public class AttachmentEvent : UnityEvent<EventData>
        { }

        [Serializable]
        public class Attachment
        {
            public GameObject prefab = null;
            public GameObject instance = null;
            public Vector3 locPosition = Vector3.zero;
            public Quaternion locRotation = Quaternion.identity;

            public Attachment(GameObject instance, GameObject prefab = null)
            {
                this.prefab = prefab;
                this.instance = instance;
                this.locPosition = instance.transform.localPosition;
                this.locRotation = instance.transform.localRotation;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        public Dictionary<HumanBodyBones, List<Attachment>> attachments { get; private set; }

        public AttachmentEvent onAttach = new AttachmentEvent();
        public AttachmentEvent onDetach = new AttachmentEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Animator animator)
        {
            this.animator = animator;
            this.attachments = new Dictionary<HumanBodyBones, List<Attachment>>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Attach(HumanBodyBones bone, GameObject prefab, Vector3 position, Quaternion rotation, Space space = Space.Self)
        {
            if (!this.attachments.ContainsKey(bone)) this.attachments.Add(bone, new List<Attachment>());

            GameObject instance = prefab;
            if (string.IsNullOrEmpty(prefab.scene.name))
            {
                instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }

            instance.transform.SetParent(this.animator.GetBoneTransform(bone));

            switch (space)
            {
                case Space.Self :
                    instance.transform.localPosition = position;
                    instance.transform.localRotation = rotation;
                    break;

                case Space.World:
                    instance.transform.position = position;
                    instance.transform.rotation = rotation;
                    break;
            }

            this.attachments[bone].Add(new Attachment(instance, prefab));

            if (this.onAttach != null)
            {
                this.onAttach.Invoke(new EventData
                {
                    attachment = instance,
                    bone = bone
                });
            }
        }

        public List<GameObject> Detach(HumanBodyBones bone)
        {
            return this.DetachOrDestroy(bone, false);
        }

        public bool Detach(GameObject instance)
        {
            return this.DetachOrDestroy(instance, false);
        }

        public void Remove(HumanBodyBones bone)
        {
            this.DetachOrDestroy(bone, true);
        }

        public void Remove(GameObject instance)
        {
            this.DetachOrDestroy(instance, true);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private List<GameObject> DetachOrDestroy(HumanBodyBones bone, bool destroy)
        {
            List<Attachment> objects = new List<Attachment>();
            List<GameObject> results = new List<GameObject>();
            if (this.attachments.ContainsKey(bone))
            {
                objects = new List<Attachment>(this.attachments[bone]);
                this.attachments.Remove(bone);

                for (int i = 0; i < objects.Count; ++i)
                {
                    if (objects[i] != null && objects[i].instance != null)
                    {
                        objects[i].instance.transform.SetParent(null);

                        if (this.onDetach != null)
                        {
                            this.onDetach.Invoke(new EventData
                            {
                                attachment = objects[i].instance,
                                bone = bone,
                                isDestroy = destroy
                            });
                        }

                        if (destroy) Destroy(objects[i].instance);
                        else results.Add(objects[i].instance);
                    }
                }
            }

            return results;
        }

        private bool DetachOrDestroy(GameObject instance, bool destroy)
        {
            foreach (KeyValuePair<HumanBodyBones, List<Attachment>> item in this.attachments)
            {
                if (item.Value == null) continue;

                int subItemIndex = -1;
                for (int i = 0; i < this.attachments[item.Key].Count; ++i)
                {
                    if (this.attachments[item.Key][i].instance == instance)
                    {
                        subItemIndex = i;
                        break;
                    }
                }

                if (subItemIndex >= 0)
                {
                    this.attachments[item.Key].RemoveAt(subItemIndex);
                    instance.transform.SetParent(null);

                    if (this.onDetach != null)
                    {
                        this.onDetach.Invoke(new EventData
                        {
                            attachment = instance,
                            bone = item.Key,
                            isDestroy = destroy
                        });
                    }

                    if (destroy) Destroy(instance);

                    if (this.attachments[item.Key].Count == 0)
                    {
                        this.attachments.Remove(item.Key);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
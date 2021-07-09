namespace GameCreator.Characters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [System.Serializable]
    public class CharacterRagdoll
    {
        public enum State
        {
            Normal,
            Ragdoll,
            Recover
        }

        public enum Bone
        {
            Root,
            L_Hip,
            L_Knee,
            R_Hip,
            R_Knee,
            Spine,
            L_Arm,
            L_Elbow,
            R_Arm,
            R_Elbow,
            Head,
            Character
        }

        public class BoneData
        {
            public string name;

            public Transform anchor;
            public Joint joint;
            public Rigidbody rigidbody;
            public Collider collider;
            public Bone parent;

            public float minLimit;
            public float maxLimit;
            public float swingLimit;

            public Vector3 axis;
            public Vector3 normalAxis;

            public float radiusScale;
            public Type colliderType;

            public List<Bone> children;
            public float density;

            public BoneData()
            {
                this.children = new List<Bone>();
                this.density = 0.1f;
            }

            public BoneData(Transform bone) : this()
            {
                this.name = bone.gameObject.name;
                this.anchor = bone;
            }
        }

        private class HumanChunk
        {
            public Transform transform;
            public Vector3 localPosition;
            public Vector3 worldPosition;

            public Quaternion worldRotation;
            public Quaternion localRotation;

            public HumanChunk(Transform transform)
            {
                this.transform = transform;
                this.Snapshot();
            }

            public void Snapshot()
            {
                this.worldPosition = this.transform.position;
                this.localPosition = this.transform.localPosition;

                this.worldRotation = this.transform.rotation;
                this.localRotation = this.transform.localRotation;
            }
        }

        private const float SMOOTH_RAGDOLL_FOLLOW = 0.2f;
        private const float SMOOTH_RAGDOLL_TRANSITION = 0.75f;

        private static readonly Vector3 PLANE_XZ = new Vector3(1f, 0f, 1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        private Character character;
        private CharacterAnimator charAnimator;

        private State state = State.Normal;
        private bool isInitialized = false;

        private Vector3 interpolation = Vector3.zero;
        private float changeTime = -100f;
        private float stableTime = -100f;

        private List<HumanChunk> chunks = new List<HumanChunk>();
        private HumanChunk rootChunk = null;

        private bool startRecover = false;
        private Vector3 startRecoverDirection = Vector3.zero;

        private bool autoStandUp = false;
        private BoneData[] bones = new BoneData[0];
        private BoneData root = null;

        private RaycastHit[] hitBuffer = new RaycastHit[10];

        // INITIALIZERS: --------------------------------------------------------------------------

        public CharacterRagdoll(Character character)
        {
            this.character = character;
            this.charAnimator = this.character.GetCharacterAnimator();

            this.Initialize(true);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Initialize(bool forceInitialize = false)
        {
            if (!forceInitialize && this.isInitialized) return;
            if (this.charAnimator == null || !this.charAnimator.animator.isHuman) return;

            bool result = (
                this.BuildBonesData() &&
                this.BuildColliders() &&
                this.BuildBodies() &&
                this.BuildJoints() &&
                this.BuildMasses() && 
                this.BuildLayers() &&
                this.BuildChunks()
            );

            this.isInitialized = result;
        }

        public void Ragdoll(bool active, bool autoStandUp)
        {
            this.autoStandUp = autoStandUp;
            this.changeTime = Time.time;
            this.stableTime = Time.time;

            for (int i = 0; i < this.bones.Length; ++i)
            {
                this.bones[i].rigidbody.isKinematic = !active;
                this.bones[i].collider.enabled = active;
            }

            switch (active)
            {
                case true  : this.ToRagdoll(); break;
                case false : this.ToRecover();  break;
            }
        }

        public State GetState()
        {
            return this.state;
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public void Update()
        {
            switch (this.state)
            {
                case State.Ragdoll : this.UpdateRagdoll(); break;
                case State.Recover: this.UpdateRecover(); break;
            }
        }

        private void UpdateRagdoll()
        {
            this.character.transform.position = Vector3.SmoothDamp(
                this.character.transform.position,
                this.root.anchor.position,
                ref this.interpolation,
                SMOOTH_RAGDOLL_FOLLOW
            );

            Vector3 ragdollDirection = this.GetRagdollDirection();
            this.character.transform.rotation = Quaternion.LookRotation(
                ragdollDirection,
                Vector3.up
            );

            if (this.autoStandUp)
            {
                if (this.root.rigidbody.velocity.magnitude > 0.1f) this.stableTime = Time.time;
                if (Time.time - this.stableTime > 0.5f) this.character.SetRagdoll(false);
            }
        }

        private void UpdateRecover()
        {
            if (this.startRecover)
            {
                this.startRecover = false;
                this.character.transform.rotation = Quaternion.LookRotation(
                    this.startRecoverDirection,
                    Vector3.up
                );
            }

            float duration = SMOOTH_RAGDOLL_TRANSITION;
            float t = (Time.time - this.changeTime) / duration;

            this.rootChunk.transform.localPosition = Vector3.Lerp(
                this.rootChunk.localPosition,
                this.rootChunk.transform.localPosition,
                t
            );

            this.rootChunk.transform.localRotation = Quaternion.Lerp(
                this.rootChunk.localRotation,
                this.rootChunk.transform.localRotation,
                t
            );

            for (int i = 0; i < this.chunks.Count; ++i)
            {
                if (this.chunks[i].transform == this.root.anchor)
                {
                    this.chunks[i].transform.position = Vector3.Lerp(
                        this.chunks[i].worldPosition,
                        this.chunks[i].transform.position,
                        t
                    );
                }

                if (this.chunks[i].localRotation != this.chunks[i].transform.localRotation)
                {
                    this.chunks[i].transform.rotation = Quaternion.Lerp(
                        this.chunks[i].worldRotation,
                        this.chunks[i].transform.rotation,
                        t
                    );
                }
            }

            if (t >= 1f)
            {
                this.state = State.Normal;
                this.character.characterLocomotion.isBusy = false;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ToRagdoll()
        {
            this.rootChunk.Snapshot();
            this.character.characterLocomotion.isBusy = true;

            this.state = State.Ragdoll;
            this.interpolation = Vector3.zero;

            Vector3 velocity = this.character.characterLocomotion.characterController.velocity;
            for (int i = 0; i < this.bones.Length; ++i)
            {
                this.bones[i].rigidbody.AddForce(velocity, ForceMode.VelocityChange);
            }
        }

        private void ToRecover()
        {
            this.state = State.Recover;
            this.startRecover = true;

            for (int i = 0; i < this.chunks.Count; ++i)
            {
                this.chunks[i].Snapshot();
            }

            int hitCount = Physics.RaycastNonAlloc(this.root.anchor.position, Vector3.down, hitBuffer, 5f);
            for (int i = 0; i < hitCount; ++i)
            {
                if (!this.hitBuffer[i].transform.IsChildOf(this.character.transform) &&
                    !this.hitBuffer[i].transform.IsChildOf(root.anchor))
                {
                    float offset = this.character.characterLocomotion.characterController.skinWidth;
                    this.character.transform.position = this.hitBuffer[i].point + (Vector3.up * offset);
                    break;
                }
            }

            Vector3 ragdollDirection = Vector3.zero;
            switch (this.root.anchor.forward.y < 0f)
            {
                case true:
                    this.charAnimator.CrossFadeGesture(this.charAnimator.standFaceDown, 1f, null, 0f, 0.25f);
                    ragdollDirection = this.GetRagdollDirection();
                    break;

                case false:
                    this.charAnimator.CrossFadeGesture(this.charAnimator.standFaceUp, 1f, null, 0f, 0.25f);
                    ragdollDirection = -1f * this.GetRagdollDirection();
                    break;
            }

            this.startRecoverDirection = ragdollDirection;
        }

        private Vector3 GetRagdollDirection()
        {
            Vector3 pointLKnee = this.bones[(int)Bone.L_Knee].anchor.position;
            Vector3 pointRKnee = this.bones[(int)Bone.R_Knee].anchor.position;
            Vector3 pointFeet = (pointLKnee + pointRKnee) / 2.0f;
            Vector3 pointHead = this.bones[(int)Bone.Head].anchor.position;

            Vector3 direction = Vector3.Scale((pointHead - pointFeet), PLANE_XZ);

            return direction;
        }

        // BUILD METHODS: -------------------------------------------------------------------------

        private bool BuildBonesData()
        {
            Animator animator = this.charAnimator.animator;
            if (animator == null) return false;

            this.bones = new BoneData[]
            {
                new BoneData(animator.GetBoneTransform(HumanBodyBones.Hips)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.Spine)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightUpperArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightLowerArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.Head)),
            };

            this.root = this.bones[(int)Bone.Root];

            Vector3 unitX = this.root.anchor.TransformDirection(Vector3.right);
            Vector3 unitY = this.root.anchor.TransformDirection(Vector3.up);
            Vector3 unitZ = this.root.anchor.TransformDirection(Vector3.forward);

            this.SetupJoint(Bone.L_Hip, Bone.Root, unitX, unitZ, -20, 70, 30, typeof(CapsuleCollider), 0.3f, 0.1f);
            this.SetupJoint(Bone.R_Hip, Bone.Root, unitX, unitZ, -20, 70, 30, typeof(CapsuleCollider), 0.3f, 0.1f);

            this.SetupJoint(Bone.L_Knee, Bone.L_Hip, unitX, unitZ, -80, 0, 0, typeof(CapsuleCollider), 0.25f, 0.05f);
            this.SetupJoint(Bone.R_Knee, Bone.R_Hip, unitX, unitZ, -80, 0, 0, typeof(CapsuleCollider), 0.25f, 0.05f);

            this.SetupJoint(Bone.Spine, Bone.Root, unitX, unitZ, -20, 20, 10, typeof(BoxCollider), 0.25f, 0.2f);
            this.SetupJoint(Bone.Head, Bone.Spine, unitX, unitZ, -40, 25, 25, typeof(SphereCollider), 1f, 0.1f);

            this.SetupJoint(Bone.L_Arm, Bone.Spine, unitY, unitZ, -70, 10, 50, typeof(CapsuleCollider), 0.25f, 0.075f);
            this.SetupJoint(Bone.R_Arm, Bone.Spine, unitY, unitZ, -70, 10, 50, typeof(CapsuleCollider), 0.25f, 0.075f);

            this.SetupJoint(Bone.L_Elbow, Bone.L_Arm, unitZ, unitY, -90, 0, 0, typeof(CapsuleCollider), 0.25f, 0.075f);
            this.SetupJoint(Bone.R_Elbow, Bone.R_Arm, unitZ, unitY, -90, 0, 0, typeof(CapsuleCollider), 0.20f, 0.075f);

            return true;
        }

        void SetupJoint(Bone bone, Bone parent, Vector3 twistAxis, Vector3 swingAxis, 
                        float minLimit, float maxLimit, float swingLimit, Type collider, 
                        float radius, float density)
        {
            int boneIndex = (int)bone;

            this.bones[boneIndex].axis = twistAxis;
            this.bones[boneIndex].normalAxis = swingAxis;

            this.bones[boneIndex].minLimit = minLimit;
            this.bones[boneIndex].maxLimit = maxLimit;
            this.bones[boneIndex].swingLimit = swingLimit;

            this.bones[boneIndex].colliderType = collider;
            this.bones[boneIndex].radiusScale = radius;
            this.bones[boneIndex].density = density;

            this.bones[boneIndex].parent = parent;
            this.bones[(int)parent].children.Add(bone);
        }

        private bool BuildColliders()
        {
            this.BuildColliderCapsule(this.bones[(int)Bone.L_Hip]);
            this.BuildColliderCapsule(this.bones[(int)Bone.R_Hip]);
            this.BuildColliderCapsule(this.bones[(int)Bone.L_Knee]);
            this.BuildColliderCapsule(this.bones[(int)Bone.R_Knee]);

            this.BuildColliderCapsule(this.bones[(int)Bone.L_Arm]);
            this.BuildColliderCapsule(this.bones[(int)Bone.R_Arm]);
            this.BuildColliderCapsule(this.bones[(int)Bone.L_Elbow]);
            this.BuildColliderCapsule(this.bones[(int)Bone.R_Elbow]);

            this.BuildColliderBox(this.bones[(int)Bone.Spine], false);
            this.BuildColliderBox(this.bones[(int)Bone.Root], true);
            this.BuildColliderSphere(this.bones[(int)Bone.Head]);

            return true;
        }

        private void BuildColliderCapsule(BoneData bone)
        {
            int direction = 0;
            float distance = 0.0f;

            if (bone.children.Count == 1)
            {
                Vector3 endPoint = this.bones[(int)bone.children[0]].anchor.position;
                RagdollUtilities.GetDirection(
                    bone.anchor.InverseTransformPoint(endPoint), 
                    out direction, out distance
                );
            }
            else
            {
                Vector3 length = bone.anchor.position - this.bones[(int)bone.parent].anchor.position;
                Vector3 endPoint = bone.anchor.position + length;

                RagdollUtilities.GetDirection(
                    bone.anchor.InverseTransformPoint(endPoint),
                    out direction, out distance
                );

                if (bone.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                {
                    Bounds bounds = RagdollUtilities.GetBounds(
                        bone.anchor,
                        bone.anchor.GetComponentsInChildren<Transform>()
                    );

                    if (distance > 0f) distance = bounds.max[direction];
                    if (distance < 0f) distance = bounds.min[direction];
                }
            }

            CapsuleCollider capsuleCollider = bone.anchor.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.enabled = false;
            capsuleCollider.direction = direction;
            bone.collider = capsuleCollider;

            Vector3 center = Vector3.zero;
            center[direction] = distance/2f;
            capsuleCollider.center = center;
            capsuleCollider.height = Mathf.Abs(distance);
            capsuleCollider.radius = Mathf.Abs(distance * bone.radiusScale);
        }

        private void BuildColliderSphere(BoneData bone)
        {
            float radius = 0.25f * Vector3.Distance(
                this.bones[(int)Bone.L_Arm].anchor.position,
                this.bones[(int)Bone.R_Arm].anchor.position
            );

            SphereCollider sphereCollider = bone.anchor.gameObject.AddComponent<SphereCollider>();
            sphereCollider.enabled = false;
            sphereCollider.radius = radius;
            bone.collider = sphereCollider;

            Vector3 center = Vector3.zero;
            int direction = 0;
            float distance = 0.0f;

            Vector3 point = bone.anchor.InverseTransformPoint(this.bones[(int)Bone.Root].anchor.position);
            RagdollUtilities.GetDirection(point, out direction, out distance);

            center[direction] = Mathf.Sign(-distance) * radius;
            sphereCollider.center = center;
        }

        private void BuildColliderBox(BoneData bone, bool below)
        {
            Transform spine = this.bones[(int)Bone.Spine].anchor;
            Vector3 axisUp = bone.anchor.TransformDirection(Vector3.up);

            Transform[] limbs = new Transform[]
            {
                this.bones[(int)Bone.L_Hip].anchor,
                this.bones[(int)Bone.R_Hip].anchor,
                this.bones[(int)Bone.L_Arm].anchor,
                this.bones[(int)Bone.R_Arm].anchor,
            };

            Bounds bounds = RagdollUtilities.GetBounds(bone.anchor, limbs);
            bounds = RagdollUtilities.ProportionalBounds(bounds);
            bounds = RagdollUtilities.Clip(bounds, bone.anchor, spine, axisUp, below);

            BoxCollider boxCollider = bone.anchor.gameObject.AddComponent<BoxCollider>();
            boxCollider.enabled = false;
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;
            bone.collider = boxCollider;
        }

        private bool BuildBodies()
        {
            for (int i = 0; i < this.bones.Length; ++i)
            {
                this.bones[i].rigidbody = this.bones[i].anchor.gameObject.AddComponent<Rigidbody>();
                this.bones[i].rigidbody.mass = this.bones[i].density;
                this.bones[i].rigidbody.isKinematic = true;
                this.bones[i].rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            return true;
        }

        private bool BuildJoints()
        {
            for (int i = 1; i < this.bones.Length; ++i)
            {
                CharacterJoint joint = this.bones[i].anchor.gameObject.AddComponent<CharacterJoint>();

                Vector3 jointAxis = this.bones[i].anchor.InverseTransformDirection(this.bones[i].axis);
                Vector3 jointNomal = this.bones[i].anchor.InverseTransformDirection(this.bones[i].normalAxis);

                joint.axis = RagdollUtilities.GetDirectionAxis(jointAxis);
                joint.swingAxis = RagdollUtilities.GetDirectionAxis(jointNomal);

                joint.anchor = Vector3.zero;
                joint.connectedBody = this.bones[(int)this.bones[i].parent].rigidbody;
                joint.enablePreprocessing = false;

                SoftJointLimit limit = new SoftJointLimit();
                limit.contactDistance = 0;

                limit.limit = this.bones[i].minLimit;
                joint.lowTwistLimit = limit;

                limit.limit = this.bones[i].maxLimit;
                joint.highTwistLimit = limit;

                limit.limit = this.bones[i].swingLimit;
                joint.swing1Limit = limit;

                limit.limit = 0;
                joint.swing2Limit = limit;

                this.bones[i].joint = joint;
            }

            return true;
        }

        private bool BuildMasses()
        {
            for (int i = 0; i < this.bones.Length; ++i)
            {
                this.bones[i].rigidbody.mass *= this.charAnimator.ragdollMass;
            }

            return true;
        }

        private bool BuildLayers()
        {
            for (int i = 0; i < this.bones.Length; ++i)
            {
                this.bones[i].collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            return true;
        }

        private bool BuildChunks()
        {
            Transform target = this.charAnimator.animator.transform;
            Transform[] children = target.GetComponentsInChildren<Transform>();

            this.rootChunk = new HumanChunk(target);
            this.chunks = new List<HumanChunk>();
            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] == target) continue;
                this.chunks.Add(new HumanChunk(children[i]));
            }

            return true;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public void OnDrawGizmos()
        {
            this.GizmoBone(Bone.Root, Bone.L_Hip);
            this.GizmoBone(Bone.Root, Bone.R_Hip);
            this.GizmoBone(Bone.L_Hip, Bone.L_Knee);
            this.GizmoBone(Bone.R_Hip, Bone.R_Knee);

            this.GizmoBone(Bone.Root, Bone.Spine);
            this.GizmoBone(Bone.Spine, Bone.Head);

            this.GizmoBone(Bone.Spine, Bone.L_Arm);
            this.GizmoBone(Bone.Spine, Bone.R_Arm);
            this.GizmoBone(Bone.L_Arm, Bone.L_Elbow);
            this.GizmoBone(Bone.R_Arm, Bone.R_Elbow);
        }

        private void GizmoBone(Bone a, Bone b)
        {
            if (this.bones == null || this.bones.Length == 0) return;
            if (this.bones[(int)a].anchor == null) return;
            if (this.bones[(int)b].anchor == null) return;

            Color tempColor = Gizmos.color;
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                this.bones[(int)a].anchor.position, 
                this.bones[(int)b].anchor.position
            );

            Gizmos.color = tempColor;
        }
    }
}
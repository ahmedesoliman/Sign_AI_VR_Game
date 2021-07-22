namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

    [AddComponentMenu("")]
	public class ActionBlendShape : IAction
	{
        private class SMRData
        {
            public SkinnedMeshRenderer skin;
            public int index;
            public float initWeight;
            public float targetWeight;

            public SMRData(SkinnedMeshRenderer skin, int index, float targetWeight)
            {
                this.skin = skin;
                this.index = index;

                this.initWeight = this.skin.GetBlendShapeWeight(this.index);
                this.targetWeight = targetWeight;
            }
        }

        public TargetGameObject skinnedMeshRenderer = new TargetGameObject();
        [Space] public StringProperty blendShape = new StringProperty("Blend-Shape-Name");

        [Space][Range(0f, 5f)] public float duration = 0.1f;
        public NumberProperty weight = new NumberProperty(1f);

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            SkinnedMeshRenderer[] candidateSkins = this
                .skinnedMeshRenderer
                .GetComponentsInChildren<SkinnedMeshRenderer>(target);
                
            string blendShapeName = this.blendShape.GetValue(target);
            float targetWeight = this.weight.GetValue(target);

            List<SMRData> skins = new List<SMRData>();
            for (int i = 0; i < candidateSkins.Length; ++i)
            {
                SkinnedMeshRenderer candidate = candidateSkins[i];
                int blendShapeIndex = candidate.sharedMesh.GetBlendShapeIndex(blendShapeName);
                if (blendShapeIndex >= 0)
                {
                    skins.Add(new SMRData(
                        candidate, 
                        blendShapeIndex,
                        targetWeight
                    ));
                }
            }
              
            if (skins.Count > 0)
            {
                float initTime = Time.time;
                while (initTime + this.duration > Time.time)
                {
                    float t = (Time.time - initTime) / this.duration;
                    this.SetWeights(skins, t);
                    yield return null;
                }
            }

            this.SetWeights(skins, 1f);
            yield return 0;
        }

        private void SetWeights(List<SMRData> skins, float t)
        {
            foreach (SMRData skin in skins)
            {
                float currentWeight = Easing.QuadInOut(skin.initWeight, skin.targetWeight, t);
                skin.skin.SetBlendShapeWeight(skin.index, currentWeight);
            }
        }

        #if UNITY_EDITOR
        public static new string NAME = "Animation/Blend Shape";
        private const string NODE_TITLE = "Change {0} blend shape: {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.skinnedMeshRenderer, this.blendShape);
        }
        #endif
    }
}

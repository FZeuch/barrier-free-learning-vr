using UnityEngine;
using UnityEngine.Events;
using FreeHandGestureFramework.EnumsAndTypes;

namespace FreeHandGestureUnity
{

[System.Serializable]
    public class FreeHandWidgetOQ : MonoBehaviour
    {
        public OVRSkeleton RightHand;
        public OVRSkeleton LeftHand;
        public GestureStageU[] ConnectedStages;
        public Collider WidgetCollider;
        public bool Active;
        private bool _touching = false;
        public UnityEvent OnTouchStart;
        public UnityEvent OnTouching;
        public UnityEvent OnTouchStop;
        public bool LeftHandTouching {get; private set;} = false;
        public bool RightHandTouching {get; private set;} = false;

        void Update()
        {
            if (!Active
                || ((RightHand == null || RightHand.Bones == null || RightHand.Bones.Count == 0)
                    && (LeftHand == null || LeftHand.Bones == null || LeftHand.Bones.Count == 0) )) return;

            if (_touching == false) 
            {
                if (CheckCollision())
                {
                    _touching = true;
                    OnTouchStart.Invoke();
                }
            }
            else
            {
                if (CheckCollision())
                {
                    OnTouching.Invoke();
                }
                else
                {
                    OnTouchStop.Invoke();
                    _touching = false;
                }
            }
        }
        private bool CheckCollision()
        {    
            if (WidgetCollider == null) 
            {
                Debug.Log("No collider attached!");
                return false;
            }

            var fhr = FreeHandRuntimeU.Instance;
            var currentStage = fhr.GetActiveGesture()?.GetCurrentStage();
            RelevantHand touchingHand = RelevantHand.Any;

            if (ConnectedStages != null && ConnectedStages.Length > 0)
            {
                if (fhr.ActiveGesture == -1) return false;
                bool found = false;
                foreach(var stage in ConnectedStages)
                {
                    if (stage == currentStage) found=true;
                }
                if (found == false) return false;
                touchingHand = currentStage.ManipulationHand;
            }

            //Find out left or right hand touches the widget
            int leftManipulationBone = currentStage == null ? -1 : currentStage.LeftManipulationBone;
            float leftManipulationSphere = currentStage == null ? 0.0f: currentStage.LeftManipulationSphere;
            LeftHandTouching = GetClosestPointDistance(LeftHand, leftManipulationBone, WidgetCollider) <= leftManipulationSphere;
            
            int rightManipulationBone = currentStage == null ? -1 : currentStage.RightManipulationBone;
            float rightManipulationSphere = currentStage == null ? 0.0f: currentStage.RightManipulationSphere;
            RightHandTouching = GetClosestPointDistance(RightHand, rightManipulationBone, WidgetCollider) <= rightManipulationSphere;

            switch(touchingHand)
            {
                case RelevantHand.Left: return LeftHandTouching;
                case RelevantHand.Right: return RightHandTouching;
                case RelevantHand.Any: return LeftHandTouching || RightHandTouching;
                case RelevantHand.Both: return LeftHandTouching && RightHandTouching;
                default:return false;
            }
        }

        private float GetClosestPointDistance(OVRSkeleton hand, int boneID, Collider collider)
        {
            if (hand == null) return float.MaxValue;

            if(boneID >= 0) 
            {
                Vector3 bonePosition = hand.Bones[boneID].Transform.position;
                Vector3 closestPoint = collider.ClosestPoint(bonePosition);
                if (bonePosition == closestPoint) return 0.0f;
                else return Vector3.Distance(bonePosition, closestPoint);
            }

            else//boneID == -1
            {
                //calculate distance of closest bone
                float returnValue = float.MaxValue;
                foreach(OVRBone bone in hand.Bones)
                {
                    Vector3 bonePosition = bone.Transform.position;
                    Vector3 closestPoint = collider.ClosestPoint(bonePosition);
                    if (bonePosition == closestPoint) returnValue = 0.0f;
                    else 
                    {
                        float distance = Vector3.Distance(bonePosition, closestPoint);
                        if (distance < returnValue) returnValue = distance;
                    }
                    if (returnValue == 0.0) break;
                }
                return returnValue;
            }
        }

    }
}

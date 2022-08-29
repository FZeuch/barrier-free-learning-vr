using UnityEngine;
using UnityEngine.Events;
using StandardTools;

namespace FreeHandGestureUnity.OculusQuest
{
    public class FreeHandSliderOQ : MonoBehaviour
    {
        public float ReleaseDistance;
        public GameObject SliderObject;
        public FreeHandWidgetOQ WidgetController;
        public float MinValue, MaxValue;
        public Vector3 StartPoint, EndPoint;
        private bool _sliding=false;
        private bool _leftHandTouching=false;
        private float _value;
        public UnityEvent OnValueChanged;
        public float Value
        {
            get {return _value;}
            set 
            {
                _value=MathTools.Clamp(value, MinValue, MaxValue);
                SetSliderPosition();
            }
        }
        public void Awake()
        {
            _value = MathTools.Clamp(_value, MinValue, MaxValue);
        }
        public void Update()
        {
            if(_sliding)
            {
                Slide();
            }
        }
        public void OnTouchStart()
        {
            _leftHandTouching = WidgetController.LeftHandTouching;
            _sliding = true;
        }
        private void Slide()
        {
            float oldValue=_value;
            Vector3 indexFingerPos = _leftHandTouching?WidgetController.LeftHand.Bones[20].Transform.position:WidgetController.RightHand.Bones[20].Transform.position;
            Vector3 projectedPos = StartPoint+Vector3.Project(indexFingerPos-StartPoint,EndPoint-StartPoint);
            float slidingLineLength=Vector3.Distance(StartPoint,EndPoint);
            if (Vector3.Distance(indexFingerPos,projectedPos)>ReleaseDistance)
            {   //Finger too far away from the sliding line?
                _sliding = false;
                return;
            }
            else if (Vector3.Distance(projectedPos,StartPoint)>slidingLineLength)
            {
                _value = MaxValue;
            }
            else if (Vector3.Distance(projectedPos,EndPoint)>slidingLineLength)
            {
                _value = MinValue;
            }
            else
            {
                float valueRange = MaxValue-MinValue;
                _value = MinValue + ((Vector3.Distance(projectedPos,StartPoint)/slidingLineLength) * valueRange);
            }
            if (_value != oldValue)
            {
                SetSliderPosition();
                OnValueChanged.Invoke();
            }
        }

        
        private void SetSliderPosition()
        {
            Vector3 lerpPosition = Vector3.Lerp(StartPoint,EndPoint,(_value-MinValue)/(MaxValue-MinValue));
            if(!(lerpPosition.x.Equals(float.NaN) || lerpPosition.y.Equals(float.NaN) || lerpPosition.z.Equals(float.NaN)))
            {
                SliderObject.transform.position = lerpPosition;
            }
        }
    }
}
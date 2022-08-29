using UnityEngine;
using UnityEngine.Events;
using FreeHandGestureFramework.DataTypes;
using FreeHandGestureUnity;
using FreeHandGestureUnity.OculusQuest;
using StandardTools;
using System;

public class FreeHandVertScrollAreaControllerOQ : MonoBehaviour
{
    public FreeHandWidgetOQ TouchArea;
    public float MinValue;
    public float MaxValue;
    public float Range;
    public Transform LowValueEdge;
    public Transform HighValueEdge;
    public UnityEvent OnValueChanged;
    private bool _touching = false;
    private float _valueAtLowEdge=0;
    private float _currentValue=0;

    public void OnTouching()
    {
        var gh = GestureHandlerU.Instance;
        var currentStage = gh.GetActiveGesture()?.GetCurrentStage();
        Position3D touchingHandPos;
        if (TouchArea.RightHandTouching)
        {
            touchingHandPos = currentStage == null ?
                ConversionTools.Vector3ToPosition3D(TouchArea.RightHand.Bones[20].Transform.position)//index finger
                : currentStage.GetHandPosition(new HandPoseInWorldSpaceUOQ(TouchArea.RightHand),true);
        }
        else if (TouchArea.LeftHandTouching)
        {
            touchingHandPos = currentStage == null ?
                ConversionTools.Vector3ToPosition3D(TouchArea.LeftHand.Bones[20].Transform.position)
                : currentStage.GetHandPosition(new HandPoseInWorldSpaceUOQ(TouchArea.LeftHand),true);
        }
        else return;

        Vector3 projectedHandPos = Vector3.Project(ConversionTools.Position3DToVector3(touchingHandPos)-LowValueEdge.position, HighValueEdge.position-LowValueEdge.position)+LowValueEdge.position;
        float distToLow = Vector3.Distance(projectedHandPos, LowValueEdge.position);
        float distToHigh = Vector3.Distance(projectedHandPos, HighValueEdge.position);
        float ratio = distToLow/(distToLow+distToHigh);

        if (_touching==false)
        {
            //on a new touch, the touching point should represent the _currentValue value,
            //_valueAtLowEdge must be set accordingly
            _valueAtLowEdge = _currentValue-(ratio*Range);
            _touching = true;
        }
        else
        {
            
            //not a "new" touch
            float newValue =  MathTools.Clamp(_valueAtLowEdge + (ratio*Range),MinValue,MaxValue);
            if (newValue!=_currentValue)
            {
                _currentValue=newValue;
                OnValueChanged.Invoke();
            }
        }
    }

    public void OnTouchStop()
    {
        _touching = false;
    }

    public float GetFloatValue(){return _currentValue;}
    public int GetIntValue(){return (int)Math.Round(_currentValue);}
    public void SetValue(float val){_currentValue = MathTools.Clamp(val, MinValue, MaxValue);}
    
}

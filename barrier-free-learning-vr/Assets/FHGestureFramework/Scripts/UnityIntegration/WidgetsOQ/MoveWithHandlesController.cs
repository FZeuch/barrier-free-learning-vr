using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeHandGestureFramework;
using FreeHandGestureUnity;
using FreeHandGestureFramework.EnumsAndTypes;

public class MoveWithHandlesController : MonoBehaviour
{
    public GameObject Go;
    public string ConnectedGesture;
    public int ConnectedStage;
    public FreeHandWidgetOQ Widget;
    private bool _leftHandTouching = false;
    private bool _rightHandTouching = false;
    private bool _moving = false;

    // Start is called before the first frame update
    void Start()
    {
        FreeHandRuntimeU fhr = FreeHandRuntimeU.Instance;
        GestureU connectedGesture=fhr?.GetGesture(ConnectedGesture);
        //TODO:if null, try to load gesture
        if (connectedGesture!=null)  
        {
            fhr.AddEventListener(GestureEventTypes.Holding, OnTouching);
            fhr.AddEventListener(GestureEventTypes.Released, OnRelease);
            Widget.ConnectedStages=new GestureStageU[1];
            Widget.ConnectedStages[0] = (GestureStageU)connectedGesture.Stages[ConnectedStage];      
        }
    }

    public void OnLeftTouchStart()
    {
        _leftHandTouching = true;
        if (_rightHandTouching) _moving = true;
    }
    public void OnRightTouchStart()
    {
        _rightHandTouching = true;
        if (_leftHandTouching) _moving = true;
    }
    public void OnTouching(object s, FreeHandEventArgs args)
    {
        if(_moving)
        {
            Vector3 leftPos = ConversionTools.Position3DToVector3(args.LeftHandPosition);
            Vector3 rightPos = ConversionTools.Position3DToVector3(args.RightHandPosition);
            Vector3 fromLeftHandToRightHand = rightPos-leftPos;
            float rotationAroundYAxis = -Vector3.SignedAngle(fromLeftHandToRightHand,Vector3.right,Vector3.up);
            Go.transform.eulerAngles = new Vector3(0,rotationAroundYAxis,0);
            Go.transform.position = Vector3.Lerp(leftPos, rightPos, .5f)+ new Vector3(0,0,.1f);//Point in the middle between hands
            //TODO: adjust center of mesh so that no adjusting by adding a vector3 is needed
        }
    }
    public void OnRelease(object s, FreeHandEventArgs args)
    {
        _moving = false;
    }
    public void OnLeftTouchStop()
    {
        _leftHandTouching = false;
    }
    public void OnRightTouchStop()
    {
        _rightHandTouching = false;
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;
using StandardTools;

public class FreeHandTouchButtonController : MonoBehaviour
{
    public const int WAIT_TIME_BETWEEN_PRESS = 500;
    public const int WAIT_TIME_BETWEEN_SAME_PRESS = 200;
    public const int WAIT_TIME_AFTER_ENABLE = 250;
    public Material Untouched_Material;
    public Material Touched_Material;
    public bool ToggleButton = false;
    private bool _isPressed = false;
    private static DateTime _anyButtonReleaseTime;// = DateTime.Now.AddMilliseconds(-WAIT_TIME_BETWEEN_PRESS);
    private static FreeHandTouchButtonController _lock = null;
    private DateTime _enableTime;
    private bool _touchOnEnable=false;
    private DateTime _thisButtonReleaseTime;
    private MeshRenderer _meshRenderer;
    public UnityEvent OnButtonPressed;
    public bool IsPressed
    {
        get{return _isPressed;}
        set
        {
            _isPressed = value;
            if (_isPressed) _meshRenderer.material = Touched_Material;
            else _meshRenderer.material = Untouched_Material;
        }
    }
    void Awake()
    {
        _meshRenderer = gameObject.GetComponent <MeshRenderer>();
        if (ToggleButton && IsPressed) _meshRenderer.material = Touched_Material;
        else _meshRenderer.material = Untouched_Material;
    }
    void OnEnable()
    {
        var now = DateTime.Now;
        _thisButtonReleaseTime = now;
        _anyButtonReleaseTime = now;
        _enableTime=now;
    }

    public void OnTouchStart()
    {
        var now = DateTime.Now;
        if(TimeTools.GetDifferenceInMilliseconds(now, _enableTime)<WAIT_TIME_AFTER_ENABLE) //The button was touched on enable - presumably unintentionally
        {
            _touchOnEnable = true;
            return;
        }
        bool lastPressWasThisInstance = _anyButtonReleaseTime.CompareTo(_thisButtonReleaseTime) == 0;
        if(_lock != null
            || (!lastPressWasThisInstance && TimeTools.GetDifferenceInMilliseconds(now, _anyButtonReleaseTime) < WAIT_TIME_BETWEEN_PRESS)
            || (lastPressWasThisInstance && TimeTools.GetDifferenceInMilliseconds(now, _thisButtonReleaseTime) < WAIT_TIME_BETWEEN_SAME_PRESS))
        {
            return;
        }
        else if(ToggleButton)
        {
            _lock = this;
            IsPressed = !_isPressed;
        }
        else
        {
            _lock = this;
            _meshRenderer.material = Touched_Material;
        }
    }
    public void OnTouchStop()
    {
        if (_touchOnEnable)
        { 
            _touchOnEnable = false;
        }
        else if (_lock == this)
        {
            var now = DateTime.Now;
            _thisButtonReleaseTime = now;
            _anyButtonReleaseTime = now;
            _lock = null;
            if (!ToggleButton) _meshRenderer.material = Untouched_Material;
            OnButtonPressed.Invoke();
        }
    }
}

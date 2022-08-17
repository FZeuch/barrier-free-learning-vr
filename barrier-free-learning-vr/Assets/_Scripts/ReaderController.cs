using UnityEngine;
using FreeHandGestureFramework.EnumsAndTypes;
using FreeHandGestureUnity;
using FreeHandGestureUnity.OculusQuest;
using SimpleWebBrowser;
using System;

public class ReaderController : MonoBehaviour
{
    private const int MAX_PAGE_TURNS_PER_SECOND = 15;
    public OVRSkeleton LeftSkeleton, RightSkeleton;
    public GameObject Cam;
    public GameObject ReaderAnchor;
    public GameObject SelectorAnchor;
    public SelectorController SelectorCont;
    public GameObject go1, go2;
    private WebBrowser _browser;
    private FreeHandRuntimeU _fhr;
    private GestureU[] _gestures = new GestureU[9];
    private DateTime _timeOfLastPageTurn;
    private Vector3 _readerOriginalScale;
    private int _rotationAxis = -1;
    void Awake()
    {
        ReaderAnchor.SetActive(true);
        FreeHandRuntimeU.SetPlatform(Platforms.OculusQuest);
        _fhr = FreeHandRuntimeU.Instance;
        if (_fhr==null) Debug.Log("FreeHandRuntimeU initialization failed.");
        InitGestureList();
        _readerOriginalScale=ReaderAnchor.transform.localScale;
    }
    void Update()
    {
        //call FreeHandRuntimuU.Update(), but only if hand tracking is ready.
        if ((LeftSkeleton.Bones != null && LeftSkeleton.Bones.Count > 0)
            || (RightSkeleton.Bones != null && RightSkeleton.Bones.Count > 0))
        {
            HandsUOQ currentHands = new HandsUOQ(LeftSkeleton, RightSkeleton);
            if (_fhr.IsActive())
            {
                _fhr.Update(currentHands, Cam.transform.forward);
            }
        }
    }
    public void ActivateReaderMode(string pdfFilename)
    {
        //The web browser plugin has problems with loading a new PDF from an already opened PDF. The solution for this is,
        //to instantiate a clone of the ReaderAnchor Game Object, destroy the old one, and use the fresh browser's
        //InitialURL attribute to show the PDF.
        GameObject newReaderAnchor = GameObject.Instantiate(ReaderAnchor, ReaderAnchor.transform.position, ReaderAnchor.transform.rotation);
        GameObject.Destroy(ReaderAnchor);
        ReaderAnchor = newReaderAnchor;
        SelectorCont.ReaderAnchor = newReaderAnchor;
        _browser = ReaderAnchor.GetComponentInChildren<WebBrowser>();
        _browser.InitialURL=GetPdfUrl(pdfFilename);

        SelectorAnchor.SetActive(false);
        SelectorCont.gameObject.SetActive(false);
        ReaderAnchor.SetActive(true);
        this.gameObject.SetActive(true);
        _fhr.StopCurrentGesture();
        _fhr.SetActive(false);
        _fhr.Gestures=_gestures;
        _fhr.SetActive(true);
    }
    private string GetPdfUrl(string filename, int page=1)
    {
        return "file:///"+Application.persistentDataPath+"/"+filename+"#page="+page+"&zoom=128&toolbar=0";
    }
    private void ShowPrevPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = _browser.GetMainEngine();
        engine.SendCharEvent(37,MessageLibrary.KeyboardEventType.Down); //37 = left arrow key
        engine.SendCharEvent(37,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowNextPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = _browser.GetMainEngine();
        engine.SendCharEvent(39,MessageLibrary.KeyboardEventType.Down); //39 = right arrow key
        engine.SendCharEvent(39,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowFirstPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = _browser.GetMainEngine();
        engine.SendCharEvent(36,MessageLibrary.KeyboardEventType.Down); //36 = Home key
        engine.SendCharEvent(36,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowLastPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = _browser.GetMainEngine();
        engine.SendCharEvent(35,MessageLibrary.KeyboardEventType.Down); //35 = End key
        engine.SendCharEvent(35,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void MoveReader(Vector3 relativeMovement)
    {
        ReaderAnchor.transform.Translate(relativeMovement, Space.Self);
    }
    private void RotateReader(Vector3 rotation)
    {
        ReaderAnchor.transform.Rotate(rotation, Space.Self);
    }

    private void ScaleReader(float multiplicator)
    {
        ReaderAnchor.transform.localScale = multiplicator * ReaderAnchor.transform.localScale;
    }
    private void RecenterView()
    {
        Vector3 lookDir = Cam.transform.forward;
        ReaderAnchor.transform.position=Cam.transform.position+lookDir;
        ReaderAnchor.transform.eulerAngles=new Vector3(0,Cam.transform.eulerAngles.y,0);
        ReaderAnchor.transform.localScale=_readerOriginalScale;
    }

    private void InitGestureList()
    {
        //For each gesture used in the reader: load it, add listeners and add it to the gesture list

        GestureU NextPage = FreeHandRuntimeU.GetPredefinedGesture("NextPage");
        NextPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowNextPageInPDF();});
        NextPage.Stages[2].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if(StandardTools.TimeTools.GetDifferenceInMilliseconds(DateTime.Now, _timeOfLastPageTurn) >= 1000/MAX_PAGE_TURNS_PER_SECOND)
                        ShowNextPageInPDF();
                });
        _gestures[0] = NextPage;

        GestureU PrevPage = FreeHandRuntimeU.GetPredefinedGesture("PrevPage");
        PrevPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowPrevPageInPDF();});
        PrevPage.Stages[2].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if(StandardTools.TimeTools.GetDifferenceInMilliseconds(DateTime.Now, _timeOfLastPageTurn) >= 1000/MAX_PAGE_TURNS_PER_SECOND)
                        ShowPrevPageInPDF();
                });
        _gestures[1] = PrevPage;

        GestureU GreetLeft = FreeHandRuntimeU.GetPredefinedGesture("GreetLeft");
        GreetLeft.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if (args.Path!=null && args.Path.HighestIndex >=1)
                    {
                        float readerDistance = Vector3.Distance(Cam.transform.position,ReaderAnchor.transform.position);
                        ReaderAnchor.transform.Translate(readerDistance*1.5f*ConversionTools.Position3DToVector3(args.Path.Path[args.Path.HighestIndex]-args.Path.Path[args.Path.HighestIndex-1]));
                    }   
                });
        _gestures[2] = GreetLeft;

        GestureU IndexDown = FreeHandRuntimeU.GetPredefinedGesture("IndexDown");
        IndexDown.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowLastPageInPDF();});
        _gestures[3] = IndexDown;

        GestureU IndexUp = FreeHandRuntimeU.GetPredefinedGesture("IndexUp");
        IndexUp.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowFirstPageInPDF();});
        _gestures[4] = IndexUp;

        GestureU IndexFingersForward = FreeHandRuntimeU.GetPredefinedGesture("IndexFingersFwd");
        IndexFingersForward.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {RecenterView();});
        _gestures[5] = IndexFingersForward;

        GestureU FishSize = FreeHandRuntimeU.GetPredefinedGesture("FishSize");
        FishSize.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    float readerDistance = Vector3.Distance(Cam.transform.position,ReaderAnchor.transform.position);
                    ReaderAnchor.transform.localScale = (5*readerDistance*args.DistanceBetweenHands)*_readerOriginalScale;
                });
        _gestures[6] = FishSize;

        GestureU Exit = FreeHandRuntimeU.GetPredefinedGesture("Exit");
        Exit.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {SelectorCont.ActivateSelectorMode();});
        _gestures[7] = Exit;

        GestureU Rotation = FreeHandRuntimeU.GetPredefinedGesture("Rotation");
        Rotation.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if (args.Path!=null && args.Path.HighestIndex >=1 && args.RightHandPosition != null)
                    {
                        Position3D lookDir = ConversionTools.Vector3ToPosition3D(Cam.transform.forward);
                        Ray xAxis = new Ray(ConversionTools.Position3DToVector3(args.Path.Path[0]), ConversionTools.Position3DToVector3(Position3D.GetRightDirection(lookDir)));
                        Ray yAxis = new Ray(ConversionTools.Position3DToVector3(args.Path.Path[0]), ConversionTools.Position3DToVector3(Position3D.GetUpDirection(lookDir)));
                        Ray zAxis = new Ray(ConversionTools.Position3DToVector3(args.Path.Path[0]), ConversionTools.Position3DToVector3(Position3D.GetForwardDirection(lookDir)));
                        Vector3 rhPos = ConversionTools.Position3DToVector3(args.RightHandPosition-args.Path.Path[0]);

                        float moveDistAlongX = Vector3.Project(rhPos, xAxis.direction).magnitude;
                        float moveDistAlongY = Vector3.Project(rhPos, yAxis.direction).magnitude;
                        float moveDistAlongZ = Vector3.Project(rhPos, zAxis.direction).magnitude;

                        //If rotationAxis is not yet set, determine the rotation axis by calculating along which 
                        //axis the user's hand is moving. The rotation axis will be used until the left hand pose is released.
                        if (_rotationAxis == -1)
                        {
                            float minDist = 0.03f;
                            if (moveDistAlongX > moveDistAlongY && moveDistAlongX > moveDistAlongZ && rhPos.magnitude >= minDist) _rotationAxis = 1; // hand moving along x axis -> rotation axis is y axis
                            else if (moveDistAlongY > moveDistAlongX && moveDistAlongY > moveDistAlongZ && rhPos.magnitude >= minDist) _rotationAxis = 2;
                            else if (moveDistAlongZ > moveDistAlongX && moveDistAlongZ > moveDistAlongY && rhPos.magnitude >= minDist) _rotationAxis = 0;
                        }

                        //If rotationAxis is set, rotate reader around that axis
                        else
                        {
                            float rotationDegree = 0;
                            int sign = 1; //determines the direction of the rotation (e.g. left or right)
                            switch(_rotationAxis)
                            {
                                case 0: //input for rotation around the x axis is hand movement along z axis
                                    if(Vector3.Dot(zAxis.direction, rhPos)>0) sign = -1;    //rot. direction can be calculated with scalar product
                                    rotationDegree = moveDistAlongZ * 360 * sign;           //1m of movement along z axis results in a 360 degree rotation
                                    //set new local rotation around x axis and leave y and z rotation untouched
                                    ReaderAnchor.transform.localEulerAngles = (new Vector3(rotationDegree,ReaderAnchor.transform.localEulerAngles.y,ReaderAnchor.transform.localEulerAngles.z));
                                    break;
                                case 1: //input for rotation around the y axis is hand movement along x axis
                                    if(Vector3.Dot(xAxis.direction, rhPos)>0) sign = -1;
                                    rotationDegree = moveDistAlongX * 360 *sign; 
                                    ReaderAnchor.transform.localEulerAngles = (new Vector3(ReaderAnchor.transform.localEulerAngles.x,rotationDegree,ReaderAnchor.transform.localEulerAngles.z));
                                    break;
                                case 2: //input for rotation around the z axis is hand movement along y axis
                                    if(Vector3.Dot(yAxis.direction, rhPos)>0) sign = -1;
                                    rotationDegree = moveDistAlongY * 360 * sign; 
                                    ReaderAnchor.transform.localEulerAngles = (new Vector3(ReaderAnchor.transform.localEulerAngles.x,ReaderAnchor.transform.localEulerAngles.y, rotationDegree));
                                    break;
                            }
                        }
                    }   
                });
        Rotation.Stages[0].AddEventListener(GestureEventTypes.Released,
                (object sender, FreeHandEventArgs args) => {_rotationAxis = -1;});

        _gestures[8] = Rotation;

    }

}

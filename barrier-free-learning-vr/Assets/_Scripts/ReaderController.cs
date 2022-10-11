using UnityEngine;
using FreeHandGestureFramework.DataTypes;
using FreeHandGestureUnity;
using FreeHandGestureUnity.OculusQuest;
using System;
using Paroxe.PdfRenderer;

//Controller for reading mode.
public class ReaderController : MonoBehaviour
{
    //Speed of fast page turning
    private const int MAX_PAGE_TURNS_PER_SECOND = 15;
    //The OVRSkeleton instances for left and right hand provided by Oculus plugin.
    //References must be set in Unity editor.
    public OVRSkeleton LeftSkeleton, RightSkeleton;
    //The anchor of the camera object.
    public GameObject Cam;
    //The anchor of the PDF reader, which is visible in reading mode.
    public GameObject ReaderAnchor;
    //The anchor of the selection tiles, that are visible in selection mode. 
    public GameObject SelectorAnchor;
    //The instance of the controller for selection mode.
    public SelectorController SelectorCont;
    //The PDFViewer instance.
    public PDFViewer Viewer;
    //The instance of the gesture handler.
    private GestureHandlerU _ghu;
    //The private gesture list with all gestures used in reading mode.
    private GestureU[] _gestures = new GestureU[9];
    //The time stamp of the last page turn.
    private DateTime _timeOfLastPageTurn;
    //The initial scale of the reader anchor.
    private Vector3 _readerOriginalScale;
    //The axis around which the reader is rotated.
    private int _rotationAxis = -1;
    void Awake()
    {
        //initialization of the reader controller

        //initialize gesture handler
        GestureHandlerU.SetPlatform(Platforms.OculusQuest);
        _ghu = GestureHandlerU.Instance;
        if (_ghu==null) Debug.Log("FreeHandRuntimeU initialization failed.");

        InitGestureList(); //load gestures used in reader mode
        _readerOriginalScale=ReaderAnchor.transform.localScale; //store original reader scale
    }
    void Update()
    {
        //called once per active frame

        //call FreeHandRuntimuU.Update(), but only if hand tracking is ready.
        if ((LeftSkeleton.Bones != null && LeftSkeleton.Bones.Count > 0)
            || (RightSkeleton.Bones != null && RightSkeleton.Bones.Count > 0))
        {
            HandsUOQ currentHands = new HandsUOQ(LeftSkeleton, RightSkeleton);
            if (_ghu.IsActive())
            {
                _ghu.Update(currentHands, Cam.transform.forward);
            }
        }
    }
    public void ActivateReaderMode(string pdfFilename)
    {
        //activates reader mode and deactivates selection  mode
    
        SelectorAnchor.SetActive(false); //hide selection tiles from selection mode
        ReaderAnchor.SetActive(true);    //show the PDF renderer
        Viewer.LoadDocumentFromFile(Application.persistentDataPath+"/"+pdfFilename); //load PDF
        _ghu.StopCurrentGesture();       //in case a gesture is still active

        //activate reader mode's gesture list
        _ghu.SetActive(false);
        _ghu.Gestures=_gestures;
        _ghu.SetActive(true);
    }
    private void ShowPrevPageInPDF()
    {
        Viewer.GoToPreviousPage();
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowNextPageInPDF()
    {
        Viewer.GoToNextPage();
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowFirstPageInPDF()
    {
        Viewer.GoToPage(0);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowLastPageInPDF()
    {
        Viewer.GoToPage(Viewer.Document.GetPageCount()-1);
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
        //Set position of reader anchor 1 unit in front of the camera
        Vector3 lookDir = Cam.transform.forward;
        ReaderAnchor.transform.position=Cam.transform.position+lookDir;
        //Set vertical rotation so that reader faces the camera.
        ReaderAnchor.transform.eulerAngles=new Vector3(0,Cam.transform.eulerAngles.y,0);//lookDir.y?
        //Set reader scale to original scale stored during initialization
        ReaderAnchor.transform.localScale=_readerOriginalScale;
    }

    private void InitGestureList()
    {
        //For each gesture used in the reader: load it, add listeners and add it to the private gesture list

        GestureU NextPage = GestureHandlerU.GetPredefinedGesture("NextPage");
        NextPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowNextPageInPDF();}); //turn on page forward
        NextPage.Stages[2].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    //fast page turning
                    if(StandardTools.TimeTools.GetDifferenceInMilliseconds(DateTime.Now, _timeOfLastPageTurn) >= 1000/MAX_PAGE_TURNS_PER_SECOND)
                        ShowNextPageInPDF();
                });
        _gestures[0] = NextPage;

        GestureU PrevPage = GestureHandlerU.GetPredefinedGesture("PrevPage");
        PrevPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowPrevPageInPDF();}); //turn one page back
        PrevPage.Stages[2].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    //fast page turning
                    if(StandardTools.TimeTools.GetDifferenceInMilliseconds(DateTime.Now, _timeOfLastPageTurn) >= 1000/MAX_PAGE_TURNS_PER_SECOND)
                        ShowPrevPageInPDF();
                });
        _gestures[1] = PrevPage;

        GestureU GreetLeft = GestureHandlerU.GetPredefinedGesture("GreetLeft");
        GreetLeft.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if (args.Path!=null && args.Path.HighestIndex >=1)
                    {
                        //while left hand holds pose, translate reader anchor according to right hand movement
                        float readerDistance = Vector3.Distance(Cam.transform.position,ReaderAnchor.transform.position);
                        ReaderAnchor.transform.Translate(readerDistance*1.5f*ConversionTools.Position3DToVector3(args.Path.Path[args.Path.HighestIndex]-args.Path.Path[args.Path.HighestIndex-1]));
                    }   
                });
        _gestures[2] = GreetLeft;

        GestureU IndexDown = GestureHandlerU.GetPredefinedGesture("IndexDown");
        IndexDown.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowLastPageInPDF();}); //show last page
        _gestures[3] = IndexDown;

        GestureU IndexUp = GestureHandlerU.GetPredefinedGesture("IndexUp");
        IndexUp.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowFirstPageInPDF();}); //show first page
        _gestures[4] = IndexUp;

        GestureU IndexFingersForward = GestureHandlerU.GetPredefinedGesture("IndexFingersFwd");
        IndexFingersForward.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {RecenterView();}); //recenter view
        _gestures[5] = IndexFingersForward;

        GestureU FishSize = GestureHandlerU.GetPredefinedGesture("FishSize");
        FishSize.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    //while holding both palms facing each other, use distance between hands to scale reader anchor
                    float readerDistance = Vector3.Distance(Cam.transform.position,ReaderAnchor.transform.position);
                    ReaderAnchor.transform.localScale = (5*readerDistance*args.DistanceBetweenHands)*_readerOriginalScale;
                });
        _gestures[6] = FishSize;

        GestureU Exit = GestureHandlerU.GetPredefinedGesture("Exit");
        Exit.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {SelectorCont.ActivateSelectorMode();}); //leave reader mode and enter selection mode
        _gestures[7] = Exit;

        GestureU Rotation = GestureHandlerU.GetPredefinedGesture("Rotation");
        Rotation.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    //While holding thumb and middle finger of the left hand together, rotate reader anchor.
                    if (args.Path!=null && args.Path.HighestIndex >=1 && args.RightHandPosition != null)
                    {
                        //Calculate direction vectors in looking direction
                        Position3D lookDir = ConversionTools.Vector3ToPosition3D(Cam.transform.forward);
                        Ray xAxis = new Ray(ConversionTools.Position3DToVector3(args.Path.Path[0]), ConversionTools.Position3DToVector3(Position3D.GetRightDirection(lookDir)));
                        Ray yAxis = new Ray(ConversionTools.Position3DToVector3(args.Path.Path[0]), ConversionTools.Position3DToVector3(Position3D.GetUpDirection(lookDir)));
                        Ray zAxis = new Ray(ConversionTools.Position3DToVector3(args.Path.Path[0]), ConversionTools.Position3DToVector3(Position3D.GetForwardDirection(lookDir)));
                        Vector3 rhPos = ConversionTools.Position3DToVector3(args.RightHandPosition-args.Path.Path[0]);

                        //Calculate the distance the right hand has moved since start along each axis
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
                (object sender, FreeHandEventArgs args) => {_rotationAxis = -1;}); //When left hand pose is released, reset rotation axis

        _gestures[8] = Rotation;

    }

}

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
    public WebBrowser Browser;
    //internal string PdfFilename;
    private FreeHandRuntimeU _fhr;
    private GestureU[] _gestures = new GestureU[7];
    private DateTime _timeOfLastPageTurn;
    private Vector3 _readerOriginalScale;
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
        Browser.InitialURL=GetPdfUrl(pdfFilename);
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
    private void LoadURL(string URL)
    {
        Debug.Log("GGGG "+URL);
            BrowserEngine engine = Browser.GetMainEngine();
            if (engine != null && engine.Initialized)
            {
                //for some strange reason, the NavigateEvent has to be sent twice in order to work properly
                engine.SendNavigateEvent(URL,false,false);
                engine.SendNavigateEvent(URL,false,false);
            }
    }
    private void ShowPrevPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(37,MessageLibrary.KeyboardEventType.Down); //37 = left arrow key
        engine.SendCharEvent(37,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowNextPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(39,MessageLibrary.KeyboardEventType.Down); //39 = right arrow key
        engine.SendCharEvent(39,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowFirstPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(36,MessageLibrary.KeyboardEventType.Down); //36 = Home key
        engine.SendCharEvent(36,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowLastPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
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
        //TODO: zurück-Geste dazu
    }

}
/*public class ReaderController : MonoBehaviour
{
    private const int MAX_PAGE_TURNS_PER_SECOND = 15;
    public OVRSkeleton LeftSkeleton, RightSkeleton;
    public GameObject Cam;
    public GameObject Star;
    public GameObject Reader;
    public WebBrowser Browser;
    private string _pdfFilename = "PDF/K10m.pdf";
    private DateTime _timeOfLastPageTurn;
    private FreeHandRuntimeU Fhr;
    private Vector3 _readerOriginalScale;
    void Awake()
    {
        SetBackgroundStars();
        Browser.InitialURL=GetPdfUrl(_pdfFilename,1);
        Reader.SetActive(true);
        FreeHandRuntimeU.SetPlatform(Platforms.OculusQuest);
        Fhr = FreeHandRuntimeU.Instance;
        if (Fhr==null) Debug.Log("FreeHandRuntimeU initialization failed.");
        InitGestureList();
        Fhr.SetActive(true);
        _readerOriginalScale=Reader.transform.localScale;
    }
    void Update()
    {
        //call FreeHandRuntimuU.Update(), but only if hand tracking is ready.
        if ((LeftSkeleton.Bones != null && LeftSkeleton.Bones.Count > 0)
            || (RightSkeleton.Bones != null && RightSkeleton.Bones.Count > 0))
        {
            HandsUOQ currentHands = new HandsUOQ(LeftSkeleton, RightSkeleton);
            if (Fhr.IsActive())
            {
                Fhr.Update(currentHands, Cam.transform.forward);
            }
        }
    }
    private void SetBackgroundStars()
    {
            int starCount = 300;
            float minDist = 10;
            float maxDist = 20;
            Position3D rndVec = new Position3D();
            for (int i=0; i<starCount; i++)
            {
                do
                {
                    rndVec.X=UnityEngine.Random.Range(-maxDist, maxDist);
                    rndVec.Y=UnityEngine.Random.Range(.5f, maxDist);
                    rndVec.Z=UnityEngine.Random.Range(-maxDist, maxDist);
                } while(rndVec.VectorLength() < minDist);
                var go = GameObject.Instantiate(Star, 
                            ConversionTools.Position3DToVector3(rndVec),
                            Quaternion.identity);
                go.SetActive(true);
            }
    }
    private string GetPdfUrl(string filename, int page)
    {
            return "file:///"+Application.persistentDataPath+"/"+filename+"#page="+page+"&zoom=128&toolbar=0";
    }
    private void LoadURL(string URL)
    {
            BrowserEngine engine = Browser.GetMainEngine();
            if (engine != null && engine.Initialized)
            {
                //for some strange reason, the NavigateEvent has to be sent twice in order to work properly
                engine.SendNavigateEvent(URL,false,false);
                engine.SendNavigateEvent(URL,false,false);
            }
    }
    private void ShowPrevPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(37,MessageLibrary.KeyboardEventType.Down); //37 = left arrow key
        engine.SendCharEvent(37,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowNextPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(39,MessageLibrary.KeyboardEventType.Down); //39 = right arrow key
        engine.SendCharEvent(39,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowFirstPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(36,MessageLibrary.KeyboardEventType.Down); //36 = Home key
        engine.SendCharEvent(36,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void ShowLastPageInPDF()
    {
        //using browser key codes, see https://unixpapa.com/js/key.html
        BrowserEngine engine = Browser.GetMainEngine();
        engine.SendCharEvent(35,MessageLibrary.KeyboardEventType.Down); //35 = End key
        engine.SendCharEvent(35,MessageLibrary.KeyboardEventType.Up);
        _timeOfLastPageTurn = DateTime.Now;
    }
    private void MoveReader(Vector3 relativeMovement)
    {
        Reader.transform.Translate(relativeMovement, Space.Self);
    }
    private void RotateReader(Vector3 rotation)
    {
        Reader.transform.Rotate(rotation, Space.Self);
    }

    private void ScaleReader(float multiplicator)
    {
        Reader.transform.localScale = multiplicator * Reader.transform.localScale;
    }
    private void RecenterView()
    {
        Vector3 lookDir = Cam.transform.forward;
        Reader.transform.position=Cam.transform.position+lookDir;
        Reader.transform.eulerAngles=new Vector3(0,Cam.transform.eulerAngles.y,0);
        Reader.transform.localScale=_readerOriginalScale;
    }

    private void InitGestureList()
    {
        GestureU NextPage = FreeHandRuntimeU.GetPredefinedGesture("NextPage");
        NextPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowNextPageInPDF();});
        NextPage.Stages[2].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if(StandardTools.TimeTools.GetDifferenceInMilliseconds(DateTime.Now, _timeOfLastPageTurn) >= 1000/MAX_PAGE_TURNS_PER_SECOND)
                        ShowNextPageInPDF();
                });
        Fhr.AddGesture(NextPage);

        GestureU PrevPage = FreeHandRuntimeU.GetPredefinedGesture("PrevPage");
        PrevPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowPrevPageInPDF();});
        PrevPage.Stages[2].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if(StandardTools.TimeTools.GetDifferenceInMilliseconds(DateTime.Now, _timeOfLastPageTurn) >= 1000/MAX_PAGE_TURNS_PER_SECOND)
                        ShowPrevPageInPDF();
                });
        Fhr.AddGesture(PrevPage);

        GestureU GreetLeft = FreeHandRuntimeU.GetPredefinedGesture("GreetLeft");
        GreetLeft.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    if (args.Path!=null && args.Path.HighestIndex >=1)
                    {
                        float readerDistance = Vector3.Distance(Cam.transform.position,Reader.transform.position);
                        Reader.transform.Translate(readerDistance*1.5f*ConversionTools.Position3DToVector3(args.Path.Path[args.Path.HighestIndex]-args.Path.Path[args.Path.HighestIndex-1]));
                    }   
                });
        Fhr.AddGesture(GreetLeft);

        GestureU IndexDown = FreeHandRuntimeU.GetPredefinedGesture("IndexDown");
        IndexDown.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowLastPageInPDF();});
        Fhr.AddGesture(IndexDown);

        GestureU IndexUp = FreeHandRuntimeU.GetPredefinedGesture("IndexUp");
        IndexUp.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {ShowFirstPageInPDF();});
        Fhr.AddGesture(IndexUp);

        GestureU IndexFingersForward = FreeHandRuntimeU.GetPredefinedGesture("IndexFingersFwd");
        IndexFingersForward.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {RecenterView();});
        Fhr.AddGesture(IndexFingersForward);

        GestureU FishSize = FreeHandRuntimeU.GetPredefinedGesture("FishSize");
        FishSize.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    float readerDistance = Vector3.Distance(Cam.transform.position,Reader.transform.position);
                    Reader.transform.localScale = (5*readerDistance*args.DistanceBetweenHands)*_readerOriginalScale;
                });
        Fhr.AddGesture(FishSize);
    }

}*/

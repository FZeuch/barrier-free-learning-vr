using UnityEngine;
using FreeHandGestureFramework.EnumsAndTypes;
using FreeHandGestureUnity;
using FreeHandGestureUnity.OculusQuest;
using SimpleWebBrowser;
using System;

public class AppController : MonoBehaviour
{
    private const int MAX_PAGE_TURNS_PER_SECOND = 15;
    public OVRSkeleton LeftSkeleton, RightSkeleton;
    public GameObject Cam;
    public GameObject Star;
    public GameObject Reader;
    public WebBrowser Browser;
    private int _currentPage=1;
    private string _pdfFilename = "PDF/K10m.pdf";
    private DateTime _timeOfLastPageTurn;
    private FreeHandRuntimeU Fhr;
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
    }
    void Update()
    {
        //call FreeHandRuntimuU.Update(), but only if hand tracking is ready.
        if ((LeftSkeleton.Bones != null && LeftSkeleton.Bones.Count > 0)
            || (RightSkeleton.Bones != null && RightSkeleton.Bones.Count > 0))
        {
            HandsUOQ2 currentHands = new HandsUOQ2(LeftSkeleton, RightSkeleton);
            if (Fhr.IsActive())
            {
                Fhr.Update(currentHands, Cam.transform.forward);
            }
        }

        /*if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(_currentPage>1) 
            {
                _currentPage--;
                ShowPrevPageInPDF();
            }
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentPage++;
            ShowNextPageInPDF();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            _currentPage=1;
            ShowFirstPageInPDF();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            MoveReader(new Vector3(0,-.05f,0));
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            MoveReader(new Vector3(0,.05f,0));
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            MoveReader(new Vector3(-.05f,0,0));
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            MoveReader(new Vector3(.05f,00));
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            MoveReader(new Vector3(0,0,.05f));
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            MoveReader(new Vector3(0,0,-.05f));
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            RotateReader(new Vector3(0,5,0));
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            RotateReader(new Vector3(0,-5,0));
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            RotateReader(new Vector3(5,0,0));
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            RotateReader(new Vector3(-5,0,0));
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            ScaleReader(1.1f);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            ScaleReader(.9f);
        }*/


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
    }

}

using UnityEngine;
using FreeHandGestureFramework.EnumsAndTypes;
using FreeHandGestureUnity;
using SimpleWebBrowser;

public class AppController : MonoBehaviour
{
    public GameObject Star;
    public GameObject Reader;
    public WebBrowser Browser;
    private int _currentPage=1;
    private string PDFfilename = "PDF/K10m.pdf";

    void Awake()
    {
        SetBackgroundStars();
        Browser.InitialURL=GetPdfUrl(PDFfilename,1);
        Reader.active=true;
    }
    void Start()
    {
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(_currentPage>1) 
            {
                _currentPage--;
                LoadURL(GetPdfUrl(PDFfilename,_currentPage));
            }
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentPage++;
            LoadURL(GetPdfUrl(PDFfilename,_currentPage));
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
                    rndVec.X=Random.Range(-maxDist, maxDist);
                    rndVec.Y=Random.Range(.5f, maxDist);
                    rndVec.Z=Random.Range(-maxDist, maxDist);
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
}

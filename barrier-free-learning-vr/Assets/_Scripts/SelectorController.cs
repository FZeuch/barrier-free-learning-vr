using UnityEngine;
using FreeHandGestureFramework.EnumsAndTypes;
using FreeHandGestureUnity;
using FreeHandGestureUnity.OculusQuest;
using SimpleWebBrowser;
using System;
using System.IO;

public class SelectorController : MonoBehaviour
{
    public OVRSkeleton LeftSkeleton, RightSkeleton;
    public GameObject Cam;
    public GameObject Star;
    public GameObject ReaderAnchor;
    public GameObject SelectorAnchor;
    public TMPro.TMP_Text[] Filenames;
    public GameObject[] Selectors;
    public GameObject[] Dwellers;
    public LineRenderer Ray;
    public ReaderController ReaderCont;
    private FreeHandRuntimeU _fhr;
    private GestureU[] _gestures = new GestureU[3];
    private Vector3 _dwellerOriginalScale;
    private int _dwellingIndex = -1;
    private DateTime _dwellStart;
    private int _dwellTime = 2000;
    private int _currentPage = 0;
    private int _numPages = 1;
    private const float _PAGEWIDTH = 6.5f;

    void Awake()
    {
        SetBackgroundStars();
        FreeHandRuntimeU.SetPlatform(Platforms.OculusQuest);
        _fhr = FreeHandRuntimeU.Instance;
        if (_fhr==null) Debug.Log("FreeHandRuntimeU initialization failed.");
        InitGestureList();
        InitSelectors();
        ActivateSelectorMode();
        _dwellerOriginalScale=Dwellers[0].transform.localScale;
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
    public void ActivateSelectorMode()
    {
        ReaderAnchor.SetActive(false);
        ReaderCont.gameObject.SetActive(false);
        SelectorAnchor.SetActive(true);
        this.gameObject.SetActive(true);
        Ray.transform.gameObject.SetActive(false);
        _fhr.StopCurrentGesture();
        _fhr.SetActive(false);
        _fhr.Gestures=_gestures;
        _fhr.SetActive(true);
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
    private void InitGestureList()
    {
        GestureU NextPage = FreeHandRuntimeU.GetPredefinedGesture("NextPage");
        NextPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => 
                {
                    if (_currentPage < _numPages-1) 
                    {
                        _currentPage++;
                        for (int i=0; i < Selectors.Length; i++)
                        {
                            Selectors[i].transform.Translate(new Vector3(-_PAGEWIDTH,0,0),Space.World);
                            Dwellers[i].transform.Translate(new Vector3(-_PAGEWIDTH,0,0),Space.World);
                        }
                    }
                });
        _gestures[0]=NextPage;

        GestureU PrevPage = FreeHandRuntimeU.GetPredefinedGesture("PrevPage");
        PrevPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => 
                {
                    if (_currentPage > 0) 
                    {
                        _currentPage--;
                        for (int i=0; i < Selectors.Length; i++)
                        {
                            Selectors[i].transform.Translate(new Vector3(_PAGEWIDTH,0,0),Space.World);
                            Dwellers[i].transform.Translate(new Vector3(_PAGEWIDTH,0,0),Space.World);
                        }
                    }
                });
        _gestures[1]=PrevPage;

        GestureU FingerPointer = FreeHandRuntimeU.GetPredefinedGesture("PointWithIndexFinger");
        FingerPointer.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    Ray.transform.gameObject.SetActive(true);
                    var bones = FingerPointer.Stages[0].ManipulationHand == RelevantHand.Left ? LeftSkeleton.Bones : RightSkeleton.Bones;
                    Ray.SetPositions(new Vector3[]{bones[6].Transform.position, 
                                    bones[6].Transform.position+(bones[20].Transform.position-bones[6].Transform.position)*30});
                    RaycastHit hitInfo;
                    if (Physics.Raycast(bones[6].Transform.position, 
                                    bones[20].Transform.position-bones[6].Transform.position, 
                                    out hitInfo))
                    {
                        bool hitDetected = false;
                        for (int i = 0; i<Selectors.Length; i++)
                        {
                            if (hitInfo.transform.gameObject == Selectors[i] ) 
                            {
                                hitDetected = true;
                                if (_dwellingIndex != i)
                                {
                                     _dwellStart = DateTime.Now;
                                     _dwellingIndex = i;
                                }
                                else if (StandardTools.TimeTools.GetDifferenceInMilliseconds(_dwellStart, DateTime.Now) >= _dwellTime)                                     
                                {
                                    ReaderCont.ActivateReaderMode("pdf/"+Filenames[i].text);
                                }
                                else
                                {
                                    float dwellerSize = (float)StandardTools.TimeTools.GetDifferenceInMilliseconds(_dwellStart, DateTime.Now) / (float)_dwellTime;
                                    Dwellers[i].transform.localScale = new Vector3(dwellerSize,dwellerSize,dwellerSize);
                                    Dwellers[i].SetActive(true);
                                }
                            }
                            else Dwellers[i].SetActive(false);
                        }
                        if (!hitDetected) //raycast hit, but with none of the Selectors
                        {
                            if (_dwellingIndex >= 0) Dwellers[_dwellingIndex].SetActive(false);
                            _dwellingIndex = -1;
                        }
                    }
                    else//no raycast hit
                    {
                        if (_dwellingIndex >= 0) Dwellers[_dwellingIndex].SetActive(false);
                        _dwellingIndex = -1;
                    }
                });
        FingerPointer.Stages[0].AddEventListener(GestureEventTypes.Released,
                (object sender, FreeHandEventArgs args) => 
                {
                    Ray.transform.gameObject.SetActive(false);
                    if (_dwellingIndex >= 0) Dwellers[_dwellingIndex].SetActive(false);
                    _dwellingIndex = -1;
                });
        _gestures[2]=FingerPointer;

        //TODO:Abbrechen-Geste hinzufügen
    }
    private void InitSelectors()
    {
        //Collect PDF files
        string[] pdfs = Directory.GetFiles(Application.persistentDataPath+"/pdf","*.pdf", SearchOption.TopDirectoryOnly);

        //If number of pdfs > 8, create new page(s)
        _numPages = (int)Math.Ceiling((double)pdfs.Length/(double)Filenames.Length);
        if (_numPages > 1)
        {
            int itemsPerPage = Filenames.Length;
            TMPro.TMP_Text[] FilenamesExtended = new TMPro.TMP_Text[_numPages*itemsPerPage];
            GameObject[] SelectorsExtended = new GameObject[_numPages*itemsPerPage];
            GameObject[] DwellersExtended = new GameObject[_numPages*itemsPerPage];
            //copy existing items
            for (int k = 0; k < itemsPerPage; k++)
            {
                SelectorsExtended[k] = Selectors[k];
                DwellersExtended[k] = Dwellers[k];
                FilenamesExtended[k] = Filenames[k];
            }
            //create new items for other pages
            for (int i = 1; i < _numPages; i++)    
            {
                for (int j = 0; j < itemsPerPage; j++)
                {
                    Vector3 pos = new Vector3(Selectors[j].transform.position.x+(i*_PAGEWIDTH), Selectors[j].transform.position.y, Selectors[j].transform.position.z);
                    SelectorsExtended[(i*itemsPerPage)+j] = GameObject.Instantiate(Selectors[j], pos, Quaternion.identity, SelectorAnchor.transform); 
                    DwellersExtended[(i*itemsPerPage)+j] = GameObject.Instantiate(Dwellers[j], pos, Quaternion.identity, SelectorAnchor.transform);
                    FilenamesExtended[(i*itemsPerPage)+j] = SelectorsExtended[(i*itemsPerPage)+j].GetComponentInChildren<TMPro.TMP_Text>();
                }
            }
            //exchange original arrays with extended arrays
            Selectors = SelectorsExtended;
            Dwellers = DwellersExtended;
            Filenames = FilenamesExtended;
        }

        for(int i = 0; i < Filenames.Length; i++)
        {
            if (i < pdfs.Length) Filenames[i].text = Path.GetFileName(pdfs[i]);
            else Filenames[i].text = "";
        }
    }

}

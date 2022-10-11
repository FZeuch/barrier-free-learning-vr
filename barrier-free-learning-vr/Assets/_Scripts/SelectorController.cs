using UnityEngine;
using FreeHandGestureFramework.DataTypes;
using FreeHandGestureUnity;
using FreeHandGestureUnity.OculusQuest;
using System;
using System.IO;

//Controller for selection mode.
public class SelectorController : MonoBehaviour
{
    //The OVRSkeleton instances for left and right hand provided by Oculus plugin.
    //References must be set in Unity editor.
    public OVRSkeleton LeftSkeleton, RightSkeleton;
    //The anchor of the camera object.
    public GameObject Cam;
    //A single star, used to instantiate stars in starfield.
    public GameObject Star;
    //The anchor of the selection tiles, that are visible in selection mode. 
    public GameObject SelectorAnchor;
    //The anchor of the PDF reader, which is visible in reading mode.
    public GameObject ReaderAnchor;
    //Text Mesh Pro text fields to display file names.
    public TMPro.TMP_Text[] Filenames;
    //Selection cubes
    public GameObject[] Selectors;
    //Cubes inside selecton cubes that dwell when user hits with ray.
    public GameObject[] Dwellers;
    //The ray used to select a cube while stretching out the index finger.
    public LineRenderer Ray;
    //The instance of the controller for the reading mode.
    public ReaderController ReaderCont;
    //The instance of the desture handler.
    private GestureHandlerU _ghu;
    //The private gesture list with all gestures used in selection mode.
    private GestureU[] _gestures = new GestureU[4];
    //The original scale of a dweller cube.
    private Vector3 _dwellerOriginalScale;
    //The index of the selection cube currently dwelling, -1 if none is dwelling.
    private int _dwellingIndex = -1;
    //The time the ray had hit the selection cube currently dwelling.
    private DateTime _dwellStart;
    //The dwell time that confirms a selection.
    private int _dwellTime = 2000;
    //The selection cube "page" currently displayed.
    private int _currentPage = 0;
    //The number of selection cube "pages". One page holds 8 selecton cubes.
    private int _numPages = 1;
    //The width of a "page" with 8 selection cubes in unity units (=meters).
    private const float _PAGEWIDTH = 6.5f;

    void Awake()
    {
        //initialization of the selector controller
        SetBackgroundStars(); //initialize starfield

        //initialize gesture handler
        GestureHandlerU.SetPlatform(Platforms.OculusQuest);
        _ghu = GestureHandlerU.Instance; 
        if (_ghu==null) Debug.Log("FreeHandRuntimeU initialization failed.");

        InitGestureList(); //load gestures used in selection mode
        InitSelectors();   //initialize selection cubes
        ActivateSelectorMode();
        _dwellerOriginalScale=Dwellers[0].transform.localScale; //store original dweller cube scales
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
    public void ActivateSelectorMode()
    {
        //activates reader mode and deactivates selection  mode
    
        ReaderAnchor.SetActive(false);   //hide PDF reader
        SelectorAnchor.SetActive(true);  //show selection cubes
        Ray.transform.gameObject.SetActive(false); //The ray used while pointing index finger must be deactivated.
        _ghu.StopCurrentGesture();       //in case a gesture is still active
        _ghu.SetActive(false);

        //activate selection mode's gesture list
        _ghu.Gestures=_gestures;
        _ghu.SetActive(true);

        _dwellingIndex = -1; //No selection is performed. 
    }
    private void SetBackgroundStars()
    {
        //Create a starfield in order to give user some orientation
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
        //For each gesture used in the reader: load it, add listeners and add it to the private gesture list

        GestureU NextPage = GestureHandlerU.GetPredefinedGesture("NextPage");
        NextPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => 
                {
                    //Scroll one "page" of 8 selection cubes to the left
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

        GestureU PrevPage = GestureHandlerU.GetPredefinedGesture("PrevPage");
        PrevPage.Stages[1].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => 
                {
                    //Scroll one "page" of 8 selection cubes to the right
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

        GestureU FingerPointer = GestureHandlerU.GetPredefinedGesture("PointWithIndexFinger");
        FingerPointer.Stages[0].AddEventListener(GestureEventTypes.Holding,
                (object sender, FreeHandEventArgs args) => 
                {
                    //Show a red ray while pointing with index finger
                    Ray.transform.gameObject.SetActive(true);
                    var bones = FingerPointer.Stages[0].ManipulationHand == RelevantHand.Left ? LeftSkeleton.Bones : RightSkeleton.Bones;
                    Ray.SetPositions(new Vector3[]{bones[6].Transform.position, 
                                    bones[6].Transform.position+(bones[20].Transform.position-bones[6].Transform.position)*30});

                    //Check if ray hits a selection cube
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
                                //A selector cube was hit by ray.
                                hitDetected = true;
                                if (_dwellingIndex != i)
                                {
                                    //First hit, initialize dwelling.
                                     _dwellStart = DateTime.Now;
                                     _dwellingIndex = i;
                                }
                                else if (StandardTools.TimeTools.GetDifferenceInMilliseconds(_dwellStart, DateTime.Now) >= _dwellTime)                                     
                                {
                                    //The selection cube was hit by the ray for longer than the dwell time.
                                    //This confirms the selection, so reader mode can be activated.
                                    ReaderCont.ActivateReaderMode("pdf/"+Filenames[i].text);
                                }
                                else
                                {
                                    //Dwelling, set size of the selection cube's dweller cube.
                                    float dwellerSize = (float)StandardTools.TimeTools.GetDifferenceInMilliseconds(_dwellStart, DateTime.Now) / (float)_dwellTime;
                                    Dwellers[i].transform.localScale = new Vector3(dwellerSize,dwellerSize,dwellerSize);
                                    Dwellers[i].SetActive(true);
                                }
                            }
                            else Dwellers[i].SetActive(false); //Something was hit, but not this selector cube. Deactivate dweller.
                        }
                        if (!hitDetected) //raycast hit, but with none of the Selectors
                        {
                            if (_dwellingIndex >= 0) Dwellers[_dwellingIndex].SetActive(false); //deactivate dweller
                            _dwellingIndex = -1;
                        }
                    }
                    else//no raycast hit
                    {
                        if (_dwellingIndex >= 0) Dwellers[_dwellingIndex].SetActive(false); //deactivate dweller
                        _dwellingIndex = -1;
                    }
                });
        FingerPointer.Stages[0].AddEventListener(GestureEventTypes.Released,
                (object sender, FreeHandEventArgs args) => 
                {
                    //Deactivate ray and reset dwelling system if the hand pose is released.
                    Ray.transform.gameObject.SetActive(false);
                    if (_dwellingIndex >= 0) Dwellers[_dwellingIndex].SetActive(false);
                    _dwellingIndex = -1;
                });
        _gestures[2]=FingerPointer;

        GestureU Exit = GestureHandlerU.GetPredefinedGesture("Exit");
        Exit.Stages[0].AddEventListener(GestureEventTypes.Start,
                (object sender, FreeHandEventArgs args) => {Application.Quit();}); //Quit the application
        _gestures[3] = Exit;
    }
    private void InitSelectors()
    {
        //Initialize the controller cubes.

        //Check if PDF path is present. Create path if necessary.
        string pdfPath = Application.persistentDataPath+"/pdf";
        if (!Directory.Exists(pdfPath)) Directory.CreateDirectory(pdfPath);

        //Collect PDF files
        string[] pdfs = Directory.GetFiles(Application.persistentDataPath+"/pdf","*.pdf", SearchOption.TopDirectoryOnly);

        //If number of pdfs > 8, create new page(s) (one page of 8 already exists)
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

        //Set filenames on the Text Mesh Pro panels located in front of the cubes.
        for(int i = 0; i < Filenames.Length; i++)
        {
            if (i < pdfs.Length) Filenames[i].text = Path.GetFileName(pdfs[i]);
            else Filenames[i].text = "";
        }
    }

}

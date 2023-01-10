using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region variables

    [SerializeField]
    Vector3 firstSlotPosition = new Vector3();

    [SerializeField]
    int currentLine = 0;
     public int CurrentLine { get { return currentLine;  } }

    [SerializeField]
    int currentSlot = -1;
    public int CurrentSlot { get { return currentSlot; } }

    [Serializable]
    public class BoardData
    {
        public List<LineData> linesList = new List<LineData>();
        public GameObject boardGameObject;
        public GameObject OkButtonGameObject;

    }
    public BoardData board = new BoardData();

    [Serializable]
    public class IALineData
    {
        public bool Hidden = true;
        public List<SlotData> slotsList = new List<SlotData>();
        public List<Color> AssignedColorsList = new List<Color>();
        public GameObject iALineGameObject;
    }
    public IALineData iALine = new IALineData();

    [Serializable]
    public class RulesData
    {
        public int slotsByLine = 4;
        public int linesByBoard = 12;

        public bool sameColorByLine = true;
    }
    public RulesData rules = new RulesData();

    [Serializable]
    public class PrefabsData
    {
        public GameObject slotPrefab;
        public GameObject slotX4Prefab;
        public GameObject colorChoicePrefab;
        public GameObject OkButtonPrefab;
    }
    public PrefabsData prefabs = new PrefabsData();

    [Serializable]
    public class ColorsData
    {
        public int maxColors = 6;
        public int selectedColorIndex = -1;
        public int PreviousSelectedColorIndex = -1;
        public List<Material> ColorsList = new List<Material>();
        public List<AvailableColorSlot> GeneratedColorsObjectsList = new List<AvailableColorSlot>();
        public Material DefaultBoardColor;
        public Material SelectedLineColor;
        public Material WhiteColor;
        public Material BlackColor;

    }
    public ColorsData Colors = new ColorsData();

    #region Events
    //Version C# on relie l'appel du delegate à l'event. 
    // La définition du delegate va déterminer quelles informations seront disponibles. 
    // ATTENTION la signature du delegate doit être respectée par les fonctions qui écoutent.
    public delegate void MessageEvent();
    public static event MessageEvent WinGame;
    public static event MessageEvent LooseGame;

    #endregion Events

    #endregion variables

    #region Init
    // Start is called before the first frame update
    void Start()
    {
        GlobalVariables.boardManager = this;

        GenerateBoard();
        GenerateIABoard();

        GenerateColorsChoicesBoard();

        GenerateCode();

        SetCurrentLineSelected();
    }

    void OnEnable()
    {
        //C# : on inscrit la fonction LooseMessage à l'event OnLoose.
        //grace à la propriété static nous devons juste trouver la classe Board
    //    ValidateLineButton.ValidateLine += VerifyLine;
    }

    #endregion Init

    #region Generate / Destroy Board
    void GenerateBoard()
    {
        // create virtual board
        board.boardGameObject = new GameObject();
        board.boardGameObject.name = "board";
        board.boardGameObject.transform.SetParent(transform);

        for (int iLine = 0; iLine < rules.linesByBoard; iLine++)
        {            
            // create a line
            GameObject newlineObj = new GameObject();
            newlineObj.transform.SetParent(board.boardGameObject.transform);
            newlineObj.name = "line "+ iLine;
            LineData newline = newlineObj.AddComponent<LineData>();

            board.linesList.Add(newline);

            for (int iSlot = 0; iSlot < rules.slotsByLine; iSlot++)
            {
              
                Vector3 newPosition = new Vector3(firstSlotPosition.x+iSlot, firstSlotPosition.y+iLine, firstSlotPosition.z);

                // create object in scene
                GameObject newSlotObj = (GameObject)Instantiate(prefabs.slotPrefab, newPosition, prefabs.slotPrefab.transform.rotation,
                     newlineObj.transform);
                SlotData newSlot = newSlotObj.GetComponent<SlotData>();
                newSlotObj.name = "Ball Slot " + iSlot;

             //   newSlot.ballSlot = newSlot.slotGameObject.GetComponent<BallSlot>();
                newSlot.ballIndex = iSlot;

                // add slot to the list
                board.linesList[iLine].slotsList.Add(newSlot);

                // set color
                AssignDefaultBoardColor(newSlot.GetComponent<MeshRenderer>());
                AssignDefaultBoardColor(newSlot.selectedCircle.GetComponent<MeshRenderer>());
                AssignDefaultBoardColor(newSlot.bar.GetComponent<MeshRenderer>());

                // hide ball
                newSlot.ballGameObject.SetActive(false);

                // generate MiniSlotsX
                if (iSlot == rules.slotsByLine-1)
                {
                   // prepare position
                    Vector3 newPosition2 = new Vector3(firstSlotPosition.x + (iSlot + 1), firstSlotPosition.y + iLine, firstSlotPosition.z);

                    // instantiate object
                    GameObject slotGameObject = (GameObject)Instantiate(prefabs.slotX4Prefab, newPosition2, 
                        prefabs.slotX4Prefab.transform.rotation,
                       newlineObj.transform);
                    slotGameObject.name = "MiniSlotsX";

                    newline.miniSlotsXManager = slotGameObject.GetComponent<MiniSlotsXManager>();

                    // set color
                    AssignDefaultBoardColor(newline.miniSlotsXManager.bar.GetComponent<MeshRenderer>());
                    AssignDefaultBoardColor(newline.miniSlotsXManager.transform.GetComponent<MeshRenderer>());


                    for (int iminiSlot = 0; iminiSlot < newline.miniSlotsXManager.MiniBallSlotsList.Count; iminiSlot++)
                    {
                        newline.miniSlotsXManager.MiniBallSlotsList[iminiSlot].selectedCircle.transform.GetComponent<MeshRenderer>().material 
                            = Colors.DefaultBoardColor;

                        newline.miniSlotsXManager.MiniBallSlotsList[iminiSlot].ballGameObject.SetActive(false);
                    }

                    // show object
                    slotGameObject.SetActive(true);
                }
            }
        }
        PrepareOkButton();
    }

    void GenerateIABoard()
    {
        // create virtual board
        iALine.iALineGameObject = new GameObject();
        iALine.iALineGameObject.name = "IA Line";
        iALine.iALineGameObject.transform.SetParent(transform);

        HideIABoard();

        for (int iSlot = 0; iSlot < rules.slotsByLine; iSlot++)
        {          
            // prepare position
            Vector3 newPosition = new Vector3(firstSlotPosition.x + iSlot, firstSlotPosition.y + (rules.linesByBoard+1), firstSlotPosition.z);

            // create object in scene
            GameObject newSlotObj = (GameObject)Instantiate(prefabs.slotPrefab, newPosition, prefabs.slotPrefab.transform.rotation,
                iALine.iALineGameObject.transform);

            SlotData newSlot = newSlotObj.GetComponent<SlotData>();

            // set color
            AssignDefaultBoardColor(newSlot.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(newSlot.selectedCircle.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(newSlot.bar.GetComponent<MeshRenderer>());

            iALine.slotsList.Add(newSlot);
        }
    }

    void GenerateColorsChoicesBoard()
    {
        GameObject AvailableColors = new GameObject();
        AvailableColors.transform.SetParent(board.boardGameObject.transform);
        AvailableColors.name = "Available Colors";

        for (int iSlot = 0; iSlot < Colors.ColorsList.Count; iSlot++)
        {
            // prepare position
            Vector3 newPosition = new Vector3(firstSlotPosition.x -2, firstSlotPosition.y + iSlot, firstSlotPosition.z);

            // create object in scene
            GameObject slot = (GameObject)Instantiate(prefabs.colorChoicePrefab, newPosition, prefabs.colorChoicePrefab.transform.rotation,
                AvailableColors.transform);

            AvailableColorSlot availableColorSlot = slot.GetComponent<AvailableColorSlot>();
            availableColorSlot.slot.name = Colors.ColorsList[iSlot].name;
            availableColorSlot.colorIndex = iSlot;

            // set color
            AssignDefaultBoardColor(availableColorSlot.slot.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(availableColorSlot.bar1.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(availableColorSlot.bar2.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(availableColorSlot.selectedCircle.GetComponent<MeshRenderer>());

            // set color for the ball
            availableColorSlot.ballGameObject.GetComponent<Renderer>().material = Colors.ColorsList[iSlot];

            Colors.GeneratedColorsObjectsList.Add(availableColorSlot);
        }
    }

    public void AssignDefaultBoardColor(MeshRenderer renderer)
    {
        renderer.material = Colors.DefaultBoardColor;
    }
    public void AssignSelectionColor(MeshRenderer renderer)
    {
        renderer.material = Colors.SelectedLineColor;
    }
    void ClearBoard()
    {
        Destroy(iALine.iALineGameObject);
        Destroy(board.boardGameObject);

        board.linesList.Clear();
        iALine.slotsList.Clear();
    }

    void HideIABoard()
    {
        iALine.iALineGameObject.SetActive(false);
    }

    void ShowIABoard()
    {
        iALine.iALineGameObject.SetActive(true);
    }
    #endregion Generate / Destroy Board

    #region Game Mechanics
    public void SetCurrentSlot(int index)
    {
        currentSlot = index;
    }

    void SetCurrentLineSelected()
    {
        for (int i = 0; i < board.linesList[currentLine].slotsList.Count; i++)
        {
            board.linesList[currentLine].slotsList[i].transform.GetComponent<MeshRenderer>().material = Colors.SelectedLineColor;
        }
    }

    void PrepareOkButton()
    {
        // prepare position
        Vector3 newPosition = new Vector3(firstSlotPosition.x + (rules.slotsByLine +1), firstSlotPosition.y + currentLine, firstSlotPosition.z);

        // create object in scene
        if (board.OkButtonGameObject == null)
        {
            board.OkButtonGameObject = (GameObject)Instantiate(prefabs.OkButtonPrefab, newPosition, prefabs.OkButtonPrefab.transform.rotation,
            board.boardGameObject.transform);
        }
        else
        {
            board.OkButtonGameObject.transform.position = newPosition;
        }

        board.OkButtonGameObject.SetActive(false);
    }

    public void ResetColorsSelection()
    {
        for (int i = 0; i < Colors.GeneratedColorsObjectsList.Count; i++)
        {
            AssignDefaultBoardColor(Colors.GeneratedColorsObjectsList[i].selectedCircle.GetComponent<MeshRenderer>());
        }
    }
    
    #endregion Game Mechanics

    #region IA Exchanges
    public int GetCurrentLineIndex()
    {
        return currentLine;
    }

     public List<Color> GetCurrentLineContent()
    {
        return board.linesList[currentLine].LineColorsList;
    }

    public void GenerateCode()
    {
        for (int i = 0; i < rules.slotsByLine; i++)
        {
            if (Colors.ColorsList.Count > 0)
            {
                // ran material
                int ran = UnityEngine.Random.Range(1, Colors.ColorsList.Count);
                iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material = Colors.ColorsList[ran];
                iALine.slotsList[i].indexAssignedColor = ran;
                iALine.slotsList[i].ballGameObject.SetActive(true);

                iALine.AssignedColorsList.Add(Colors.ColorsList[ran].color);
            }
        }
    }
    /*
    private void VerifyLine()
    {
        int CorrectSlots = 0;
        int WrongPlace = 0;
        int Repetition = 0;
                
        List<Color> usedColorsList = new List<Color>();

        for (int i = 0; i < board.linesList[currentLine].slotsList.Count; i++)
        {           
            if (board.linesList[currentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color
                == iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color)
            {               
                CorrectSlots ++;

                if (usedColorsList.Contains(iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color) == false)                
                    usedColorsList.Add(iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color);
                else
                {
                //    Repetition++;
                }
            }
            else
            {
                // color correct but wrong place
                if (iALine.AssignedColorsList.Contains
                    (board.linesList[currentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color) == true)
                {
                    WrongPlace++;

                    if (usedColorsList.Contains(iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color) == false)
                        usedColorsList.Add(iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color);
                    else
                    {
                        Repetition++;
                    }

                 //   usedColorsList.Add(iALine.slotsList[i].ballSlot.ballGameObject.GetComponent<MeshRenderer>().material.color);
                }
            }
            // disable slot
            DisableSlot(board.linesList[currentLine].slotsList[i]);
        }
        // set balls
        for (int i = 0; i < board.linesList[currentLine].miniSlotsXManager.MiniBallSlotsList.Count; i++)
        {
            int i1 = (i + CorrectSlots);
            int i2 = (WrongPlace + CorrectSlots+ Repetition);

            if (i < CorrectSlots)
            {
                board.linesList[currentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                    = Colors.BlackColor;

                board.linesList[currentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
            }
            else if (i1-1 < i2)
            {
                board.linesList[currentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                   = Colors.WhiteColor;

                board.linesList[currentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
            }    
    }

        // win game
        if (CorrectSlots == rules.slotsByLine)
        {
            WinGame?.Invoke();
        }
        else if(currentLine == rules.linesByBoard
            && CorrectSlots == rules.slotsByLine)
        {
            LooseGame?.Invoke();
        }

        SelectNextLine();
    }
*/
    public void SelectNextLine()
    {
        if (currentLine < rules.linesByBoard)
        {
            currentLine = currentLine + 1;
            currentSlot = -1;
            Colors.selectedColorIndex = -1;
            ResetColorsSelection();
            SetCurrentLineSelected();
            HideOkButton();
            PrepareOkButton();
        }
    }

    public void DisableSlot(SlotData slot)
    {
        // Disable slot
        AssignDefaultBoardColor(slot.selectedCircle.GetComponent<MeshRenderer>());
        slot.transform.GetComponent<Collider>().enabled = false;
    }

    public void ShowOkButton()
    {
        board.OkButtonGameObject.SetActive(true);
    }

    private void HideOkButton()
    {
        board.OkButtonGameObject.SetActive(false);
    }

    #endregion IA Exchanges
}

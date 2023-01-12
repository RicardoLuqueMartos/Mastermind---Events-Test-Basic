using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    #region variables
    [SerializeField]
    GameSettingsData gameSettings;
    public GameSettingsData GameSettings { get { return gameSettings; } }

    [SerializeField]
    bool testMode = false;
    public bool TestMode { get { return testMode; } }

    [SerializeField]
    Vector3 firstSlotPosition = new Vector3();

    [SerializeField]
    int currentLine = 0;
    public int CurrentLine { get { return currentLine; } set { } }
    
    [SerializeField]
    int currentSlot = -1;
    public int CurrentSlot { get { return currentSlot; } }

    [Serializable]
    public class BoardData
    {
        public List<LineData> linesList = new List<LineData>();
        public GameObject boardGameObject;
        public GameObject OkButtonGameObject;
        public GameObject DraggedBallGameObject;

    }
    [SerializeField]
    BoardData board = new BoardData();
    public BoardData Board { get { return board;  } }
       
    [Serializable]
    public class IALineData
    {
        public bool Hidden = true;
        public List<SlotData> slotsList = new List<SlotData>();
        public List<Color> AssignedColorsList = new List<Color>();
        public GameObject iALineGameObject;
    }
    [SerializeField]
    IALineData iALine = new IALineData();

    public IALineData IALine { get { return iALine; } }

    [Serializable]
    public class ColorsData
    {
        public int selectedColorIndex = -1;
        public int PreviousSelectedColorIndex = -1;
        public List<AvailableColorSlot> GeneratedColorsObjectsList = new List<AvailableColorSlot>();
    }
    [SerializeField] 
    ColorsData colors = new ColorsData();

    public ColorsData Colors { get { return colors; } }

    public delegate void MessageEvent();
    public static event MessageEvent BoardGenerated;
    public static event MessageEvent GameStarted;
    #endregion variables

    #region Getters & setters
    public int GetCurrentLineIndex()
    {
        return currentLine;
    }
    public List<Color> GetCurrentLineContent()
    {
        return board.linesList[currentLine].LineColorsList;
    }
    public void SetCurrentSlot(int index)
    {
        currentSlot = index;
    }

    #endregion Getters & setters

    #region Init
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        GlobalVariables.boardManager = this;

        AIController.SelectNextLine += SelectNextLine;
    }

    #endregion Init

    #region Generate Destroy Board

    public void StartGame()
    {
        GenerateBoard();
        GenerateIABoard();

        GenerateColorsChoicesBoard();

        BoardGenerated?.Invoke();
        GameStarted?.Invoke();
    }

    void GenerateBoard()
    {
        // create virtual board
        board.boardGameObject = new GameObject();
        board.boardGameObject.name = "board";
        board.boardGameObject.transform.SetParent(transform);

        for (int iLine = 0; iLine < GameSettings.Rules.linesByBoard; iLine++)
        {            
            // create a line
            GameObject newlineObj = new GameObject();
            newlineObj.transform.SetParent(board.boardGameObject.transform);
            newlineObj.name = "line "+ iLine;
            LineData newline = newlineObj.AddComponent<LineData>();

            board.linesList.Add(newline);


            for (int iSlot = 0; iSlot < GameSettings.Rules.slotsByLine; iSlot++)
            {
              
                Vector3 newPosition = new Vector3(firstSlotPosition.x+iSlot, firstSlotPosition.y+iLine, firstSlotPosition.z);

                // create object in scene
                GameObject newSlotObj = (GameObject)Instantiate(GameSettings.Prefabs.slotPrefab, newPosition, GameSettings.Prefabs.slotPrefab.transform.rotation,
                     newlineObj.transform);
                SlotData newSlot = newSlotObj.GetComponent<SlotData>();
                newSlotObj.name = "Ball Slot " + iSlot;

                newSlot.ballIndex = iSlot;

                // add slot to the list
                board.linesList[iLine].slotsList.Add(newSlot);

                // set color
                newSlot.AssignDefaultBoardColor(this);

                // hide ball
                newSlot.DisactivateBallGameObject();

                // generate MiniSlotsX
                if (iSlot == GameSettings.Rules.slotsByLine-1)
                {
                    int neededSlotX4Prefab = 1;
                    if (GameSettings.Rules.slotsByLine > 4) neededSlotX4Prefab = 2;

                    for (int i3 = 0; i3 < neededSlotX4Prefab; i3++)
                    {
                        // prepare position
                        Vector3 newPosition2 = new Vector3(firstSlotPosition.x + (iSlot + 1+i3), firstSlotPosition.y + iLine, firstSlotPosition.z);

                        // instantiate object
                        GameObject slotGameObject = (GameObject)Instantiate(GameSettings.Prefabs.slotX4Prefab, newPosition2,
                            GameSettings.Prefabs.slotX4Prefab.transform.rotation,
                           newlineObj.transform);
                        slotGameObject.name = "MiniSlotsX";

                        if (i3 == 0)                        
                            newline.miniSlotsXManager = slotGameObject.GetComponent<MiniSlotsXManager>();
                         

                        // set color
                        newline.miniSlotsXManager.AssignDefaultBoardColor(this);

                        if (i3 > 0)
                        {
                            slotGameObject.GetComponent<MiniSlotsXManager>().AssignDefaultBoardColor(this);
                            slotGameObject.GetComponent<MiniSlotsXManager>().bar.SetActive(false);

                            for (int iminiSlot = 0; iminiSlot < 4; iminiSlot++)
                            {
                                if (iminiSlot < GameSettings.Rules.slotsByLine - 4)
                                {
                                    slotGameObject.GetComponent<MiniSlotsXManager>().MiniBallSlotsList[iminiSlot].selectedCircle.transform.GetComponent<MeshRenderer>().material
                                        = GameSettings.Colors.DefaultBoardColor;

                                    slotGameObject.GetComponent<MiniSlotsXManager>().MiniBallSlotsList[iminiSlot].ballGameObject.SetActive(false);

                                    MiniBallSlot miniBallSlot = slotGameObject.GetComponent<MiniSlotsXManager>().MiniBallSlotsList[iminiSlot];

                                    // add it to the first manager
                                    newline.miniSlotsXManager.MiniBallSlotsList.Insert(iminiSlot + 2 , miniBallSlot);
                                 //   slotGameObject.GetComponent<MiniSlotsXManager>().MiniBallSlotsList.Remove(miniBallSlot);
                                }
                                else
                                {
                                    // remove the unused minislot
                                    Destroy(slotGameObject.GetComponent<MiniSlotsXManager>().MiniBallSlotsList[iminiSlot].gameObject);
                                }
                            }
                        }
                        else
                        {
                            newline.miniSlotsXManager.bar.SetActive(false);

                            for (int iminiSlot = 0; iminiSlot < newline.miniSlotsXManager.MiniBallSlotsList.Count; iminiSlot++)
                            {
                                newline.miniSlotsXManager.MiniBallSlotsList[iminiSlot].selectedCircle.transform.GetComponent<MeshRenderer>().material
                                    = GameSettings.Colors.DefaultBoardColor;

                                newline.miniSlotsXManager.MiniBallSlotsList[iminiSlot].ballGameObject.SetActive(false);
                            }
                        }
                        // show object
                        slotGameObject.SetActive(true);
                    }
                }
            }                     
            if (iLine == 0) newline.SetCurrentLineSelected(this);
        }

        PrepareOkButton();
    }

    void GenerateIABoard()
    {
        // create virtual board
        iALine.iALineGameObject = new GameObject();
        iALine.iALineGameObject.name = "IA Line";
        iALine.iALineGameObject.transform.SetParent(transform);

        for (int iSlot = 0; iSlot < GameSettings.Rules.slotsByLine; iSlot++)
        {          
            // prepare position
            Vector3 newPosition = new Vector3(firstSlotPosition.x + iSlot, firstSlotPosition.y + (GameSettings.Rules.linesByBoard+1), firstSlotPosition.z);

            // create object in scene
            GameObject newSlotObj = (GameObject)Instantiate(GameSettings.Prefabs.slotPrefab, newPosition, GameSettings.Prefabs.slotPrefab.transform.rotation,
                iALine.iALineGameObject.transform);

            SlotData newSlot = newSlotObj.GetComponent<SlotData>();

            // set color
            newSlot.AssignDefaultBoardColor(this);

            iALine.slotsList.Add(newSlot);
        }
    }

    void GenerateColorsChoicesBoard()
    {
        GameObject AvailableColors = new GameObject();
        AvailableColors.transform.SetParent(board.boardGameObject.transform);
        AvailableColors.name = "Available Colors";

        for (int iSlot = 0; iSlot < GameSettings.Colors.maxColors; iSlot++)
        {
            // prepare position
            Vector3 newPosition = new Vector3(firstSlotPosition.x -2, firstSlotPosition.y + iSlot, firstSlotPosition.z);

            // create object in scene
            GameObject slot = (GameObject)Instantiate(GameSettings.Prefabs.colorChoicePrefab, newPosition, GameSettings.Prefabs.colorChoicePrefab.transform.rotation,
                AvailableColors.transform);

            AvailableColorSlot availableColorSlot = slot.GetComponent<AvailableColorSlot>();
            availableColorSlot.slot.name = GameSettings.Colors.ColorsList[iSlot].name;
            availableColorSlot.colorIndex = iSlot;

            // set color
            availableColorSlot.AssignDefaultBoardColor(this);

            // set color for the ball
            availableColorSlot.ballGameObject.GetComponent<Renderer>().material = GameSettings.Colors.ColorsList[iSlot];

            Colors.GeneratedColorsObjectsList.Add(availableColorSlot);
        }
    }

    #endregion Generate Destroy Board

    #region about Validate button    
    private void PrepareOkButton()
    {
        // prepare position
        Vector3 newPosition = new Vector3(firstSlotPosition.x + (GameSettings.Rules.slotsByLine 
            +( GameSettings.Rules.slotsByLine / 4 )+ 1), firstSlotPosition.y + currentLine, firstSlotPosition.z);

        // create object in scene
        if (board.OkButtonGameObject == null)
        {
            board.OkButtonGameObject = (GameObject)Instantiate(GameSettings.Prefabs.OkButtonPrefab, newPosition, GameSettings.Prefabs.OkButtonPrefab.transform.rotation,
            board.boardGameObject.transform);
        }
        else
        {
            board.OkButtonGameObject.transform.position = newPosition;
        }

        board.OkButtonGameObject.SetActive(false);
    }

    public void ShowOkButton()
    {
        board.OkButtonGameObject.SetActive(true);
    }

    private void HideOkButton()
    {
        board.OkButtonGameObject.SetActive(false);
    }
    #endregion about Validate button

    private void SelectNextLine()
    {
        if (currentLine < GameSettings.Rules.linesByBoard)
        {
            board.linesList[currentLine].SetCurrentLineUnselected(this);

            currentLine = currentLine + 1;
            currentSlot = -1;
            Colors.selectedColorIndex = -1;
            board.linesList[currentLine].SetCurrentLineSelected(this);
            HideOkButton();
            PrepareOkButton();
        }
    }

    private void OnDisable()
    {
        AIController.SelectNextLine -= SelectNextLine;
    }
}

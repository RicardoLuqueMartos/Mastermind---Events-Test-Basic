using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BoardManager;

public class BoardManager : MonoBehaviour
{
    #region variables

    [SerializeField]
    Vector3 firstSlotPosition = new Vector3();

    [SerializeField]
    int currentLine = 0;

    [SerializeField]
    int currentSlot = -1;

    [Serializable]
    public class SlotData
    {
        public GameObject slotGameObject;
        public BallSlot ballSlot;
        public int indexAssignedColor;
    }

    [Serializable]
    public class LineData
    {
        public List<SlotData> slotsList = new List<SlotData>();
        public List<int> LineColorsIndexList = new List<int>();
        public List<SlotData> miniSlotsList = new List<SlotData>();
        public MiniSlotsXManager miniSlotsXManager;
    //    public GameObject lineGameObject;
    }

    [Serializable]
    public class BoardData
    {
        public List<LineData> linesList = new List<LineData>();
        public GameObject boardGameObject;
    }
    public BoardData board = new BoardData();

    [Serializable]
    public class IALineData
    {
        public bool Hidden = true;
        public List<SlotData> slotsList = new List<SlotData>();
        public List<Material> AssignedColorsList = new List<Material>();
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

    #endregion variables

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
            LineData newLine = new LineData();
            board.linesList.Add(newLine);

            for (int iSlot = 0; iSlot < rules.slotsByLine; iSlot++)
            {
                SlotData newSlot = new SlotData();
              
                Vector3 newPosition = new Vector3(firstSlotPosition.x+iSlot, firstSlotPosition.y+iLine, firstSlotPosition.z);

                // create object in scene
                newSlot.slotGameObject = (GameObject)Instantiate(prefabs.slotPrefab, newPosition, prefabs.slotPrefab.transform.rotation,
                    board.boardGameObject.transform);

                newSlot.ballSlot = newSlot.slotGameObject.GetComponent<BallSlot>();
                newSlot.ballSlot.ballIndex = iSlot;

                // add slot to the list
                board.linesList[iLine].slotsList.Add(newSlot);

                // set color
                AssignDefaultBoardColor(newSlot.slotGameObject.GetComponent<MeshRenderer>());
                AssignDefaultBoardColor(newSlot.ballSlot.selectedCircle.GetComponent<MeshRenderer>());
                AssignDefaultBoardColor(newSlot.ballSlot.bar.GetComponent<MeshRenderer>());

                // hide ball
                newSlot.ballSlot.ballGameObject.SetActive(false);

                // generate MiniSlotsX
                if (iSlot == rules.slotsByLine-1)
                {
                   // prepare position
                    Vector3 newPosition2 = new Vector3(firstSlotPosition.x + (iSlot + 1), firstSlotPosition.y + iLine, firstSlotPosition.z);

                    // instantiate object
                    GameObject slotGameObject = (GameObject)Instantiate(prefabs.slotX4Prefab, newPosition2, 
                        prefabs.slotX4Prefab.transform.rotation,
                       board.boardGameObject.transform);

                    newLine.miniSlotsXManager = slotGameObject.GetComponent<MiniSlotsXManager>();

                    // set color
                    AssignDefaultBoardColor(newLine.miniSlotsXManager.bar.GetComponent<MeshRenderer>());
                    AssignDefaultBoardColor(newLine.miniSlotsXManager.transform.GetComponent<MeshRenderer>());


                    for (int iminiSlot = 0; iminiSlot < newLine.miniSlotsXManager.MiniBallSlotsList.Count; iminiSlot++)
                    {
                        newLine.miniSlotsXManager.MiniBallSlotsList[iminiSlot].selectedCircle.transform.GetComponent<MeshRenderer>().material = Colors.DefaultBoardColor;
                        newLine.miniSlotsXManager.MiniBallSlotsList[iminiSlot].ballGameObject.SetActive(false);
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
            SlotData newSlot = new SlotData();
           
            // prepare position
            Vector3 newPosition = new Vector3(firstSlotPosition.x + iSlot, firstSlotPosition.y + (rules.linesByBoard+1), firstSlotPosition.z);

            // create object in scene
            newSlot.slotGameObject = (GameObject)Instantiate(prefabs.slotPrefab, newPosition, prefabs.slotPrefab.transform.rotation,
                iALine.iALineGameObject.transform);

            newSlot.ballSlot = newSlot.slotGameObject.GetComponent<BallSlot>();

            // set color
            AssignDefaultBoardColor(newSlot.slotGameObject.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(newSlot.ballSlot.selectedCircle.GetComponent<MeshRenderer>());
            AssignDefaultBoardColor(newSlot.ballSlot.bar.GetComponent<MeshRenderer>());

            iALine.slotsList.Add(newSlot);
        }
    }

    void GenerateColorsChoicesBoard()
    {
        for (int iSlot = 0; iSlot < Colors.ColorsList.Count; iSlot++)
        {
            // prepare position
            Vector3 newPosition = new Vector3(firstSlotPosition.x -2, firstSlotPosition.y + iSlot, firstSlotPosition.z);

            // create object in scene
            GameObject slot = (GameObject)Instantiate(prefabs.colorChoicePrefab, newPosition, prefabs.colorChoicePrefab.transform.rotation,
                board.boardGameObject.transform);

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

    void AssignDefaultBoardColor(MeshRenderer renderer)
    {
        renderer.material = Colors.DefaultBoardColor;
    }
    void AssignSelectionColor(MeshRenderer renderer)
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
    void SetCurrentLineSelected()
    {
        for (int i = 0; i < board.linesList[currentLine].slotsList.Count; i++)
        {
            board.linesList[currentLine].slotsList[i].slotGameObject.GetComponent<MeshRenderer>().material = Colors.SelectedLineColor;

        }
    }

    public void SelectSlot(int SlotIndex)
    {
        if (currentSlot != -1)
        {
            AssignDefaultBoardColor(board.linesList[currentLine].slotsList[currentSlot].ballSlot.selectedCircle.GetComponent<MeshRenderer>());
        }
        if (currentSlot != SlotIndex)
        {
            currentSlot = SlotIndex;

            AssignSelectionColor(board.linesList[currentLine].slotsList[currentSlot].ballSlot.selectedCircle.GetComponent<MeshRenderer>());
        }
        else Colors.selectedColorIndex = -1;

        AssignColorToSlot();
    }

    private void AssignColorToSlot()
    {
        if (Colors.selectedColorIndex != -1 && currentSlot != -1)
        {
            board.linesList[currentLine].slotsList[currentSlot].ballSlot.ballGameObject.GetComponent<MeshRenderer>().material
                = Colors.ColorsList[Colors.GeneratedColorsObjectsList[Colors.selectedColorIndex].colorIndex];

            board.linesList[currentLine].slotsList[currentSlot].ballSlot.ballGameObject.SetActive(true);
        }
    }

    void PrepareOkButton()
    {
        // prepare position
        Vector3 newPosition = new Vector3(firstSlotPosition.x + (rules.slotsByLine +1), firstSlotPosition.y + currentLine, firstSlotPosition.z);

        // create object in scene
        GameObject slot = (GameObject)Instantiate(prefabs.OkButtonPrefab, newPosition, prefabs.OkButtonPrefab.transform.rotation,
            board.boardGameObject.transform);
    }

    #region Select a color
    public void SelectColor(int colorIndex)
    {
        if (Colors.selectedColorIndex != -1)
        {
            AssignDefaultBoardColor(Colors.GeneratedColorsObjectsList[Colors.selectedColorIndex].selectedCircle.GetComponent<MeshRenderer>());
        }

        if (Colors.selectedColorIndex != colorIndex)
        {
            Colors.selectedColorIndex = colorIndex;

            AssignSelectionColor(Colors.GeneratedColorsObjectsList[colorIndex].selectedCircle.GetComponent<MeshRenderer>());
        }
        else Colors.selectedColorIndex = -1;

        AssignColorToSlot();
    }

    #endregion Select a color
    #endregion Game Mechanics

    #region IA Exchanges
    public int GetCurrentLineIndex()
    {
        return currentLine;
    }
    public List<int> GetCurrentLineContent()
    {
        return board.linesList[currentLine].LineColorsIndexList;
    }
    public void GenerateCode()
    {
        for (int i = 0; i < rules.slotsByLine; i++)
        {
            if (Colors.ColorsList.Count > 0)
            {
                // ran material
                int ran = UnityEngine.Random.Range(1, Colors.ColorsList.Count);
                iALine.slotsList[i].ballSlot.ballGameObject.GetComponent<MeshRenderer>().material = Colors.ColorsList[ran];
                iALine.slotsList[i].indexAssignedColor = ran;
                iALine.slotsList[i].ballSlot.ballGameObject.SetActive(true);

            }
        }
    }
    #endregion IA Exchanges
}

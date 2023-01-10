using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AvailableColorSlot : MonoBehaviour, UsableObject
{
    #region Variables
    BoardManager boardManager;

    public GameObject slot;
    public GameObject ballGameObject;
    public GameObject selectedCircle;
    public GameObject bar1;
    public GameObject bar2;
    public int colorIndex;
    #endregion Variables

    public void UseObject(BoardManager _boardManager) 
    {
        Debug.Log("AvailableColorSlot UseObject");
        boardManager = _boardManager;
        SelectColor(colorIndex);
    }

    void SelectColor(int colorIndex)
    {
    //    boardManager.ResetColorsSelection();

        if (boardManager.Colors.selectedColorIndex != colorIndex)
        {
            boardManager.Colors.selectedColorIndex = colorIndex;

            boardManager.AssignSelectionColor(boardManager.Colors.GeneratedColorsObjectsList[colorIndex].selectedCircle.GetComponent<MeshRenderer>());
        }
        else boardManager.Colors.selectedColorIndex = -1;

        AssignColorToSlot();
    }

    void AssignColorToSlot()
    {
        if (boardManager.Colors.selectedColorIndex != -1 && boardManager.CurrentSlot != -1)
        {
            boardManager.board.linesList[boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].ballSlot.ballGameObject.GetComponent<MeshRenderer>().material
                = boardManager.Colors.ColorsList[boardManager.Colors.GeneratedColorsObjectsList[boardManager.Colors.selectedColorIndex].colorIndex];

            boardManager.board.linesList[boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].ballSlot.ballGameObject.SetActive(true);
        }

        int ActiveAmount = 0;

        // if all slots done
        for (int i = 0; i < boardManager.board.linesList[boardManager.CurrentLine].slotsList.Count; i++)
        {

            if (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballSlot.ballGameObject.activeInHierarchy == true)
            {
                ActiveAmount = ActiveAmount + 1;
            }
        }

        if (ActiveAmount == boardManager.rules.slotsByLine)
            boardManager.ShowOkButton();

    }

}

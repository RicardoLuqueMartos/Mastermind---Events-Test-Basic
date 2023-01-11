using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotData : SlotBase, UsableObject
{
    #region Variables
    
    public int indexAssignedColor;

    #endregion Variables

    public void UseObject(BoardManager _boardManager)
    {  
        boardManager = _boardManager;
        SelectSlot(ballIndex);
    }

    void SelectSlot(int SlotIndex)
    {
     /*   if (boardManager.CurrentSlot != -1)
        {
            AssignDefaultBoardColor(boardManager.Board.linesList[boardManager.CurrentLine].slotsList
                [boardManager.CurrentSlot].selectedCircle.GetComponent<MeshRenderer>());
        }*/
        if (boardManager.CurrentSlot != SlotIndex)
        {
            boardManager.SetCurrentSlot(SlotIndex);

        //    AssignSelectionColor(boardManager.Board.linesList
         //       [boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].selectedCircle.GetComponent<MeshRenderer>());
        }
        else boardManager.SetCurrentSlot(-1);

        AssignColorToSlot();
    }
    public delegate void MessageEvent();
    public static event MessageEvent ResetColorsSelection;

    void AssignColorToSlot()
    {
        if (boardManager.Colors.selectedColorIndex != -1 && boardManager.CurrentSlot != -1)
        {
            boardManager.Board.linesList[boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].AssignMaterialToBall(
                boardManager.Colors.ColorsList[boardManager.Colors.GeneratedColorsObjectsList[boardManager.Colors.selectedColorIndex].colorIndex]);

            boardManager.Board.linesList[boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].ActivateBallGameObject();

            ResetColorsSelection?.Invoke();
        }

        int ActiveAmount = 0;

        // if all slots done
        for (int i = 0; i < boardManager.Board.linesList[boardManager.CurrentLine].slotsList.Count; i++)
        {
            if (boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].IsBallActiveInHierarchy())
            {
                ActiveAmount = ActiveAmount + 1;
            }
        }

        if (ActiveAmount == boardManager.Rules.slotsByLine)
            boardManager.ShowOkButton();

    }

    public void DisableSlot()
    {
        // Disable slot
        AssignDefaultBoardColor(selectedCircle.GetComponent<MeshRenderer>());
        transform.GetComponent<Collider>().enabled = false;
    }

    public void AssignDefaultBoardColor(BoardManager _boardManager)
    {
        boardManager = _boardManager;

        AssignDefaultBoardColor(transform.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(selectedCircle.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(bar.GetComponent<MeshRenderer>());
    }

    void AssignDefaultBoardColor(MeshRenderer renderer)
    {
        renderer.material = boardManager.Colors.DefaultBoardColor;
    }
    void AssignSelectionColor(MeshRenderer renderer)
    {
        renderer.material = boardManager.Colors.SelectedLineColor;
    }
}

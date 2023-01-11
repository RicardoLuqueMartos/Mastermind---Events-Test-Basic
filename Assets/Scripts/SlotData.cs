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
        if (boardManager.CurrentSlot != -1)
        {
            AssignDefaultBoardColor(boardManager.Board.linesList[boardManager.CurrentLine].slotsList
                [boardManager.CurrentSlot].selectedCircle.GetComponent<MeshRenderer>());
        }
        if (boardManager.CurrentSlot != SlotIndex)
        {
            boardManager.SetCurrentSlot(SlotIndex);

            AssignSelectionColor(boardManager.Board.linesList
                [boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].selectedCircle.GetComponent<MeshRenderer>());
        }
        else boardManager.SetCurrentSlot(-1);
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

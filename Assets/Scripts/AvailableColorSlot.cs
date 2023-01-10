using System.Collections;
using System.Collections.Generic;
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
        boardManager = _boardManager;
        SelectColor(colorIndex);
    }

    void SelectColor(int colorIndex)
    {    
   //     boardManager.ResetColorsSelection();

        if (boardManager.Colors.selectedColorIndex != colorIndex)
        {
            boardManager.Colors.selectedColorIndex = colorIndex;

        //    AssignSelectionColor(boardManager.Colors.GeneratedColorsObjectsList[colorIndex].selectedCircle.GetComponent<MeshRenderer>());
        }
        else boardManager.Colors.selectedColorIndex = -1;

        AssignColorToSlot();
    }

    void AssignColorToSlot()
    {
        if (boardManager.Colors.selectedColorIndex != -1 && boardManager.CurrentSlot != -1)
        {
            boardManager.Board.linesList[boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].AssignMaterialToBall(
                boardManager.Colors.ColorsList[boardManager.Colors.GeneratedColorsObjectsList[boardManager.Colors.selectedColorIndex].colorIndex]);

            boardManager.Board.linesList[boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].ActivateBallGameObject();
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

    public void AssignDefaultBoardColor(BoardManager _boardManager)
    {
        boardManager = _boardManager;

        AssignDefaultBoardColor(transform.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(selectedCircle.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(bar1.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(bar2.GetComponent<MeshRenderer>());
    }

    public void AssignDefaultBoardColor(MeshRenderer renderer)
    {
        renderer.material = boardManager.Colors.DefaultBoardColor;
    }
    public void AssignSelectionColor(MeshRenderer renderer)
    {
        renderer.material = boardManager.Colors.SelectedLineColor;
    }
}

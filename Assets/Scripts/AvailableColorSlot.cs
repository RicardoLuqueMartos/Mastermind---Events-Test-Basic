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

    void OnEnable()
    {
        SlotData.ResetColorsSelection += ResetColorsSelectionLine;
    }


    public void UseObject(BoardManager _boardManager) 
    {
        boardManager = _boardManager;
        SelectColor(colorIndex);
    }

    void SelectColor(int colorIndex)
    {    
        ResetColorsSelection();

        if (boardManager.Colors.selectedColorIndex != colorIndex)
        {
            boardManager.Colors.selectedColorIndex = colorIndex;

            AssignSelectionColor(boardManager.Colors.GeneratedColorsObjectsList[colorIndex].selectedCircle.GetComponent<MeshRenderer>());

            HideColorBall();
            ShowDraggedBall();
        }
        else
        {
            ShowColorBall();
            HideDraggedBall();
            boardManager.Colors.selectedColorIndex = -1;       
        }
    }

    void ShowDraggedBall()
    {
        boardManager.Board.DraggedBallGameObject.GetComponent<MeshRenderer>().material = boardManager.Colors.ColorsList[boardManager.Colors.selectedColorIndex];
        boardManager.Board.DraggedBallGameObject.SetActive(true);
    }

    void HideDraggedBall()
    {
        boardManager.Board.DraggedBallGameObject.SetActive(false);
    }

    void HideColorBall()
    {
        boardManager.Colors.GeneratedColorsObjectsList[boardManager.Colors.selectedColorIndex].ballGameObject.SetActive(false);
    }
    void ShowColorBall()
    {
        boardManager.Colors.GeneratedColorsObjectsList[boardManager.Colors.selectedColorIndex].ballGameObject.SetActive(true);
    }
    private void ResetColorsSelectionLine()
    {
        HideDraggedBall();
        boardManager.Colors.selectedColorIndex = -1;
        ResetColorsSelection();
    }
       
    private void ResetColorsSelection()
    {
        for (int i = 0; i < boardManager.Colors.GeneratedColorsObjectsList.Count; i++)
        {
            boardManager.Colors.GeneratedColorsObjectsList[i].ballGameObject.SetActive(true);

            boardManager.Colors.GeneratedColorsObjectsList[i].AssignDefaultBoardColor();
        }
    }

    public void AssignDefaultBoardColor(BoardManager _boardManager)
    {
        boardManager = _boardManager;

        AssignDefaultBoardColor();
    }
     
    public void AssignDefaultBoardColor()
    {
        AssignDefaultBoardColor(transform.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(selectedCircle.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(bar1.GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(bar2.GetComponent<MeshRenderer>());
    }

    void AssignDefaultBoardColor(MeshRenderer renderer)
    {
        renderer.material = boardManager.Colors.DefaultBoardColor;
    }
    void AssignSelectionColor(MeshRenderer renderer)
    {
        renderer.material = boardManager.Colors.SelectedLineColor;
    }

    void OnDisable()
    {
        SlotData.ResetColorsSelection -= ResetColorsSelectionLine;
    }

}

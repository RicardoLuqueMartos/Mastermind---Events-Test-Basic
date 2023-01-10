using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSlotsXManager : MonoBehaviour
{
    [SerializeField]
    BoardManager boardManager;

    public List<MiniBallSlot> MiniBallSlotsList = new List<MiniBallSlot>();
    public GameObject bar;
    public void AssignDefaultBoardColor(BoardManager _boardManager)
    {
        boardManager = _boardManager;

        AssignDefaultBoardColor(GetComponent<MeshRenderer>());
        AssignDefaultBoardColor(bar.GetComponent<MeshRenderer>());
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

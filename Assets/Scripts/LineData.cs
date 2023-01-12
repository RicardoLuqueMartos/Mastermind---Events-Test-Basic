using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineData : MonoBehaviour
{
    [SerializeField]
    BoardManager boardManager;

    public List<SlotData> slotsList = new List<SlotData>();
    public List<Color> LineColorsList = new List<Color>();
    public MiniSlotsXManager miniSlotsXManager;

    public void SetCurrentLineSelected(BoardManager _boardManager)
    {
        boardManager = _boardManager;

        for (int i = 0; i < slotsList.Count; i++)
        {
            slotsList[i].transform.GetComponent<MeshRenderer>().material = boardManager.GameSettings.Colors.SelectedLineColor;
        }
    }

    public void SetCurrentLineUnselected(BoardManager _boardManager)
    {
        boardManager = _boardManager;

        for (int i = 0; i < slotsList.Count; i++)
        {
            slotsList[i].transform.GetComponent<MeshRenderer>().material = boardManager.GameSettings.Colors.DefaultBoardColor;
        }
    }

  /*  public int[] GetLineColors()
    {
     //   return 
    }*/
}

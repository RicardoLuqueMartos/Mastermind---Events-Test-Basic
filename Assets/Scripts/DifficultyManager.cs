using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BoardManager;

public class DifficultyManager : MonoBehaviour
{    
    [SerializeField]
    BoardManager boardManager;
   
    public int minimumColorsAmount = 4;
    public int maximumColorsAmount = 10;
    public Slider colorsAmountSlider;
    public Text colorsAmountText;

    public int minimumSlotsByLineAmount = 3;
    public int maximumSlotsByLineAmount = 8;
    public Slider slotsByLineSlider;
    public Text slotsByLineText;

    public int minimumLinesAmount = 6;
    public int maximumLinesAmount = 14;
    public Slider linesSlider;
    public Text maximumLinesAmountText;
       
    private void Start()
    {
        if (boardManager == null) boardManager = FindObjectOfType<BoardManager>();

        // assign values to the sliders
        slotsByLineSlider.minValue = minimumSlotsByLineAmount;
        slotsByLineSlider.maxValue = maximumSlotsByLineAmount;
        slotsByLineSlider.value = boardManager.GameSettings.Rules.slotsByLine;

        colorsAmountSlider.minValue = minimumColorsAmount;
        colorsAmountSlider.maxValue = maximumColorsAmount;
        colorsAmountSlider.value = boardManager.GameSettings.Colors.maxColors;

        linesSlider.minValue = minimumLinesAmount;
        linesSlider.maxValue = maximumLinesAmount;
        linesSlider.value = boardManager.GameSettings.Rules.linesByBoard;
    }

    public void ChangeSlotsAmount()
    {
        boardManager.GameSettings.Rules.slotsByLine = Mathf.RoundToInt(slotsByLineSlider.value);
        slotsByLineText.text = boardManager.GameSettings.Rules.slotsByLine.ToString();
    }

    public void ChangeLinesAmount()
    {
        boardManager.GameSettings.Rules.linesByBoard = Mathf.RoundToInt(linesSlider.value);
        maximumLinesAmountText.text = boardManager.GameSettings.Rules.linesByBoard.ToString();
    }

    public void ChangeColorsAmount()
    {
        boardManager.GameSettings.Colors.maxColors = Mathf.RoundToInt(colorsAmountSlider.value);
        colorsAmountText.text = boardManager.GameSettings.Colors.maxColors.ToString();
    }
}

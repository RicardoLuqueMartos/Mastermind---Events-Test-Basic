using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    GameObject WinPanel;

    [SerializeField]
    GameObject LoosePanel;

    [SerializeField]
    GameObject InGamePanel;

    [SerializeField]
    GameObject StartMenuPanel;

    #endregion Variables

    void OnEnable()
    {
        WinPanel.SetActive(false); 
        LoosePanel.SetActive(false);

        //C# : on inscrit la fonction à l'event
        AIController.WinGame += ShowWinPanel;
        AIController.LooseGame += ShowLoosePanel;
        BoardManager.GameStarted += StartGameUI;
    }

    void StartGameUI()
    {
        HideStartMenu();
        ShowGameUI();
    }

    void HideStartMenu()
    {
        StartMenuPanel.SetActive(false);
    }

    void ShowGameUI()
    {
        InGamePanel.SetActive(true);
    }

    private void ShowWinPanel()
    {       
        InGamePanel.SetActive(false);        
        WinPanel.SetActive(true);
    }

    private void ShowLoosePanel()
    {
        InGamePanel.SetActive(false);
        LoosePanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        AIController.WinGame -= ShowWinPanel;
        AIController.LooseGame -= ShowLoosePanel;
        BoardManager.GameStarted -= StartGameUI;

    }
}

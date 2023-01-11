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

    #endregion Variables

    void OnEnable()
    {
        WinPanel.SetActive(false); 
        LoosePanel.SetActive(false);

        //C# : on inscrit la fonction à l'event
        AIController.WinGame += ShowWinPanel;
        AIController.LooseGame += ShowLoosePanel;
    }

    private void ShowWinPanel()
    {       
        WinPanel.SetActive(true);
    }

    private void ShowLoosePanel()
    {       
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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        //C# : on inscrit la fonction LooseMessage à l'event
        //grace à la propriété static nous devons juste trouver la classe
        AIController.WinGame += ShowWinPanel;
        AIController.LooseGame += ShowLoosePanel;
    }

    private void ShowWinPanel()
    {
        Debug.Log("ShowWinPanel");
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
}

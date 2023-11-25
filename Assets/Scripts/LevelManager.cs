using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangePanel(bool menu)
    {
        if (menu)
        {
            panel1.SetActive(false);
            panel2.SetActive(true);
        }
        if (!menu)
        {
            panel1.SetActive(true);
            panel2.SetActive(false);
        }
    }
    
}

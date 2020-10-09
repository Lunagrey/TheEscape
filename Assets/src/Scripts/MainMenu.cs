using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public Button playLife;
    public Button playTimer;
    public Button quit;

    // Start is called before the first frame update
    void Start()
    {
        playLife.onClick.AddListener(SwitchSceneLife);
        playTimer.onClick.AddListener(SwitchSceneTime);
        quit.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void SwitchSceneLife()
    {
        SceneManager.LoadScene("PlayLife");
    }

    void SwitchSceneTime()
    {
        SceneManager.LoadScene("PlayTime");
    }
}

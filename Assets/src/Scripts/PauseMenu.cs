using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Button Resume;
    public Button Restart;
    public Button otherMode;
    public Button quit;
    public string mode;

    // Start is called before the first frame update
    void Start()
    {
        Resume.onClick.AddListener(ResumeScene);
        Restart.onClick.AddListener(ReloadScene);
        otherMode.onClick.AddListener(SwitchScene);
        quit.onClick.AddListener(QuitGame);
    }

    void ResumeScene()
    {
        Time.timeScale = 1.0f;
        this.gameObject.SetActive(false);
    }
    void QuitGame()
    {
        Application.Quit();
    }
    void ReloadScene()
    {
        Time.timeScale = 1.0f;

        if (mode == "life")
            SceneManager.LoadScene("PlayLife");
        else
            SceneManager.LoadScene("PlayTime");
    }
    void SwitchScene()
    {
        Time.timeScale = 1.0f;

        if (mode == "life")
            SceneManager.LoadScene("PlayTime");
        else
            SceneManager.LoadScene("PlayLife");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public Button Restart;
    public Button otherMode;
    public Button quit;
    public string mode;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
        Restart.onClick.AddListener(ReloadScene);
        otherMode.onClick.AddListener(SwitchScene);
        quit.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
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

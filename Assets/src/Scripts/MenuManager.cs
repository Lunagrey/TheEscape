using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public Button play;
    public Button quit;

    // Start is called before the first frame update
    void Start()
    {
        play.onClick.AddListener(SwitchScene);//adds a listener for when you click the button
        quit.onClick.AddListener(QuitGame);//adds a listener for when you click the button
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void SwitchScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pause;
    public GameObject win;
    public GameObject gameOver;
    public string mode;
    public float time;
    public Text textTime;
    public int life;
    public GameObject lifeAsset1;
    public GameObject lifeAsset2;
    public static MenuManager instance;

    void Start()
    {
        instance = this;
        time *= 60;
        life = 3;
    }

    // Start is called before the first frame update
    void Update()
    {
        time -= Time.deltaTime;

        if (mode == "life") {
            if (life == 1)
                lifeAsset1.SetActive(false);
            if (life == 2)
                lifeAsset2.SetActive(false);
        }
        if (mode == "time") {
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            textTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        if (Input.GetKeyDown("escape"))
        {
            Time.timeScale = 0.0f;
            pause.SetActive(true);
        }
        if (mode == "life" && life < 0) {
            gameOver.SetActive(true);
        }
        else if (mode == "time" && time <= 0.0f) {
            gameOver.SetActive(true);
        }
        if (this.gameObject.transform.childCount <= 0)
        {
            Time.timeScale = 0.0f;
            win.SetActive(true);
        }
    }

    public void DeleteOneLife()
    {
        --life;
    }

    void QuitGame()
    {
        Application.Quit();
    }
}

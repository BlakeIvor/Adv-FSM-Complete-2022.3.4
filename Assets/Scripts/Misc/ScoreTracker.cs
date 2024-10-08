using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{

    private float bestTime;
    private float currTime;
    [SerializeField] Text timerText;

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!LevelManager.instance.gameOverPanel.activeInHierarchy)
            {
                currTime += Time.deltaTime;
                int min = (int)(currTime / 60f) % 60;
                int sec = (int)(currTime % 60);
                int millisec = (int)(currTime * 1000f) % 1000;

                if (min > 0)
                {
                    timerText.text = min.ToString("00") + ":" + sec.ToString("00") + ":" + millisec.ToString("000");
                }
                else 
                {
                    timerText.text = sec.ToString("00") + ":" + millisec.ToString("000");
                }
            }
        }
    }

    void ResetTime()
    {
        currTime = 0.0f;
    }
}

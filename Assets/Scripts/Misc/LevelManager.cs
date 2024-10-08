using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform tankParent;
    [SerializeField] PlayerTankController player;
    public GameObject gameOverPanel;

    public static LevelManager instance { get; private set;}

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tankParent.childCount == 0 && !gameOverPanel.activeInHierarchy || player.health <= 0)
        {
            OpenGameOverPanel();
        }
    }

    void OpenGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
}

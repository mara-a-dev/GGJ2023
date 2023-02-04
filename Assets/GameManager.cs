using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void GameOver()
    {
        //TODO : Show UI that PLAYER IS DEAD
        ResetGame();
        //TODO : go to main screen
    }
    public void ResetGame()
    {
        PlayerStats.Instance.ResetStats();
    }
}

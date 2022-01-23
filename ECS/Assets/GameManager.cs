using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int enemyCount;
    public int maxEnemy;
    public int multiplier;
    private WaveSystem ws;
    public int wave;
    public bool waveSpawned;
    public GameObject Panel;

    public Text enemyCounttxt;
    public Text waveCountertxt;

    private void Awake()
    {
        //if (instance != null && instance != this) 
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        instance = this;

    }
    // Start is called before the first frame update
    void Start()
    {
        enemyCount = 0;
        wave = 1;
        waveSpawned = false;
        Time.timeScale = 1; //Make sure the game is not paused

    }

    // Update is called once per frame
    void Update()
    {
        enemyCounttxt.text = "Enemies Defeated: " + enemyCount;
        waveCountertxt.text = "Wave " + wave;
        if (wave > 4) 
        {
            enemyCounttxt.enabled = false;
            waveCountertxt.enabled = false;
            Panel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void IncreaseEnemyCount() 
    {
        enemyCount++;
    }

    public void IncreaseWave() 
    {
        wave++;
        //Reset Enemy Count
        enemyCount = 0;
    }

    public int GetSpawnNumber(int wave) 
    {
        if (wave == 1)
        {
            multiplier = 5;
            maxEnemy = multiplier;
        }
        else if (wave == 2)
        {
            multiplier = 10;
            maxEnemy = multiplier;
        }
        else if (wave == 3)
        {
            multiplier = 20;
            maxEnemy = multiplier;
        }
        else if (wave == 4) 
        {
            multiplier = 200;
            maxEnemy = multiplier;
        }
        return multiplier;
    }

    public void RestartLevel() 
    {
        SceneManager.LoadScene(0);
        
    }
}

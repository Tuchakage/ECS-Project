using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int enemyCount;
    public int multiplier;

    public int wave;

    private void Awake()
    {
        if (instance != null && instance != this) 
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyCount = 0;
        wave = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseEnemyCount() 
    {
        enemyCount++;
    }

    public void IncreaseWave() 
    {

    }
    public int GetEnemyCount() 
    {
        return enemyCount;
    }

    public int GetWaveNumber() 
    {
        return wave;
    }

    public int GetSpawnNumber(int wave) 
    {
        if (wave == 1) 
        {
            multiplier = 5;
        }
        return multiplier;
    }
}

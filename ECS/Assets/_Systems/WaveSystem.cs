using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
public class WaveSystem : SystemBase
{
    private Entity prefab;
    private Entity playerPrefab;
    private int waveNumber;
    private bool hasWaveSpawned;
    protected override void OnStartRunning()
    {
        //Get the data components
        prefab = GetSingleton<SpawnComponent>().spawnPrefab;
        playerPrefab = GetSingleton<SpawnComponent>().playerPrefab;
        //Entity playerPrefab = GetSingleton<SpawnComponent>().playerPrefab;
        //Gets the Wave Number the player is on
        waveNumber = GameManager.instance.wave;
        //Get Value from game manager
        hasWaveSpawned = GameManager.instance.waveSpawned;
        //Spawn the first wave
        SpawnWave();



        //Spawns Player
        Entity player = EntityManager.Instantiate(playerPrefab);
        //Set the position of where the Player will be spawned
        EntityManager.SetComponentData(player, new Translation
        {
            Value = new float3(-1.94000006f, -6.57079887f, -7.19999981f)
        });
    }
    protected override void OnUpdate()
    {
        //Get Value from game manager and keep checking it just incase it changes
        hasWaveSpawned = GameManager.instance.waveSpawned;
        //Keeps checking the wave number the player is on
        waveNumber = GameManager.instance.wave;
        //Debug.Log("Works");
        //Debug.Log("Wave Number: " + waveNumber);
        //Debug.Log("Has Waved Spawned = " + hasWaveSpawned);
        if ((waveNumber > 0 && waveNumber < 5) && !hasWaveSpawned) 
        {
            SpawnWave();
            
        }
    }

    public void SpawnWave() 
    {
        for (int i = 0; i < GameManager.instance.GetSpawnNumber(waveNumber); i++) //Different amount of enemies will spawn depending on what wave they are on
        {
            //Spawns Entity
            Entity entity = EntityManager.Instantiate(prefab);
            //Randomly Spawns Entity
            EntityManager.SetComponentData(entity, new Translation
            {
                Value = new float3(
                    UnityEngine.Random.Range(-8f, 8f),
                    -8.1f,
                    UnityEngine.Random.Range(0f, 10f))
            });
            GameManager.instance.waveSpawned = true;
            //Randomly Changes Speed of each Entity
            //EntityManager.SetComponentData(entity, new MoveComponent
            //{
            //    SpeedValue = UnityEngine.Random.Range(2f, 7f)
            //});
        }
    }
}

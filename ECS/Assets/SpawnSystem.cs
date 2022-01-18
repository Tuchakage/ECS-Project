using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class SpawnSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        //Get the data components
        Entity prefab = GetSingleton<SpawnComponent>().spawnPrefab;
        Entity playerPrefab = GetSingleton<SpawnComponent>().playerPrefab;
        int multiplier = GetSingleton<SpawnComponent>().multiplier;

        for (int i = 0; i < multiplier; i++) 
        {
            //Spawns Entity
            Entity entity = EntityManager.Instantiate(prefab);
            //Randomly Spawns Entity
            EntityManager.SetComponentData(entity, new Translation 
            {
                Value = new float3(
                    UnityEngine.Random.Range(-8f,8f),
                    UnityEngine.Random.Range(-5f, 5f),
                    UnityEngine.Random.Range(0f, 10f))
            });

            //Randomly Changes Speed of each Entity
            //EntityManager.SetComponentData(entity, new MoveComponent
            //{
            //    SpeedValue = UnityEngine.Random.Range(2f, 7f)
            //});
        }

        //Spawns Entity
        Entity player = EntityManager.Instantiate(playerPrefab);
        //Set the position of where the Player will be spawned
        EntityManager.SetComponentData(player, new Translation
        {
            Value = new float3(-1.48000002f, -8.69f, -10.8000002f)
        });
    }
    protected override void OnUpdate()
    {

    }


}

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class FindPlayerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //Cycle through all the Entities with the Enemy tag
        Entities.WithAll<EnemyTag>().ForEach((Entity entity) =>
        {
            //Then Cycle through all the Entities with the Player Tag (Which will be 1)
            Entities.WithAll<PlayerTag>().ForEach((Entity player) =>
            {
                //set target variable inside of the Entities with the Enemy tag to be the player
                EntityManager.SetComponentData(entity, new EnemyTag
                {
                    target = player
                });
            });
        });

 

    }

    
}

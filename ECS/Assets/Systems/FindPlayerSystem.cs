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

        //For Each Enemy that has a Translation and Enemy tag
        Entities.ForEach((Entity entity, ref Translation translation, ref EnemyTag getPlayer) => {

            //Gets the translation of the entity assigned to the target variable in the EnemyTag Script
            Translation targetTranslation = World.EntityManager.GetComponentData<Translation>(getPlayer.target);
            //Debug.DrawLine(translation.Value, targetTranslation.Value);
        });

 

    }

    
}

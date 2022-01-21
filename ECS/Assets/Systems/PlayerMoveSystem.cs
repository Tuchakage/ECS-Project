using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {

        float deltaTime = Time.DeltaTime;
        
        
        //For each Entity that has the Player Move Component, move them depending on the Target Direction and move speed
        Entities.ForEach((ref Translation translation, in PlayerMoveComponent pMovement) => {
            translation.Value += pMovement.targetDirection * pMovement.moveSpeed * deltaTime;
        }).Schedule();
    }
}

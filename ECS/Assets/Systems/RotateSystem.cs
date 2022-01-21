using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RotateSystem : SystemBase
{
    protected override void OnUpdate()
    {

        
        
        
        Entities.ForEach((ref  Rotation rotation, in PlayerMoveComponent movement, in RotateComponent rotate) => {

            //If there is input (This if statement prevents the player from rotating back to 0 when you dont press anything)
            if (!movement.targetDirection.Equals(float3.zero)) 
            {
                //Lerp Current rotation to new input direction
                quaternion targetRotation = quaternion.LookRotationSafe(movement.targetDirection, math.up());
                rotation.Value = math.slerp(rotation.Value, targetRotation, rotate.rotateSpeed);
            }

        }).Schedule();
    }
}

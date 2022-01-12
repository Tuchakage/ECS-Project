using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //Make it so that Time.DeltaTime works in the Entity For Each loop
        var deltaTime = Time.DeltaTime;

        //For each Entity that has the Enemy Tag, MoveComponent and LocalToWorld
        Entities.ForEach((ref Translation translation, in LocalToWorld transform,in MoveComponent moveComponent, in EnemyTag getPlayer)  => {

            //Get the Player Transform
            LocalToWorld targetTransform = GetComponent<LocalToWorld>(getPlayer.target);

            //Convert the position of the player into a float3 so we can use it for calculation
            float3 targetPosition = targetTransform.Position;

            //Calculate the direction the Entity needs to go to get to the player
            float3 direction = math.normalize(targetPosition - translation.Value);

            //Move the Entity
            translation.Value += direction * moveComponent.SpeedValue * deltaTime;

        }).Schedule();
    }
}

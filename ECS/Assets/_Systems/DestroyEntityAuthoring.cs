using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;




// This system sets the PhysicsGravityFactor of any dynamic body that enters a Trigger Volume.
// A Trigger Volume is defined by a PhysicsShapeAuthoring with the `Is Trigger` flag ticked and a
// TriggerGravityFactor behaviour added.
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class DestroyEntityAuthoringSystem : JobComponentSystem
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct DestroyEntityAuthoringJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<TriggerGravityFactor> TriggerGravityFactorGroup; //Used to Check For Triggers
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup; //Used to Check For Dynamic Bodies
        public ComponentDataFromEntity<EnemyTag> enemyTagGroup; //Used to Check if the Entity has the Enemy Tag
        public EntityCommandBuffer commandBuffer;
        public void Execute(TriggerEvent triggerEvent)
        {
            //Get References to the Trigger and to whatever is colliding with the Trigger
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            //Checks which Entity is a trigger
            bool isBodyATrigger = TriggerGravityFactorGroup.HasComponent(entityA);
            bool isBodyBTrigger = TriggerGravityFactorGroup.HasComponent(entityB);

            // Ignoring Triggers overlapping other Triggers
            if (isBodyATrigger && isBodyBTrigger)
                return;

            //Checks which body is a dynamic
            bool isBodyADynamic = PhysicsVelocityGroup.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.HasComponent(entityB);

            // Ignoring overlapping static bodies
            if ((isBodyATrigger && !isBodyBDynamic) ||
                (isBodyBTrigger && !isBodyADynamic))
                return;

            //If Body A is a trigger then set entity A to be the Trigger Entity, if Body A is not a trigger then set Entity B to be the Trigger Entity
            var triggerEntity = isBodyATrigger ? entityA : entityB;

            // If Body A is the the trigger then set Entity B to be the dynamic Entity, if Body A is not the trigger then Entity A is the Dynamic Entity
            var dynamicEntity = isBodyATrigger ? entityB : entityA;

            //Check whether the dynamic entity has the Enemy tag
            bool isBodyEnemy = enemyTagGroup.HasComponent(dynamicEntity);
            bool checkIfPlayerAttacking = enemyTagGroup[dynamicEntity].isPlayerAttacking;

            if (isBodyEnemy && checkIfPlayerAttacking) //If the Dynamic Body has the Enemy Tag
            {  
                //Destroy It
                commandBuffer.DestroyEntity(dynamicEntity);
                GameManager.instance.IncreaseEnemyCount();
                if (GameManager.instance.enemyCount == GameManager.instance.maxEnemy) //If this was the last Enemy left
                {

                    GameManager.instance.IncreaseWave();
                    //Make Wave Spawned False so that a new wave will start
                    GameManager.instance.waveSpawned = false;
                    Debug.Log("Wave");
                }
                //Increase Enemy Count In Game Manager

               // Debug.Log("Increase Enemy Count");
            }
            
            //Debug.Log("Trigger Colliding With: " + dynamicEntity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DestroyEntityAuthoringJob
        {
            //Gets References to everything in the scene with these Data Components
            TriggerGravityFactorGroup = GetComponentDataFromEntity<TriggerGravityFactor>(true),
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            enemyTagGroup = GetComponentDataFromEntity<EnemyTag>(),
            commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer()
        }.Schedule(m_StepPhysicsWorldSystem.Simulation, ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        return job;
    }
}

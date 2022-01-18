using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;



// This system sets the PhysicsGravityFactor of any dynamic body that enters a Trigger Volume.
// A Trigger Volume is defined by a PhysicsShapeAuthoring with the `Is Trigger` flag ticked and a
// TriggerGravityFactor behaviour added.
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class DestroyEntityAuthoringSystem : SystemBase
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;

    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    struct DestroyEntityAuthoringJob : ITriggerEventsJob 
    {
        [ReadOnly] public ComponentDataFromEntity<TriggerGravityFactor> TriggerGravityFactorGroup; //Used to Check For Triggers
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup; //Used to Check For Dynamic Bodies
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


            Debug.Log("Trigger Colliding With: "+dynamicEntity);
        }
    }
    protected override void OnUpdate()
    {
        Dependency = new DestroyEntityAuthoringJob
        {
            //Gets References to everything in the scene with these Data Components
            TriggerGravityFactorGroup = GetComponentDataFromEntity<TriggerGravityFactor>(true),
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
    ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);
    }
}

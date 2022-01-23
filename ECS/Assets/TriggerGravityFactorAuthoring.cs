using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public struct TriggerGravityFactor : IComponentData
{
    public float GravityFactor;
    public float DampingFactor;
}

//Adds Data Components to the object that this script is on
public class TriggerGravityFactorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float GravityFactor = 0f;
    public float DampingFactor = 0.9f;

    void OnEnable() {}

    //Called when the game object is converted to an Entity
    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            dstManager.AddComponentData(entity, new TriggerGravityFactor()
            {
                GravityFactor = GravityFactor,
                DampingFactor = DampingFactor,
            });
        }
    }
}


// This system sets the PhysicsGravityFactor of any dynamic body that enters a Trigger Volume.
// A Trigger Volume is defined by a PhysicsShapeAuthoring with the `Is Trigger` flag ticked and a
// TriggerGravityFactor behaviour added.
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerGravityFactorSystem : SystemBase
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;
    EntityQuery m_TriggerGravityGroup;

    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        //Look for everything with the TriggerGravityFactor Component
        m_TriggerGravityGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(TriggerGravityFactor)
            }
        });
    }

    struct TriggerGravityFactorJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<TriggerGravityFactor> TriggerGravityFactorGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> PhysicsGravityFactorGroup;
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;

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

            //Get the TriggerGravityFactor Component from the Trigger Entity
            var triggerGravityComponent = TriggerGravityFactorGroup[triggerEntity];
            // tweak PhysicsGravityFactor
            {
                //Gets the component from the Dynamic Entity
                var component = PhysicsGravityFactorGroup[dynamicEntity];
                //Sets the component value to be whatever the value on the trigger is
                component.Value = triggerGravityComponent.GravityFactor;
                //Finalises the changes
                PhysicsGravityFactorGroup[dynamicEntity] = component;
            }
            // damp velocity
            {
                var component = PhysicsVelocityGroup[dynamicEntity];
                component.Linear *= triggerGravityComponent.DampingFactor;
                PhysicsVelocityGroup[dynamicEntity] = component;
            }
        }
    }

    protected override void OnUpdate()
    {
        //If there is nothing in the scene with the TriggerGravityFactor Component then stop
        if (m_TriggerGravityGroup.CalculateEntityCount() == 0)
        {
            Debug.Log("Nothing Was Found");
            return;
        }

        Dependency = new TriggerGravityFactorJob
        {
            //Gets References to everything in the scene with these Data Components
            TriggerGravityFactorGroup = GetComponentDataFromEntity<TriggerGravityFactor>(true),
            PhysicsGravityFactorGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(),
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
            ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);
    }
}

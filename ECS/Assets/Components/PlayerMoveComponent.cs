using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerMoveComponent : IComponentData
{
    public float moveSpeed;
    public float3 targetDirection;
}

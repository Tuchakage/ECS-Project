using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemyTag : IComponentData
{
    public Entity target;
}
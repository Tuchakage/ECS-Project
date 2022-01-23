using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnComponent : IComponentData
{
    public Entity spawnPrefab;
    public Entity playerPrefab;
}

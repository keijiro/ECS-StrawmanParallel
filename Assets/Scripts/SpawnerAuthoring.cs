using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float Radius;
    public int Count;
    public uint Seed;
}

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject _prefab = null;
    public float _radius = 1;
    public int _count = 10;
    public uint _seed = 100;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring src)
        {
            var data = new Spawner()
            {
                Prefab = GetEntity(src._prefab, TransformUsageFlags.Dynamic),
                Radius = src._radius,
                Count = src._count,
                Seed = src._seed
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}

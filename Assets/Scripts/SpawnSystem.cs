using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
      => state.RequireForUpdate<Config>();

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var instances = state.EntityManager.Instantiate
          (config.Prefab, config.SpawnCount, Allocator.Temp);

        var rand = new Random(config.RandomSeed);
        foreach (var entity in instances)
        {
            var xform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            var dancer = SystemAPI.GetComponentRW<Dancer>(entity);
            var walker = SystemAPI.GetComponentRW<Walker>(entity);

            xform.ValueRW = LocalTransform.FromPositionRotation
              (rand.NextOnDisk() * config.SpawnRadius, rand.NextYRotation());

            dancer.ValueRW = Dancer.Random(rand.NextUInt());
            walker.ValueRW = Walker.Random(rand.NextUInt());
        }

        state.Enabled = false;
    }
}

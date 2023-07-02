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

    [BurstCompile(CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var random = new Random(config.RandomSeed);

        var instances = state.EntityManager.Instantiate
          (config.Prefab, config.SpawnCount, Allocator.Temp);

        foreach (var entity in instances)
        {
            ref var xform = ref SystemAPI.GetComponentRW<LocalTransform>(entity).ValueRW;
            ref var dancer = ref SystemAPI.GetComponentRW<Dancer>(entity).ValueRW;
            ref var walker = ref SystemAPI.GetComponentRW<Walker>(entity).ValueRW;
            xform.Position = random.NextOnDisk() * config.SpawnRadius;
            xform.Rotation = random.NextYRotation();
            dancer.Speed = random.NextFloat(1, 8);
            walker.ForwardSpeed = random.NextFloat(0.1f, 0.8f);
            walker.AngularSpeed = random.NextFloat(0.5f, 4);
        }

        state.Enabled = false;
    }
}

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SpawnerSystem : ISystem
{
    static float3 RandomOnDisk(ref Random random, float radius)
    {
        while (true)
        {
            var v = random.NextFloat2(-1, 1);
            if (math.length(v) <= 1) return math.float3(v.x, 0, v.y) * radius;
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (spawner, local) in
                 SystemAPI.Query<RefRO<Spawner>,
                                 RefRO<LocalTransform>>())
        {
            var center = local.ValueRO.Position;
            var radius = spawner.ValueRO.Radius;
            var count = spawner.ValueRO.Count;

            var random = new Random(spawner.ValueRO.Seed);
            random.NextFloat4();

            using var instances = new NativeArray<Entity>(count, Allocator.Temp);
            state.EntityManager.Instantiate(spawner.ValueRO.Prefab, instances);

            foreach (var entity in instances)
            {
                ref var xform = ref SystemAPI.GetComponentRW<LocalTransform>(entity).ValueRW;
                ref var dancer = ref SystemAPI.GetComponentRW<Dancer>(entity).ValueRW;
                ref var walker = ref SystemAPI.GetComponentRW<Walker>(entity).ValueRW;
                xform.Position = center + RandomOnDisk(ref random, radius);
                xform.Rotation = quaternion.RotateY(random.NextFloat(math.PI * 2));
                dancer.Speed = random.NextFloat(1, 8);
                walker.ForwardSpeed = random.NextFloat(0.1f, 0.8f);
                walker.AngularSpeed = random.NextFloat(0.5f, 4);
            }
        }

        state.Enabled = false;
    }
}

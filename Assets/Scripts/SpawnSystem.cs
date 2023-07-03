#define USE_ENTITIES_FOREACH

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SpawnSystem : SystemBase
{
    protected override void OnCreate()
      => RequireForUpdate<Config>();

    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();

        var instances = EntityManager.Instantiate
          (config.Prefab, config.SpawnCount, Allocator.Temp);

#if USE_ENTITIES_FOREACH
        Entities.ForEach((int entityInQueryIndex,
                          ref LocalTransform xform,
                          ref Dancer dancer,
                          ref Walker walker) =>
        {
            var random = Random.CreateFromIndex
              (config.RandomSeed + (uint)entityInQueryIndex);

            xform.Position = random.NextOnDisk() * config.SpawnRadius;
            xform.Rotation = random.NextYRotation();

            dancer.Speed = random.NextFloat(1, 8);

            walker.ForwardSpeed = random.NextFloat(0.1f, 0.8f);
            walker.AngularSpeed = random.NextFloat(0.5f, 4);
        })
        .ScheduleParallel();
#else
        var random = new Random(config.RandomSeed);
        foreach (var entity in instances)
        {
            var xform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            var dancer = SystemAPI.GetComponentRW<Dancer>(entity);
            var walker = SystemAPI.GetComponentRW<Walker>(entity);

            xform.ValueRW.Position = random.NextOnDisk() * config.SpawnRadius;
            xform.ValueRW.Rotation = random.NextYRotation();

            dancer.ValueRW.Speed = random.NextFloat(1, 8);

            walker.ValueRW.ForwardSpeed = random.NextFloat(0.1f, 0.8f);
            walker.ValueRW.AngularSpeed = random.NextFloat(0.5f, 4);
        }
#endif

        Enabled = false;
    }
}

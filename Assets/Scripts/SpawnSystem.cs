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
            var rnd = Random.CreateFromIndex
              (config.RandomSeed + (uint)entityInQueryIndex);

            xform = LocalTransform.FromPositionRotation
              (rnd.NextOnDisk() * config.SpawnRadius, rnd.NextYRotation());

            dancer = Dancer.Random(rnd.NextUInt());
            walker = Walker.Random(rnd.NextUInt());
        })
        .ScheduleParallel();
#else
        var rnd = new Random(config.RandomSeed);
        foreach (var entity in instances)
        {
            var xform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            var dancer = SystemAPI.GetComponentRW<Dancer>(entity);
            var walker = SystemAPI.GetComponentRW<Walker>(entity);

            xform.ValueRW = LocalTransform.FromPositionRotation
              (rnd.NextOnDisk() * config.SpawnRadius, rnd.NextYRotation());

            dancer.ValueRW = Dancer.Random(rnd.NextUInt());
            walker.ValueRW = Walker.Random(rnd.NextUInt());
        }
#endif

        Enabled = false;
    }
}

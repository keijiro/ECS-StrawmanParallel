using Unity.Burst;
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

        /*
        var random = new Random(config.RandomSeed);
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
        */

        Entities.ForEach(
            (int entityInQueryIndex,
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
            }
        ).ScheduleParallel();

        Enabled = false;
    }
}

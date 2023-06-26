using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PulseSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new PulseUpdateJob()
          { Time = (float)SystemAPI.Time.ElapsedTime };
        job.ScheduleParallel();
    }
}

[BurstCompile(CompileSynchronously = true)]
partial struct PulseUpdateJob : IJobEntity
{
    public float Time;

    void Execute(ref LocalTransform xform,
                 in Dancer dancer,
                 in Walker walker)
    {
        var t = dancer.Speed * Time;
        xform.Scale = 1.1f - 0.3f * math.abs(math.cos(t));
    }
}

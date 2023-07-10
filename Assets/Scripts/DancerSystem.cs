using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct DancerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new DancerUpdateJob()
          { Elapsed = (float)SystemAPI.Time.ElapsedTime };
        job.ScheduleParallel();
    }
}

[BurstCompile]
partial struct DancerUpdateJob : IJobEntity
{
    public float Elapsed;

    void Execute(in Dancer dancer,
                 ref LocalTransform xform)
    {
        var t = dancer.Speed * Elapsed;
        var y = math.abs(math.sin(t)) * 0.1f;
        var bank = math.cos(t) * 0.5f;

        var fwd = xform.Forward();
        var rot = quaternion.AxisAngle(fwd, bank);
        var up = math.mul(rot, math.float3(0, 1, 0));

        xform.Position.y = y;
        xform.Rotation = quaternion.LookRotation(fwd, up);
    }
}

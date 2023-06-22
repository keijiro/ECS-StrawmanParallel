using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct DancerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
      => new DancerUpdateJob() { Time = (float)SystemAPI.Time.ElapsedTime }
         .ScheduleParallel();
}

[BurstCompile(CompileSynchronously = true)]
partial struct DancerUpdateJob : IJobEntity
{
    public float Time;

    void Execute(ref LocalTransform xform,
                 in Dancer dancer)
    {
        var t = dancer.Speed * Time;
        var y = math.abs(math.sin(t)) * 0.1f;
        var bank = math.cos(t) * 0.5f;

        var fwd = xform.Forward();
        var rot = quaternion.AxisAngle(fwd, bank);
        var up = math.mul(rot, math.float3(0, 1, 0));

        xform.Position.y = y;
        xform.Rotation = quaternion.LookRotation(fwd, up);
    }
}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct WalkerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new WalkerUpdateJob()
          { DeltaTime = SystemAPI.Time.DeltaTime };
        job.ScheduleParallel();
    }
}

[BurstCompile]
partial struct WalkerUpdateJob : IJobEntity
{
    public float DeltaTime;

    void Execute(in Walker walker,
                 ref LocalTransform xform)
    {
        var rot = quaternion.RotateY(walker.AngularSpeed * DeltaTime);
        var fwd = math.mul(rot, xform.Forward());
        xform.Position += fwd * walker.ForwardSpeed * DeltaTime;
        xform.Rotation = quaternion.LookRotation(fwd, xform.Up());
    }
}

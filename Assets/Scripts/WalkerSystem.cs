using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct WalkerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
      => new WalkerUpdateJob() { DeltaTime = SystemAPI.Time.DeltaTime }
         .ScheduleParallel();
}

[BurstCompile(CompileSynchronously = true)]
partial struct WalkerUpdateJob : IJobEntity
{
    public float DeltaTime;

    void Execute(ref LocalTransform xform,
                 in Walker walker)
    {
        var rot = quaternion.RotateY(walker.AngularSpeed * DeltaTime);
        var fwd = math.mul(rot, xform.Forward());
        xform.Position += fwd * walker.ForwardSpeed * DeltaTime;
        xform.Rotation = quaternion.LookRotation(fwd, xform.Up());
    }
}

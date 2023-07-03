using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct Walker : IComponentData
{
    public float ForwardSpeed;
    public float AngularSpeed;

    public static Walker Random(uint seed)
    {
        var random = new Random(seed);
        return new Walker() { ForwardSpeed = random.NextFloat(0.1f, 0.8f),
                              AngularSpeed = random.NextFloat(0.5f, 4) };
    }
}

public class WalkerAuthoring : MonoBehaviour
{
    public float ForwardSpeed = 1;
    public float AngularSpeed = 1;

    class Baker : Baker<WalkerAuthoring>
    {
        public override void Bake(WalkerAuthoring src)
        {
            var data = new Walker()
            {
                ForwardSpeed = src.ForwardSpeed,
                AngularSpeed = src.AngularSpeed
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}

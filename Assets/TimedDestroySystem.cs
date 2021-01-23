using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;

public class TimedDestroySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;
        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, ref Translation position, ref LifeTimeData lifetimeData) =>
        {
            lifetimeData.lifeLeft -= dt;
            if (lifetimeData.lifeLeft <= 0f)
            {
                EntityManager.DestroyEntity(entity);
            }
        }).Run();

        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, ref Translation position, ref VirusData virusData) =>
        {
            if(!virusData.alive)
            {
                for (int i = 0; i < 100; i++)
                {
                    float3 offset = (float3)UnityEngine.Random.insideUnitSphere * 2.0f;
                    var splat = ECSManager.manager.Instantiate(ECSManager.whiteBlood);
                    float3 randomDir = new float3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
                    ECSManager.manager.SetComponentData(splat, new Translation { Value = position.Value + offset });
                    ECSManager.manager.SetComponentData(splat, new PhysicsVelocity { Linear = randomDir * 2 });
                }



                EntityManager.DestroyEntity(entity);
            }
        }).Run();



        return inputDeps;
    }
}





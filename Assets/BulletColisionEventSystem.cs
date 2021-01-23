using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;


[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BulletColisionEventSystem : JobComponentSystem
{
    BuildPhysicsWorld m_BuildPhysicsWorldSytem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;

    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSytem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    struct CollisionEventImpulseJob: ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<BulletData> BulletGroup;
        public ComponentDataFromEntity<VirusData> VirusGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isTargetA = VirusGroup.HasComponent(entityA);
            bool isTargetB = VirusGroup.HasComponent(entityB);

            bool isBulletA = BulletGroup.HasComponent(entityA);
            bool isBulletB = BulletGroup.HasComponent(entityB);

            if (isBulletA && isTargetB)
            {
                var aliveComponent = VirusGroup[entityB];
                aliveComponent.alive = false;
                VirusGroup[entityB] = aliveComponent;
            }

            if (isBulletB && isTargetA)
            {
                var aliveComponent = VirusGroup[entityA];
                aliveComponent.alive = false;
                VirusGroup[entityA] = aliveComponent;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new CollisionEventImpulseJob
        {
            BulletGroup = GetComponentDataFromEntity<BulletData>(),
            VirusGroup = GetComponentDataFromEntity<VirusData>()
        }.Schedule(m_StepPhysicsWorldSystem.Simulation, ref m_BuildPhysicsWorldSytem.PhysicsWorld, inputDeps);

        jobHandle.Complete();

        return jobHandle;
    }
}

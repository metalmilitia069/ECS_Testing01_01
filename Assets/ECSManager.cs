using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ECSManager : MonoBehaviour
{
    public static EntityManager manager;
    public GameObject virusPrefab;
    public GameObject redBloodPrefab;
    public GameObject whiteBloodPrefab;
    public GameObject bulletPrefab;
    public GameObject player;

    int numVirus = 500;
    int numBlood = 500;
    int numBullet = 10;
    BlobAssetStore store;

    Entity bullet;
    public static Entity whiteBlood;

    // Start is called before the first frame update
    void Start()
    {
        store = new BlobAssetStore();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        Entity virus = GameObjectConversionUtility.ConvertGameObjectHierarchy(virusPrefab, settings);
        Entity redBlood = GameObjectConversionUtility.ConvertGameObjectHierarchy(redBloodPrefab, settings);
        bullet = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);
        whiteBlood = GameObjectConversionUtility.ConvertGameObjectHierarchy(whiteBloodPrefab, settings);

        for (int i = 0; i < numVirus; i++)
        {
            var instance = manager.Instantiate(virus);
            float x = UnityEngine.Random.Range(-50, 50);
            float y = UnityEngine.Random.Range(-50, 50);
            float z = UnityEngine.Random.Range(-50, 50);

            float3 position = new float3(x, y, z);

            manager.SetComponentData(instance, new Translation { Value = position });

            float rSpeed = UnityEngine.Random.Range(1, 10) / 10.0f;
            manager.SetComponentData(instance, new FloatData { speed = rSpeed });
        }

        for (int i = 0; i < numBlood; i++)
        {
            var instance = manager.Instantiate(redBlood);
            float x = UnityEngine.Random.Range(-50, 50);
            float y = UnityEngine.Random.Range(-50, 50);
            float z = UnityEngine.Random.Range(-50, 50);

            float3 position = new float3(x, y, z);

            manager.SetComponentData(instance, new Translation { Value = position });

            float rSpeed = UnityEngine.Random.Range(1, 10) / 10.0f;
            manager.SetComponentData(instance, new FloatData { speed = rSpeed });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < numBullet; i++)
            {
                var instance = manager.Instantiate(bullet);
                var startPos = player.transform.position + UnityEngine.Random.insideUnitSphere * 2;
                manager.SetComponentData(instance, new Translation { Value = startPos });
                manager.SetComponentData(instance, new Rotation { Value = player.transform.rotation });
            }
        }
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}

using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GenerateObject : MonoBehaviour
{

    [Header("Objects")]
    [SerializeField, Tooltip("Area to used where the objects will be created.")]
    private BoxCollider area;
    [SerializeField, Tooltip("Possible objects to be created in the area.")]
    private GameObject[] gameObjectToBeCreated;

    [SerializeField, Tooltip("Number of objects to be created.")]
    private uint count;

    [Space(10)] 
    [Header("Variation")] 
    [SerializeField]
    private Vector3 randomRotationMinimal;
    [SerializeField] 
    private Vector3 randomRotationMaximal;
    [Header("Obstruction Settings")]
    [SerializeField, Tooltip("Layers considered as obstacles for object spawning.")]
    private LayerMask obstructionLayers;

   
    public void RemoveChildren()
    {
        for (var i = transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

  // it does a check before generating to ensure it dont generate anything inside another object
  // if it collides with another object, pick another position
private bool IsPositionValid(Vector3 position, GameObject prefab)
{
    var renderer = prefab.GetComponentInChildren<Renderer>();
    if (renderer == null) return true; // Assume it's fine if no renderer

    Vector3 size = renderer.bounds.size;
    Vector3 halfExtents = size * 0.5f;

    Collider[] overlaps = Physics.OverlapBox(position, halfExtents, Quaternion.identity, obstructionLayers);
    return overlaps.Length == 0;
}


   public List<GameObject> RegenerateObjects()
{
    RemoveChildren();

    var newObjects = new List<GameObject>();
    int attemptsPerObject = 3;

    for (uint i = 0; i < count; i++)
    {
        GameObject prefab = gameObjectToBeCreated[Random.Range(0, gameObjectToBeCreated.Length)];

        Vector3 spawnPos;
        Quaternion spawnRot;
        

        for (int attempt = 0; attempt < attemptsPerObject; attempt++)
        {
            spawnPos = GetRandomPositionInWorldBounds();
            spawnRot = GetRandomRotation();

            if (IsPositionValid(spawnPos, prefab))
            {
                GameObject created = Instantiate(prefab, spawnPos, spawnRot);
                created.transform.parent = transform;
                newObjects.Add(created);
                break;
            }
        }


    }

    return newObjects;
}

      private Vector3 GetRandomPositionInWorldBounds()
    {
        var randomPoint = new Vector3(
            Random.Range(area.bounds.min.x, area.bounds.max.x),
            transform.position.y,
            Random.Range(area.bounds.min.z, area.bounds.max.z)
        );
        return randomPoint;
    }

  
    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(randomRotationMinimal.x, randomRotationMaximal.x),
            Random.Range(randomRotationMinimal.y, randomRotationMaximal.y),
            Random.Range(randomRotationMinimal.z, randomRotationMaximal.z));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager Instance;
    public static List<GameObject> School = new List<GameObject>();
    public static Bounds SwimLimit;
    [Header("Spawn Settings")]
    [SerializeField] private GameObject fishPrefab; // Hate Unity not letting me import the fish assets, I will go rogue and make them cubes in protest.
    [SerializeField, Range(5, 100)] private int fishCount;
    [SerializeField] private Vector3 spawnLimit = new Vector3(5, 5, 5);
    [SerializeField] private int swimLimitsSize = 2;


    [Header("Fish Settings")]
    [Range(0.1f, 1f)] public float MinSpeed = 0.3f;
    [Range(1f, 5f)] public float MaxSpeed = 1.6f;
    [Range(0.2f, 10.0f)] public float NeighbourDistance = 1f;
    [Range(0.5f, 2f)] public float AvoidDistance = 1f;
    [Range(1f, 5f)] public float RotationSpeed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SwimLimit = new Bounds(transform.position, spawnLimit * swimLimitsSize);
        for (int i=0; i<fishCount; i++){
            Vector3 spawnBuffer = GetSpawnBuffer();
            School.Add(Instantiate(fishPrefab, transform.position + spawnBuffer, Quaternion.identity));
        }
    }

    Vector3 GetSpawnBuffer() => new Vector3(Random.Range(-spawnLimit.x, spawnLimit.x), Random.Range(-spawnLimit.y, spawnLimit.y), Random.Range(-spawnLimit.z, spawnLimit.z));
}

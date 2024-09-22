using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float lookTime = 1f;
    [SerializeField] private float initialLookTime = 3f;

    private UnityEngine.AI.NavMeshAgent nav;

    void Start()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    bool CanSeePlayer(){
        Vector3 diff = player.position - transform.position;
        diff.Normalize();
        return Vector3.Dot(transform.forward, diff) > 0;
    }
    void Update()
    {
        if (CanSeePlayer()){
            Debug.Log("Can See Player");
        }
        else{
            Debug.Log("Cant See Player");
        }
    }
}

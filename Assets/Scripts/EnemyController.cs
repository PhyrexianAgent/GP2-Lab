using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float lookTime = 1f;
    [SerializeField] private float initialLookTime = 3f;
    [SerializeField, Range(0, 1)] private float minimumDot = 0.1f;

    private UnityEngine.AI.NavMeshAgent nav;

    void Start()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    float MinValue(float value, float min){
        if (value > 0 && value < min)
            value = -1;
        return value;
    }

    bool CanSeePlayer(){
        Vector3 diff = player.position - transform.position;
        diff.Normalize();
        //Debug.Log(Vector3.Dot(transform.forward, diff));
        return MinValue(Vector3.Dot(transform.forward, diff), minimumDot) > 0;
    }
    void Update()
    {
        if (CanSeePlayer()){
            Debug.Log("Can See Player");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    public float Speed;
    private List<Transform> group = new List<Transform>();
    private bool isTurning = false;
    void Start()
    {
        Speed = Random.Range(FlockManager.Instance.MinSpeed, FlockManager.Instance.MaxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        isTurning = !FlockManager.SwimLimit.Contains(transform.position);
        if (isTurning){
            Vector3 newDir = FlockManager.Instance.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDir), FlockManager.Instance.RotationSpeed);
        }
        else{
            if (Random.Range(0, 100) < 40){
                ResetSpeed();
            }
            ApplyRules();
        }
        transform.Translate(0, 0, Speed * Time.deltaTime);
    }

    void ResetSpeed(){
        Speed = Random.Range(FlockManager.Instance.MinSpeed, FlockManager.Instance.MaxSpeed);
    }

    void ApplyRules(){
        Vector3 groupCenter = Vector3.zero;
        Vector3 groupAvoid = Vector3.zero;
        float totalGroupSpeed = 0;
        int groupSize = 0;
        group.Clear();

        foreach(GameObject fish in FlockManager.School){
            if (fish == this) continue;

            float neighbourDistance = Vector3.Distance(fish.transform.position, transform.position);
            if (neighbourDistance >= FlockManager.Instance.NeighbourDistance) continue;

            groupCenter += fish.transform.position;
            groupSize++;
            group.Add(fish.transform);

            if (neighbourDistance < FlockManager.Instance.AvoidDistance)
            {
                groupAvoid += transform.position - fish.transform.position;
            }

            totalGroupSpeed += fish.GetComponent<FlockAgent>().Speed;
        }

        if (groupSize > 0){
            groupCenter = groupCenter / groupSize;
            Speed = totalGroupSpeed / groupSize;
            if (Speed > FlockManager.Instance.MaxSpeed) Speed = FlockManager.Instance.MaxSpeed;
            Vector3 newDirection = (groupCenter + groupAvoid) - transform.position;
            if (newDirection != Vector3.zero){
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDirection),
                    FlockManager.Instance.RotationSpeed * Time.deltaTime);
            }
        }
    }
}

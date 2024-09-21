using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] private float lookSensitivity = 100f;
    [SerializeField] private float clampAngle = 80f;
    [SerializeField] private Camera camera;

    private float lookRotX;
    void Start()
    {
        lookRotX = transform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        float rotAdd = Mathf.Clamp(Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime, -clampAngle, clampAngle);

        lookRotX += rotAdd;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotX, 0f);
    }
}

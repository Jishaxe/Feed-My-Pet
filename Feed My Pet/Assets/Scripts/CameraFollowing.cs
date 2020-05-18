using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Transform target;
    public Transform centerPoint;

    public float xSpeed;
    public Vector2 xLimits;

    public float ySpeed;
    public Vector2 yLimits;

    public Vector2 zLimits;
    public float zSpeed;

    public float followSpeed;
    
    Vector3 currentTracked;
    Quaternion centerRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        currentTracked = target.transform.position;
        centerRotation = transform.localRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTracked = Vector3.Lerp(currentTracked, target.transform.position, followSpeed);

        float xOffset = (centerPoint.position.z - currentTracked.z) * xSpeed;
        float yOffset = (centerPoint.position.x - currentTracked.x) * ySpeed;
        float zOffset = (currentTracked.z - centerPoint.position.z) * zSpeed;

        transform.localRotation = centerRotation * Quaternion.Euler(Mathf.Clamp(yOffset, yLimits.x, yLimits.y), Mathf.Clamp(xOffset, xLimits.x, xLimits.y), Mathf.Clamp(zOffset, zLimits.x, zLimits.y));
    }
}

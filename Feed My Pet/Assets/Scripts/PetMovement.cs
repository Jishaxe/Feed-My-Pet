using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetMovement : MonoBehaviour
{
    JellyMesh _jellyMesh;

    /// <summary>
    /// Movement direction in world space
    /// </summary>
    public Vector3 movementDirection;
    public float movementForce;
    public float centerMovementForce;

    // Start is called before the first frame update
    void Start()
    {
        _jellyMesh = GetComponent<JellyMesh>();
    }

    /// <summary>
    /// Finds out which 4 reference points are at the top of the cube in world space
    /// </summary>
    /// <returns>4 highest reference points</returns>
    List<JellyMesh.ReferencePoint> GetTopFourReferencePoints() {
        List<JellyMesh.ReferencePoint> refPointsSortedByY = _jellyMesh.ReferencePoints
        .OrderByDescending(refPoint => refPoint.GameObject.transform.position.y)
        .Take(4)
        .ToList();

        return refPointsSortedByY;
    }


    /// <summary>
    /// Finds out which 4 reference points are at the bottom of the cube in world space
    /// </summary>
    /// <returns>4 highest reference points</returns>
    List<JellyMesh.ReferencePoint> GetBottomFourReferencePoints() {
        List<JellyMesh.ReferencePoint> refPointsSortedByY = _jellyMesh.ReferencePoints
        .OrderBy(refPoint => refPoint.GameObject.transform.position.y)
        .Take(4)
        .ToList();

        return refPointsSortedByY;
    }

    
    void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        foreach (JellyMesh.ReferencePoint referencePoint in _jellyMesh.ReferencePoints) {
            Gizmos.DrawWireSphere(referencePoint.GameObject.transform.position, 0.1f);
            //Gizmos.DrawLine(referencePoint.transform.position, referencePoint.transform.position + movementForce);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (JellyMesh.ReferencePoint referencePoint in GetTopFourReferencePoints()) {
            referencePoint.GameObject.GetComponent<Rigidbody>().AddForceAtPosition(movementDirection * movementForce, _jellyMesh.CentralPoint.transform.position, ForceMode.Impulse);
        }

        foreach (JellyMesh.ReferencePoint referencePoint in GetBottomFourReferencePoints()) {
            referencePoint.GameObject.GetComponent<Rigidbody>().AddForceAtPosition(-movementDirection * movementForce, _jellyMesh.CentralPoint.transform.position, ForceMode.Impulse);
        }

        _jellyMesh.CentralPoint.GameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(movementDirection.z, 0, movementDirection.x) * centerMovementForce);
    }

    /// <summary>
    /// Whether the pet has settled and stopped moving so much
    /// </summary>
    /// <returns></returns>
    public bool IsSettled() {
        float total = 0;

        foreach (JellyMesh.ReferencePoint referencePoint in _jellyMesh.ReferencePoints) {
            total += referencePoint.GameObject.GetComponent<Rigidbody>().velocity.magnitude;
        }

        return total < 3;
    }
}

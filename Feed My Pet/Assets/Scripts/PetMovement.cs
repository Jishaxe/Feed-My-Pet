using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetMovement : MonoBehaviour
{
    JellyMesh _jellyMesh;
    PetStats _stats;
    

    /// <summary>
    /// Movement direction in world space
    /// </summary>
    public Vector3 movementDirection;
    public float centerMovementForce;

    [Space(30)]
    public float _wanderHeading; // 0 is facing right
    bool _wanderDirection = true; // true is +, false is -
    public bool _wanderMoving = true; // whether we are moving right now
    [SerializeField] float _wanderMovingSwitchChance;
    [SerializeField] float _wanderChangeDirectionChance;
    [SerializeField] float _wanderChangeRate; // how fast the wander heading changes in the direction

    // Start is called before the first frame update
    void Start()
    {
        _jellyMesh = GetComponent<JellyMesh>();
        _stats = GetComponent<PetStats>();
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
        float movementForce = _stats.GetMovementForce();

        foreach (JellyMesh.ReferencePoint referencePoint in GetTopFourReferencePoints()) {
            referencePoint.GameObject.GetComponent<Rigidbody>().AddForceAtPosition(movementDirection * movementForce, _jellyMesh.CentralPoint.transform.position, ForceMode.Impulse);
        }

        foreach (JellyMesh.ReferencePoint referencePoint in GetBottomFourReferencePoints()) {
            referencePoint.GameObject.GetComponent<Rigidbody>().AddForceAtPosition(-movementDirection * movementForce, _jellyMesh.CentralPoint.transform.position, ForceMode.Impulse);
        }

        _jellyMesh.CentralPoint.GameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(movementDirection.z, 0, movementDirection.x) * centerMovementForce);
    }

    /// <summary>
    /// Direction for random wander
    /// </summary>
    /// <returns></returns>
    public Vector3 Wander() {
        if (Random.value < _wanderChangeDirectionChance) _wanderDirection = !_wanderDirection;
        if (Random.value < _wanderMovingSwitchChance) _wanderMoving = !_wanderMoving;

        if (_wanderMoving) {
            float wanderChange = Random.Range(0f, _wanderChangeRate);
            
            if (_wanderDirection) {
                _wanderHeading += wanderChange;
            } else {
                _wanderHeading -= wanderChange;
            }


            _wanderHeading = Util.Clamp0360(_wanderHeading);

            Quaternion rotation = Quaternion.Euler(0, _wanderHeading, 0);
            return rotation * Vector3.right;
        } else {
            return Vector3.zero;
        }

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

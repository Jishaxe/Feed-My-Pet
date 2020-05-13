using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PetFaceDirection {
    UP, DOWN, LEFT, RIGHT, FORWARD, BACK
}

public class PetInteractor : MonoBehaviour
{
    [SerializeField] float _holdForce = 1f;
    [SerializeField] float _holdYOffset = 0.1f;

    bool _shouldBeEating = true;

    public bool isHoldingObject = false;
    public FoodObject currentHeldObject = null;
    float _holdDistance = 0.4f;
    float _prevObjectDrag; // store previous rb drag so we can restore after dropping
    PetFaceDirection _heldFaceDirection;

    /// <summary>
    /// The Vector3 direction that the pet is currently holding at
    /// </summary>
    /// <value></value>
    Vector3 _holdDirection {
        get {
            return PetFaceDirectionToVector(_heldFaceDirection);
        }
    }

    PetMovement _petMovement;
 

    /// <summary>
    /// Whether we should be trying to eat food right now
    /// If set to false while eating, it will drop the food
    /// </summary>
    public bool shouldBeEating {
        get {
            return _shouldBeEating;
        }

        set {
            _shouldBeEating = value;
        }
    }

    /// <summary>
    /// How close we need to be to an object to interact with it
    /// </summary>
    public float interactionDistance;

    Coroutine _eatFoodCoroutine;

    /// <summary>
    /// Start eating the food with a sequence of picking up the food, taking bites until finished, and dropping it
    /// </summary>
    /// <param name="food"></param>
    /// <returns>true if started eating food, false if already eating a food</returns>
    public bool EatFood(FoodObject food) {
        if (_eatFoodCoroutine != null) return false;

        _eatFoodCoroutine = StartCoroutine(EatFoodCoroutine(food));

        return true;
    }

    public void PickUpObject(FoodObject food) {
        if (currentHeldObject != null) {
            if (currentHeldObject == food) return; // already holding this food
            else DropObject(); // drop whatever was being held before
        }

        _prevObjectDrag = food.GetComponent<Rigidbody>().drag;
        food.GetComponent<Rigidbody>().drag = 10f; // stop it from osciliating when being picked up
        _heldFaceDirection = GetClosestFacePosition(food.transform.position);
        isHoldingObject = true;
        currentHeldObject = food;
    }

    public void DropObject() {
        Debug.Log("dropping object " + currentHeldObject.name);
        if (currentHeldObject == null) return;

        isHoldingObject = false;
        currentHeldObject.GetComponent<Rigidbody>().drag = _prevObjectDrag;
        currentHeldObject = null;
    }

    IEnumerator EatFoodCoroutine(FoodObject food) {
        PickUpObject(food);

         _holdDistance = 0.3f; 

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => _petMovement.IsSettled());


        while (food.bitesLeft > 0 && shouldBeEating) {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.8f));
            _holdDistance = 0.2f;
            yield return new WaitForSeconds(0.2f);

            // calculate plane to use for slicing the food
            Vector3 planePosition = this.transform.position + _holdDirection * (_holdDistance + 0.05f);
            food.Bite(planePosition, _holdDirection);

            yield return new WaitForSeconds(0.8f);

            _holdDistance = 0.3f;


            gameObject.SendMessage("FoodBitten", food);
        }

        DropObject();

        if (food.bitesLeft == 0) {
            gameObject.SendMessage("FoodEaten", food);
        }

        _eatFoodCoroutine = null;
    }

    void Update() {
        if (isHoldingObject) {
           
            // if the holding face is pointing up or down, find a horizontal one
            // for example, if the cube fell over whilst holding
            if (Vector3.Dot(_holdDirection, Vector3.up) > 0.5f || Vector3.Dot(_holdDirection, Vector3.down) > 0.5f) {
                _heldFaceDirection = GetClosestFacePosition(currentHeldObject.transform.position);
            }

            Vector3 targetPoint = this.transform.position + new Vector3(0, _holdYOffset, 0) + (_holdDirection * _holdDistance);
            Vector3 force = targetPoint - currentHeldObject.transform.position;
            //if (force.magnitude > 1) force.Normalize();

            force *= _holdForce;

            currentHeldObject.GetComponent<Rigidbody>().AddForce(force);

            Debug.DrawLine(this.transform.position, targetPoint, Color.red);
        }
    }

    void Start() {
        _petMovement = GetComponent<PetMovement>();
    }

    /// <summary>
    /// Finds the closest cube face to the given position, excluding the ones facing the top and bottom
    /// </summary>
    /// <param name="position"></param>
    /// <returns>PetFaceDirection representing the face side</returns>
    PetFaceDirection GetClosestFacePosition(Vector3 position) {
        Vector3 closestDirection = Vector3.zero;
        PetFaceDirection closestFaceDirection = 0;

        // highest dot product of direction vector to position is most in line with item
        float highestDotProduct = -1;


        // iterate all the values of PetFaceDirection (up,down,left, right etc)
        for (int i = 0; i < System.Enum.GetValues(typeof(PetFaceDirection)).Length; i++) {
            PetFaceDirection petFaceDirection = (PetFaceDirection)i;

            // convert a face direction to a world space direction from that face
            Vector3 potentialDirection = PetFaceDirectionToVector(petFaceDirection);

            // If this direction points up or down, skip it
            if (Vector3.Dot(potentialDirection, Vector3.up) > 0.5f) continue;
            if (Vector3.Dot(potentialDirection, Vector3.down) > 0.5f) continue;


            float dot = Vector3.Dot(potentialDirection, position - this.transform.position);

            if (dot > highestDotProduct) {
                highestDotProduct = dot;
                closestFaceDirection = petFaceDirection;
                closestDirection = potentialDirection;
            }
        }
        
        return closestFaceDirection;
    }

    Vector3 PetFaceDirectionToVector(PetFaceDirection faceDirection) {
        switch (faceDirection) {
            case PetFaceDirection.UP:
                return transform.up;
            case PetFaceDirection.DOWN:
                return -transform.up;
            case PetFaceDirection.LEFT:
                return -transform.right;
            case PetFaceDirection.RIGHT:
                return transform.right;
            case PetFaceDirection.FORWARD:
                return transform.forward;
            case PetFaceDirection.BACK:
                return -transform.forward;
        }

        throw new Exception("Invalid facedirection: " + faceDirection.ToString());
    }
}

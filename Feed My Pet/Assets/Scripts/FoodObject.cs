using System.Collections;
using System.Collections.Generic;
using EzySlice;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    public ParticleSystem munchFx;
    public FoodSpawner spawner;
    public GameObject meshObject;
    public int bitesLeft = 0;
    public float hungerPerBite = 0.2f;

    // stockpile this food belongs to, or null if not
    public StockpileArea stockpileArea;

    public void Bite(Vector3 slicePlanePosition, Vector3 slicePlaneDirection) {
        bitesLeft--;
        Debug.DrawLine(slicePlanePosition, slicePlanePosition + slicePlaneDirection, Color.blue, 1f);
        SlicedHull slicedHull = meshObject.Slice(slicePlanePosition, slicePlaneDirection, meshObject.GetComponent<Renderer>().material);

        if (slicedHull != null) {
            GameObject slicedMesh = slicedHull.CreateUpperHull();
            slicedMesh.GetComponent<Renderer>().material = meshObject.GetComponent<Renderer>().material;
            slicedMesh.transform.parent = this.transform;
            slicedMesh.transform.position = meshObject.transform.position;
            slicedMesh.transform.rotation = meshObject.transform.rotation;
            slicedMesh.transform.localScale = meshObject.transform.localScale;
            slicedMesh.tag = "Food";
            
            MeshCollider meshCollider = (MeshCollider)slicedMesh.AddComponent(typeof(MeshCollider));
            meshCollider.sharedMesh = slicedMesh.GetComponent<MeshFilter>().mesh;
            meshCollider.convex = true;

            meshObject.tag = "Untagged";
            Destroy(meshObject);
            meshObject = slicedMesh;

        }

        munchFx.Play();
    }

    void Start() {
        Color color = Random.ColorHSV(0f, 1f, 0.25f, 0.8f, 1f, 1f);
        GetComponentInChildren<Renderer>().material.color = color;
        var main = munchFx.main;
        main.startColor = color;
    }

    public void Eaten() {
        //GetComponentInChildren<Renderer>().enabled = false;
        munchFx.Emit(100);
        munchFx.gameObject.GetComponent<PSDieAfterLastParticle>().enabled = true;
        munchFx.gameObject.transform.parent = null;
        spawner.foodInScene.Remove(this);
    }
}

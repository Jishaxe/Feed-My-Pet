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

    [Space(30)]
    public AudioSource _audioSource;
    public SoundClipContainer _beanSounds;

    /// <summary>
    /// X us minimum relativeVelocity magnitute to play hitting sound, Y is maximum speed before volume is not increased any further
    /// </summary>
    public Vector2 hittingGroundThreshold;

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

    public void SetColor(Color color) {
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


    void OnCollisionEnter(Collision collision) {
        // if above threshold to play squelch sound
        if (collision.relativeVelocity.magnitude > hittingGroundThreshold.x) {
            //Debug.Log("collision with velocity " + collision.Collision.relativeVelocity.magnitude);
            // calculate a force value from 0 - 1, 0 being the lower threshold and 1 being the upper threshold
            float volume = (collision.relativeVelocity.magnitude / hittingGroundThreshold.y) + (hittingGroundThreshold.x / hittingGroundThreshold.y);

            volume = Mathf.Clamp01(volume);

            _audioSource.PlayOneShot(_beanSounds.GetRandomClip(), volume);
        }
    }
}

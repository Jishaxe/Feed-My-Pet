using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// represents a food before it's spawned
/// </summary>
public class FoodSpawnDefinition {
    public Color color;
}

public class FoodSpawner : MonoBehaviour
{
    public FoodObject foodPrefab;
    public List<FoodObject> foodInScene;
    public Material indicatorMaterial;
    public Transform spawnPoint;
    public Text countText;
    [Space(30)]
    public AudioSource _audioSource;
    public SoundClipContainer _windingSounds;
    public SoundClipContainer _spawningSounds;
    [Space(30)]
    public int maxFoodInScene;
    public float spawnForce;
    
    /// <summary>
    /// Whether we are actively spawning or not
    /// </summary>
    public bool isOpen;
    
    bool _isFiring = false;
    Queue<FoodSpawnDefinition> _foodSpawnQueue = new Queue<FoodSpawnDefinition>();
    Animator _animator;

    void Start() {
        _animator = GetComponent<Animator>();

        SetIndicatorColor(Color.black);
    }

    public void QueueSpawn(int count) {
        // dont let queue get too big
        if (_foodSpawnQueue.Count > 100) return;

        for (int i = 0; i < count; i++) {
            FoodSpawnDefinition spawn = new FoodSpawnDefinition();
            spawn.color = Random.ColorHSV(0f, 1f, 0.25f, 0.8f, 1f, 1f);
            _foodSpawnQueue.Enqueue(spawn);
        }
    }
    
    IEnumerator StartFiring() {
        _isFiring = true;
        _animator.SetBool("Winding", true);
        _audioSource.PlayOneShot(_windingSounds.GetRandomClip());

        yield return new WaitForSeconds(1.5f);

        // Make sure we haven't been interrupted
        if (_foodSpawnQueue.Count == 0 || !isOpen) {
            _animator.SetBool("Winding", false);
            _isFiring = false;
            yield break;
        }
        
        _animator.SetBool("Firing", true);
        UpdateAnimatorSpeed();

        yield return new WaitUntil(() => _foodSpawnQueue.Count == 0 || !isOpen);

  
        SetIndicatorColor(Color.black);
        _isFiring = false;
        _animator.SetBool("Winding", false);
        _animator.SetBool("Firing", false);

        yield return new WaitForSeconds(1f);
        _animator.speed = 1f;
    }

    void SetIndicatorColor(Color color) {
        indicatorMaterial.color = color;
        indicatorMaterial.SetColor("_EmissionColor", color * 2f);
    }

    void UpdateAnimatorSpeed() {
        _animator.speed = (_foodSpawnQueue.Count / 100f);
        _animator.speed = Mathf.Clamp(_animator.speed, 0.7f, 100f);
    }

    /// <summary>
    /// Called by the animator when we are ready to spawn food
    /// </summary>
    public void AnimatorSpawnFood() {
        if (_foodSpawnQueue.Count == 0 || !isOpen || !_isFiring) return;



        UpdateAnimatorSpeed();
        
        FoodSpawnDefinition food = _foodSpawnQueue.Dequeue();
    
        FoodObject newFood = Instantiate(foodPrefab.gameObject).GetComponent<FoodObject>();
        newFood.transform.position = spawnPoint.position;
        newFood.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * spawnForce * (Random.value + 1f), ForceMode.Impulse);
        newFood.spawner = this;

        newFood.SetColor(food.color);

        foodInScene.Add(newFood);
    }

    public void AnimatorAboutToSpawn() {
        if (_foodSpawnQueue.Count == 0) return;

        FoodSpawnDefinition nextFood = _foodSpawnQueue.Peek();
        SetIndicatorColor(nextFood.color);
        _audioSource.PlayOneShot(_spawningSounds.GetRandomClip());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) {
            QueueSpawn(1);
        }

        if (isOpen && !_isFiring && _foodSpawnQueue.Count > 0) {
            StartCoroutine(StartFiring());
        }

        if (foodInScene.Count > maxFoodInScene) isOpen = false;
        else isOpen = true;

        countText.text = _foodSpawnQueue.Count.ToString();
    }

    void OnFoodEaten(FoodObject food) {
        this.foodInScene.Remove(food);
    }
}

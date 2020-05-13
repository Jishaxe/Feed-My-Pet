using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSDieAfterLastParticle : MonoBehaviour
{
    ParticleSystem _ps;

    void Start() {
        _ps = GetComponent<ParticleSystem>();
        StartCoroutine(WaitTillLastParticle());
    }

    IEnumerator WaitTillLastParticle() {
        yield return new WaitUntil(() => _ps.particleCount == 0);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 1f;

    private Vector3 direction;

    public LayerMask saberLayerMask;

    public LayerMask hitLayerMask;

    private bool reflected = false;

    [SerializeField]
    private GameObject HitParticle;

    private void Start()
    {
        direction = transform.forward;
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        transform.position += Time.deltaTime * Speed * direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!reflected && saberLayerMask == (saberLayerMask | (1 << other.gameObject.layer)))
        {
            reflected = true;

            Vector3 newDirection = -Vector3.Reflect(direction, other.gameObject.transform.up).normalized;
            direction = newDirection;
        }

        if (hitLayerMask == (hitLayerMask | (1 << other.gameObject.layer)))
        {
            if (HitParticle != null)
            {
                Instantiate(HitParticle, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}

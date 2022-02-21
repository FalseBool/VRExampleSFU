using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 1f;

    private Vector3 direction;

    public LayerMask layerMask;

    private bool reflected = false;

    private void Start()
    {
        direction = Vector3.forward;
    }

    void Update()
    {
        transform.Translate(Time.deltaTime * Speed * direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!reflected && layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            reflected = true;
            
            direction = -Vector3.Reflect(direction, other.gameObject.transform.up).normalized;
        }
    }
}

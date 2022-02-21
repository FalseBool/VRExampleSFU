using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public Transform[] MovementPoints;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private AnimationCurve movementCurve;

    [SerializeField]
    private GameObject bulletPrafab;

    [SerializeField]
    private Transform muzzle;

    private void Start()
    {
        MakeNextMove();
        StartCoroutine(PerformShots());
    }

    private void Update()
    {
        transform.LookAt(player);
    }

    private IEnumerator PerformMovement(Transform targetPosition) 
    {
        Vector3 currPosition = transform.position;
        float time = 0f;
        float lerpDelta = 0f;
        while (true)
        {
            time += Time.deltaTime;
            if (time < 1)
            {
                lerpDelta = movementCurve.Evaluate(time);
                transform.position = Vector3.LerpUnclamped(currPosition, targetPosition.position, lerpDelta);
            }
            else
            {
                transform.position = Vector3.LerpUnclamped(currPosition, targetPosition.position, 1);
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0f,1f));
        MakeNextMove();
    }

    private IEnumerator PerformShots()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            Shoot();
        }
    }

    private void MakeNextMove() 
    {
        Transform nextPos = PickRandomPoint();
        StartCoroutine(PerformMovement(nextPos));
    }

    private Transform PickRandomPoint() 
    {
        return MovementPoints[Random.Range(0, MovementPoints.Length-1)];
    }

    private void Shoot() 
    {
        Vector3 playerOffsetedPos = new Vector3(
            player.position.x - Random.Range(-0.1f, 0.1f), 
            player.position.y - Random.Range(0, 0.2f), 
            player.position.z - Random.Range(-0.1f, 0.1f));
        Vector3 dir = (playerOffsetedPos - transform.position).normalized;
        Instantiate(bulletPrafab, muzzle.position, Quaternion.LookRotation(dir));
    }
}

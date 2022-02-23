using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Drone : MonoBehaviour
{
    public Transform[] MovementPoints;

    public float speed =  1f;

    public float minFireDelay = 0.5f;
    public float maxFireDelay = 2f;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private AnimationCurve movementCurve;

    private Coroutine movementCoroutine = null;

    [SerializeField]
    private GameObject bulletPrafab;

    [SerializeField]
    private Transform muzzle;

    [SerializeField]
    private Transform initTransform;

    private bool droneInited = false;

    private bool droneShoots = false;

    [SerializeField]
    private LightSaber playersLightSaber;

    private XRGrabInteractable xRGrabInteractable;

    private void OnEnable()
    {
        xRGrabInteractable = playersLightSaber.gameObject.GetComponent<XRGrabInteractable>();
        if (xRGrabInteractable != null)
        {
            xRGrabInteractable.selectEntered.AddListener(OnLightsaberSelected);
        }
        playersLightSaber.LightSaberStateChanged += OnLightSaberStateChanged;
    }

    private void OnDisable()
    {
        if (xRGrabInteractable != null) 
        {
            xRGrabInteractable.selectEntered.RemoveListener(OnLightsaberSelected);
        }
        playersLightSaber.LightSaberStateChanged -= OnLightSaberStateChanged;
    }

    private void OnLightSaberStateChanged(bool state)
    {
        StartShooting();
    }

    private void OnLightsaberSelected(SelectEnterEventArgs arg0)
    {
        InitDrone();
    }

    private void Update()
    {
        transform.LookAt(player);
    }

    public void InitDrone() 
    {
        if (!droneInited)
        {
            droneInited = true;
            movementCoroutine = StartCoroutine(PerformMovement(initTransform));
        }
    }

    public void StartShooting() 
    {
        if (!droneShoots)
        {
            droneShoots = true;
            MakeNextMove();
            StartCoroutine(Delay(0.2f, () => { StartCoroutine(PerformShots()); }));
        }
    }

    private IEnumerator Delay(float duration, System.Action action) 
    {
        yield return new WaitForSeconds(duration);
        action?.Invoke();
    }

    private IEnumerator PerformMovement(Transform targetPosition, System.Action moveEndedCallback = null) 
    {
        Vector3 currPosition = transform.position;

        float time = 0f;
        float lerpDelta = 0f;
        float speedWithDistance = speed / Vector3.Distance(currPosition, targetPosition.position);
        while (true)
        {
            time += Time.deltaTime * speedWithDistance;
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
        if (moveEndedCallback != null)
        {
            moveEndedCallback?.Invoke();
        }
    }

    private IEnumerator PerformShots()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minFireDelay, maxFireDelay));
            Shoot();
        }
    }

    private void MakeNextMove() 
    {
        Transform nextPos = PickRandomPoint();
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(PerformMovement(nextPos, MakeNextMove));
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
        Vector3 dir = (playerOffsetedPos - muzzle.position).normalized;
        Instantiate(bulletPrafab, muzzle.position, Quaternion.LookRotation(dir));
    }
}

using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float speed;
    private float gravity;
    private float damage;
    private float damageHeadshot;
    private Vector3 startPosition;
    private Vector3 startForward;
    private GameObject hitMarker;

    private bool isInitialised = false;

    private float startTime = -1;

    public void Initialise(Transform startPoint, float speed, float gravity, float damage, float damageHeadshot, GameObject hitMarkerUI)
    {
        startPosition = startPoint.position;
        startForward = startPoint.forward.normalized;
        this.speed = speed;
        this.gravity = gravity;
        this.damageHeadshot = damageHeadshot;
        this.damage = damage;
        hitMarker = hitMarkerUI;
        isInitialised = true;
    }

    private Vector3 FindPointOnParabola(float time)
    {
        Vector3 point = startPosition + (startForward * time * speed);
        Vector3 gravityVec = Vector3.down * time * time * gravity;
        return point + gravityVec;
    }

    private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        Debug.DrawRay(startPoint, endPoint - startPoint, Color.green, 5);
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
    }

    private void OnHit(RaycastHit hit)
    {
        if (hit.transform.CompareTag("Player/Generic"))
        {
            StartCoroutine(ToggleHitMarker(0.1f));
            Damagable target = hit.transform.GetComponentInParent<Damagable>();           
            target.Damage(damage);
        } else if (hit.transform.CompareTag("Player/Head"))
        {
            StartCoroutine(ToggleHitMarker(0.1f));
            Damagable target = hit.transform.GetComponentInParent<Damagable>();
            target.Damage(damageHeadshot);
        }
        StartCoroutine(DestroyAfterDelay(gameObject, 0.1f));
    }

    IEnumerator ToggleHitMarker(float timeToShow)
    {
        hitMarker.SetActive(true);
        yield return new WaitForSeconds(timeToShow);
        hitMarker.SetActive(false);
    }

    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }

    private void FixedUpdate()
    {
        if (!isInitialised) return;
        if (startTime < 0) startTime = Time.time;

        float currentTime = Time.time - startTime;
        float prevTime = currentTime - Time.fixedDeltaTime;
        float nextTime = currentTime + Time.fixedDeltaTime;

        RaycastHit hit;
        Vector3 currentPoint = FindPointOnParabola(currentTime);

        if (prevTime > 0)
        {
            Vector3 prevPoint = FindPointOnParabola(prevTime);
            if (CastRayBetweenPoints(prevPoint, currentPoint, out hit))
            {
                OnHit(hit);
            }
        }

        Vector3 nextPoint = FindPointOnParabola(nextTime);
        if (CastRayBetweenPoints(currentPoint, nextPoint, out hit))
        {
            OnHit(hit);
        }
    }

    void Update()
    {
        if (!isInitialised || startTime < 0) return;

        float currentTime = Time.time - startTime;
        Vector3 currentPoint = FindPointOnParabola(currentTime);
        transform.position = currentPoint;
    }
}
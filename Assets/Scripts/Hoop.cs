using System.Collections;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    [SerializeField] private Transform hoopNet;
    [SerializeField] private Transform ballPoint;
    [SerializeField] private Vector3 ballOffset;
    [SerializeField] private Transform ballHolder;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Transform moneyPoint;
    [SerializeField] private GameObject money;

    [Header("Random events")]
    [SerializeField] private float chanceObstacle;
    [SerializeField] private float chanceRotation;
    [SerializeField] private float chanceMoney;

    private float _minTension = 1f;
    private float _maxTension = 1.7f;
    private int _lastObstacle;

    public bool IsActive { get; set; }

    private void OnEnable()
    {
        IsActive = true;
    }

    public Vector3 GetStartScale()
    {
        return startScale;
    }

    public void SetHoopNetTension(float progress)
    {
        hoopNet.localScale = new Vector3(1f, Mathf.Lerp(_minTension, _maxTension, progress), 1f);
        ballHolder.position = ballPoint.position;
    }

    public void SetRotationByTouchScreen(Vector3 screenPosition)
    {
        screenPosition.z = -Camera.main.transform.position.z;

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector3 direction = targetPosition - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, -direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetDefaultRotation()
    {
        if (transform.rotation != Quaternion.identity) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
    }

    public void PutBallInHoop(Ball ball)
    {
        StartCoroutine(ScaleChange(obstacles[_lastObstacle].transform, Vector3.zero, 0.15f, false));

        ball.DisablePhysics();
        Transform ballTransform = ball.transform;
        ballHolder.position = ballPoint.position;
        ballTransform.SetParent(ballHolder);
        ballTransform.SetLocalPositionAndRotation(ballOffset, Quaternion.identity);
    }

    public void PerformRandomEvents()
    {
        float randomValue = Random.value;

        if (randomValue <= chanceObstacle)
        {
            obstacles[_lastObstacle].SetActive(false);

            int randomObstacle = Random.Range(0, obstacles.Length);

            obstacles[randomObstacle].SetActive(true);
            _lastObstacle = randomObstacle;
        }
        else if (randomValue <= chanceObstacle + chanceRotation)
        {
            float rotationZ = transform.position.x < 0 ? -Random.Range(15f, 46f) : Random.Range(15f, 46f);
            transform.Rotate(0f, 0f, rotationZ);
        }

        randomValue = Random.value;

        if (randomValue <= chanceMoney)
        {
            moneyPoint.rotation = Quaternion.identity;
            StartCoroutine(ScaleChange(money.transform, Vector3.one, 0.15f, true));
        }
    }

    public IEnumerator ScaleChange(Transform targetTransform, Vector3 targetScale, float duration, bool active)
    {
        if (active) targetTransform.gameObject.SetActive(active);
        Vector3 initialScale = targetTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            targetTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetTransform.localScale = targetScale;
        if (!active) targetTransform.gameObject.SetActive(active);
    }
}

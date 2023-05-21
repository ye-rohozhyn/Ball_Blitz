using UnityEngine;

public class Hoop : MonoBehaviour
{
    [SerializeField] private Transform hoopNet;
    [SerializeField] private Transform ballPoint;
    [SerializeField] private Vector3 ballOffset;
    [SerializeField] private Transform ballHolder;
    [SerializeField] private float rotationSpeed;
    
    private float _minTension = 1f;
    private float _maxTension = 1.7f;

    public bool IsActive { get; set; }

    private void OnEnable()
    {
        IsActive = true;
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
        ball.DisablePhysics();
        Transform ballTransform = ball.transform;
        ballHolder.position = ballPoint.position;
        ballTransform.SetParent(ballHolder);
        ballTransform.SetLocalPositionAndRotation(ballOffset, Quaternion.identity);
    }
}

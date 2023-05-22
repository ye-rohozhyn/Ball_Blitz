using System.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelType levelType;
    [SerializeField] private Hoop[] hoops;
    [SerializeField] private float xStep;
    [SerializeField] private float yStep;
    [SerializeField] private float xRange;
    [SerializeField] private float yRange;

    private int _currentIndex = 0;

    public void EnableNextHoop()
    {
        if (levelType == LevelType.challenge) return;

        StartCoroutine(ScaleChange(hoops[_currentIndex].transform, Vector3.zero, 0.2f, false));

        _currentIndex = (_currentIndex + 1) % hoops.Length;
        int nextHoopIndex = (_currentIndex + 1) % hoops.Length;

        Vector3 currentPosition = hoops[_currentIndex].transform.position;
        float x = Random.Range(xStep - xRange, xStep + xRange);
        float y = Random.Range(yStep - yRange, yStep + yRange);

        hoops[nextHoopIndex].transform.SetPositionAndRotation(new Vector3(currentPosition.x < 0 ? x : -x, 
            currentPosition.y + y, 0f), Quaternion.identity);

        hoops[nextHoopIndex].gameObject.SetActive(true);
        hoops[nextHoopIndex].PerformRandomEvents();

        StartCoroutine(ScaleChange(hoops[nextHoopIndex].transform, hoops[nextHoopIndex].GetStartScale(), 0.2f, true));
    }

    public Hoop GetCurrentHoop()
    {
        return hoops[_currentIndex];
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

public enum LevelType
{
    infinity, challenge
}
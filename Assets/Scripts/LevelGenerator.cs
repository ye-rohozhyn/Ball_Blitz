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

        hoops[_currentIndex].gameObject.SetActive(false);

        _currentIndex = (_currentIndex + 1) % hoops.Length;
        int nextHoopIndex = (_currentIndex + 1) % hoops.Length;

        Vector3 currentPosition = hoops[_currentIndex].transform.position;
        float x = Random.Range(xStep - xRange, xStep + xRange);
        float y = Random.Range(yStep - yRange, yStep + yRange);

        hoops[nextHoopIndex].transform.position = new Vector3(currentPosition.x < 0 ? x : -x, currentPosition.y + y, 0f);
        hoops[nextHoopIndex].transform.rotation = Quaternion.identity;
        hoops[nextHoopIndex].gameObject.SetActive(true);
    }

    public Hoop GetCurrentHoop()
    {
        return hoops[_currentIndex];
    }
}

public enum LevelType
{
    infinity, challenge
}
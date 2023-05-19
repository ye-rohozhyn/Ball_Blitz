using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] private int dotsNumber;
    [SerializeField] private float spacing;
    [SerializeField] private Transform dotsHolder;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private float dotsMinScale;
    [SerializeField] private float dotsMaxScale;

    private Transform[] _dots;
    private float _timeStamp;
    private Vector3 _dotPosition;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _dots = new Transform[dotsNumber];

        float scale = dotsMaxScale;
        float scaleFactor = dotsMaxScale / dotsNumber;

        for (int i = 0; i < dotsNumber; i++)
        {
            _dots[i] = Instantiate(dotPrefab, null).transform;
            _dots[i].SetParent(dotsHolder);
            _dots[i].localScale = Vector3.one * scale;
            if (scale > dotsMinScale) scale -= scaleFactor;
        }

        Hide();
    }

    public void DrawDots(Vector3 ballPosition, Vector2 forceApplied)
    {
        _timeStamp = spacing;
        for (int i = 0; i < dotsNumber; i++)
        {
            _dotPosition.x = ballPosition.x + forceApplied.x * _timeStamp;
            _dotPosition.y = (ballPosition.y + forceApplied.y * _timeStamp) - (Physics2D.gravity.magnitude * _timeStamp * _timeStamp) / 2f;

            _dots[i].position = _dotPosition;
            _timeStamp += spacing;
        }
    }

    public void Show()
    {
        dotsHolder.gameObject.SetActive(true);
    }

    public void Hide()
    {
        dotsHolder.gameObject.SetActive(false);
    }
}

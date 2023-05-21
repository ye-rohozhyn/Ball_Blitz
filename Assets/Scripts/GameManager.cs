using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Camera playerCamera;
	[SerializeField] private Ball ball;
	[SerializeField] private Trajectory trajectory;
	[SerializeField] private float pushForce = 4f;
	[SerializeField] private float minForce = 4f;
	[SerializeField] private float maxForce = 10f;

	[Header("Walls")]
	[SerializeField] private Transform leftWall;
	[SerializeField] private Transform rightWall;

	[Header("Score")]
	[SerializeField] private TMP_Text scoreText;

	private Vector2 _startPoint;
	private Vector2 _endPoint;
	private Vector2 _direction;
	private Vector2 _force;
	private float _distance;
	private int _score = 0;
	
	public int CountShots { get; set; }
	public bool IsLose { get; set; }

	private void Start()
    {
		SetWallsPosition();
	}

    private void Update()
	{
		if (ball.InAir) return;

		if (Input.GetMouseButtonDown(0))
		{
			OnDragStart();
		}
		else if (Input.GetMouseButton(0))
        {
			OnDrag();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			OnDragEnd();
		}
        else
        {
			ball.GetCurrentHoop().SetDefaultRotation();
		}
	}

	public void AddScore(int value)
    {
		_score += value;
		scoreText.text = _score.ToString();
	}

	private void SetWallsPosition()
    {
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		Vector3 rightWallPosition = playerCamera.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight / 2, playerCamera.nearClipPlane));
		rightWall.position = rightWallPosition;

		Vector3 leftWallPosition = playerCamera.ScreenToWorldPoint(new Vector3(0, screenHeight / 2, playerCamera.nearClipPlane));
		leftWall.position = leftWallPosition;
	}

    #region - Drag ball -

    private void OnDragStart()
	{
		_startPoint = ball.transform.position;

		trajectory.Show();
	}

	private void OnDrag()
	{
		_endPoint = playerCamera.ScreenToWorldPoint(Input.mousePosition);
		_distance = Vector2.Distance(_startPoint, _endPoint);
		_direction = (_startPoint - _endPoint).normalized;
		_force = _distance * pushForce * _direction;

		_force = Vector2.ClampMagnitude(_force, maxForce);

		ball.GetCurrentHoop().SetHoopNetTension(_force.magnitude / maxForce);
		ball.GetCurrentHoop().SetRotationByTouchScreen(Input.mousePosition);

		trajectory.DrawDots(ball.transform.position, _force);
	}

	private void OnDragEnd()
	{
		ball.GetCurrentHoop().SetHoopNetTension(0f);
		float forceMagnitude = _force.magnitude;

		if (forceMagnitude < minForce || forceMagnitude > maxForce)
		{
			trajectory.Hide();
			return;
		}

		ball.EnablePhysics();
		ball.Push(_force);
		trajectory.Hide();
	}

    #endregion
}

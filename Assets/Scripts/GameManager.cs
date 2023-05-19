using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Camera playerCamera;
	[SerializeField] private Ball ball;
	[SerializeField] private Trajectory trajectory;
	[SerializeField] private float pushForce = 4f;
	[SerializeField] private float minForce = 4f;
	[SerializeField] private float maxForce = 10f;

	private Vector2 _startPoint;
	private Vector2 _endPoint;
	private Vector2 _direction;
	private Vector2 _force;
	private float _distance;

    private void Update()
	{
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
	}

	private void OnDragStart()
	{
		ball.DisablePhysics();
		_startPoint = playerCamera.ScreenToWorldPoint(Input.mousePosition);

		trajectory.Show();
	}

	private void OnDrag()
	{
		_endPoint = playerCamera.ScreenToWorldPoint(Input.mousePosition);
		_distance = Vector2.Distance(_startPoint, _endPoint);
		_direction = (_startPoint - _endPoint).normalized;
		_force = _distance * pushForce * _direction;

		_force = Vector2.ClampMagnitude(_force, maxForce);

		trajectory.DrawDots(ball.transform.position, _force);
	}

	private void OnDragEnd()
	{
		float forceMagnitude = Mathf.Abs(_force.magnitude);

		if (forceMagnitude < minForce || forceMagnitude > maxForce)
		{
			trajectory.Hide();
			return;
		}

		ball.EnablePhysics();
		ball.Push(_force);
		trajectory.Hide();
	}
}

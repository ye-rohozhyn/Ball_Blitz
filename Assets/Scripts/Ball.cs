using UnityEngine;

public class Ball : MonoBehaviour
{
	[SerializeField] private Rigidbody2D ballRigidbody;
	[SerializeField] private CircleCollider2D ballCollider;

	private Hoop _currentHoop;

	public void Push(Vector2 force)
	{
		_currentHoop = null;
		ballRigidbody.AddForce(force, ForceMode2D.Impulse);
	}

	public void EnablePhysics()
	{
		ballRigidbody.constraints = RigidbodyConstraints2D.None;
	}

	public void DisablePhysics()
	{
		ballRigidbody.velocity = Vector3.zero;
		ballRigidbody.angularVelocity = 0f;
		ballRigidbody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
	}

	public void SetCurrentHoop(Hoop hoop)
    {
		_currentHoop = hoop;
	}

	public Hoop GetCurrentHoop()
	{
		return _currentHoop;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag("Hoop"))
        {
			Hoop hoop = collision.GetComponentInParent<Hoop>();

            if (hoop)
            {
				hoop.PutBallInHoop(this);
			}
		}
    }
}
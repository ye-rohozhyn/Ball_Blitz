using UnityEngine;

public class Ball : MonoBehaviour
{
	[SerializeField] private Rigidbody2D ballRigidbody;
	[SerializeField] private CircleCollider2D ballCollider;

	public void Push(Vector2 force)
	{
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
}
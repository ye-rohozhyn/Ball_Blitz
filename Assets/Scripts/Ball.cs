using UnityEngine;

public class Ball : MonoBehaviour
{
	[SerializeField] private Rigidbody2D ballRigidbody;
	[SerializeField] private CircleCollider2D ballCollider;
	[SerializeField] private LevelGenerator levelGenerator;

	public bool InAir = true;

	public void Push(Vector2 force)
	{
		InAir = true;
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

	public Hoop GetCurrentHoop()
	{
		return levelGenerator.GetCurrentHoop();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag("Hoop"))
        {
			Hoop hoop = collision.GetComponentInParent<Hoop>();

            if (hoop)
            {
				InAir = false;

				hoop.PutBallInHoop(this);

                if (hoop.IsActive & GameManager.CountShots > 0)
                {
					levelGenerator.EnableNextHoop();
				}

				hoop.IsActive = false;
				GameManager.CountShots++;
			}
		}
    }
}
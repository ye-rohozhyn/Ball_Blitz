using UnityEngine;

public class Ball : MonoBehaviour
{
	[SerializeField] private GameManager gameManager;
	[SerializeField] private Rigidbody2D ballRigidbody;
	[SerializeField] private CircleCollider2D ballCollider;
	[SerializeField] private LevelGenerator levelGenerator;
	[SerializeField] private SpriteRenderer ballSprite;
	[SerializeField] private BallData[] balls;
	[SerializeField] private ParticleSystem smoke;
	[SerializeField] private ParticleSystem darkSmoke;
	[SerializeField] private ParticleSystem ballEffect;

	public bool InAir = true;

	private int _hoopCollisions = 0;
	private int _perfectShots = 0;
	private int _maxPerfectShots = 3;
	private int _currentBallIndex = 0;

    private void Awake()
    {
		_currentBallIndex = PlayerPrefs.GetInt("BallIndex", 0);
		ballSprite.sprite = balls[_currentBallIndex].ballSprite;
		Transform ballEffectTransform = Instantiate(balls[_currentBallIndex].ballEffectPrefab, null).transform;
		ballEffectTransform.SetParent(transform);
		ballEffectTransform.localPosition = Vector3.zero;
		ballEffect = ballEffectTransform.GetComponent<ParticleSystem>();
	}

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

	public BallData[] GetBalls()
    {
		return balls;
    }

	public void SetNewData(int index)
    {
		Destroy(ballEffect.gameObject);

		PlayerPrefs.SetInt("BallIndex", index);

		_currentBallIndex = index;

		ballSprite.sprite = balls[_currentBallIndex].ballSprite;
		Transform ballEffectTransform = Instantiate(balls[_currentBallIndex].ballEffectPrefab, null).transform;
		ballEffectTransform.SetParent(transform);
		ballEffectTransform.localPosition = Vector3.zero;
		ballEffect = ballEffectTransform.GetComponent<ParticleSystem>();
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

                if (hoop.IsActive & gameManager.CountShots > 0)
                {
					levelGenerator.EnableNextHoop();
					if(_hoopCollisions > 0)
                    {
						if (_perfectShots == 1) smoke.Stop();
						else if (_perfectShots == 2) darkSmoke.Stop();
						else if (_perfectShots == 3) ballEffect.Stop();

						_perfectShots = 0;
						gameManager.AddScore(1);
					}
                    else
                    {
						if (_perfectShots < _maxPerfectShots)
						{
							_perfectShots++;

                            if (_perfectShots == 1)
                            {
								smoke.Play();
                            }
							else if (_perfectShots == 2)
                            {
								smoke.Stop();
								darkSmoke.Play();
							}
							else if (_perfectShots == 3)
							{
								darkSmoke.Stop();
								ballEffect.Play();
							}
						}
						gameManager.AddScore(_perfectShots * 2);
					}
				}
                else
                {
					if (_perfectShots == 1) smoke.Stop();
					else if (_perfectShots == 2) darkSmoke.Stop();
					else if (_perfectShots == 3) ballEffect.Stop();

					_perfectShots = 0;
				}

				_hoopCollisions = 0;

				hoop.IsActive = false;
				gameManager.CountShots++;
			}
		}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.CompareTag("HoopRing"))
        {
			_hoopCollisions++;
		}
    }
}
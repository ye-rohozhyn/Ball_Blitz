using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;

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
	[SerializeField] private TMP_Text bestScoreText;
	[SerializeField] private TMP_Text scoreText;

	[Header("Score")]
	[SerializeField] private TMP_Text moneyText;

	[Header("Panels")]
	[SerializeField] private GameObject losePanel;
	[SerializeField] private GameObject shopPanel;
	[SerializeField] private Transform ballMarker;

	[Header("Buttons")]
	[SerializeField] private GameObject shopButton;
	[SerializeField] private GameObject pauseButton;

	[Header("Shop")]
	[SerializeField] private Transform itemsParent;
	[SerializeField] private GameObject itemPrefab;

	[Header("Audio")]
	[SerializeField] private Toggle soundsToggle;
	[SerializeField] private Toggle musicToggle;
	[SerializeField] private AudioSource sounds;
	[SerializeField] private AudioSource music;

	[Header("Ads")]
	[SerializeField] private RewardedAds rewardedAds;

	[Header("Settings")]
	[SerializeField] private int targetFrameRate;

	private Vector2 _startPoint;
	private Vector2 _endPoint;
	private Vector2 _direction;
	private Vector2 _force;
	private float _distance;
	private int _bestScore = 0;
	private int _score = 0;
	private int _money = 0;
	private GameObject _lastOpenPanel;
	private Transform[] shopItems;
	private float _startSoundsVolume;
	private float _startMusicVolume;

	public int CountShots { get; set; }
	public bool IsLose { get; set; }
	public bool IsPause { get; set; }

	private void Awake()
    {
		Time.timeScale = 1f;

		Application.targetFrameRate = targetFrameRate;

		_startSoundsVolume = sounds.volume;
		_startMusicVolume = music.volume;

		sounds.volume = PlayerPrefs.GetFloat("Sounds", _startSoundsVolume);
		music.volume = PlayerPrefs.GetFloat("Music", _startMusicVolume);

		soundsToggle.isOn = sounds.volume > 0f;
		musicToggle.isOn = music.volume > 0f;

		SetWallsPosition();

		_bestScore = PlayerPrefs.GetInt("BestScore", 0);
		_money = PlayerPrefs.GetInt("Money", 0);
		moneyText.text = _money.ToString();

		GenerateShop();
	}

    private void Start()
    {
		music.Play();
	}

    private void Update()
	{
		if (ball.InAir || EventSystem.current.IsPointerOverGameObject() || IsPause) return;

		if (Input.GetMouseButtonDown(0))
		{
            if (shopButton.activeSelf)
            {
				shopButton.SetActive(false);
				pauseButton.SetActive(true);
			}

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

    #region - UI -

    public void ShowLosePanel()
    {
		losePanel.SetActive(true);

		if(_bestScore < _score)
        {
			PlayerPrefs.SetInt("BestScore", _score);
			bestScoreText.text = "Best score: " + _score.ToString();
		}
        else
        {
			bestScoreText.text = "Best score: " + _bestScore.ToString();
        }
	}

	public void ShowPanel(GameObject panel)
	{
		if (_lastOpenPanel) _lastOpenPanel.SetActive(false);

		Time.timeScale = 0f;
		panel.SetActive(true);
		_lastOpenPanel = panel;
		IsPause = true;
	}

	public void ClosePanel(GameObject panel)
	{
		Time.timeScale = 1f;
		panel.SetActive(false);
		_lastOpenPanel = null;
		IsPause = false;
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void SelectBall(int index)
    {
		BallData[] balls = ball.GetBalls();

		string key = $"Ball {index} {(int)balls[index].typeValue} {balls[index].amount}";

		if (balls[index].typeValue == TypeValue.Money)
        {
            if (_money >= balls[index].amount)
            {
				_money -= balls[index].amount;
				PlayerPrefs.SetInt("Money", _money);
				PlayerPrefs.SetInt(key, balls[index].amount);
				moneyText.text = _money.ToString();
				shopItems[index].GetComponentInParent<ShopItem>().DisablePriceBlock();
			}
        }
		else if (balls[index].typeValue == TypeValue.Ads)
        {
			int countAds = PlayerPrefs.GetInt(key, 0);

            if (countAds < balls[index].amount)
            {
				rewardedAds.ShowAd();
				rewardedAds.LoadAd();

                if (rewardedAds.GetLastAdStatus().Equals(UnityAdsShowCompletionState.COMPLETED))
                {
					countAds++;
					PlayerPrefs.SetInt(key, countAds);

                    if (balls[index].amount == countAds)
                    {
						shopItems[index].GetComponentInParent<ShopItem>().DisablePriceBlock();
					}
                    else
                    {
						shopItems[index].GetComponentInParent<ShopItem>().UpdateCount(balls[index].amount - countAds);
					}
				}
			}
		}

        if (PlayerPrefs.GetInt(key, 0) == balls[index].amount)
        {
			ball.SetNewData(index);
			ballMarker.position = shopItems[index].position;
		}
    }

	private void GenerateShop()
	{
		BallData[] balls = ball.GetBalls();
		shopItems = new Transform[balls.Length];

		shopPanel.SetActive(true);

		for (int i = 0; i < balls.Length; i++)
		{
			int index = i; 

			shopItems[i] = Instantiate(itemPrefab, itemsParent).transform;
			shopItems[i].GetComponent<ShopItem>().SetItem(balls[i], index);
			shopItems[i].GetComponentInChildren<Button>().onClick.AddListener(() => SelectBall(index));
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(itemsParent.GetComponent<RectTransform>());

		shopPanel.SetActive(false);

		ballMarker.position = shopItems[PlayerPrefs.GetInt("BallIndex", 0)].position;
	}

	public void ToggleSounds(Toggle toggle)
    {
		if (toggle.isOn) sounds.volume = _startSoundsVolume;
		else sounds.volume = 0f;

		PlayerPrefs.SetFloat("Sounds", sounds.volume);
	}

	public void ToggleMusic(Toggle toggle)
	{
		if (toggle.isOn) music.volume = _startMusicVolume;
		else music.volume = 0f;

		PlayerPrefs.SetFloat("Music", music.volume);
	}

	#endregion

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

	#region - Other -

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

	public void PlaySound(AudioClip clip)
    {
		sounds.PlayOneShot(clip);
	}

	public void AddMoney(int value)
	{
		_money += value;
		PlayerPrefs.SetInt("Money", _money);
		moneyText.text = _money.ToString();
	}

	#endregion
}

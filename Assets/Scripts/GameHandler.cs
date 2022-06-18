using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Handler;

    [Header("Components")]
    [SerializeField] private GameObject damageShakeCamera;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI comboStreakText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreGameOverText;
    [SerializeField] private TextMeshProUGUI startLabel;
    [SerializeField] private Image experienceFillImage;
    
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private GameObject gameUIScreen;
    [SerializeField] private GameObject gameOverScreen;

    public TankController playerController;
    
    private int level = 1;
    private int _score;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            scoreText.text = "Score: " + _score;
        }
    }
    
    private float _currentExperience;
    private float _experienceToNextLevel = 1000;

    public float CurrentExperience
    {
        get => _currentExperience;
        set
        {
            _currentExperience = value;
            
            if (_currentExperience >= _experienceToNextLevel)
            {
                level++;
                levelText.text = "LVL " + level;
                UpgradeShop.Handler.SkillPoints += 1;
                _currentExperience -= _experienceToNextLevel;
                _experienceToNextLevel += 100 * level;
            }
            
            experienceFillImage.DOFillAmount(_currentExperience / _experienceToNextLevel, 0.2f);
        }
    }

    private int _comboStreak;
    public int ComboStreak
    {
        get => _comboStreak;
        set
        {
            _comboStreak = value;
            comboStreakText.text = _comboStreak > 1 ? "COMBO X" + _comboStreak : String.Empty;
            comboStreakText.transform.DOShakeScale(0.1f, 0.25f * _comboStreak);
            
            CancelInvoke(nameof(BreakComboStreak));
            Invoke(nameof(BreakComboStreak), 6f);
        }
    }

    private void BreakComboStreak()
    {
        comboStreakText.transform.DOShakeScale(0.4f, 0.5f).onComplete += () => {ComboStreak = 0;};
        comboStreakText.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    private bool destroyed;
    private int _playerHealth = 3;
    [SerializeField] private GameObject[] hitPointsIcons;
    
    public int PlayerHealth
    {
        get => _playerHealth;
        set
        {
            if (destroyed) return;
            
            if (value <= 0)
            {
                Time.timeScale = 0;
                destroyed = true;
                value = 0;
                gameUIScreen.SetActive(false);
                gameOverScreen.SetActive(true);
                scoreGameOverText.text = "Score: " + _score;
            }

            if (isImmortal)
            {
                if (value > _playerHealth)
                {
                    for (int i = 1; i <= hitPointsIcons.Length; i++)
                    {
                        hitPointsIcons[i-1].SetActive(i <= value);
                    }
            
                    _playerHealth = value;
                }

                return;
            }
            
            if (value < _playerHealth)
            {
                CancelInvoke(nameof(DisableDamageShakeCamera));
                CancelInvoke(nameof(DisableImmortality));
                
                isImmortal = true;
                damageShakeCamera.SetActive(true);
                
                Invoke(nameof(DisableDamageShakeCamera), 0.2f);
                Invoke(nameof(DisableImmortality), 3f);
            }
            
            for (int i = 1; i <= hitPointsIcons.Length; i++)
            {
                hitPointsIcons[i-1].SetActive(i <= value);
            }
            
            _playerHealth = value;
        }
    }

    private bool isImmortal;
    private void DisableDamageShakeCamera()
    {
        damageShakeCamera.SetActive(false);
    }

    private void DisableImmortality()
    {
        isImmortal = false;
    }

    private void Awake()
    {
        if (Handler != null)
        {
            Destroy(gameObject);
        }
        
        Handler ??= this;

        Time.timeScale = 0;

        gameUIScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        startLabel.DOFade(0.2f, 1f).SetLoops(-1, LoopType.Yoyo).SetUpdate(UpdateType.Normal, true);
        CurrentExperience = 0;
    }

    void Start()
    {
        PlayerHealth = hitPointsIcons.Length;
    }

    private void OnDestroy()
    {
        Handler = null;
    }

    public void StartPlaying()
    {
        welcomeScreen.SetActive(false);
        gameUIScreen.SetActive(true);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

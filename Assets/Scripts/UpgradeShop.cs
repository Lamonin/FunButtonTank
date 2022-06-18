using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    public static UpgradeShop Handler;
    
    [SerializeField] private Transform upgradeRotateSpeedGrades;
    [SerializeField] private Transform upgradeBulletSpeedGrades;
    [SerializeField] private Transform buyHeartsValues;

    [SerializeField] private TextMeshProUGUI skillPointsText;

    [SerializeField] private Button rotateSpeedUpgradeButton;
    [SerializeField] private Button bulletSpeedUpgradeButton;
    [SerializeField] private Button restoreHealthButton;
    
    private int _skillPoints;
    public int SkillPoints
    {
        get => _skillPoints;
        set
        {
            _skillPoints = value;
            skillPointsText.text = "SKILL POINTS: " + _skillPoints;
        }
    }

    private int rotateSpeedGrades;
    [SerializeField] private int maxRotateSpeedGrades = 4;
    private int bulletSpeedGrades;
    public int BulledSpeedGrades => bulletSpeedGrades;
    [SerializeField] private int maxBulletSpeedGrades = 4;

    private void Awake()
    {
        if (Handler != null)
        {
            Destroy(gameObject);
        }
        
        Handler ??= this;

        gameObject.SetActive(false);
    }

    void Start()
    {
        SkillPoints = 0;
        
        for (int i = 0; i < maxRotateSpeedGrades-1; i++)
        {
            Instantiate(upgradeRotateSpeedGrades.GetChild(0).gameObject, upgradeRotateSpeedGrades);
        }
        
        for (int i = 0; i < maxBulletSpeedGrades-1; i++)
        {
            Instantiate(upgradeBulletSpeedGrades.GetChild(0).gameObject, upgradeBulletSpeedGrades);
        }
        
        UpdateGradesValues();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUpgradePanel();
        }
    }

    private void OnDestroy()
    {
        Handler = null;
    }

    private void UpdateGradesValues()
    {
        for (int i = 1; i <= upgradeRotateSpeedGrades.childCount; i++)
        {
            upgradeRotateSpeedGrades.GetChild(i-1).GetChild(0).gameObject.SetActive(i <= rotateSpeedGrades);
        }
        
        for (int i = 1; i <= upgradeBulletSpeedGrades.childCount; i++)
        {
            upgradeBulletSpeedGrades.GetChild(i-1).GetChild(0).gameObject.SetActive(i <= bulletSpeedGrades);
        }
    }

    private void UpdateRestoreHealthButtonState()
    {
        if (GameHandler.Handler.PlayerHealth < 3)
        {
            restoreHealthButton.interactable = true;
            restoreHealthButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "RESTORE";
        }
        else
        {
            restoreHealthButton.interactable = false;
            restoreHealthButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FULL";
        }

        for (int i = 1; i <= buyHeartsValues.childCount; i++)
        {
            buyHeartsValues.GetChild(i-1).GetChild(0).gameObject.SetActive(i<=GameHandler.Handler.PlayerHealth);
        }
    }

    public void OpenUpgradePanel()
    {
        gameObject.SetActive(true);
        UpdateGradesValues();
        UpdateRestoreHealthButtonState();

        Time.timeScale = 0;
    }

    public void CloseUpgradePanel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void ShowWarningWhenCannotBuy()
    {
        skillPointsText.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo).SetUpdate(UpdateType.Normal, true);
        skillPointsText.transform.DOShakePosition(0.2f, 5f).SetUpdate(UpdateType.Normal, true);
    }

    public void BuyRotateSpeedUpgrade()
    {
        if (_skillPoints == 0)
        {
            ShowWarningWhenCannotBuy();
            return;
        }

        //BUY GRADE
        SkillPoints -= 1;
        rotateSpeedGrades += 1;
        
        GameHandler.Handler.playerController.AddRotateSpeed();
        UpdateGradesValues();

        if (rotateSpeedGrades == maxRotateSpeedGrades)
        {
            rotateSpeedUpgradeButton.interactable = false;
            rotateSpeedUpgradeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FULL";
        }
    }

    public void BuyBulletSpeedUpgrade()
    {
        if (_skillPoints == 0)
        {
            ShowWarningWhenCannotBuy();
            return;
        }

        //BUY GRADE
        SkillPoints -= 1;
        bulletSpeedGrades += 1;
        
        UpdateGradesValues();

        if (bulletSpeedGrades == maxBulletSpeedGrades)
        {
            bulletSpeedUpgradeButton.interactable = false;
            bulletSpeedUpgradeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FULL";
        }
    }

    public void RestoreHealth()
    {
        if (_skillPoints == 0)
        {
            ShowWarningWhenCannotBuy();
            return;
        }
        
        //BUY GRADE
        SkillPoints -= 1;
        GameHandler.Handler.PlayerHealth += 1;
        
        UpdateRestoreHealthButtonState();
    }
}

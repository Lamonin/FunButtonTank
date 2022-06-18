using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TankController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90;
    [SerializeField] private float maxImpulseSpeed = 20;
    [SerializeField] private float shotImpulsePower = 1;

    [Space] 
    [SerializeField] private AudioSource buttonClickSound;
    [SerializeField] private Camera cameraMain;
    [SerializeField] private GameObject shakeCamera;
    [SerializeField] private Transform tankPivot;
    [SerializeField] private Transform tankTransform;
    [SerializeField] private Rigidbody tankRigidbody;
    [SerializeField] private Animator tankAnimator;

    [Header("Shot")]
    [SerializeField] private Light gunShotLight;
    [SerializeField] private Transform spawnBulletPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject smokeParticlesPrefab;

    private MainInput _input;
    private static readonly int Shot = Animator.StringToHash("Shot");

    private Coroutine _shotRoutine;
    private bool isCanShot = true;

    private void Awake()
    {
        _input = new MainInput();
        _input.Player.PressButton.performed += PressButtonPerformed;
        _input.Player.Click.performed += ClickPerformed;
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        tankTransform.Rotate(0,0,rotationSpeed * Time.deltaTime);
        tankPivot.Rotate(0,rotationSpeed * Time.deltaTime, 0);
    }

    public void AddRotateSpeed()
    {
        rotationSpeed += 15;
    }

    private void ClickPerformed(InputAction.CallbackContext callbackContext)
    {
        if (!isCanShot) return;
        
        if(Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out var hit, 1000))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (_shotRoutine != null) StopCoroutine(_shotRoutine);
                _shotRoutine = StartCoroutine(GunShot());
            }
        }
    }

    private void PressButtonPerformed(InputAction.CallbackContext callbackContext)
    {
        if (!isCanShot) return;
        
        if (_shotRoutine != null) StopCoroutine(_shotRoutine);
        _shotRoutine = StartCoroutine(GunShot());
    }

    private IEnumerator GunShot()
    {
        isCanShot = false;
        buttonClickSound.Play();
        tankAnimator.SetTrigger(Shot);
        shakeCamera.SetActive(true);
        yield return new WaitForSeconds(0.08f);
        SpawnBullet();
        gunShotLight.DOIntensity(2, 0.05f).SetLoops(2, LoopType.Yoyo);

        tankRigidbody.AddForce(tankTransform.right * shotImpulsePower, ForceMode.Impulse);
        tankRigidbody.velocity = Vector3.ClampMagnitude(tankRigidbody.velocity, maxImpulseSpeed);
        yield return new WaitForSeconds(0.2f);
        shakeCamera.SetActive(false);
        isCanShot = true;
    }

    private void SpawnBullet()
    {
        //gunShotSound.Play();
        
        var bullet = Instantiate(bulletPrefab, spawnBulletPoint.position, tankPivot.rotation);
        bullet.transform.Rotate(0,-90,0);
        
        var smokeParticles = Instantiate(smokeParticlesPrefab, spawnBulletPoint.position, tankPivot.rotation);
        smokeParticles.transform.Rotate(0,-90,0);
        
        Destroy(smokeParticles, 1.5f);
    }
}

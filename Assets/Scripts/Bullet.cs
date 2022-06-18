using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private float lifeTime = 1;
    
    void Start()
    {
        bulletSpeed += UpgradeShop.Handler.BulledSpeedGrades * 3;
        Invoke(nameof(DestroyAfterTime), lifeTime);
    }
    
    void Update()
    {
        transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
    }

    private void DestroyAfterTime()
    {
        Destroy(gameObject);
    }
}

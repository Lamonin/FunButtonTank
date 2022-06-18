using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private int bonusExperience = 200;
    [SerializeField] private GameObject smokeParticles;
    
    [HideInInspector] public Transform playerTransform;
    
    private bool isDestroyed;
    void Start()
    {
        moveSpeed += moveSpeed * GameHandler.Handler.ComboStreak / 10;
    }

    void Update()
    {
        if (isDestroyed) return;
        transform.LookAt(playerTransform, Vector3.up);
        if (Vector3.Distance(transform.position, playerTransform.position)>2.5)
        {
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            GameHandler.Handler.CurrentExperience += bonusExperience + bonusExperience * GameHandler.Handler.ComboStreak / 8;
            GameHandler.Handler.ComboStreak += 1;
            EnemyDeath();
            Destroy(other.gameObject);
            
        }
        else if (other.CompareTag("Player"))
        {
            GameHandler.Handler.PlayerHealth--;
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        isDestroyed = true;
        GameHandler.Handler.Score++;
        Destroy(gameObject, 0.3f);
            
        var particles = Instantiate(smokeParticles, transform.position, Quaternion.identity);
        Destroy(particles, 3f);
    }
}

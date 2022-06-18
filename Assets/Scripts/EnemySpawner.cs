using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnRepeatDelay = 4f;

        [Space]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameObject enemyPrefab;


        public bool spawnState;
        
        private void Start()
        {
            StartCoroutine(Spawner());
        }

        private Vector3 GetSpawnPosition()
        {
            float xPos, zPos;
            
            do
            {
                xPos = playerTransform.position.x + Random.Range(-40, 41);
                xPos = Math.Clamp(xPos, -49, 49);
            
                zPos = playerTransform.position.x + Random.Range(-40, 41);
                zPos = Math.Clamp(zPos, -49, 49);
                
            } while (Vector3.Distance(playerTransform.position, new Vector3(xPos, 0, zPos))<17);
            
            return new Vector3(xPos, 0, zPos);
        }

        private IEnumerator Spawner()
        {
            while (spawnState)
            {
                var enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
                enemy.GetComponent<EnemyController>().playerTransform = playerTransform;
                yield return new WaitForSeconds(spawnRepeatDelay);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawn : BossPattern
{
    [SerializeField] private Monster[] _monsters;
    [SerializeField] private GameObject[] _spawnPoints;
    
    protected override IEnumerator PatternRoutine()
    {
        for (int i = 0; i < 2; i++)
        {
            int randommosnterindex = Random.Range(0, _monsters.Length);
            int randomspawnindex = Random.Range(0, _spawnPoints.Length);

            Instantiate(
                _monsters[randommosnterindex],
                _spawnPoints[randomspawnindex].transform.position,
                _spawnPoints[randomspawnindex].transform.rotation
            );
        }
        yield return null;
    }
}

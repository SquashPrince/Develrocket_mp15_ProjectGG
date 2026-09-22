using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawn : MonoBehaviour
{
    [SerializeField] private Monster[] _monsters;
    [SerializeField] private GameObject[] _spawnPoints;

    // [SerializeField] private Collider2D spawnArea;

    [SerializeField] private BossControl _bossControl;
    [SerializeField] private float _cooldown = 5f;

    private void Start()
    {
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cooldown);

            _bossControl.AddPattern(SpawnMonster());
        }
    }

    public IEnumerator SpawnMonster()
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


    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            Spawn();
        }*/
    }

    private void Spawn()
    {
        // 문제 같은 장소에 몬스터 2마리가 나올 수도 있음
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




        //Bounds bounds = spawnArea.bounds;

        // 범위 내에서 랜덤 X, Y 좌표 생성
        // float randomX = Random.Range(bounds.min.x, bounds.max.x);
        // float randomY = Random.Range(bounds.min.y, bounds.max.y);
        // int randomindex = Random.Range(0,_monsters.Length);
        /*Debug.Log(randomindex);
        Debug.Log($"{bounds.min.x}  /   {bounds.max.x}");
        Debug.Log($"{bounds.min.y}  /   {bounds.max.y}");
        Debug.Log($"{randomX}  /  {randomY}");*/

        // Vector2 spawnPos = new Vector2(randomX/2, randomY/2);

        // 오브젝트 생성
        // Instantiate(_monsters[randomindex], spawnPos, Quaternion.identity);

    }
}

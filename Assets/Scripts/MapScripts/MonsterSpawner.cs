using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Monster[] _monsterList;
    [SerializeField] private Transform[] _spawnPoint;
    private List<Monster> _currentMonster = new();

    private void Start() => SpawnMonster();

    private void SpawnMonster()
    {
        for(int i = 0; i < _spawnPoint.Length; i++)
        {
            Monster tempMonster = Instantiate(_monsterList[0], _spawnPoint[i].position, _spawnPoint[i].rotation);
            tempMonster.gameObject.SetActive(false);
            _currentMonster.Add(tempMonster);
        }
    }

    public bool CheckEliminated()
    {
        foreach(Monster monster in _currentMonster)
        {
            if(monster == null)
            {
                _currentMonster.Remove(monster);
            }
        }

        if (_currentMonster.Count == 0) return true;

        return false;
    }

    public void ActiveMonster()
    {
        foreach(Monster monster in _currentMonster)
        {
            monster.gameObject.SetActive(true);
        }
    }
}

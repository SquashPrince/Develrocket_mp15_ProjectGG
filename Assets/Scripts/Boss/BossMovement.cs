using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    //[SerializeField] private Transform _player;
    private Transform _player;

    public Transform Player
    {
        get { return _player; }
        set { _player = value; }
    }
    
    private void Start()
    {
        StartCoroutine(InitRoutine());
    }
    
    public IEnumerator InitRoutine()
    {
        yield return new WaitUntil(()=> GameManager.Instance !=null && GameManager.Instance.PlayerTransform != null);
        Player = GameManager.Instance.PlayerTransform;
    }

    private Monster _mon;
    private void Update()
    {
        if (!_monster.isDead && _player != null)
        {
            MoveToPlayer();    
        }
        
    }

    private void MoveToPlayer()
    {
        /*Vector3 direction1 = _player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction1);*/

        LookPlayer();
        
        float distance = Vector3.Distance(transform.transform.position, _player.transform.position);
        if (distance > 5)
        {
            Vector3 direction =
                (_player.position - transform.position).normalized;

            transform.position +=
                direction * _monster.MoveSpeed * Time.deltaTime;
        }
        
    }

    public bool canRotate = true;
    private void LookPlayer()
    {
        if (!canRotate || _player == null) return;
        transform.LookAt(Player.position);
    }

    public void SetTargetChange(Transform target)
    {
        canRotate = false;
        Player = target;
    }
    
}

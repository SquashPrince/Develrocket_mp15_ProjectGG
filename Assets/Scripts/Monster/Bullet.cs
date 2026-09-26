using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    public static readonly HashSet<Bullet> ActiveBullets = new HashSet<Bullet>();
    private void OnEnable() => ActiveBullets.Add(this);
    private void OnDisable() => ActiveBullets.Remove(this);
    private Vector3 _direction;

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        Vector3 next = transform.position + transform.TransformDirection(_direction * _speed * Time.deltaTime);
        if (BulletBarrier.BlocksSegment(transform.position, next, gameObject.layer))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        transform.position = next;
    }
}

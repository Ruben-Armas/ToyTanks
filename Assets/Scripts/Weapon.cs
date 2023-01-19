using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform muzzle;
    public Bullet bulletPrefab;

    public void Fire()
    {
        Debug.Log(muzzle.position);
        Instantiate(
            bulletPrefab,
            muzzle.position,
            transform.rotation);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Rifle : MonoBehaviour
{
    [SerializeField] private GunScriptableObject gunData;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private VisualEffect muzzleFlash;
    [SerializeField] private GameObject hitMarker;

    /*[SerializeField] private float recoil = 0.1f;
    [SerializeField] private float accuracyNotScoped = 0.5f;
    [SerializeField] private float accuracyScoped = 0.1f;
    [SerializeField] private float aimCone = 5f;*/

    [SerializeField] private float gravityForce;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float bulletLifetime;

    [SerializeField] private GameObject scope;

    float nextTimeToFire;

    /*private void Update()
    {
        gunData.Scope(scope);

        *//*Quaternion shootPointRotation = new Quaternion(Random.Range(-aimCone, aimCone), Random.Range(-aimCone, aimCone), 0, 0);
        shootPoint.rotation = shootPointRotation;*//*
        
        if (Input.GetKey(gunData.fireKey) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / gunData.fireRate;
            Shoot();
        }
    }*/

    /*void Shoot()
    {
        muzzleFlash.Play();

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        RifleBullet bulletScript = bullet.GetComponent<RifleBullet>();
        if (bulletScript)
        {
            bulletScript.Initialise(shootPoint, shootSpeed, gravityForce, gunData.damage, hitMarker);
        }
        Destroy(bullet, bulletLifetime);
    }*/
}

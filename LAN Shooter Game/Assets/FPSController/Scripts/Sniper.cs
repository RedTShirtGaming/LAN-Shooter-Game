using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Sniper : MonoBehaviour
{
    public GunScriptableObject gunData;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public VisualEffect muzzleFlash;
    public GameObject hitMarker;

    public float gravityForce;
    public float shootSpeed;
    public float bulletLifetime;

    public GameObject scope;

    float nextTimeToFire;

    /*private void Update()
    {
        gunData.Scope(scope);

        if (Input.GetKeyDown(gunData.fireKey) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / gunData.fireRate;
            Shoot();
        }
    }*/

    /*void Shoot()
    {
        muzzleFlash.Play();
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        SniperBullet bulletScript = bullet.GetComponent<SniperBullet>();
        if (bulletScript)
        {
            bulletScript.Initialise(shootPoint, shootSpeed, gravityForce, gunData.damage, hitMarker);
        }
        Destroy(bullet, bulletLifetime);
    }*/
}

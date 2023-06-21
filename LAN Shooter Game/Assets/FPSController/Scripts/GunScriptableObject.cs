using UnityEngine;
using System;
using UnityEngine.VFX;

/*
 * THE CLASS "GunScriptableObject" IS UNUSED
 * 
 * THE CLASSES "RecoilVariables, GunKeys, GunInfo, BulletInfo" ARE USED IN "Gun.cs"
 */



[CreateAssetMenu]
public class GunScriptableObject : ScriptableObject
{
    public string gunName = "New gun";

    [Space(10)]
    public RecoilVariables recoil;

    [Space(10)]
    public GunKeys gunKeys;

    [Space(10)]
    public GunInfo gunInfo;

    [Space(10)]
    public BulletInfo bulletInfo;

    float nextTimeToFire;

    public void Shoot()
    {
        if (!gunInfo.suppressed) gunInfo.muzzleFlash.Play();

        GameObject bullet = Instantiate(bulletInfo.bulletPrefab, gunInfo.shootPoint.position, gunInfo.shootPoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript)
        {
            bulletScript.Initialise(gunInfo.shootPoint, bulletInfo.shootSpeed, bulletInfo.gravityForce, gunInfo.damage, gunInfo.headshotDamage, gunInfo.hitMarker);
        }
        Destroy(bullet, bulletInfo.bulletLifetime);
    }

    public void CycleFiringMode()
    {
        if (gunInfo.gunFireMode == GunFireMode.Burst) { gunInfo.gunFireMode = GunFireMode.SemiAutomatic; }
        else if (gunInfo.gunFireMode == GunFireMode.SemiAutomatic) { gunInfo.gunFireMode = GunFireMode.Automatic; }
        else if (gunInfo.gunFireMode == GunFireMode.Automatic) { gunInfo.gunFireMode = GunFireMode.Burst; }

        Debug.Log(gunInfo.gunFireMode);
    }
}

[Serializable]
public class RecoilVariables
{
    public Recoil recoilScript;
    
    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public float adsRecoilX;
    public float adsRecoilY;
    public float adsRecoilZ;

    /*[SerializeField] private float crouchingRecoilX;
    [SerializeField] private float crouchingRecoilY;
    [SerializeField] private float crouchingRecoilZ;*/
}

[Serializable]
public class GunKeys
{
    public KeyCode fire = KeyCode.Mouse0;
    public KeyCode altFire = KeyCode.Mouse1;
    public KeyCode reload = KeyCode.R;
    public KeyCode switchFiringMode = KeyCode.B;
}

[Serializable]
public class GunInfo
{
    public float damage;
    public float headshotDamage;
    public float fireRate;

    public int ammo;
    public int ammoPerMags;
    public int startingMagazines;

    public GunFireMode gunFireMode;    

    public GameObject suppressor;
    public bool suppressed;

    public Transform shootPoint;

    public VisualEffect muzzleFlash;

    public GameObject hitMarker;

    public bool scoped = true;
}

[Serializable]
public class BulletInfo
{
    public GameObject bulletPrefab;

    public float gravityForce = 9.81f;
    public float shootSpeed = 800f;
    public float bulletLifetime = 15f;
}

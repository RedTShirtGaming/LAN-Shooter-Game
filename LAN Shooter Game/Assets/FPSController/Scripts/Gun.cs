using System.Collections;
using UnityEngine;
using QFSW.QC;
using UnityEngine.VFX;
using Unity.Netcode;

public enum GunFireMode { Burst, SemiAutomatic, Automatic };

public class Gun : NetworkBehaviour
{
    public string gunName;

    [Space(10), Header("Recoil")]
    public Recoil recoilScript;

    public float recoilX = -2;
    public float recoilY = 2;
    public float recoilZ = 0.35f;

    public float adsRecoilX = -1.5f;
    public float adsRecoilY = 2;
    public float adsRecoilZ = 0.3f;

    [Space(10), Header("Gun controls")]
    public GunKeys gunKeys;

    [Space(10), Header("Gun Stats")]
    public float damage = 10;
    public float headshotDamage = 25;
    public float fireRate = 3;

    public int ammo = 30;
    public int ammoPerMags = 30;
    public int startingMagazines = 5;

    public GunFireMode gunFireMode;

    public GameObject suppressor;
    public bool suppressed;

    public Transform shootPoint;

    public VisualEffect muzzleFlash;

    public GameObject hitMarker;

    public bool scoped = true;

    [Space(10), Header("Bullet")]
    public GameObject bulletPrefab;

    public float gravityForce = 9.81f;
    public float shootSpeed = 800f;
    public float bulletLifetime = 15f;

    public Animator animator;

    float nextTimeToFire = 0;

    public bool sniperScope = false;
    public GameObject scopeCamera;
    public float sniperScopeCameraSwitchDelay;

    public void Shoot()
    {
        if (!suppressed) muzzleFlash.Play();

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript)
        {
            bulletScript.Initialise(shootPoint, shootSpeed, gravityForce, damage, headshotDamage, hitMarker);
        }
        Destroy(bullet, bulletLifetime);
    }

    public void CycleFiringMode()
    {
        if (gunFireMode == GunFireMode.Burst) { gunFireMode = GunFireMode.SemiAutomatic; }
        else if (gunFireMode == GunFireMode.SemiAutomatic) { gunFireMode = GunFireMode.Automatic; }
        else if (gunFireMode == GunFireMode.Automatic) { gunFireMode = GunFireMode.Burst; }

        Debug.Log(gunFireMode);
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        suppressed = suppressor.activeSelf;

        if (Input.GetKeyDown(gunKeys.switchFiringMode))
        {
            CycleFiringMode();
        }

        if (Input.GetKeyDown(gunKeys.altFire) && !sniperScope)
        {
            animator.SetBool("ScopeState", true);
            scoped = true;
        }
        else if (Input.GetKeyUp(gunKeys.altFire) && !sniperScope)
        {
            animator.SetBool("ScopeState", false);
            scoped = false;
        }

        if (Input.GetKeyDown(gunKeys.altFire) && sniperScope)
        {
            if (scoped)
            {
                StartCoroutine(SniperScopeOut());
            } else
            {
                StartCoroutine(SniperScopeIn());
            }
        }

        if (Input.GetKey(gunKeys.fire) && Time.time >= nextTimeToFire && gunFireMode == GunFireMode.Automatic)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            recoilScript.RecoilFire(recoilX, recoilY, recoilZ, adsRecoilX, adsRecoilY, adsRecoilZ, scoped);
        }
        if (Input.GetKeyDown(gunKeys.fire) && Time.time >= nextTimeToFire && gunFireMode == GunFireMode.SemiAutomatic)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            recoilScript.RecoilFire(recoilX, recoilY, recoilZ, adsRecoilX, adsRecoilY, adsRecoilZ, scoped);
        }
    }

    private void Start()
    {
        nextTimeToFire = 0;
    }

    IEnumerator SniperScopeIn()
    {
        animator.SetBool("ScopeState", true);
        yield return new WaitForSeconds(sniperScopeCameraSwitchDelay);
        scopeCamera.SetActive(true);
        scoped = true;
    }
    IEnumerator SniperScopeOut()
    {
        scopeCamera.SetActive(false);
        animator.SetBool("ScopeState", false);
        yield return new WaitForSeconds(sniperScopeCameraSwitchDelay);
        scoped = false;
    }
}

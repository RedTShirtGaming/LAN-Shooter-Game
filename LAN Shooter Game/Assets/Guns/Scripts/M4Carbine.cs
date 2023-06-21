using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class M4Carbine : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform playerBody;
    
    [SerializeField] private GunScriptableObject gunData;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private VisualEffect muzzleFlash;
    [SerializeField] private GameObject hitMarker;

    public bool scoped;

    [SerializeField] private float gravityForce;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float bulletLifetime;

    [SerializeField] private bool suppressed;
    [SerializeField] private GameObject suppressor;

    // Recoil

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float adsRecoilX;
    [SerializeField] private float adsRecoilY;
    [SerializeField] private float adsRecoilZ;
    /*[SerializeField] private Vector2 bulletSpawnPointRotation = new Vector2(1, 1);
    [SerializeField] private float acuraccyRegainSpeed = 2f;*/

    private Recoil recoilScript;

    float nextTimeToFire;

    private void Start()
    {
        recoilScript = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
    }

    void Shoot()
    {
        /*// Generate a random angle for the x-axis rotation within the specified range
        float randomXRotation = Random.Range(-bulletSpawnPointRotation.x, bulletSpawnPointRotation.x);

        // Generate a random angle for the y-axis rotation within the specified range
        float randomYRotation = Random.Range(-bulletSpawnPointRotation.y, bulletSpawnPointRotation.y);

        // Combine the random x-axis and y-axis rotations into a single Quaternion
        Quaternion randomRotation = Quaternion.Euler(randomXRotation, randomYRotation, 0f);

        // Apply the random rotation to the shootPoint GameObject
        shootPoint.rotation = randomRotation * shootPoint.rotation;*/



        if (!suppressed) muzzleFlash.Play();

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        RifleBullet bulletScript = bullet.GetComponent<RifleBullet>();
        if (bulletScript)
        {
            bulletScript.Initialise(shootPoint, shootSpeed, gravityForce, gunData.gunInfo.damage, hitMarker);
        }
        Destroy(bullet, bulletLifetime);
    }

    private void Update()
    {
        suppressed = suppressor.activeSelf;

        /*if (Input.GetKeyUp(gunData.fireKey)) 
        {
            Vector3 currentRot;
            currentRot = new Vector3(shootPoint.rotation.x, shootPoint.rotation.y, shootPoint.rotation.z);

            Vector3 returnRot = Vector3.Slerp(currentRot, new Vector3(0, -90, 0), acuraccyRegainSpeed * Time.fixedDeltaTime);
            Quaternion targetRot;
            targetRot = new Quaternion(returnRot.x, returnRot.y, returnRot.z, 0);
            shootPoint.rotation = targetRot;
        }*/
        
        if (Input.GetKeyDown(gunData.gunKeys.switchFiringMode))
        {
            CycleFiringMode();
        }

        if (Input.GetKeyDown(gunData.gunKeys.altFire))
        {
            animator.SetBool("ScopeState", true);
            scoped = true;
        }
        else if (Input.GetKeyUp(gunData.gunKeys.altFire))
        {
            animator.SetBool("ScopeState", false);
            scoped = false;
        }

        /*Quaternion shootPointRotation = new Quaternion(Random.Range(-aimCone, aimCone), Random.Range(-aimCone, aimCone), 0, 0);
        shootPoint.rotation = shootPointRotation;*/

        if (Input.GetKey(gunData.gunKeys.fire) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / gunData.gunInfo.fireRate;
            Shoot();
            recoilScript.RecoilFire(recoilX, recoilY, recoilZ, adsRecoilX, adsRecoilY, adsRecoilZ, scoped);
        }
    }

    void CycleFiringMode()
    {
        if (gunData.gunInfo.gunFireMode == GunFireMode.Burst) { gunData.gunInfo.gunFireMode = GunFireMode.SemiAutomatic; }
        else if (gunData.gunInfo.gunFireMode == GunFireMode.SemiAutomatic) { gunData.gunInfo.gunFireMode = GunFireMode.Automatic; }
        else if (gunData.gunInfo.gunFireMode == GunFireMode.Automatic) { gunData.gunInfo.gunFireMode = GunFireMode.Burst; }

        print(gunData.gunInfo.gunFireMode);
    }
}

using Unity.Netcode;
using UnityEngine;

public class Recoil : NetworkBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    void Update()
    { 
        if (!IsOwner) return;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(float recoilX, float recoilY, float recoilZ, float adsRecoilX, float adsRecoilY, float adsRecoilZ, bool scoped)
    {
        if (scoped) targetRotation += new Vector3(adsRecoilX, Random.Range(-adsRecoilY, adsRecoilY), Random.Range(-adsRecoilZ, adsRecoilZ));
        else targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    [ServerRpc]
    public void RecoilFireServerRpc(float recoilX, float recoilY, float recoilZ, float adsRecoilX, float adsRecoilY, float adsRecoilZ, bool scoped)
    {
        RecoilFire(recoilX, recoilY, recoilZ, adsRecoilX, adsRecoilY, adsRecoilZ, scoped);
    }
}

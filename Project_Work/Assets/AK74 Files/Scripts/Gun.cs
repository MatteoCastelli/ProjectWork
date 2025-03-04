using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform cam;

    float timeSinceLastShot;

    private void Start()
    {
        PlayerShoot.shootInput += Shoot; // Collega lo sparo
        PlayerShoot.reloadInput += StartReload;
    }

    private void OnDisable() => gunData.reloading = false;

    public void StartReload()
    {
        if (!gunData.reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        gunData.reloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;

        gunData.reloading = false;
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    private void Shoot()
    {
        Debug.Log("Funzione Shoot chiamata!"); // Debug per verificare se la funzione viene chiamata

        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                Debug.Log("Arma pronta a sparare!"); // Debug per verificare che l'arma sia pronta a sparare
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, gunData.maxDistance))
                {
                    Debug.Log("Colpito qualcosa!"); // Debug per tracciare se il colpo ha colpito qualcosa
                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    damageable?.TakeDamage(gunData.damage);
                }

                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
            else
            {
                Debug.Log("Non è possibile sparare (arma in ricarica o rateo di fuoco non rispettato).");
            }
        }
        else
        {
            Debug.Log("Munizioni esaurite!");
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(cam.position, cam.forward * gunData.maxDistance);
    }

    private void OnGunShot() { }
}

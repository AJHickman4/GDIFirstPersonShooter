using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLife = 3f;
    public Camera playerCamera;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        // Align bullet spawn with the camera's forward direction
        bulletSpawn.forward = playerCamera.transform.forward;

        //Debug.Log($"Applying force: {bulletSpawn.forward.normalized}");
        //Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward.normalized * 10, Color.red, 2.0f);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLife));
    }

    IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}

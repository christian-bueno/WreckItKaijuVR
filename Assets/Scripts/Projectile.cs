using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ProjectileVR : MonoBehaviour
{
    public GameObject magicProjectile;
    public float shootForce;
    public float timeBetweenShots;

    private bool readyToShoot;
    private bool shooting;

    public SteamVR_Behaviour_Pose pose;  
    public SteamVR_Action_Boolean fireAction;  

    public bool allowInvoke = true;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Update()
    {
        // Check if the fire button is being pressed using SteamVR input
        shooting = fireAction.GetState(pose.inputSource);

        // Shooting 
        if (readyToShoot && shooting)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Create a ray from the controller
        Ray ray = new Ray(pose.transform.position, pose.transform.forward);
        RaycastHit hit;

        // Determine the target point using raycast
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); // Default distant point

        // Calculate direction from the current position to the target point
        Vector3 projectileDirection = targetPoint - pose.transform.position;

        // Instantiate and configure the projectile
        GameObject currentMagicProjectile = Instantiate(magicProjectile, pose.transform.position, Quaternion.identity);
        currentMagicProjectile.transform.forward = projectileDirection.normalized;
        currentMagicProjectile.GetComponent<Rigidbody>().AddForce(projectileDirection.normalized * shootForce, ForceMode.Impulse);

        Destroy(currentMagicProjectile, 1f);  // Adjust the time as needed

        // Reset shooting mechanism
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShots);
            allowInvoke = false;
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

}

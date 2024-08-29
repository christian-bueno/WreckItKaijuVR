using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Road") // Example: Only destroy on hitting an enemy
        {
            Destroy(gameObject);
        }
    }

}

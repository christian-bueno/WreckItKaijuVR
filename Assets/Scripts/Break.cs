using UnityEngine;

public class SimpleBreakable : MonoBehaviour
{
    public GameObject[] brokenPieces;  // Array to store the broken pieces prefabs
    public float explosionForce = 5f; // The force of explosion
    public float explosionRadius = 1f; // Radius in which pieces are affected
    public float explosionUpward = 0.1f; // Upward modifier to make explosion lift objects
    public string[] allowedTags;        // Tags to ignore during collision checks

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object's tag is in the ignored tags list
        foreach (var tag in allowedTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                Break();
                return;
            }
        }
    }

    public void Break()
    {
        foreach (var piece in brokenPieces)
        {
            GameObject pieceClone = Instantiate(piece, transform.position, transform.rotation);
            Rigidbody rb = pieceClone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward, ForceMode.Impulse);
            }
        }
        gameObject.SetActive(false);
    }   

}

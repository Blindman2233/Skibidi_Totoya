using UnityEngine;
using System.Collections;

public class WaterBall : MonoBehaviour
{
    public float lifeTime = 4f; // Delete after 4 seconds
    public float squishAmount = 1.3f; // How much it jiggles when hitting walls

    void Start()
    {
        // 1. Safety Timer: Destroy self automatically to prevent lag
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 2. The "Jelly" Effect: Squish when hitting something hard
        if (collision.relativeVelocity.magnitude > 2f)
        {
            StartCoroutine(SquishAnimation());
        }
    }

    IEnumerator SquishAnimation()
    {
        // Make it flat (Wide X, Short Y)
        transform.localScale = new Vector3(squishAmount, 1f / squishAmount, 1f);
        yield return new WaitForSeconds(0.05f);

        // Make it tall (Thin X, Tall Y)
        transform.localScale = new Vector3(1f / squishAmount, squishAmount, 1f);
        yield return new WaitForSeconds(0.05f);

        // Back to normal
        transform.localScale = Vector3.one;
    }
}
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public Marble marble;
    public Heart heart;
    public GameObject explosion;

    // Update marble speed
    public void UpdateSpeed(float speed)
    {
        marble.speed = speed;
    }

    public void Destroy()
    {
        Destroy(Instantiate(explosion, heart.transform.position, Quaternion.identity), 1);
        Destroy(gameObject);
    }
}
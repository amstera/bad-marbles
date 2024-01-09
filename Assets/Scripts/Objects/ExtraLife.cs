using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public Marble marble;
    public Heart heart;
    public GameObject explosion;
    public GameObject floatingHeart;

    // Update marble speed
    public void UpdateSpeed(float speed)
    {
        marble.speed = speed;
    }

    public void Destroy()
    {
        Destroy(Instantiate(explosion, marble.transform.position, Quaternion.identity), 1);
        Instantiate(floatingHeart, marble.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Update()
    {
        if (marble.transform.position.z < -18)
        {
            Destroy(gameObject);
        }
    }
}
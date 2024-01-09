using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public Marble marble;
    public Heart heart;
    public GameObject explosion;
    public PointsText pointsTextPrefab;

    // Update marble speed
    public void UpdateSpeed(float speed)
    {
        marble.speed = speed;
    }

    public void Destroy()
    {
        Destroy(Instantiate(explosion, heart.transform.position, Quaternion.identity), 1);
        PointsText pointsTextInstance = Instantiate(pointsTextPrefab, new Vector3(heart.transform.position.x, heart.transform.position.y, -18f), Quaternion.identity);
        pointsTextInstance.SetLives(false);
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
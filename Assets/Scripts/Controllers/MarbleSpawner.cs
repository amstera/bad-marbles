using UnityEngine;

public class MarbleSpawner : MonoBehaviour
{
    public Marble GreenMarble;
    public Marble RedMarble;
    public float spawnInterval = 0.7f;
    private float timer = 0;
    private float speed = 5f;
    private float maxSpeed = 30f;
    private float acceleration = 0.38f;

    void Update()
    {
        if (GameManager.Instance.Lives <= 0)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (speed < maxSpeed)
        {
            // Calculate the rate of growth, which decreases as speed approaches maxSpeed
            float growthRate = (maxSpeed - speed) / maxSpeed;
            speed = Mathf.Min(speed + growthRate * acceleration * Time.deltaTime, maxSpeed);
        }

        if (timer <= 0f)
        {
            SpawnMarble();
            float newInterval = Mathf.Max(0.15f, spawnInterval - (speed/1.5f / maxSpeed));
            timer = newInterval;
        }
    }

    void SpawnMarble()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-3.9f, 3.9f), 14f, 23f);

        Marble marbleToSpawn;
        int randomValue = Random.Range(0, 100);
        if (randomValue < 45)
        {
            marbleToSpawn = GreenMarble;
        }
        else
        {
            marbleToSpawn = RedMarble;
        }

        Marble spawnedMarble = Instantiate(marbleToSpawn, spawnPosition, Quaternion.identity);
        spawnedMarble.speed = speed;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarbleSpawner : MonoBehaviour
{
    public Marble GreenMarble;
    public Marble RedMarble;
    public Marble FireMarble;
    public Tier TierPrefab;
    public float spawnInterval = 0.6f;
    private float timer = 0;
    private float speed = 8f;
    private float maxSpeed = 40f;
    private float acceleration = 0.6f;
    private bool isSpawningPaused = true;
    private List<Marble> allMarbles = new List<Marble>();

    void Update()
    {
        if (GameManager.Instance.Lives <= 0 || isSpawningPaused)
        {
            return;
        }

        UpdateSpeed();
        TrySpawnMarble();
    }

    void UpdateSpeed()
    {
        if (speed < maxSpeed)
        {
            float growthRate = (maxSpeed - speed) / maxSpeed;
            speed = Mathf.Min(speed + growthRate * acceleration * Time.deltaTime, maxSpeed);
        }
    }

    void TrySpawnMarble()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnMarble();
            UpdateTimer();
        }
    }

    Vector3 GetSpawnPosition(bool isTier = false)
    {
        float xPosition = isTier ? 0 : Random.Range(-3.9f, 3.9f);
        return new Vector3(xPosition, 14f, 23f);
    }

    void SpawnMarble()
    {
        int tier = GameManager.Instance.Tier;
        int randomValue = Random.Range(0, 100);
        Marble marbleToSpawn;

        if (tier >= 5)
        {
            if (randomValue < 25) marbleToSpawn = FireMarble;
            else if (randomValue < 55) marbleToSpawn = RedMarble;
            else marbleToSpawn = GreenMarble;
        }
        else if (tier >= 3)
        {
            if (randomValue < 10) marbleToSpawn = FireMarble;
            else if (randomValue < 50) marbleToSpawn = RedMarble;
            else marbleToSpawn = GreenMarble;
        }
        else
        {
            marbleToSpawn = randomValue < 50 ? RedMarble : GreenMarble;
        }

        Marble spawnedMarble = Instantiate(marbleToSpawn, GetSpawnPosition(), marbleToSpawn.transform.rotation);
        spawnedMarble.speed = speed;
        allMarbles.Add(spawnedMarble);
    }

    void UpdateTimer()
    {
        float newInterval = Mathf.Max(0.15f, spawnInterval - (speed / 1.5f / maxSpeed));
        timer = newInterval;
    }

    public void UpdateTier(int tier)
    {
        if (tier > 1)
        {
            Tier newTier = Instantiate(TierPrefab, GetSpawnPosition(true), Quaternion.identity);
            newTier.text.text = $"Tier {tier}";
            newTier.marble.speed = speed;

            Destroy(newTier.gameObject, 10);
        }

        StartCoroutine(PauseSpawning());
    }

    public void DestroyAll()
    {
        foreach (var marble in allMarbles)
        {
            if (marble != null)
            {
                marble.Destroy();
            }
        }
    }

    IEnumerator PauseSpawning()
    {
        isSpawningPaused = true;
        float pauseDuration = -0.1f * speed + 3f;

        pauseDuration = Mathf.Clamp(pauseDuration, 0.75f, 2.0f);

        yield return new WaitForSeconds(pauseDuration);
        isSpawningPaused = false;
    }
}
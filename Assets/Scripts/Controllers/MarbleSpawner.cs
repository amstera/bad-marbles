using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MarbleSpawner : MonoBehaviour
{
    public Marble GreenMarble;
    public Marble RedMarble;
    public Tier TierPrefab;
    public float spawnInterval = 0.65f;
    private float timer = 0;
    private float speed = 5f;
    private float maxSpeed = 30f;
    private float acceleration = 0.4f;
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
        Marble marbleToSpawn = Random.Range(0, 100) < 45 ? GreenMarble : RedMarble;
        Marble spawnedMarble = Instantiate(marbleToSpawn, GetSpawnPosition(), Quaternion.identity);
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
        Tier newTier = Instantiate(TierPrefab, GetSpawnPosition(true), Quaternion.identity);
        newTier.text.text = $"Tier {tier}";
        newTier.marble.speed = speed;

        Destroy(newTier.gameObject, 10);

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

        pauseDuration = Mathf.Clamp(pauseDuration, 0.5f, 2.0f);

        yield return new WaitForSeconds(pauseDuration);
        isSpawningPaused = false;
    }
}
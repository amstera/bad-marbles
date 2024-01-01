using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarbleSpawner : MonoBehaviour
{
    public Marble GreenMarble;
    public Marble AngelMarble;
    public Marble GoldMarble;
    public Marble RedMarble;
    public Marble BigRedMarble;
    public Marble FireMarble;
    public TopMarble TopRedMarble;
    public Tier TierPrefab;
    public Ramp ramp;
    public float speed = 8f;
    private float spawnInterval = 1f;
    private float timer = 0;
    private float maxSpeed = 45f;
    private float acceleration = 0.55f;
    private bool isSpawningPaused = true;
    private bool hasUpdatedTier;
    private int frameCounter = 0;
    private int tier;
    private List<Marble> allMarbles = new List<Marble>();

    private SaveObject savedData;
    private bool hasAngelMarble;
    private bool hasGoldMarble;

    void Start()
    {
        savedData = SaveManager.Load();
        CheckForPerks();
        ramp.UpdateScrollSpeed(speed);
    }

    void Update()
    {
        if (GameManager.Instance.Lives <= 0 || isSpawningPaused)
        {
            return;
        }

        UpdateSpeed();

        // Update marble spawning every second frame
        if (frameCounter % 2 == 0)
        {
            TrySpawnMarble();
        }

        frameCounter++;
    }


    void UpdateSpeed()
    {
        if (speed < maxSpeed)
        {
            float growthRate = (maxSpeed - speed) / maxSpeed;
            if (tier < 3)
            {
                growthRate *= 2.125f;
            }
            else
            {
                growthRate *= 1.825f;
            }
            speed = Mathf.Min(speed + growthRate * acceleration * Time.deltaTime, maxSpeed);

            ramp.UpdateScrollSpeed(speed);
        }
    }

    void TrySpawnMarble()
    {
        timer -= Time.deltaTime * 2;  // Adjusting for every second frame update
        if (timer <= 0f)
        {
            SpawnMarble();
            UpdateTimer();
        }
    }

    Vector3 GetSpawnPosition(MarbleColor type, bool isTier = false)
    {
        float xPosition;
        float yPosition = 14f;
        if (isTier)
        {
            xPosition = 0;
        }
        else if (type == MarbleColor.BigRed)
        {
            xPosition = Random.Range(-3f, 3f);
            yPosition = 14.5f;
        }
        else
        {
            xPosition = Random.Range(-3.9f, 3.9f);
        }
        return new Vector3(xPosition, yPosition, 23.5f);
    }

    void SpawnMarble()
    {
        int tier = GameManager.Instance.Tier;
        int randomValue = Random.Range(0, 100);
        Marble marbleToSpawn = DetermineMarbleToSpawn(tier, randomValue);

        if (marbleToSpawn != null)
        {
            Marble spawnedMarble = Instantiate(marbleToSpawn, GetSpawnPosition(marbleToSpawn.color), marbleToSpawn.transform.rotation);
            spawnedMarble.speed = speed;
            allMarbles.Add(spawnedMarble);
        }
    }

    Marble DetermineMarbleToSpawn(int tier, int randomValue)
    {
        if (tier >= 12)
        {
            if (randomValue < 8)
            {
                SpawnPairedMarble(RedMarble, TopRedMarble);
                return null;
            }
            if (randomValue < 20) return BigRedMarble;
            else if (randomValue < 40) return FireMarble;
            else if (randomValue < 55) return RedMarble;
        }
        if (tier >= 10)
        {
            if (randomValue < 5)
            {
                SpawnPairedMarble(RedMarble, TopRedMarble);
                return null;
            }
            if (randomValue < 15) return BigRedMarble;
            else if (randomValue < 35) return FireMarble;
            else if (randomValue < 55) return RedMarble;
        }
        else if (tier >= 7)
        {
            if (randomValue < 4)
            {
                SpawnPairedMarble(RedMarble, TopRedMarble);
                return null;
            }
            if (randomValue < 8) return BigRedMarble;
            else if (randomValue < 20) return FireMarble;
            else if (randomValue < 55) return RedMarble;
        }
        else if (tier >= 5)
        {
            if (randomValue < 2)
            {
                SpawnPairedMarble(RedMarble, TopRedMarble);
                return null;
            }
            if (randomValue < 6) return BigRedMarble;
            else if (randomValue < 15) return FireMarble;
            else if (randomValue < 50) return RedMarble;
        }
        else if (tier >= 4)
        {
            if (randomValue < 5) return BigRedMarble;
            else if (randomValue < 15) return FireMarble;
            else if (randomValue < 50) return RedMarble;
        }
        else if (tier >= 3)
        {
            if (randomValue < 2) return BigRedMarble;
            if (randomValue < 10) return FireMarble;
            else if (randomValue < 50) return RedMarble;
        }
        else if (tier >= 2)
        {
            if (randomValue < 5) return FireMarble;
            else if (randomValue < 50) return RedMarble;
        }
        else
        {
            if (randomValue < 45) return RedMarble;
        }

        int totalMarbles = 100;
        int angelMarbles = hasAngelMarble ? 10 : 0; // 10% chance for angel marbles
        int goldMarbles = hasGoldMarble ? 3 : 0;   // 3% chance for gold marbles
        int greenMarbles = totalMarbles - angelMarbles - goldMarbles;
        if (greenMarbles == totalMarbles)
        {
            return GreenMarble;
        }

        int pickedMarble = Random.Range(0, totalMarbles);
        if (pickedMarble < angelMarbles)
        {
            return AngelMarble;
        }
        else if (pickedMarble < angelMarbles + goldMarbles)
        {
            return GoldMarble;
        }
        
        return GreenMarble;
    }

    void UpdateTimer()
    {
        float intervalConstant = 0.95f;
        if (tier > 3)
        {
            intervalConstant = 1f;
        }
        if (tier > 6)
        {
            intervalConstant = 1.05f;
        }
        float newInterval = Mathf.Clamp(spawnInterval - (speed / intervalConstant / maxSpeed), 0.13f, spawnInterval);
        timer = newInterval;
    }

    public void UpdateTier(int tier)
    {
        if (tier > 1)
        {
            Tier newTier = Instantiate(TierPrefab, GetSpawnPosition(MarbleColor.Unknown, true), Quaternion.identity);
            newTier.UpdateTier(tier);
            newTier.UpdateSpeed(hasUpdatedTier ? speed : 15);

            Destroy(newTier.gameObject, 5);
        }

        this.tier = tier;

        StartCoroutine(PauseSpawning(tier));

        hasUpdatedTier = true;
    }

    public void DestroyAll(bool onlyBad = false)
    {
        foreach (var marble in allMarbles)
        {
            if (marble == null)
            {
                continue;
            }

            if (!onlyBad || marble.livesLost > 0)
            {
                marble.Destroy();
            }
        }
    }

    IEnumerator PauseSpawning(int tier)
    {
        isSpawningPaused = true;
        float pauseDuration;

        if (tier == 1)
        {
            pauseDuration = 1.5f;
        }
        else
        {
            pauseDuration = -0.1f * speed + 3f;
            pauseDuration = Mathf.Clamp(pauseDuration, 0.75f, 2f);
            if (!hasUpdatedTier)
            {
                pauseDuration = tier > 3 ? 2.5f : 2;
            }
        }

        yield return new WaitForSeconds(pauseDuration);
        isSpawningPaused = false;
    }

    private void CheckForPerks()
    {
        foreach (var perk in savedData.SelectedPerks.SelectedSpecial)
        {
            if (perk == PerkEnum.AngelMarble)
            {
                hasAngelMarble = true;
            }
            else if (perk == PerkEnum.GoldMarble)
            {
                hasGoldMarble = true;
            }
        }
    }

    void SpawnPairedMarble(Marble bottomMarblePrefab, TopMarble topMarblePrefab)
    {
        Vector3 spawnPosition = GetSpawnPosition(bottomMarblePrefab.color);
        Marble bottomMarble = Instantiate(bottomMarblePrefab, spawnPosition, bottomMarblePrefab.transform.rotation);
        TopMarble topMarble = Instantiate(topMarblePrefab, new Vector3(spawnPosition.x, spawnPosition.y + 1.5f, spawnPosition.z), topMarblePrefab.transform.rotation);

        topMarble.bottomMarble = bottomMarble;
        bottomMarble.speed = speed;
        topMarble.speed = speed;

        allMarbles.Add(bottomMarble);
        allMarbles.Add(topMarble);
    }
}
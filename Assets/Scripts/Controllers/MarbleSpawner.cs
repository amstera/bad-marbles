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

    public float speed = 8f;
    public bool canUpdateSpeed = true;
    public MarbleColor OnlyMarbleColor;

    private float maxSpeed = 45f;
    private float spawnInterval = 1f;
    private float timer = 0;
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
    }

    void Update()
    {
        if (GameManager.Instance.Lives <= 0 || isSpawningPaused)
        {
            return;
        }

        UpdateSpeed();

        // Update marble spawning every fourth frame
        if (frameCounter % 4 == 0)
        {
            TrySpawnMarble();
        }

        frameCounter++;
    }


    void UpdateSpeed()
    {
        if (speed >= maxSpeed || !canUpdateSpeed)
        {
            return;
        }

        float growthRate = (maxSpeed - speed) / maxSpeed;
        growthRate *= (tier < 3) ? 2.2f : 1.8f;
        speed = Mathf.Min(speed + growthRate * acceleration * Time.deltaTime, maxSpeed);
    }

    void TrySpawnMarble()
    {
        timer -= Time.deltaTime * 4;  // Adjusting for every second frame update
        if (timer <= 0f)
        {
            if (tier > 1 && Random.Range(0, 100) < 5 && SpawnMarbleCouple())
            {
                // spawned marble couple
            }
            else
            {
                SpawnMarble();
            }
            UpdateTimer();
        }
    }

    Vector3 GetSpawnPosition(MarbleColor type, bool isTier = false)
    {
        float xRange = (type == MarbleColor.BigRed) ? 3f : 3.9f;
        float xPosition = isTier ? 0 : Random.Range(-xRange, xRange);
        float yPosition = (type == MarbleColor.BigRed) ? 14.5f : 14f;
        return new Vector3(xPosition, yPosition, 23.5f);
    }

    void SpawnMarble()
    {
        int randomValue = Random.Range(0, 100);
        Marble marbleToSpawn = DetermineMarbleToSpawn(tier, randomValue);

        if (marbleToSpawn != null)
        {
            Vector3 spawnPosition = GetSpawnPosition(marbleToSpawn.color);
            InstantiateMarble(marbleToSpawn, spawnPosition);
        }
    }

    Marble DetermineMarbleToSpawn(int tier, int randomValue)
    {
        if (OnlyMarbleColor != MarbleColor.Unknown)
        {
            if (OnlyMarbleColor == MarbleColor.Red) return RedMarble;
            if (OnlyMarbleColor == MarbleColor.Green) return GreenMarble;
            if (OnlyMarbleColor == MarbleColor.Fire) return FireMarble;
            if (OnlyMarbleColor == MarbleColor.BigRed) return BigRedMarble;
            if (OnlyMarbleColor == MarbleColor.Angel) return AngelMarble;
            if (OnlyMarbleColor == MarbleColor.Gold) return GoldMarble;
        }

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
            if (randomValue < 1)
            {
                SpawnPairedMarble(RedMarble, TopRedMarble);
                return null;
            }
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
        if (tier >= 10)
        {
            intervalConstant = 1.125f;
        }
        else if (tier > 6)
        {
            intervalConstant = 1.15f;
        }
        else if (tier > 3)
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
            newTier.UpdateSpeed(hasUpdatedTier ? speed * 1.1f : 15);

            Destroy(newTier.gameObject, 5);
        }

        this.tier = tier;

        StartCoroutine(PauseSpawning(tier));

        hasUpdatedTier = true;
    }

    public void DestroyAll(bool onlyBad = false)
    {
        for (int i = allMarbles.Count - 1; i >= 0; i--)
        {
            Marble marble = allMarbles[i];
            if (marble == null)
            {
                continue;
            }

            if (!onlyBad || marble.livesLost > 0)
            {
                marble.Destroy();
                allMarbles.RemoveAt(i);
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

    bool SpawnMarbleCouple()
    {
        var randomValue = Random.Range(0, 100);
        Marble firstMarble = DetermineMarbleToSpawn(tier, randomValue);
        if (firstMarble.color != MarbleColor.Red && firstMarble.color != MarbleColor.Green && firstMarble.color != MarbleColor.Fire)
        {
            return false;
        }

        Vector3 firstPosition = GetSpawnPosition(firstMarble.color);
        InstantiateMarble(firstMarble, firstPosition);

        Marble secondMarble = DetermineMarbleToSpawn(tier, randomValue);

        Vector3 secondPosition;
        do
        {
            secondPosition = GetSpawnPosition(firstMarble.color);
        } while (Mathf.Abs(secondPosition.x - firstPosition.x) < 2);
        InstantiateMarble(secondMarble, secondPosition);

        return true;
    }

    void InstantiateMarble(Marble marble, Vector3 position)
    {
        Marble spawnedMarble = Instantiate(marble, position, marble.transform.rotation);
        spawnedMarble.speed = speed;
        allMarbles.Add(spawnedMarble);
    }
}
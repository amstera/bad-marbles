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
    public Marble BombMarble;
    public Marble BlueFireMarble;
    public TopMarble TopRedMarble;
    public Tier TierPrefab;
    public ExtraLife ExtraLife;

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
    private int lastBombMarbleTier;
    private int lastExtraLifeTier;
    private int tier;
    private List<Marble> allMarbles = new List<Marble>();
    private HashSet<MarbleColor> allowedDupMarbleColors = new HashSet<MarbleColor> { MarbleColor.Red, MarbleColor.Green, MarbleColor.Fire, MarbleColor.Bomb, MarbleColor.Gold, MarbleColor.Angel, MarbleColor.BlueFire };

    private SaveObject savedData;
    private GameManager gameManager;
    private PoolManager poolManager;
    private bool hasAngelMarble;
    private bool hasGoldMarble;
    private bool hasNoBombs;

    struct MarbleProbability
    {
        public int Probability;
        public Marble MarbleType;

        public MarbleProbability(int probability, Marble marbleType)
        {
            Probability = probability;
            MarbleType = marbleType;
        }
    }

    void Start()
    {
        savedData = SaveManager.Load();
        gameManager = GameManager.Instance;
        poolManager = PoolManager.Instance;

        CheckForPerks();
    }

    void Update()
    {
        if (gameManager.Lives <= 0 || isSpawningPaused)
        {
            return;
        }

        UpdateSpeed();

        // Update marble spawning every n frame
        if (frameCounter % 10 == 0)
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
        timer -= Time.deltaTime * 8.5f;
        if (timer > 0f)
        {
            return;
        }

        int marbleCount = CalculateMarbleCount(tier);
        SpawnMarbles(marbleCount);
        UpdateTimer();
    }

    int CalculateMarbleCount(int currentTier)
    {
        if (currentTier <= 1)
        {
            return 1;
        }

        int randomNumber = Random.Range(0, 100);
        if (randomNumber >= 5 || (tier < 5 && randomNumber >= 3))
        {
            return 1;
        }

        if (currentTier > 6)
        {
            if (randomNumber < (currentTier >= 15 ? 3 : 2))
            {
                if (currentTier >= 20 && randomNumber < 2)
                {
                    return 4;
                }
                return 3;
            }
        }

        return 2;
    }

    Vector3 GetSpawnPosition(MarbleColor type)
    {
        float xRange = (type == MarbleColor.BigRed) ? 3f : 3.8f;
        float xPosition = type == MarbleColor.Tier ? 0 : Random.Range(-xRange, xRange);
        float yPosition = (type == MarbleColor.BigRed) ? 14.5f : 14f;
        return new Vector3(xPosition, yPosition, 23.5f);
    }

    Marble DetermineMarbleToSpawn(int tier, int randomValue)
    {
        if (OnlyMarbleColor != MarbleColor.Unknown)
        {
            return GetSpecificMarble(OnlyMarbleColor);
        }

        if (ShouldSpawnBombMarble(tier))
        {
            lastBombMarbleTier = tier;
            return BombMarble;
        }

        if (ShouldSpawnExtraLife(tier))
        {
            lastExtraLifeTier = tier;
            CreateExtraLife();
            return null;
        }

        var tierData = GetTierData(tier);
        foreach (var marbleProb in tierData)
        {
            if (randomValue < marbleProb.Probability)
            {
                if (marbleProb.MarbleType == TopRedMarble)
                {
                    SpawnPairedMarble(RedMarble, TopRedMarble);
                    return null;
                }
                return marbleProb.MarbleType;
            }
        }

        return GetGoodMarble();
    }

    bool ShouldSpawnBombMarble(int currentTier)
    {
        return !hasNoBombs && currentTier >= 6 && currentTier - lastBombMarbleTier >= 1 && Random.Range(0, 10) == 0;
    }

    bool ShouldSpawnExtraLife(int currentTier)
    {
        return currentTier > 1 && (currentTier > 12 ? currentTier % 6 == 0 : currentTier < 5 || currentTier % 3 == 0) && currentTier != lastExtraLifeTier && gameManager.Lives < gameManager.StartingLives;
    }

    Marble GetGoodMarble()
    {
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

    Marble GetSpecificMarble(MarbleColor color)
    {
        switch (color)
        {
            case MarbleColor.Red:
                return RedMarble;
            case MarbleColor.Green:
                return GreenMarble;
            case MarbleColor.Fire:
                return FireMarble;
            case MarbleColor.BigRed:
                return BigRedMarble;
            case MarbleColor.Angel:
                return AngelMarble;
            case MarbleColor.Gold:
                return GoldMarble;
            default:
                return null;
        }
    }

    List<MarbleProbability> GetTierData(int tier)
    {
        List<MarbleProbability> tierData = new List<MarbleProbability>();

        if (tier >= 15)
        {
            tierData.Add(new MarbleProbability(8, TopRedMarble));
            tierData.Add(new MarbleProbability(20, BigRedMarble));
            tierData.Add(new MarbleProbability(30, BlueFireMarble));
            tierData.Add(new MarbleProbability(45, FireMarble));
            tierData.Add(new MarbleProbability(60, RedMarble));
        }
        if (tier >= 12)
        {
            tierData.Add(new MarbleProbability(8, TopRedMarble));
            tierData.Add(new MarbleProbability(20, BigRedMarble));
            tierData.Add(new MarbleProbability(30, BlueFireMarble));
            tierData.Add(new MarbleProbability(40, FireMarble));
            tierData.Add(new MarbleProbability(55, RedMarble));
        }
        else if (tier >= 10)
        {
            tierData.Add(new MarbleProbability(5, TopRedMarble));
            tierData.Add(new MarbleProbability(15, BigRedMarble));
            tierData.Add(new MarbleProbability(20, BlueFireMarble));
            tierData.Add(new MarbleProbability(35, FireMarble));
            tierData.Add(new MarbleProbability(55, RedMarble));
        }
        else if (tier >= 7)
        {
            tierData.Add(new MarbleProbability(4, TopRedMarble));
            tierData.Add(new MarbleProbability(8, BigRedMarble));
            tierData.Add(new MarbleProbability(20, FireMarble));
            tierData.Add(new MarbleProbability(55, RedMarble));
        }
        else if (tier >= 5)
        {
            tierData.Add(new MarbleProbability(2, TopRedMarble));
            tierData.Add(new MarbleProbability(6, BigRedMarble));
            tierData.Add(new MarbleProbability(15, FireMarble));
            tierData.Add(new MarbleProbability(50, RedMarble));
        }
        else if (tier >= 3)
        {
            tierData.Add(new MarbleProbability(2, BigRedMarble));
            tierData.Add(new MarbleProbability(10, FireMarble));
            tierData.Add(new MarbleProbability(50, RedMarble));
        }
        else if (tier >= 2)
        {
            tierData.Add(new MarbleProbability(5, FireMarble));
            tierData.Add(new MarbleProbability(50, RedMarble));
        }
        else
        {
            tierData.Add(new MarbleProbability(45, RedMarble));
        }

        return tierData;
    }

    void UpdateTimer()
    {
        float intervalConstant = 2.25f;
        if (tier >= 25)
        {
            intervalConstant = 1.1315f;
        }
        else if (tier >= 20)
        {
            intervalConstant = 1.1325f;
        }
        else if (tier >= 15)
        {
            intervalConstant = 1.134f;
        }
        else if (tier >= 10)
        {
            intervalConstant = 1.1325f;
        }
        else if (tier >= 5)
        {
            intervalConstant = 1.1175f;
        }
        else if (tier == 4)
        {
            intervalConstant = 1.075f;
        }
        else if (tier == 3)
        {
            intervalConstant = 0.975f;
        }
        else if (tier == 2)
        {
            intervalConstant = 0.95f;
        }
        float newInterval = Mathf.Clamp(spawnInterval - (speed / intervalConstant / maxSpeed), 0.1375f, spawnInterval);
        timer = newInterval;
    }

    public void UpdateTier(int tier)
    {
        if (tier > 1)
        {
            Tier newTier = Instantiate(TierPrefab, GetSpawnPosition(MarbleColor.Tier), Quaternion.identity);
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
            if (marble == null || !marble.gameObject.activeSelf)
            {
                continue;
            }

            if (!onlyBad || marble.livesLost > 0)
            {
                marble.Destroy();
                allMarbles.RemoveAt(i);
            }
        }

        if (!onlyBad)
        {
            var extraLife = FindObjectOfType<ExtraLife>();
            if (extraLife != null)
            {
                Destroy(extraLife.gameObject);
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

    private void CreateExtraLife()
    {
        ExtraLife extraLife = Instantiate(ExtraLife, GetSpawnPosition(MarbleColor.Life), Quaternion.identity);
        extraLife.UpdateSpeed(speed);

        Destroy(extraLife.gameObject, 5);
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
            else if (perk == PerkEnum.NoBombs)
            {
                hasNoBombs = true;
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

    void SpawnMarbles(int marbleCount)
    {
        var randomValue = Random.Range(0, 100);
        Marble marbleToSpawn = DetermineMarbleToSpawn(tier, randomValue);

        if (marbleToSpawn == null)
        {
            return;
        }

        List<Vector3> spawnedPositions = new List<Vector3>();

        // Spawn the first marble
        Vector3 firstPosition = GetSpawnPosition(marbleToSpawn.color);
        InstantiateMarble(marbleToSpawn, firstPosition);
        spawnedPositions.Add(firstPosition);

        if (marbleCount == 2 && tier < 5 && marbleToSpawn.livesLost > 0 && Random.Range(0, 2) == 0)
        {
            return;
        }

        bool multipleBombCheck = marbleCount == 2 || tier >= 10 || marbleToSpawn.color != MarbleColor.Bomb;
        if (marbleCount > 1 && multipleBombCheck && allowedDupMarbleColors.Contains(marbleToSpawn.color))
        {
            // Only attempt to spawn more marbles if marbleCount is greater than 1 and color is allowed
            for (int i = 1; i < marbleCount; i++)
            {
                Vector3 spawnPosition;
                int attemptCounter = 0;
                bool positionValid;

                do
                {
                    spawnPosition = GetSpawnPosition(marbleToSpawn.color);
                    positionValid = CheckSpawnPosition(spawnPosition, spawnedPositions);
                    attemptCounter++;
                } while (!positionValid && attemptCounter < 3);

                if (positionValid)
                {
                    InstantiateMarble(marbleToSpawn, spawnPosition);
                    spawnedPositions.Add(spawnPosition);
                }
            }
        }
    }

    bool CheckSpawnPosition(Vector3 position, List<Vector3> spawnedPositions)
    {
        foreach (Vector3 existingPosition in spawnedPositions)
        {
            if (Mathf.Abs(existingPosition.x - position.x) < 1.7f)
            {
                return false;
            }
        }
        return true;
    }

    void InstantiateMarble(Marble marblePrefab, Vector3 position)
    {
        Marble spawnedMarble = poolManager.GetOrCreateObject(marblePrefab);
        spawnedMarble.transform.position = position;
        spawnedMarble.transform.rotation = marblePrefab.transform.rotation;
        spawnedMarble.speed = speed;
        spawnedMarble.gameObject.SetActive(true);
        spawnedMarble.FadeIn();

        allMarbles.Add(spawnedMarble);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<string, Queue<Marble>> poolDictionary = new Dictionary<string, Queue<Marble>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Marble GetOrCreateObject(Marble marblePrefab)
    {
        string poolKey = marblePrefab.name;
        Marble marble;

        if (poolDictionary.TryGetValue(poolKey, out Queue<Marble> objectQueue) && objectQueue.Count > 0)
        {
           marble = objectQueue.Dequeue();
        }
        else
        {
            marble = Instantiate(marblePrefab);
            marble.name = marblePrefab.name;
        }

        marble.gameObject.SetActive(false);
        return marble;
    }

    public void ReturnObjectToPool(Marble marble)
    {
        var rb = marble.rb;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        marble.gameObject.SetActive(false);

        string poolKey = marble.name;

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary[poolKey] = new Queue<Marble>();
        }

        poolDictionary[poolKey].Enqueue(marble);
    }
}
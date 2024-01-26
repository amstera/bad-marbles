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
            Debug.Log($"Queue is empty for {poolKey}. Creating new object.");
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

        string poolKey = marble.name.Replace("(Clone)", "");

        if (!poolDictionary.ContainsKey(poolKey))
        {
            Debug.Log($"Creating new queue for {poolKey}");
            poolDictionary[poolKey] = new Queue<Marble>();
        }

        poolDictionary[poolKey].Enqueue(marble);
    }
}
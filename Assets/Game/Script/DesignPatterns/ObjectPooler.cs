using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Pool
{
    public string name => prefab.name;       // Tên định danh pool
    public GameObject prefab;                // Prefab để tạo object
    public int size = 10;                    // Số lượng khởi tạo
    public bool expandable = true;           // Có thể mở rộng không
    public int level = 0;                    // Mức (level) để lọc
}

public class ObjectPooler : SingletonMonoBehaviour<ObjectPooler>
{
    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabLookup;
    private Dictionary<string, bool> expandableLookup;

    protected override void Awake()
    {
        base.Awake();   

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabLookup = new Dictionary<string, GameObject>();
        expandableLookup = new Dictionary<string, bool>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                obj.name = pool.name;

                objectPool.Enqueue(obj);
            }

            poolDictionary[pool.name] = objectPool;
            prefabLookup[pool.name] = pool.prefab;
            expandableLookup[pool.name] = pool.expandable;
        }
    }

    public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
        {
            Debug.LogWarning($"Pool with name '{name}' doesn't exist.");
            return null;
        }

        Queue<GameObject> poolQueue = poolDictionary[name];
        GameObject objectToSpawn;

        if (poolQueue.Count == 0 && expandableLookup[name])
        {
            objectToSpawn = Instantiate(prefabLookup[name], transform);
            objectToSpawn.name = name;
        }
        else if (poolQueue.Count > 0)
        {
            objectToSpawn = poolQueue.Dequeue();
        }
        else
        {
            Debug.LogWarning($"Pool '{name}' is empty and not expandable.");
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        string name = obj.name;

        if (!poolDictionary.ContainsKey(name))
        {
            Debug.LogWarning($"Cannot return object: Pool '{name}' does not exist.");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(this.transform, false);
        poolDictionary[name].Enqueue(obj);
    }

    public void ClearPoolsOfPreviousLevels(int currentLevel)
    {
        List<string> keysToRemove = new List<string>();

        foreach (var pool in pools)
        {
            if (pool.level < currentLevel)
            {
                keysToRemove.Add(pool.name);

                if (poolDictionary.TryGetValue(pool.name, out var queue))
                {
                    while (queue.Count > 0)
                    {
                        var obj = queue.Dequeue();
                        if (obj != null)
                            Destroy(obj);
                    }

                    poolDictionary.Remove(pool.name);
                }
            }
        }

        foreach (var key in keysToRemove)
        {
            prefabLookup.Remove(key);
            expandableLookup.Remove(key);
            pools.RemoveAll(p => p.name == key);
        }
    }
}

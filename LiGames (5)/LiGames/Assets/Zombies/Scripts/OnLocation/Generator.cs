using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    int minAllowableZombieCount = 2;
    [SerializeField]
    int minZombieSpawnCount = 2;
    [SerializeField]
    int maxZombieSpawnCount = 4;

    List<Spawner> spawners;
    List<Spawner> InvisibleSpawners
    {
        get
        {
            List<Spawner> res = new List<Spawner>();
            foreach (Spawner spawner in spawners)
                if (!spawner.IsVisible)
                    res.Add(spawner);
            return res;
        }
    }
    int AliveZombiesCount
    {
        get
        {
            int count = 0;
            foreach (ZombieController zombie in FindObjectsOfType<ZombieController>())
                if (zombie.Alive)
                    ++count;
            return count;
        }
    }
    void Awake()
    {
        spawners = new List<Spawner>(FindObjectsOfType<Spawner>());
    }
    void ZombieGen()
    {
        int zombieCount = AliveZombiesCount;
        if (zombieCount > minAllowableZombieCount)
            return;
        List<Spawner> usefulSpawners = InvisibleSpawners;
        if (usefulSpawners.Count == 0)
            return;

        System.Random numberGenerator = new System.Random();
        int zombiesCountToSpawn = Mathf.Min(
            numberGenerator.Next(maxZombieSpawnCount - minZombieSpawnCount + 1) + minZombieSpawnCount,
            usefulSpawners.Count
        );
        for(int i = 0; i < zombiesCountToSpawn; ++i)
        {
            int nextIndex = numberGenerator.Next(usefulSpawners.Count);
            Spawner currentSpawner = usefulSpawners[nextIndex];
            usefulSpawners.Remove(currentSpawner);
            currentSpawner.SpawnZombie();
        }
    }
    public void ExecuteGen()
    {
        ZombieGen();
    }
}

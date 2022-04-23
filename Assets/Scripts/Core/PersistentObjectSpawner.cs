using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;
        static bool hasSpawned = false;

        // Start is called before the first frame update
        void Awake()
        {
            if (hasSpawned) return;
            hasSpawned = true;
            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects() {
            GameObject persistentObjects = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObjects);
        }
    }
}
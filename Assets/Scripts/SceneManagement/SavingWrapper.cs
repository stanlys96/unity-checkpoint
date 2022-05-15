using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement {
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultFileName = "save";
        [SerializeField] float fadeInTime = 0.5f;

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene() {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultFileName);
            yield return fader.FadeIn(fadeInTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L)) {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                Save();
            }
        }

        public void Save() {
            GetComponent<SavingSystem>().Save(defaultFileName);
        }

        public void Load() {
            GetComponent<SavingSystem>().Load(defaultFileName);
        }
    }
}
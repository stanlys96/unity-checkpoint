using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Stats {
    public class LevelDisplay : MonoBehaviour
    {
        GameObject player;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", player.GetComponent<BaseStats>().GetLevel());
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Attributes;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter player;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            Health target = player.GetTarget();
            if (target == null) {
                GetComponent<Text>().text = "N/A";
            } else {
                GetComponent<Text>().text = String.Format("{0:0}%", target.GetPercentage());
            }
        }
    }
}
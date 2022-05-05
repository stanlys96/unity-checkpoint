using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

namespace RPG.Core {
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float hideTime = 5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(hideTime));
            }
        }

        private IEnumerator HideForSeconds(float time) {
            ShowPickup(false);
            yield return new WaitForSeconds(time);
            ShowPickup(true);
        }

        private void ShowPickup(bool show) {
            Collider collider = GetComponent<Collider>();
            collider.enabled = show;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(show);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !collected)
        {
            //give ammo
            PlayerController.instance.activeGun.getAmmo();

            Destroy(gameObject);
            collected = true;
            AudioManager.instance.PlaySoundEffects(3);
        }
    }
}

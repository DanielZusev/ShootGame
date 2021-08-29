using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{

    public string theGun;
    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !collected)
        {
            //give ammo
            PlayerController.instance.addGun(theGun);

            Destroy(gameObject);
            collected = true;

            AudioManager.instance.PlaySoundEffects(4);
        }
    }
}

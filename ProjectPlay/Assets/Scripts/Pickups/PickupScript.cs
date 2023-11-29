using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    public int PickupType = 0; //0=Health 1=Ammo

    public int Amount;

    public int WeaponType;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController PC = collision.collider.GetComponent<PlayerController>();

        if (PC)
        {
            switch (PickupType)
            {
                case 0:
                    PC.TakeDamage(-Amount);
                    break;
                case 1:
                    PC.AddAmmo(WeaponType, Amount);
                    break;
            }

            Destroy(gameObject);
        }
    }
}

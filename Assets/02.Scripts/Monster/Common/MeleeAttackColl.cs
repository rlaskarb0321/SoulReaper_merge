using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackColl : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerState player = other.GetComponent<PlayerState>();
            player.GetHit(transform.forward);
        }
    }
}

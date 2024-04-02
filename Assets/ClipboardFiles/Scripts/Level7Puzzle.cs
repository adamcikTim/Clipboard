using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level7Puzzle : MonoBehaviour
{
    public GameObject weight;
    public Collider scale;
    public GameObject transferToLevel;

    void OnTriggerStay(Collider other){     // if the heavy objecet remains inside the scale's collider for a certain amount of time, door opens and player is free to finish the game
        if(other.gameObject.tag == "Weight"){
            transferToLevel.SetActive(true);
        }
    }
}

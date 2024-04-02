using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastLaser : MonoBehaviour{
    // script handles the creation and updates of the laser
    // all the necessary variables and actual code is in Laser script
    public Material material;
    Laser beam;
    void Update(){
        if(beam != null){
            Destroy(beam.laserObj);
        }
        beam = new Laser(gameObject.transform.position,gameObject.transform.right,material);
    }
}
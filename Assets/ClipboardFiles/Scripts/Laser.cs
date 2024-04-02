using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Laser{
    // the only code example where I used an exempt from youtube tutorial
    // on laser creation.
    Vector3 pos, dir;
    public GameObject laserObj;
    LineRenderer laser;
    LayerMask layerMaskGround = 1 << 10;
    LayerMask layerMaskPlayer = 1 << 11;
    List <Vector3> laserIndices = new List<Vector3>();
    // declaration of all the necessary variables and following assignment
    public Laser(Vector3 pos, Vector3 dir, Material material){
        this.laser = new LineRenderer();
        this.laserObj = new GameObject();
        this.laserObj.name = "Laser";
        this.pos = pos;
        this.dir = dir;
        this.laser = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        this.laser.startWidth = 0.1f;
        this.laser.endWidth = 0.1f;
        this.laser.material = material;
        this.laser.startColor = Color.red;
        this.laser.endColor = Color.red;
        CastRay(pos, dir, laser);
    }
    // method handles raycast of the laser to the world and detecting if
    // it hit anything. if it hit, we go to checkhit method where we 
    // establish if the laser hit player, which would result in player's death
    // or anything else means the laser will stop and not continue any further
    void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser){
        laserIndices.Add(pos);
        Ray ray = new Ray(pos,dir);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 300)){
            CheckHit(hit,dir,laser);
        }
        else{
            laserIndices.Add(ray.GetPoint(30));
            UpdateLaser();
        }
    }
    void UpdateLaser(){
        int count = 0;
        laser.positionCount = laserIndices.Count;
        foreach(Vector3 idx in laserIndices){
            laser.SetPosition(count,idx);
            count++;
        }
    }
    void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer laser){
        if(hitInfo.collider.gameObject.tag == "Player"){
            SceneManager.LoadScene("DeathScreen");
        }
        else{                                           // everything else
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }
}
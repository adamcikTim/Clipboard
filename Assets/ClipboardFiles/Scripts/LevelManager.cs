using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject key;
    RaycastHit hit;
    public GameObject NextLevelVolume;
    float RaycastHitLength = 100f;

    Scene scene;

    void Start(){                   // determines which level is player currently at, saves to player prefs file for later reloads if player died due to lasers
        scene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt("lastScene", scene.buildIndex);
    }
    
    void Update(){                  // method handles key pickup to destroy the key from the level, and set active the object that allows player to travel to next level
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetKeyDown(KeyCode.E)){
            if (Physics.Raycast(ray, out hit, RaycastHitLength)){
                if(hit.transform.gameObject.tag == "Key"){
                    Destroy(key);
                    NextLevelVolume.SetActive(true);
                }
            }
        }
    }
}

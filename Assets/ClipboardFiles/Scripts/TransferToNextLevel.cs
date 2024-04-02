using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferToNextLevel : MonoBehaviour{
    Scene scene;        // defines the scene player is currently at
    void Start(){
        scene = SceneManager.GetActiveScene();      // returns currently active scene
    }
    // method handling level switching after completion, 
    // once the last level is finished, player returns to the main menu
    private void OnTriggerEnter(Collider other){            
        if(other.gameObject.tag == "Player"){
            if(scene.buildIndex == 6){
                SceneManager.LoadScene("MainMenu");
                Cursor.lockState = CursorLockMode.None;
            }
            else{
                SceneManager.LoadScene(scene.buildIndex + 1, LoadSceneMode.Single);
            }
        }
    }
}
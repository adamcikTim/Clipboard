using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public bool isToggled = false;
    private GameObject doorRef;
    private GameObject finalPos;
    private Vector3 originalLocation;
    private Vector3 finalLocation;
    private float time = 0f;
    private float duration = 10f;

    // dynamic system to find a door object inside the level
    // for this purpose i created a prefab with the empty object 'final location'
    // which tells the script the location where to move the door after button press
    void Start(){           
        doorRef = GameObject.Find("Door");
        originalLocation = doorRef.transform.position;
        finalPos = GameObject.Find("FinalLocation");
        finalLocation = finalPos.transform.position;
    }

    void Update(){      // checking on every frame if the player pressed the button
        ButtonToggle();
    }

    // method handles the button press behavior
    // if player presses the button, the door opens - moves upwards by a significant amount, for player to pass through
    public void ButtonToggle(){         
         if(isToggled){
            doorRef.transform.position = Vector3.Lerp(originalLocation,finalLocation,time/duration);
            time += Time.deltaTime;
            doorRef.transform.position = finalLocation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Movement : MonoBehaviour{
    // headers are being used to separate values in the inspector
    // and for easier code reading while inside visual studio
    [Header("General")]
    public CharacterController controller;
    public Camera camReference;
    [Header("Jump")]
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    Vector3 move;
    bool isGrounded;
    [Header("Crouch")]
    float fullHeight;
    public float halfHeight;
    [Header("Crop")]
    RaycastHit hit;
    GameObject copiedGameObject;
    GameObject originalGameObject;
    public float RaycastHitLength = 1000f;
    private bool isClipboardEmpty = true;
    Vector3 offset;
    Vector3 offsetCalc;
    [Header("Pickup")]
    private bool holdingObject;
    public Transform pickupObjectLocation;
    Rigidbody pickupRB;
    private float minSpeed = 0f;
    [SerializeField]private float maxSpeed = 1000f;
    private float maxDistance = 1f;
    [SerializeField]private float currentSpeed = 10f;
    private float currentDistance = 0f;
    // here we determine the size of the player's controller on the start and store it in a variable
    // later on we use it to determine the size of the collider to return to after player stops
    // crouching
    void Start(){
        fullHeight = controller.height;
    }
    // update method runs on every frame of the gameplay, therefore we have to make sure all the inputs
    // are covered - walking, crouching, jumping, crop/copy and pasting, other interactions and of course pause menu
    // pressing escape and prompting pause menu however moves player back to main menu
    private void Update(){
        Walk();
        Crouch();
        Jump();
        Crop();
        Copy();
        Interaction();
        PauseMenu();
    }
    // fixed update is independant of the time delta time, which was needed to determine
    // the players ability to carry and drop object. this was achieved by lerping in between
    // two positions - object's original location, and empty game object in front of players
    // camera.
    private void FixedUpdate() {
        if(holdingObject){
            currentDistance = Vector3.Distance(pickupObjectLocation.position, pickupRB.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDistance/maxDistance);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = pickupObjectLocation.position - pickupRB.position;
            pickupRB.velocity = direction.normalized * currentSpeed;
        }
    }
    // walking is split into walking forward and to right, hence 2 axis of x and z
    // by flipping forward and right values by -1 we achieve backwards movement 
    // on these axis, therefore back and left.
    // sprinting is achieved by holding down shift button, and simply adding a value
    // to existing walk speed of the player. once the shift is released, the value returns
    // back to normal walk speed
    void Walk(){   
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }
    // crouching is in cooperation with the start method, where we
    // store the value of player's original full height. when pressed ctrl,
    // the height halves down by 2, 
    void Crouch(){
        if (Input.GetKeyDown(KeyCode.LeftControl)){
            controller.height = halfHeight;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl)){
            controller.height = fullHeight;
        }
    }
    // jumping is prompted when player presses spacebar
    // firstly we have to check if the player is on the ground
    // to avoid any double jumps or issues of this kind
    // following, we launch the player upwards, resulting in a jump
    void Jump(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0){
            velocity.y = -2f;
            controller.stepOffset = 0.5f;
        }
        if (Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            controller.stepOffset = 0f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    // cropping mechanic works by clicking left mouse button, hence we declare a ray from camera to center of the screen
    // next we check if the clipboard is empty, and can continue with cropping the object
    // afterwards we cast a ray to the world, pointing to the crosshair (center of the screen)
    // and check if we hit anything with a tag 'CopyCrop'
    // if so, we set the clipboard state to full (boolean false), and make a reference to cropped object
    // for this we require 2 variables, as the reference is being destroyed, therefore the script wouldn't work
    // properly. statement is ended by setting offset size to half of the size of the cropped object,
    // as that is the amount of offset necessary to offset the object from the point of insertion (or pasting), 
    // as otherwise, it'd cause issues of spawning inside of the wall, and not being able to use it properly
    // the next is the else statement which works only if the clipboard is full, then we cast another ray from camera
    // // forward to detect a place to paste the cropped object to. first we set the offset of the object of the 
    // surface, next the switch statement works as a crossroad of which direction to offset the object from the 
    // geometry. once this handled, we reset the state of clipboard to empty, and destroy the original game object
    // that the player originally cropped.
    void Crop(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Mouse0)){
            if (isClipboardEmpty == true){
                if (Physics.Raycast(ray, out hit, RaycastHitLength)){
                    if (hit.transform.tag == "CopyCrop"){
                        isClipboardEmpty = false;
                        originalGameObject = hit.transform.gameObject;
                        copiedGameObject = hit.transform.gameObject;
                        offset = new Vector3 (copiedGameObject.GetComponent<Collider>().bounds.size.x / 2,copiedGameObject.GetComponent<Collider>().bounds.size.y / 2,copiedGameObject.GetComponent<Collider>().bounds.size.z / 2);
                    }
                }
            }
            else{
                    if (Physics.Raycast(ray, out hit, RaycastHitLength)){
                        if (hit.point != null){
                            offsetCalc = Vector3.Scale(hit.normal, offset);
                            switch (hit.normal){ 
                                case Vector3 v when v.Equals(Vector3.up):
                                    Instantiate(copiedGameObject, hit.point + copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.down):
                                    Instantiate(copiedGameObject, hit.point - copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.left):
                                    Instantiate(copiedGameObject, hit.point - copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.right):
                                    Instantiate(copiedGameObject, hit.point + copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.forward):
                                    Instantiate(copiedGameObject, hit.point + copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.back):
                                    Instantiate(copiedGameObject, hit.point - copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                default:
                                    Instantiate(copiedGameObject, hit.point + offsetCalc, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                            }
                            isClipboardEmpty = true;
                            Destroy(originalGameObject);
                        }
                    }
            }
        }
    }
    // copying mechanic works by clicking right mouse button, hence we declare a ray from camera to center of the screen
    // next we check if the clipboard is empty, and can continue with copying the object
    // afterwards we cast a ray to the world, pointing to the crosshair (center of the screen)
    // and check if we hit anything with a tag 'CopyCrop'
    // if so, we set the clipboard state to full (boolean false), and make a reference to copied object.
    // statement is ended by setting offset size to half of the size of the copied object,
    // as that is the amount of offset necessary to offset the object from the point of insertion (or pasting), 
    // as otherwise, it'd cause issues of spawning inside of the wall, and not being able to use it properly
    // the next is the else statement which works only if the clipboard is full, then we cast another ray from camera
    // forward to detect a place to paste the copied object to. first we set the offset of the object of the 
    // surface, next the switch statement works as a crossroad of which direction to offset the object from the 
    // geometry. once this handled, we reset the state of clipboard to empty, and the copy cycle is finished

    void Copy(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse1)){
            if (isClipboardEmpty == true){
                if (Physics.Raycast(ray, out hit, RaycastHitLength)){
                    if (hit.transform.tag == "CopyCrop"){
                        isClipboardEmpty = false;
                        copiedGameObject = hit.transform.gameObject;
                        offset = new Vector3 (copiedGameObject.GetComponent<Collider>().bounds.size.x / 2, copiedGameObject.GetComponent<Collider>().bounds.size.y / 2,copiedGameObject.GetComponent<Collider>().bounds.size.z / 2);
                    }
                }
            }
            else{
                if (isClipboardEmpty == false){
                    if (Physics.Raycast(ray, out hit, RaycastHitLength)){
                        if (hit.point != null){
                            offsetCalc = Vector3.Scale(hit.normal, offset);
                            switch (hit.normal){ 
                                case Vector3 v when v.Equals(Vector3.up):
                                    Instantiate(copiedGameObject, hit.point + copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.down):
                                    Instantiate(copiedGameObject, hit.point - copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.left):
                                    Instantiate(copiedGameObject, hit.point - copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.right):
                                    Instantiate(copiedGameObject, hit.point + copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.forward):
                                    Instantiate(copiedGameObject, hit.point + copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                case Vector3 v when v.Equals(Vector3.back):
                                    Instantiate(copiedGameObject, hit.point - copiedGameObject.GetComponent<Collider>().bounds.size / 2, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                                default:
                                    Instantiate(copiedGameObject, hit.point + offsetCalc, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                                    break;
                            }
                            isClipboardEmpty = true;
                        }
                    }
                }
            }
        }
    }
    // interaction in the game was a broader term, therefore 
    // includes multiple cases of use, hence we decided to use
    // switch - determines outcome depending on what is player
    // trying to interact with. first we check if player hit anything
    // at all, and if they did, continue to find out what are we dealing with
    // in case of button, we switch the value to true, making the button toggled
    // in case of the pickable object we use the dedicated methods to interpolate
    // the object however needed
    void Interaction(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         
        if (Input.GetKeyDown(KeyCode.E)){
          if (Physics.Raycast(ray, out hit, RaycastHitLength)){
              if(hit.point != null){
                  switch(hit.transform.tag){
                        case "Button":
                            hit.transform.gameObject.GetComponent<ButtonPress>().isToggled = true;
                            break;
                        case "Pickable":
                            if(holdingObject == false){ // if doesnt have any object
                                PickupObject(hit.transform.gameObject);
                            }
                            else{
                                DropObject(hit.transform.gameObject);
                            }
                                break;
                        default:
                            Debug.Log("Object not recognized");
                            break;
                    } 
                }
            }
        }
    }
    // dedicated method to pickup the object, which gets the
    // rigidbody of the desired object, and continues to 
    // restrict the object from moving or rotating to avoid any
    // potential glitches. the holding bool variable is set to true
    private void PickupObject(GameObject pickedUpObject){
        pickupRB = pickedUpObject.GetComponent<Rigidbody>();
        pickupRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        holdingObject = true;
    }
    // separate method to drop object if player is holding one
    // disables all the rotational and movement restrictions to defaults
    // just as well resets the bool value to false
    private void DropObject(GameObject pickedUpObject){
        pickupRB.constraints = RigidbodyConstraints.None;
        holdingObject = false;
    }
    // pause menu is prompted when pressing escape, however that returns player to main menu
    private void PauseMenu(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("MainMenu");
        }
    }
}
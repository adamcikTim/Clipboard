using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour{
    // this script is attached on canvas to main menu
    // it handles all the necessary functionality to run the main menu like buttons, scene loading, shutting down game
    // MAIN MENU BOX
    void Update(){
        Cursor.lockState = CursorLockMode.None;  
    }
    public void StartButton(){      // starts the game by loading the first level
        SceneManager.LoadScene("Level1");
    }
    public void Quit(){
        Application.Quit();
    }
    // CHAPTER SELECT BOX
    public void Level1(){            // starts the game by loading the first level
        SceneManager.LoadScene("Level1");
    }
    public void Level2(){            // starts the game by loading the second level
        SceneManager.LoadScene("Level2");
    }
    public void Level3(){            // starts the game by loading the third level
        SceneManager.LoadScene("Level3");
    }
    public void Level4(){            // starts the game by loading the fourth level
        SceneManager.LoadScene("Level5");
    }
    public void Level5(){            // starts the game by loading the fifth level
        SceneManager.LoadScene("Level6");
    }
    public void Level6(){            // starts the game by loading the sixth level
        SceneManager.LoadScene("Level7");
    }
}
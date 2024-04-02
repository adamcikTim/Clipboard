using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;         // shows cursor on the screen
    }

    public void Reload(){                               // used for loading the last played level
        SceneManager.LoadScene(PlayerPrefs.GetInt("lastScene"));
    }

    public void BackToMenu(){                           // prompts player back to main menu by loading the main menu level
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit(){                                 // shuts down the game process/task and quits the game
        Application.Quit();
    }
}
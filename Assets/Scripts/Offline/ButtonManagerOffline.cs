using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class ButtonManagerOffline : MonoBehaviour, IPointerUpHandler {

    public Sprite pressedSprite,normalSprite;
    private static Sprite currentSprite;

    public void Awake() {
        //if(PlayerPrefs.GetInt("Sound") == 1) 
        //    GetComponent<Image>().sprite = normalSprite;
        //else if(PlayerPrefs.GetInt("Sound") == 0)
        //    GetComponent<Image>().sprite = pressedSprite;
    }

    public void Update() {
        // back button on mobile
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (SceneManager.GetActiveScene().name == "Start") {
                Application.Quit();
            } else {
                SceneManager.LoadScene("Start");
            }
        }
    }

    public void ChangeScene(string name) {
        if (name == "Game" && (PlayerPrefs.GetInt("FirstGame") == 1)) name = "Tutorial";
        SceneManager.LoadScene(name);
    }

    public void Restart(TutGameWorld gw) {
        if(TutSquare.moveNum != 5) gw.Restart();
        else SceneManager.LoadScene("Game");
    }

    public void OnPointerUp(PointerEventData eventData) {
        GetComponent<Image>().sprite = (GetComponent<Image>().sprite == normalSprite)? pressedSprite : normalSprite;
    }


}

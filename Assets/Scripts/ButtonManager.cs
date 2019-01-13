using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour, IPointerUpHandler {

    public Sprite pressedSprite,normalSprite;
    private static Sprite currentSprite;

    public GameObject Menus;
    public GameObject MainMenu, PlayMenu;
    public GameObject OnlineMenu;
    public GameObject HostMenu, JoinMenu;

    public GameObject backButton;

    public static bool isMainActive = true;


    public void Awake() {
        if (normalSprite == null) {
            return;
        }

        if (PlayerPrefs.GetInt("Sound") == 1)
            GetComponent<Image>().sprite = normalSprite;
        else if (PlayerPrefs.GetInt("Sound") == 0)
            GetComponent<Image>().sprite = pressedSprite;
    }

    public void Start() {
        if(PlayMenu != null) {
            if (isMainActive) {
                BackToMain();
            } else {
                OpenPlayMenu();
            }
        } 
    }

    public void Update() {
        // back button on mobile
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (SceneManager.GetActiveScene().name == "Start") {
                Application.Quit();
            } 
        }
    }

    public void ChangeScene(string name) {
        if (name == "Game" && (PlayerPrefs.GetInt("FirstGame") == 1)) name = "Tutorial";

        if (name == "Start" && SceneManager.GetActiveScene().name == "Game" && (HostMenu.activeSelf || JoinMenu.activeSelf)) {
            if (HostMenu.activeSelf) HostMenu.SetActive(false);
            if (JoinMenu.activeSelf) JoinMenu.SetActive(false);
            OnlineMenu.SetActive(true);
            name = "Game";
        }  else if(name == "Start" && SceneManager.GetActiveScene().name == "Start") {
            isMainActive = true;
            BackToMain();
            return;
        }

        SceneManager.LoadScene(name);

    }

    public void Restart(TutGameWorld gw) {
        if(TutSquare.moveNum != 5) gw.Restart();
        else SceneManager.LoadScene("Game");
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (normalSprite == null) return;
        GetComponent<Image>().sprite = (GetComponent<Image>().sprite == normalSprite)? pressedSprite : normalSprite;
    }

    public void OpenPlayMenu() {
        isMainActive = false;
        MainMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }

    public void BackToMain() {
        isMainActive = true;
        PlayMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void OpenHostMenu()
    {
        OnlineMenu.SetActive(false);
        HostMenu.SetActive(true);
    }

    public void OpenJoinMenu()
    {
        OnlineMenu.SetActive(false);
        JoinMenu.SetActive(true);
    }

    public void ActivateMenus()
    {
        Menus.SetActive(true);
        OnlineMenu.SetActive(true);
        HostMenu.SetActive(false);
        JoinMenu.SetActive(false);
        backButton.SetActive(true);
        if(GameObject.Find("Disconnect Button")!=null)
            GameObject.Find("Disconnect Button").SetActive(false);
    }
}

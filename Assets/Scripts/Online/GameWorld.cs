using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameWorld : NetworkBehaviour {

    public SyncListInt syncList = new SyncListInt();
    public Button[] buttonList;
    public Button backButton, soundButton;
    public Vector3[] startPositions = new Vector3[25];
    public Image[] banners;
    public List<Square> boxList = new List<Square>();
    public GameObject restartButton, gameOverPanel;
    public Text gameOverText, p1PH, p2PH;
    public Sprite playerSide, firstSprite, secondSprite, disabledSprite, normalSprite;
    private ClickSound sound;
    private Player player;
    public bool canceled;

    public Text timeOutText;
    public Text timerText;
    public bool canTimerStart;
    public bool isTimeOut;
    private float timer = 16f;

    public GameObject expButton;

    private int br1, br2;

    public bool isGameOver, justAlones, blueFirst, blueTake, bannerChange, blowUp, blueWin;
    private float a, b, c, d, speed, speed2;
    public float xDifference;
    private int p1WinCounter, p2WinCounter, sumOfAlones, numOfInteractables, blowIndex;
    public bool canPlay = true;
    public int numOfselected;

    public bool client=false;
    public Text Turn;
    [HideInInspector]
    public bool restartedAlready = false;


    public void Awake() {
        
        player = FindObjectOfType<Player>();
        sound = ClickSound.instance;
        Input.multiTouchEnabled = false;
        SetUpButtons();
        blueFirst = true;
        playerSide = firstSprite;
        restartButton.SetActive(false);
        gameOverPanel.SetActive(false);

        //vazan je poredak kvadratica u buttonListi za ovo
        xDifference = Mathf.Round(GetPosition(1).x - GetPosition(0).x);

        //pozicije bannera (a -> prvi, b -> drugi)
        a = GetBannerPos(0).x;
        b = GetBannerPos(1).x;
    }

    public void SetUpButtons() {
        for (int i = 0; i < buttonList.Length; i++) {
            buttonList[i].GetComponent<Square>().SetGwReference(this);
            startPositions[i] = buttonList[i].transform.localPosition;
        }
        //DoRandom();
    }


    public void CheckSquares(string tag) {
        int i = int.Parse(tag);
        buttonList[i].GetComponent<Image>().sprite = playerSide;
        buttonList[i].transform.localScale = new Vector3(1.08f, 1.08f, 0);
        buttonList[i].interactable = false;
        buttonList[i].GetComponent<Square>().SetSelected(true);
        numOfselected++;
        boxList.Add(buttonList[i].GetComponent<Square>());
        
    }

    private float timeDiff;
    private DateTime currTime;
    private DateTime lastTime;

    private void OnApplicationPause(bool pause) {
        if (pause) {
            lastTime = DateTime.UtcNow;
        } else {
            currTime = DateTime.UtcNow;
            timeDiff = (float) (currTime - lastTime).TotalSeconds;
            timer -= timeDiff;
            if(timer <= 0) {
                timerText.text = "0";
            }
        }
    }

    public void Update() {

        if (canTimerStart) {
            if(timer >= 0) timerText.text = ((int)timer).ToString();
            timer -= Time.deltaTime;
            if (timer <= 0) {
                isTimeOut = true;
                canTimerStart = false;
                timer = 16f;
            }
        }

        if (isTimeOut) {
            if (restartedAlready)
                restartedAlready = false;
            isGameOver = true;
            canPlay = false;
            if(Turn.text == "Opponent's turn") {
                gameOverText.text = "YOU WON";
                if (client) {
                    blueWin = false;
                    p2WinCounter++;
                } else {
                    blueWin = true;
                    p1WinCounter++;
                }
            } else {
                gameOverText.text = "YOU LOST";
                if (client) {
                    blueWin = true;
                    p1WinCounter++;
                } else {
                    blueWin = false;
                    p2WinCounter++;
                }
            }
            
            GoPanel();
            isTimeOut = false;
        }

        for (int i = 0; i < syncList.Count; i++) {
            if (syncList[i] == i) {
                buttonList[i].GetComponent<Image>().sprite = disabledSprite;
                buttonList[i].interactable = false;
            }
        }

        //interpolacija bannera
        if (bannerChange) BannerSwitch();

        //interpolacija zadnjeg button-a 
        if (blowUp) BlowUp();


    }

    private void BannerSwitch() {

        canPlay = false;
        banners[0].transform.localPosition = new Vector3(Mathf.SmoothStep(a, -b, speed), GetBannerPos(0).y, GetBannerPos(0).z);
        banners[1].transform.localPosition = new Vector3(Mathf.SmoothStep(b, -a, speed), GetBannerPos(1).y, GetBannerPos(1).z);


        speed += 1.85f * Time.deltaTime;

        if (speed > 1f) {
            a = GetBannerPos(0).x;
            b = GetBannerPos(1).x;
            speed = 0f;
            bannerChange = false;
            canPlay = true;
        }
    }

        private void BlowUp() {
        if (speed2 > 1f) {
            // kada se poveca i smanji -> radi shake i poziva put prema scoru
            if (d == Square.width) {
                blowUp = false;
                for (int i = 0; i < buttonList.Length; i++) {
                    if ((blueWin && buttonList[i].GetComponent<Image>().sprite == secondSprite)
                        || (!blueWin && buttonList[i].GetComponent<Image>().sprite == firstSprite)) {
                        // if (PlayerPrefs.GetInt("Sound") == 1) // Handheld.Vibrate();
                        iTween.ShakePosition(buttonList[i].gameObject, 
                            iTween.Hash("x", 8f, "y", 8f, "time", 1.7f, "oncompletetarget", 
                            this.gameObject, "oncomplete", "TravelToScore", "oncompleteparams", i));
                    }
                }
            }
            // smanjenje kada se poveca do cilja 
            speed2 = 0f;
            c = d;
            d = Square.width;

        }
        // povecanje velicine
        speed = (d == Square.width) ? speed2 += 5f * Time.deltaTime : speed2 += 2f * Time.deltaTime;
        if (numOfInteractables != 0)
            buttonList[blowIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.SmoothStep(c, d, speed2), Mathf.SmoothStep(c, d, speed2));
    }

    public void TravelToScore(int i) {
        // put do score bannera
        if (blueWin)
            iTween.MoveTo(buttonList[i].gameObject, iTween.Hash("x", p1PH.transform.position.x * 1.03f, "y", p1PH.transform.position.y * 1.03f, "time", 1.5f,
                                                                "easetype", "easeOutExpo", "oncompletetarget", this.gameObject, "oncomplete", "GoPanel"));
        else
            iTween.MoveTo(buttonList[i].gameObject, iTween.Hash("x", p2PH.transform.position.x * 1.03f, "y", p2PH.transform.position.y * 1.03f, "time", 1.5f,
                                                               "easetype", "easeOutExpo", "oncompletetarget", this.gameObject, "oncomplete", "GoPanel"));
    }

    public void GoPanel() {
        // game over panel
        gameOverPanel.SetActive(true);
        p1PH.text = p1WinCounter.ToString();
        p2PH.text = p2WinCounter.ToString();
        restartButton.SetActive(true);
    }

    public void EndTurn() {
        if (restartedAlready)
            restartedAlready = false;
        CheckForAlones();
        timer = 16f;
        if (!isGameOver) {  
            // sound.GameSound(2);
            //UnSelect();
            playerSide = (playerSide == firstSprite) ? secondSprite : firstSprite;
            bannerChange = true;

            if (playerSide == firstSprite)
            {
                if (client)
                    Turn.text = "Opponent's turn";
                else Turn.text = "Your turn";
            }
            else
            {
                if (client)
                    Turn.text = "Your turn";
                else Turn.text = "Opponent's turn";
            }
        }
    }

    private void CheckForAlones() {
        //provjera kutnih button-a
        if (InterAndNotAlone(0) && !buttonList[1].interactable && !buttonList[5].interactable) SetAlone(0);
        if (InterAndNotAlone(4) && !buttonList[3].interactable && !buttonList[9].interactable) SetAlone(4);
        if (InterAndNotAlone(20) && !buttonList[21].interactable && !buttonList[15].interactable) SetAlone(20);
        if (InterAndNotAlone(24) && !buttonList[23].interactable && !buttonList[19].interactable) SetAlone(24);

        for (int i = 1; i < buttonList.Length - 1; i++) {
            if (i != 4 && i != 20 && InterAndNotAlone(i)) {
                // gornji red
                if (GetPosition(i).y == GetPosition(0).y) {
                    if (!buttonList[i - 1].interactable && !buttonList[i + 1].interactable && !buttonList[i + 5].interactable)
                        SetAlone(i);
                    // donji red
                } else if (GetPosition(i).y == GetPosition(24).y) {
                    if (!buttonList[i - 1].interactable && !buttonList[i + 1].interactable && !buttonList[i - 5].interactable)
                        SetAlone(i);
                    // lijevi red
                } else if (GetPosition(i).x == GetPosition(0).x) {
                    if (!buttonList[i + 1].interactable && !buttonList[i + 5].interactable && !buttonList[i - 5].interactable)
                        SetAlone(i);
                    // desni red    
                } else if (GetPosition(i).x == GetPosition(24).x) {
                    if (!buttonList[i - 1].interactable && !buttonList[i + 5].interactable && !buttonList[i - 5].interactable)
                        SetAlone(i);
                    // izmedju
                } else {
                    if (!buttonList[i - 1].interactable && !buttonList[i + 1].interactable && !buttonList[i + 5].interactable && !buttonList[i - 5].interactable)
                        SetAlone(i);
                }
            }
        }

        //izracun koliko je slobodnih i koliko je samih
        numOfInteractables = 0;
        for (int i = 0; i < buttonList.Length; i++) {
            if (buttonList[i].interactable) numOfInteractables++;
            if (!buttonList[i].interactable && buttonList[i].GetComponent<Square>().GetIsAlone()) {
                buttonList[i].GetComponent<Square>().SetIsAlone(false);
                sumOfAlones--;
            }
        }

        if (numOfInteractables == sumOfAlones) GameOver();
    }

    public void GameOver() {

        if (sumOfAlones % 2 == 0) {
            if(playerSide == firstSprite && !client) {
                gameOverText.text = "YOU LOSE";
                blueWin = false;
                p2WinCounter++;
            } else if(playerSide == secondSprite && client) {
                gameOverText.text = "YOU LOSE";
                blueWin = true;
                p1WinCounter++;
            } else if(playerSide == firstSprite && client) {
                gameOverText.text = "YOU WIN";
                blueWin = false;
                p2WinCounter++;
            } else if(playerSide == secondSprite && !client) {
                gameOverText.text = "YOU WIN";
                blueWin = true;
                p1WinCounter++;
            }
        } else {
            if (playerSide == firstSprite && !client) {
                gameOverText.text = "YOU WIN";
                blueWin = true;
                p1WinCounter++;
            } else if (playerSide == secondSprite && client) {
                gameOverText.text = "YOU WIN";
                blueWin = false;
                p2WinCounter++;
            } else if (playerSide == firstSprite && client) {
                gameOverText.text = "YOU LOSE";
                blueWin = true;
                p1WinCounter++;
            } else if (playerSide == secondSprite && !client) {
                gameOverText.text = "YOU LOSE";
                blueWin = false;
                p2WinCounter++;
            }
        }


        //if (!client && blueWin) {
        //    DataPersistence.data.exp += 10;
        //    DataPersistence.data.Save();
        //    DataPersistence.data.Load();
        //    expButton.GetComponentInChildren<Text>().text = "EXP: " + DataPersistence.data.exp.ToString();
        //} else if (client && !blueWin) {
        //    DataPersistence.data.exp += 10;
        //    DataPersistence.data.Save();
        //    DataPersistence.data.Load();
        //    expButton.GetComponentInChildren<Text>().text = "EXP: " + DataPersistence.data.exp.ToString();
        //}

        canTimerStart = false;
        isGameOver = true;
        canPlay = false;
        StartCoroutine("GameOverRoutine", playerSide);
    }

    private IEnumerator GameOverRoutine(Sprite ps) {
        // popunjavanje slobodnih kada je gameover
        for (int i = 0; i < buttonList.Length; i++) {
            if (buttonList[i].interactable && numOfInteractables > 1) {
                numOfInteractables--;
                yield return new WaitForSeconds(0.35f);
                ps = (ps == firstSprite) ? secondSprite : firstSprite;
                // sound.GameSound(1);
                buttonList[i].GetComponent<Image>().sprite = ps;

            } else if (buttonList[i].interactable) {
                //pokretanje blowUpa zadnjeg buttona u updateu
                blowIndex = i;
                buttonList[i].transform.SetSiblingIndex(26);
                c = Square.width;
                d = c * 4f;
                blowUp = true;
            } else if (numOfInteractables == 0) {
                speed2 = 1.01f;
                blowUp = true;

            }
        }
    }

    public void UnSelect() {

        for (int i = 0; i < buttonList.Length; i++) {
            if (buttonList[i].GetComponent<Square>().GetSelected()) {
                buttonList[i].GetComponent<Square>().SetSelected(false);
                buttonList[i].transform.localScale = new Vector3(1f, 1f, 0);
                boxList.Remove(buttonList[i].GetComponent<Square>());
                numOfselected = 0;
            }
        }
    }

    public void CancelMove() {
        for (int i = 0; i < buttonList.Length; i++) {
            if (buttonList[i].GetComponent<Square>().GetSelected()) buttonList[i].GetComponent<Square>().Restart();
        }
        
    }

    public void RestartButtons() {
        for (int i = 0; i < buttonList.Length; i++) {
            buttonList[i].GetComponent<Square>().button.interactable = true;
            buttonList[i].GetComponent<Square>().SetIsAlone(false);
            buttonList[i].GetComponent<Image>().sprite = normalSprite;
            buttonList[i].transform.localPosition = startPositions[i];
        }
    }

    public void Restart() {
        if (restartedAlready)
            return;

        timer = 16f;
        canTimerStart = true;
        isGameOver = false;
        restartButton.SetActive(false);
        gameOverPanel.SetActive(false);
        sumOfAlones = 0;
        numOfInteractables = 0;
        speed = 0;

        //ko igra prvi 
        blueTake = (playerSide == firstSprite) ? true : false;
        playerSide = blueFirst ? secondSprite : firstSprite;
        blueFirst = !blueFirst;
        //treba li se banner mjenjati
        if ((blueTake && !blueFirst) || (!blueTake && blueFirst))
        {
            bannerChange = true;
        }
        else
        {
            canPlay = true;
        }

        GameObject localPlayer = GameObject.Find("localPlayer");
        GameObject opponentPlayer = GameObject.Find("Player(Clone)");

        localPlayer.GetComponent<Player>().acceptedRematch = false;
        localPlayer.GetComponent<Player>().requestedRematch = false;
        localPlayer.GetComponent<Player>().changedText = false;
        opponentPlayer.GetComponent<Player>().acceptedRematch = false;
        opponentPlayer.GetComponent<Player>().requestedRematch = false;
        opponentPlayer.GetComponent<Player>().changedText = false;

        if (playerSide == firstSprite)
        {
            if (client)
                Turn.text = "Opponent's turn";
            else Turn.text = "Your turn";
        }
        else
        {
            if (client)
                Turn.text = "Your turn";
            else Turn.text = "Opponent's turn";
        }

        restartedAlready = true;
        //DoRandom();
    }
    
    public void SetAlone(int i) {
        buttonList[i].GetComponent<Square>().SetIsAlone(true);
        sumOfAlones++;
    }

    public bool InterAndNotAlone(int i) {
        return buttonList[i].interactable && !buttonList[i].GetComponent<Square>().GetIsAlone();
    }

    public Sprite GetPlayerSide() {
        return playerSide;
    }

    public Vector3 GetBannerPos(int i) {
        return banners[i].transform.localPosition;
    }

    public Rect GetBannerSize(int i) {
        return banners[i].GetComponent<RectTransform>().rect;
    }

    public Vector3 GetPosition(int i) {
        return buttonList[i].transform.position;
    }

    public bool GetGameOver() {
        return isGameOver;
    }
}
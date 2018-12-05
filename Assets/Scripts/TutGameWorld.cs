
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutGameWorld : MonoBehaviour {

    public Button[] buttonList;
    public Vector3[] startPositions = new Vector3[25];
    public Image[] banners;
    public GameObject[] fingerPos;
    public List<TutSquare> boxList = new List<TutSquare>();
    public GameObject restartButton, gameOverPanel;
    public Text gameOverText, p1PH, p2PH;
    public Sprite playerSide, firstSprite, secondSprite, disabledSprite, normalSprite, startButton;
    public Image finger;
    public Text tutText;
    private ClickSound sound;
    public bool cancelSquare;


    public bool isGameOver, justAlones, blueFirst, blueTake, bannerChange, blowUp, goodMove, blueWin, moveFinger, xMove4;
    private float a, b, c, d, speed, speed2, fingerSpeed, fingerSpeed2, bFinger, eFinger;
    public float xDifference;
    private int p1WinCounter, p2WinCounter, blowIndex;
    public bool canPlay = true;
    public int numOfselected;

    public void Awake() {
       // sound = ClickSound.instance;
        Input.multiTouchEnabled = false;
        SetUpButtons();
        blueFirst = true;
        blueTake = true;
        moveFinger = true;
        playerSide = firstSprite;
        restartButton.SetActive(false);
        gameOverPanel.SetActive(false);
        finger.raycastTarget = false;

        //vazan je poredak kvadratica u buttonListi za ovo
        xDifference = Mathf.Round(GetPosition(1).x - GetPosition(0).x);
        //pozicije bannera (a -> prvi, b -> drugi)
        a = GetBannerPos(0).x;
        b = GetBannerPos(1).x;
    }

    public void SetUpButtons() {
        for (int i = 0; i < buttonList.Length; i++) {
            buttonList[i].GetComponent<TutSquare>().SetGwReference(this);
            startPositions[i] = buttonList[i].transform.localPosition;
            // disabled buttons za prvi tutorijal
            if (i == 7 || i == 8 || i == 12 || i == 13 || i == 17) {
                buttonList[i].interactable = false;
                buttonList[i].GetComponent<Image>().sprite = disabledSprite;
            }
        }
    }

    public void Update() {
        //interpolacija bannera
        if (bannerChange) BannerSwitch();
        //interpolacija zadnjeg button-a 
        if (blowUp) BlowUp();

        if (moveFinger) {
            if (TutSquare.moveNum == 0 || (TutSquare.moveNum == 4 && !xMove4) || TutSquare.moveNum == 1 || TutSquare.moveNum == 5) {
                bFinger = finger.transform.position.y;
                eFinger = fingerPos[TutSquare.moveNum * 2 + 1].transform.position.y;
                FingerMove();
            } else {
                if (!xMove4) {
                    bFinger = finger.transform.position.x;
                    eFinger = fingerPos[TutSquare.moveNum * 2 + 1].transform.position.x;
                }
                FingerMove();
            }
        }
    }

    private void FingerMove() {
        float x;
        if (xMove4) {
            fingerSpeed += 2.4f * Time.deltaTime;
            x = 2.4f;
        } else {
            fingerSpeed += 0.55f * Time.deltaTime;
            x = 0.55f;
        }

        finger.transform.position = (TutSquare.moveNum == 3 || TutSquare.moveNum == 2 || xMove4) ? new Vector3(Mathf.SmoothStep(bFinger, eFinger, fingerSpeed), finger.transform.position.y, finger.transform.position.z) : new Vector3(finger.transform.position.x, Mathf.SmoothStep(bFinger, eFinger, fingerSpeed), finger.transform.position.z);

        if (fingerSpeed >= x) {
            fingerSpeed = 0;
            if (TutSquare.moveNum == 4 && !xMove4 && !cancelSquare) {
                bFinger = fingerPos[9].transform.position.x;
                eFinger = fingerPos[12].transform.position.x;
                xMove4 = true;
            } else {
                xMove4 = false;
                finger.transform.position = fingerPos[TutSquare.moveNum * 2].transform.position;
                CancelMove();
            }
        }

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
            if (d == TutSquare.width) {
                blowUp = false;
                for (int i = 0; i < buttonList.Length; i++) {
                    if ((blueWin && buttonList[i].GetComponent<Image>().sprite == secondSprite)
                        || (!blueWin && buttonList[i].GetComponent<Image>().sprite == firstSprite)) {
                       // if (PlayerPrefs.GetInt("Sound") == 1) Handheld.Vibrate();
                        iTween.ShakePosition(buttonList[i].gameObject, iTween.Hash("x", 8f, "y", 8f, "time", 1.7f, "oncompletetarget", this.gameObject, "oncomplete", "TravelToScore", "oncompleteparams", i));
                    }
                }
            }
            // smanjenje kada se poveca do cilja 
            speed2 = 0f;
            c = d;
            d = TutSquare.width;
        }
        // povecanje velicine
        speed = (d == TutSquare.width) ? speed2 += 5f * Time.deltaTime : speed2 += 2f * Time.deltaTime;
        buttonList[blowIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.SmoothStep(c, d, speed2), Mathf.SmoothStep(c, d, speed2));
    }

    public void TravelToScore(int i) {
        // put do score bannera
        if (blueWin)
            iTween.MoveTo(buttonList[i].gameObject, iTween.Hash("x", p1PH.transform.position.x * 1.03f, "y", p1PH.transform.position.y, "time", 1.5f,
                                                                "easetype", "easeOutExpo", "oncompletetarget", this.gameObject, "oncomplete", "GoPanel"));
        else
            iTween.MoveTo(buttonList[i].gameObject, iTween.Hash("x", p2PH.transform.position.x * 1.03f, "y", p2PH.transform.position.y, "time", 1.5f,
                                                               "easetype", "easeOutExpo", "oncompletetarget", this.gameObject, "oncomplete", "GoPanel"));
    }

    public void GoPanel() {
        // game over panel
        SetTutText();
        gameOverPanel.SetActive(true);
        p1PH.text = p1WinCounter.ToString();
        p2PH.text = p2WinCounter.ToString();
        restartButton.SetActive(true);
    }

    public void EndTurn() {
        if (!isGameOver) {
            xMove4 = false;
            CheckMove();
            if (goodMove) {
                blueTake = !blueTake;
                StartCoroutine("RedPlayAnim");
               // if (!isGameOver) sound.GameSound(2);
            } else {
                for (int i = 0; i <= 5; i++) if (TutSquare.moveNum == i) finger.transform.position = fingerPos[i * 2].transform.position;
                fingerSpeed = 0;
                finger.transform.SetAsLastSibling();
                moveFinger = true;
            }
        }
    }

    public void SetTutText() {
        if (TutSquare.moveNum == 0)
            tutText.text = "You can play horizontally or vertically, but not both!";
        else if (TutSquare.moveNum == 3 && !cancelSquare)
            tutText.text = "You can cancel your move by going in opposite direction";
        else if (cancelSquare)
            tutText.text = "You're almost ready";
        else if (TutSquare.moveNum == 4)
            tutText.text = "You can also leave the opponent with multiple secluded squares and still win!";
        else if (TutSquare.moveNum == 5)
            tutText.text = "Have fun outsmarting your friends!";

    }

    public void CheckMove() {

        if (TutSquare.moveNum == 0 && IsFirstSprite(9) && IsFirstSprite(14) && IsFirstSprite(19) && IsFirstSprite(24)) {
            SetFingerStart(0);
        } else if (TutSquare.moveNum == 1 && IsFirstSprite(21) && IsFirstSprite(16) && IsFirstSprite(11) && IsFirstSprite(6)) {
            SetFingerStart(1);
        } else if (TutSquare.moveNum == 2 && IsFirstSprite(22) && IsFirstSprite(23)) {
            SetFingerStart(2);
        } else if (TutSquare.moveNum == 3 && IsFirstSprite(19) && IsFirstSprite(18) && IsFirstSprite(17) && !IsFirstSprite(16)) {
            SetFingerStart(3);
        } else if (TutSquare.moveNum == 4 && IsFirstSprite(4) && IsFirstSprite(9) && IsFirstSprite(14) && cancelSquare) {
            cancelSquare = false;
            SetFingerStart(4);
        } else if (TutSquare.moveNum == 5 && IsFirstSprite(8) && IsFirstSprite(13)) {
            SetGoodMove();
        } else {
            CancelMove();
        }

    }

    public void SetFingerStart(int i) {
        fingerSpeed = 0;
        finger.transform.position = fingerPos[i * 2 + 2].transform.position;
        if (!cancelSquare) SetGoodMove();
    }

    public void SetGoodMove() {
        if (TutSquare.moveNum == 2) {
            goodMove = true;
            UnSelect();
        } else {
            goodMove = true;
            playerSide = (playerSide == firstSprite) ? secondSprite : firstSprite;
            bannerChange = true;
            UnSelect();
        }
    }

    public IEnumerator RedPlayAnim() {
        if (TutSquare.moveNum == 0) {
            for (int i = 4; i >= 1; i--) {
                yield return new WaitForSeconds(0.3f);
                RedPlay(i);
            }
            SetRedGoodMove();
        } else if (TutSquare.moveNum == 1) {
            int i = 0;
            for (i = i + (i * 4); i + (i * 4) <= 20; i++) {
                yield return new WaitForSeconds(0.3f);
                RedPlay(i + (i * 4));
            }
            SetRedGoodMove();
        } else if (TutSquare.moveNum == 3) {
            for (int i = 20; i <= 24; i++) {
                yield return new WaitForSeconds(0.3f);
                RedPlay(i);
            }
            SetRedGoodMove();
        } else if (TutSquare.moveNum == 4) {
            for (int i = 2; i <= 3; i++) {
                yield return new WaitForSeconds(0.3f);
                RedPlay(i);
            }
            yield return new WaitForSeconds(0.3f);
            SetRedGoodMove();
        } else {
            bannerChange = false;
            GameOver();
        }
    }

    public void RedPlay(int i) {
      //  sound.GameSound(1);
        buttonList[i].GetComponent<Image>().sprite = secondSprite;
        buttonList[i].interactable = false;
    }

    public void SetRedGoodMove() {
       // if (!isGameOver) sound.GameSound(2);
        finger.transform.SetAsLastSibling();
        moveFinger = true;
        goodMove = false;
        SetTutText();
        TutSquare.moveNum++;
        playerSide = (playerSide == firstSprite) ? secondSprite : firstSprite;
        bannerChange = true;
        blueTake = !blueTake;
    }

    public void GameOver() {
        gameOverText.text = "YOU WON";
        blueWin = true;
        p1WinCounter++;

        isGameOver = true;
        canPlay = false;
        GameOverRoutine(playerSide);
    }

    private void GameOverRoutine(Sprite ps) {
        if (TutSquare.moveNum == 5) {
            StartCoroutine("EndAnim", ps);
        } else {
            //pokretanje blowUpa zadnjeg buttona u updateu
            blowIndex = 18;
            buttonList[18].transform.SetSiblingIndex(FindObjectOfType<Canvas>().transform.childCount - 3);
            c = TutSquare.width;
            d = c * 4f;
            blowUp = true;
        }
    }

    private IEnumerator EndAnim(Sprite ps) {
        for (int i = 0; i < buttonList.Length; i++) {
            if (buttonList[i].interactable && i != 16) {
                yield return new WaitForSeconds(0.3f);
                //sound.GameSound(1);
                buttonList[i].GetComponent<Image>().sprite = ps;
                ps = (ps == secondSprite) ? firstSprite : secondSprite;
            }
        }
        blowIndex = 16;
        buttonList[16].transform.SetSiblingIndex(FindObjectOfType<Canvas>().transform.childCount - 3);
        c = TutSquare.width;
        d = c * 4f;
        blowUp = true;
        restartButton.GetComponent<Image>().sprite = startButton;
    }

    public void UnSelect() {
        for (int i = 0; i < buttonList.Length; i++)
            if (buttonList[i].GetComponent<TutSquare>().GetSelected()) {
                buttonList[i].GetComponent<TutSquare>().SetSelected(false);
                buttonList[i].transform.localScale = new Vector3(1f, 1f, 0);
                boxList.Remove(buttonList[i].GetComponent<TutSquare>());
                numOfselected = 0;
            }
    }

    public void CancelMove() {

        for (int i = 0; i < buttonList.Length; i++)
            if (buttonList[i].GetComponent<TutSquare>().GetSelected()) buttonList[i].GetComponent<TutSquare>().Restart();

        if (!blueTake) return;
        xMove4 = false;
        goodMove = false;
        for (int i = 0; i <= 5; i++) if (TutSquare.moveNum == i) finger.transform.position = fingerPos[i * 2].transform.position;
        fingerSpeed = 0;
        moveFinger = true;
        finger.transform.SetAsLastSibling();

    }

    public void Restart() {
        isGameOver = false;
        restartButton.SetActive(false);
        gameOverPanel.SetActive(false);
        speed = 0;
        TutSquare.moveNum++;
        blueTake = true;

        for (int i = 0; i < buttonList.Length; i++) {
            buttonList[i].GetComponent<TutSquare>().button.interactable = true;
            buttonList[i].GetComponent<TutSquare>().SetIsAlone(false);
            buttonList[i].GetComponent<Image>().sprite = normalSprite;
            buttonList[i].transform.localPosition = startPositions[i];
            if (i == 1 || i == 5 || i == 7 || i == 11 || i == 15) {
                buttonList[i].interactable = false;
                buttonList[i].GetComponent<Image>().sprite = disabledSprite;
            }
        }

        finger.transform.SetAsLastSibling();
        moveFinger = true;
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
        return buttonList[i].GetComponent<TutSquare>().transform.position;
    }

    public bool IsFirstSprite(int i) {
        return buttonList[i].GetComponent<Image>().sprite == firstSprite;
    }

    public bool GetGameOver() {
        return isGameOver;
    }
}

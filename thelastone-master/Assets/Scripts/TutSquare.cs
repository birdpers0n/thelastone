using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TutSquare : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler {

    private TutGameWorld gw;
    public Button button;
    private ClickSound sound;


    public static bool pressed, sameXdistance, sameYdistance, canceledMove, isHor, firstCheck;
    private static float buttonY, buttonX;
    public static float width;
    public bool selected, pointerUp, isAlone, pressedButton;
    public static int moveNum;
    public static bool goodMove, up, down, left, right;

    public void Awake() {
       // sound = FindObjectOfType<AudioSource>().GetComponent<ClickSound>();
        width = GetComponent<RectTransform>().rect.width;
        moveNum = 0;
    }

    public void SetGwReference(TutGameWorld gw) {
        this.gw = gw;
    }

    public void OnPointerDown(PointerEventData eventData) {
        gw.CancelMove();
        gw.canPlay = false;
        for (int i = 0; i <= 5; i++) if (moveNum == i && button.tag == i.ToString()) FingerPressed();
        if (button.interactable && gw.canPlay) {
         //   sound.GameSound(1);
            firstCheck = true;
            pressed = true;
            pressedButton = true;
            selected = true;
            pointerUp = true;
            canceledMove = false;
            SetUpSprites();
            buttonY = GetPosition().y;
            buttonX = GetPosition().x;
            button.interactable = false;
        }
    }

    private void FingerPressed() {
        gw.canPlay = true;
        gw.moveFinger = false;
        gw.finger.transform.SetAsFirstSibling();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (transform.tag == "13" && moveNum == 4) {
            gw.CancelMove();
            return;
        }
        if (gw.isGameOver) return;

        selected = true;
        SetUpSprites();
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (pressed && !canceledMove) {
            if (button.interactable) {
                sameXdistance = IsSameDistance(GetPosition().x, buttonX);
                sameYdistance = IsSameDistance(GetPosition().y, buttonY);
            }

            // provjera je li se igra horizontalno ili vertikalno na pocetku poteza
            if (firstCheck) {
                isHor = (GetPosition().y == buttonY) ? true : false;
                firstCheck = false;
            }

            if (isHor) {
                // horizontalno igranje
                if (button.interactable) {
                    //normala igranje
                    Play(sameXdistance, GetPosition().y, buttonY);
                } else if (GetPosition().y == buttonY && selected) {
                    //brisanje u potezu
                    DeleteInMove(GetPosition().x, buttonX);
                } else if (GetPosition().y != buttonY) {
                    //illegal move
                  //  sound.GameSound(3);
                    gw.CancelMove();

                }
            } else {
                //vertikalno igranje
                if (button.interactable) {
                    //normala igranje
                    Play(sameYdistance, GetPosition().x, buttonX);
                } else if (GetPosition().x == buttonX && selected) {
                    //brisanje u potezu
                    DeleteInMove(GetPosition().y, buttonY);
                } else if (GetPosition().x != buttonX) {
                    //illegal move
                  //  sound.GameSound(3);
                    gw.CancelMove();

                }
            }
        }
    }



    public void OnPointerUp(PointerEventData eventData) {
        //gw.canPlay = false;
        pressed = false;
        pressedButton = false;
        if (!canceledMove && !gw.GetGameOver() && pointerUp) {
            gw.EndTurn();
        }
        pointerUp = false;
    }

    public void Play(bool boolean, float thisPos, float buttonPos) {
        if (boolean && thisPos == buttonPos) {
         //   sound.GameSound(1);
            buttonY = GetPosition().y;
            buttonX = GetPosition().x;
            SetUpSprites();
            selected = true;
            button.interactable = false;
        } else if (thisPos != buttonPos) {
        //    sound.GameSound(3);
            if (GetPosition() == gw.buttonList[13].transform.position) {
                gw.cancelSquare = true;
                gw.SetTutText();
                gw.SetFingerStart(4);
            }
            gw.CancelMove();
        }
    }

    public void DeleteInMove(float thisPos, float lastPos) {
        if (gw.boxList.IndexOf(this) != gw.boxList.Count - 2) return;

        // sound.GameSound(3);
        gw.numOfselected--;
        gw.boxList[gw.numOfselected].GetComponent<TutSquare>().DeleteOneByOne();
        gw.boxList.RemoveAt(gw.numOfselected);
        buttonY = GetPosition().y;
        buttonX = GetPosition().x;

    }

    public void DeleteOneByOne() {

        button.interactable = true;
        button.GetComponent<Image>().sprite = gw.normalSprite;
        button.transform.localScale = new Vector3(1f, 1f, 0);
    }

    public void Restart() {
        button.interactable = true;
        // da se prvi ne brise
        if (moveNum.ToString() != transform.tag) SetUpResetSprites();
        canceledMove = true;
        gw.numOfselected = 0;
        gw.boxList.Remove(this);
    }

    public void SetSelected(bool b) {
        selected = b;
    }

    public void SetUpSprites() {
        button.GetComponent<Image>().sprite = gw.GetPlayerSide();
        gw.numOfselected++;
        gw.boxList.Add(this);
        button.transform.localScale = new Vector3(1.08f, 1.08f, 0);
    }

    private void SetUpResetSprites() {
        button.GetComponent<Image>().sprite = gw.normalSprite;
        button.transform.localScale = new Vector3(1f, 1f, 0);
    }

    public bool IsSameDistance(float thisPos, float buttonPos) {
        return (Mathf.Round(Mathf.Abs(thisPos - buttonPos)) == gw.xDifference);
    }

    public Vector3 GetPosition() {
        return button.transform.position;
    }

    public bool GetSelected() {
        return selected;
    }

    public void SetIsAlone(bool b) {
        isAlone = b;
    }

    public bool GetIsAlone() {
        return isAlone;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Square : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler {

    private GameWorld gw;
    private GameWorldOffline gwOffline;
    public Button button;
    private ClickSound sound;
    private Player player;

    public static bool pressed, sameXdistance, sameYdistance, canceledMove, isHor, firstCheck;
    private static float buttonY, buttonX;
    public static float width;
    public bool selected, pointerUp, isAlone, right, up;

    public void Awake() {
        width = GetComponent<RectTransform>().rect.width;
        sound = ClickSound.instance;
    }

    public void SetGwReference(GameWorld gw) {
        this.gw = gw;
    }

    public void SetGwReferenceOffline(GameWorldOffline gwO)
    {
        this.gwOffline = gwO;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (button.interactable && gw.canPlay) {
            // sound.GameSound(1);
            if(!(gw.client && gw.playerSide == gw.firstSprite) && !(!gw.client && gw.playerSide == gw.secondSprite)) FindObjectOfType<Player>().CmdCheckSquares(button.tag);
            firstCheck = true;
            pressed = true;
            //  selected = true;
            pointerUp = true;
            canceledMove = false;
            // SetUpSprites();
            buttonY = GetPosition().y;
            buttonX = GetPosition().x;
            //button.interactable = false;

        }
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
                    // DeleteInMove(GetPosition().x, buttonX);
                    FindObjectOfType<Player>().CmdDeleteInMove(button.tag, GetPosition().x, buttonX);
                } else if (GetPosition().y != buttonY) {
                    //illegal move
                    //  sound.GameSound(3);
                    //   gw.CancelMove();
                    FindObjectOfType<Player>().CmdCancelMove();
                }
            } else {
                //vertikalno igranje
                if (button.interactable) {
                    //normala igranje
                    Play(sameYdistance, GetPosition().x, buttonX);
                } else if (GetPosition().x == buttonX && selected) {
                    //brisanje u potezu
                    // DeleteInMove(GetPosition().y, buttonY);
                    FindObjectOfType<Player>().CmdDeleteInMove(button.tag, GetPosition().y, buttonY);
                } else if (GetPosition().x != buttonX) {
                    //illegal move
                    //  sound.GameSound(3);
                    //  gw.CancelMove();
                    FindObjectOfType<Player>().CmdCancelMove();
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        pressed = false;
        FindObjectOfType<Player>().CmdUnSelect();
        if (!canceledMove && !gw.GetGameOver() && pointerUp) {
            //  gw.EndTurn();
            if (!(gw.client && gw.playerSide == gw.firstSprite) && !(!gw.client && gw.playerSide == gw.secondSprite)) FindObjectOfType<Player>().CmdEndTurn();
        }
        pointerUp = false;

    }

    public void Play(bool boolean, float thisPos, float buttonPos) {
        if (boolean && thisPos == buttonPos) {

            //  sound.GameSound(1);
            buttonY = GetPosition().y;
            buttonX = GetPosition().x;
            // SetUpSprites();
            //selected = true;
            //button.interactable = false;
            if (!(gw.client && gw.playerSide == gw.firstSprite) && !(!gw.client && gw.playerSide == gw.secondSprite)) FindObjectOfType<Player>().CmdCheckSquares(button.tag);
        } else if (thisPos != buttonPos) {
            //sound.GameSound(3);
            //gw.CancelMove();
            FindObjectOfType<Player>().CmdCancelMove();
        }
    }

    public void DeleteInMove(float thisPos, float lastPos) {
        if (gw.boxList.IndexOf(this) != gw.boxList.Count - 2) return;

        // sound.GameSound(3);
        gw.numOfselected--;
        gw.boxList[gw.numOfselected].GetComponent<Square>().DeleteOneByOne();
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
        SetUpResetSprites();
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
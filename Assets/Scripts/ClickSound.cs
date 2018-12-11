using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickSound : MonoBehaviour {

    public static ClickSound instance = null;
    public AudioSource aus;
    public AudioClip[] clipArray;  

	void Awake () {
        
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }

        // - prva skripta koja se pokrece
        //prvo pokretanje (namjestanje PlayerPrefsa)
        if (!PlayerPrefs.HasKey("Sound")) PlayerPrefs.SetInt("Sound", 1);
        if (!PlayerPrefs.HasKey("FirstGame")) 
            PlayerPrefs.SetInt("FirstGame", 1);
         else 
            PlayerPrefs.SetInt("FirstGame", 0);
        //

        
    }

    public void ButtonSound(Button button) {
        if (button.tag == "Sound") {
            if(PlayerPrefs.GetInt("Sound") == 1) 
                PlayerPrefs.SetInt("Sound", 0);
             else 
                PlayerPrefs.SetInt("Sound", 1);
        }

        if (PlayerPrefs.GetInt("Sound") == 1) {
            if (instance.aus.clip != instance.clipArray[0]) instance.aus.clip = instance.clipArray[0];
            instance.aus.Play();
        }
    }

    public void GameSound(int index) {
        if (PlayerPrefs.GetInt("Sound") == 1) {
            instance.aus.clip = instance.clipArray[2];
            if (index == 2) 
                instance.aus.Play();
             else 
                instance.aus.PlayOneShot(instance.clipArray[index]);
        }
    }


}

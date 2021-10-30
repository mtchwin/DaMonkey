using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

bool track1Fade, track2Fade;

public int musicTrack;
public float volume;
public GameObject virtuaalCam, manager;
public GameManager gmanager;
public SpriteRenderer controls;

    private void Awake(){
        manager = GameObject.Find("GameManager");
        gmanager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        if(transform.name=="Room2"){
            controls.color = new Color(1,1,1,0);
        }
        if(other.CompareTag("Player")){
            virtuaalCam.SetActive(true);
            foreach (AudioSource sound in manager.GetComponents<AudioSource>())
            {
                if(musicTrack==0){
                    if(sound.isPlaying){
                        Debug.Log("fading out a track...");
                        StartCoroutine(fadeOutMusic(sound));
                    }
                }
                else if(!sound.Equals(manager.GetComponents<AudioSource>()[musicTrack-1])&&sound.isPlaying){
                    Debug.Log("fading out a track...");
                    StartCoroutine(fadeOutMusic(sound));
                }
            }
            if(musicTrack==0)
                return;
            if(!manager.GetComponents<AudioSource>()[musicTrack-1].isPlaying)
            {
                Debug.Log("fading in a new track");
                StartCoroutine(fadeInMusic(manager.GetComponents<AudioSource>()[musicTrack-1]));
            }else{
                Debug.Log("the track is already playing!");
            }
        }
    }

    private IEnumerator fadeOutMusic(AudioSource sound){
        switch(musicTrack){
            case 0:
                gmanager.track1Fade = true;
                gmanager.track2Fade = true;
                break;
            case 1:
                gmanager.track2Fade = true;
                break;
            case 2:
                gmanager.track1Fade = true;
                break;
        }
        while(sound.volume>0.06f){
            sound.volume = Mathf.Lerp(sound.volume,0,Time.deltaTime*8);
            yield return new WaitForEndOfFrame();
        }
        sound.Stop();
        switch(musicTrack){
            case 0:
                gmanager.track1Fade = false;
                gmanager.track2Fade = false;
                break;
            case 1:
                gmanager.track2Fade = false;
                break;
            case 2:
                gmanager.track1Fade = false;
                break;
        }

    }
    private IEnumerator fadeInMusic(AudioSource sound){
        sound.Play();
        while(sound.volume<volume-.03f){
            sound.volume = Mathf.Lerp(sound.volume,volume,Time.deltaTime*2);
            yield return new WaitForEndOfFrame();
            switch(musicTrack){
            case 1:
                if(gmanager.track1Fade)
                {
                    yield break;
                }
                break;
            case 2:
                if(gmanager.track2Fade)
                {
                    yield break;
                }
                break;
        }
            
        }
        sound.volume = volume;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            virtuaalCam.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool track1Fade, track2Fade;
    public bool cutscenesOn;
    public bool introCutscene;
    public SpriteRenderer title;
    public SpriteRenderer black;
    public SpriteRenderer controls;
    public Animator looney;
    public SpriteRenderer looney2;
    GameObject monk;
    // Start is called before the first frame update
    void Start()
    {
        monk = GameObject.FindGameObjectWithTag("Player");
        if(cutscenesOn){
            StartCoroutine("intro");
        }else{
            GameObject.Find("intro").GetComponent<SpriteRenderer>().enabled = false;
            black.color = new Color(0,0,0,0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator winGame(){
        float time = 0;
        GetComponents<AudioSource>()[1].Stop();
        looney.Play("zoomin");
        while(time<3){
            looney2.color = new Color(1,1,1,Mathf.Lerp(looney2.color.a,1,Time.deltaTime*4));
            time+=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);

    }

    private IEnumerator intro(){
        introCutscene = true;
        float time =0;
        while(time<.3f){
            time+=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while(time<1.5f){
            time+=Time.deltaTime;
            title.color = new Color(1,1,1,Mathf.Lerp(title.color.a,1,Time.deltaTime*2));
            yield return new WaitForEndOfFrame();
        }
        title.color = new Color(1,1,1,1);
        while(time<2f){
            time+=Time.deltaTime;
            black.color = new Color(1,1,1,(2-time)*2);
            yield return new WaitForEndOfFrame();
        }
        black.color = new Color(0,0,0,0);
        while(time<2.5f){
            time+=Time.deltaTime;
            title.color = new Color(1,1,1,(2.5f-time)*2);
            yield return new WaitForEndOfFrame();
        }
        GameObject.Find("intro").GetComponent<Animator>().Play("introAnim");
        while(time<3.2f){
            time+=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        monk.GetComponent<Movement>().direction = -1;
        StartCoroutine(monk.GetComponent<Movement>().swat("b"));
        while(time<4.5f){
            time+=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while(time<5){
            controls.color = new Color(1,1,1,Mathf.Lerp(controls.color.a,.75f,Time.deltaTime*3));
            time+=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameObject.Destroy(GameObject.Find("intro"));
        title.color = new Color(0,0,0,0);
        introCutscene = false;
    }
}

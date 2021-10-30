using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEntry : MonoBehaviour
{

    Movement movement;
    public GameObject splashBounce;
    public GameManager gmanager;
    public RaycastHit2D gdetect;

    // Start is called before the first frame update
    void Start()
    {
        gmanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void waterSplash(){
        movement.sounds[7].pitch = Random.Range(0.8f,1.2f);
        movement.sounds[7].Play();
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.down,1,(1<<4));
        if(hit==null){
            if(hit.transform.tag!="water"){
                hit = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.up,1,(1<<4));
            }
        }
        Object.Instantiate(splashBounce,new Vector3(transform.position.x,hit.point.y,transform.position.z)+Vector3.up*.5f,transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.tag=="water")
            waterSplash();
        else{

        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        int layerMask = ~((1<<LayerMask.NameToLayer("Default"))|(1<<LayerMask.NameToLayer("Ignore Raycast")));
        gdetect = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.down,(99999),(layerMask));
        if(!GetComponent<Collider2D>().enabled) {
            return;
        }
        if(other.transform.name=="girlmonkey"){
            gmanager.introCutscene = true;
            gmanager.StartCoroutine("winGame");
        }
        Debug.Log("entered");
        float temp = movement.extraLength;
        movement.extraLength = 0.3f;
        if(movement.bonk&&(gdetect.point-new Vector2(transform.position.x,transform.position.y)).magnitude<.6f&&gdetect.transform.tag!="water"&&gdetect.normal==Vector2.up){
            //movement.bod.velocity = new Vector2(movement.bod.velocity.x,0);
            movement.bonkland = true;
            return;
        }
        if(GetComponent<CircleCollider2D>()){
            if((gdetect.point-new Vector2(transform.position.x,transform.position.y)).magnitude>.6f){
                Debug.Log("bonk should be occuring, gdetect did not find any surface beneath player");
                movement.extraLength = temp;
                movement.bonk = true;
                foreach(GameObject water in GameObject.FindGameObjectsWithTag("water")){
                    water.GetComponent<BoxCollider2D>().isTrigger = true;
                }
                movement.airTime = 0;
                movement.sounds[4].Play();
                //movement.bod.velocity=Vector2.left*movement.direction*(movement.bod.velocity.x)+movement.bod.velocity.y*Vector2.up;
                return;
            }else {
                Debug.Log("failed to bonk because we hit the ground, gdetect found "+gdetect.transform.name+"and collided at ("+gdetect.point.x+","+gdetect.point.y+")");
            }
        }
        if(movement.bonkland){
            if(other.transform.tag!="water"){
            }
            else{
                waterSplash();
            }
                return;
        }
        Debug.Log("grounded");
        movement.airTime = 0;
        if(!movement.bonk){
            if(movement.bouncesRemaining<=0&&gdetect.transform.tag=="water"){

            }else{
                movement.bouncesRemaining-=1;
                foreach(GameObject water in GameObject.FindGameObjectsWithTag("water")){
                    water.GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }
        }
        if(movement.bouncesRemaining==0){
            movement.airTime= 0;
            if(movement.bonk){
                //movement.bod.velocity = Vector2.zero;
                //movement.splat = true;
                movement.bouncesRemaining-=1;
                return;
            }
            else if(movement.hitType=="b"){
                if(other.transform.tag!="water"){
                    movement.sounds[6].pitch = Random.Range(0.8f,1.2f);
                    movement.sounds[6].Play();
                }else{
                    waterSplash();
                }
                movement.bod.velocity=Vector2.left*movement.direction*3*(movement.strength+0.5f)+Vector2.up*4*(movement.strength+0.5f);
            }else if(movement.hitType=="t"){
                if(other.transform.tag!="water"){
                    movement.sounds[6].pitch = Random.Range(0.8f,1.2f);
                    movement.sounds[6].Play();
                }else{
                    waterSplash();
                }
                movement.bod.velocity=Vector2.right*2*movement.direction*3*(movement.strength+0.5f)+Vector2.up*2*(movement.strength+0.5f);
            }
        }
        if(movement.bouncesRemaining<0&&movement.inBall){
            movement.bod.velocity = Vector2.zero;
            //transform.parent.position = other.
            movement.hit = false;
            movement.inBall = false;
            
        }
        if(!movement.inBall){
            movement.bod.velocity = Vector2.zero;
        }
        //Debug.Log("collided w ground");
        movement.extraLength = temp;
    }
}

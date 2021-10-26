using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEntry : MonoBehaviour
{

    Movement movement;
    public GameObject splashBounce;
    public GameManager gmanager;

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
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.down,1,(1<<3));
        if(hit!=null){
            if(hit.transform.tag!="water"){
                hit = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.up,1,(1<<3));
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
        if(other.transform.name=="girlmonkey"){
            gmanager.introCutscene = true;
            gmanager.StartCoroutine("winGame");
        }
        Debug.Log("entered");
        float temp = movement.extraLength;
        movement.extraLength = 0.3f;
        movement.collidedGround = true;
        if(movement.bonk&&movement.isGrounded()){
            movement.bod.velocity = new Vector2(movement.bod.velocity.x,0);
            movement.bonkland = true;
            return;
        }
        if(!movement.isGrounded()&&movement.inBall){
            movement.extraLength = temp;
            movement.bonk = true;
            movement.airTime = 0;
            movement.sounds[4].Play();
            //movement.bod.velocity=Vector2.left*movement.direction*(movement.bod.velocity.x)+movement.bod.velocity.y*Vector2.up;
            return;
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
        movement.bouncesRemaining-=1;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Rigidbody2D bod;
    public Animator[] animators;
    public AudioSource[] sounds;
    public bool bonkland, collidedGround, hit, uncurl, canSwat, swatting, inBall, bonk, splat, inputL, inputR, inputZ, inputX;
    public string state,hitType;
    public float strength,airTime,extraLength, bonkTime;
    public int direction, bouncesRemaining;
    public GameManager gmanager;


    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D hitt = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.down,9999,(1<<3));
        transform.position = hitt.point-transform.Find("BoxCollider").GetComponent<BoxCollider2D>().offset+transform.Find("BoxCollider").GetComponent<BoxCollider2D>().size.y/1.9f*Vector2.up;
        gmanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enableBox();
        transform.Find("BoxCollider").GetComponent<BoxCollider2D>().sharedMaterial.bounciness = 0.4f;
        direction = 1;
        canSwat = true;
        bod = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        sounds = GetComponents<AudioSource>();
        extraLength = 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        if(canSwat){
            transform.Find("Hand").GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
        }
        getControls();
    }

    private void FixedUpdate() {
        move();
        gravity();
        swat();
        stateAssign();
    }

    public void swat(){
        if(!gmanager.introCutscene&&canSwat){
            if(inputZ){
                canSwat=false;
                StartCoroutine(swat("b"));
            }
            else if(inputX){
                canSwat=false;
                StartCoroutine(swat("t"));
            }
        }
    }

    void LateUpdate(){
        animate();
    }

    public void enableBox(){
        transform.Find("CircleCollider").GetComponent<CircleCollider2D>().enabled = false;
        transform.Find("BoxCollider").GetComponent<BoxCollider2D>().enabled = true;
    }

    public void enableCircle(){
        transform.Find("BoxCollider").GetComponent<BoxCollider2D>().enabled = false;
        transform.Find("CircleCollider").GetComponent<CircleCollider2D>().enabled = true;
    }


    public IEnumerator swat(string type){
        float time = 0;
        bonkTime = 0;
        hitType = type;
        strength  = 0;
        if(gmanager.introCutscene){
            strength = 1;
        }
        bod.velocity = Vector3.zero;
        swatting = true;
        animators[2].speed = 3;
        animators[2].Play(type+"windup",0,0);
        if(!gmanager.introCutscene){
            while(inputZ||inputX){
                transform.Find("Hand").GetComponent<SpriteRenderer>().color = new Color(1,1,1,time*4);
                time+=Time.deltaTime*1.5f;
                if(time>1)
                    time =1;
                yield return new WaitForEndOfFrame();
                strength = time+.1f;
            }
        }
        if(strength<0.15)
            strength = 0.15f;
        //swat goes here
        animators[2].speed = 0.33f+time*1f; 
        animators[2].Play(type+"swing",0,.3f-time);
        if(time<0.25f){
            sounds[3].Play();
        }else if(time<0.5f){
            sounds[2].Play();
        }else if(time<0.75f){
            sounds[1].Play();
        }else if(time<=1){
            sounds[0].Play();
        }
        yield return new WaitForEndOfFrame();
        float targetTime = 0;
        if(type=="b"){
            targetTime = 0.9f;
        }else if(type=="t"){
            targetTime = 0.5f;
        }
        while(animators[2].GetCurrentAnimatorStateInfo(0).normalizedTime<targetTime){
            time+=Time.deltaTime;
            if(time>1)
                time =1;
            if(!gmanager.introCutscene)
                transform.Find("Hand").GetComponent<SpriteRenderer>().color = new Color(1,1,1,time*4);
            yield return new WaitForEndOfFrame();
        }
        //connect with monkey
        bod.isKinematic = false;
        uncurl = false;
        enableCircle();
        time = 0;
        hit = true;
        inBall = true;
        if(type=="b"){
            bod.velocity=(Vector3.up*1.1f+Vector3.right*direction)*13*strength;
        }else if(type=="t"){
            bod.velocity=(Vector3.up/2+Vector3.right*direction)*13*strength;
        }
        Transform hand = transform.Find("Hand");
        hand.parent = null;
        bouncesRemaining = 1;
        yield return new WaitForSeconds(0.1f);
        swatting = false;
        //airborn portion
        if(type=="b"){
            foreach(GameObject water in GameObject.FindGameObjectsWithTag("water")){
                water.GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }else if(type=="t"){
            foreach(GameObject water in GameObject.FindGameObjectsWithTag("water")){
                water.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
        while(true){
            
            if(type=="b"){
                if(bouncesRemaining<=0){
                    transform.Find("Monkey").localEulerAngles=Vector3.zero;
                    transform.Find("Shadow").localEulerAngles=Vector3.zero;
                }else{
                    transform.Find("Monkey").Rotate(0,0,1500*Time.deltaTime*strength);
                    transform.Find("Shadow").Rotate(0,0,1500*Time.deltaTime*strength);
                }
            }else if(type=="t"){
                
                if(bouncesRemaining<0){
                    //transform.Find("Monkey").localEulerAngles=Vector3.zero;
                    //transform.Find("Shadow").localEulerAngles=Vector3.zero;
                }else{
                    transform.Find("Monkey").Rotate(0,0,1500*-Time.deltaTime*strength);
                    transform.Find("Shadow").Rotate(0,0,1500*-Time.deltaTime*strength);
                }
                if(bouncesRemaining<1){
                        
                }
            }
            if(bouncesRemaining<1&&isGrounded()){
                RaycastHit2D hittt = Physics2D.Raycast(new Vector2(transform.position.x+transform.Find("CircleCollider").GetComponent<CircleCollider2D>().radius,transform.position.y),Vector2.right,(.1f),1<<3);
                if(new Vector2(Mathf.Abs(hittt.normal.x),hittt.normal.y)==new Vector2(1,0)){
                    bouncesRemaining = -1;
                }
                
            }
            if(bod.velocity.magnitude<0.2f){
                bouncesRemaining=-1;
            }
            if(!gmanager.introCutscene)
                hand.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-time*4);
            time+=Time.deltaTime;
            if(bonk)
            {
                transform.Find("CircleCollider").GetComponent<CircleCollider2D>().sharedMaterial.friction = 0f;
                bonkTime+=Time.deltaTime;
                transform.Find("Monkey").localEulerAngles=Vector3.zero;
                transform.Find("Shadow").localEulerAngles=Vector3.zero;
                RaycastHit2D spltHit = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.down,0.2f+extraLength,(1<<3));
                if(spltHit!=null&&(spltHit.normal==Vector2.up||bod.velocity.magnitude<=0.2f)){
                    if(bonkland){
                        break;
                    }
                    foreach(GameObject water in GameObject.FindGameObjectsWithTag("water")){
                        water.GetComponent<BoxCollider2D>().isTrigger = true;
                    }
                }
            }
            if(bouncesRemaining<0)
            {
                if(type=="b"||type=="t")
                    break;
                else if(type=="t"){
                    //bod.constraints = RigidbodyConstraints2D.None;
                    if(bod.velocity.magnitude<0.5f){
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        //RaycastHit2D hitt = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),Vector2.down,.4f,(1<<3));
        airTime = 0;
        
        //Debug.Log("gdetect.point was located at "+transform.Find("BoxCollider").GetComponent<ColliderEntry>().gdetect.point.x+","+transform.Find("BoxCollider").GetComponent<ColliderEntry>().gdetect.point.y);
        hit = false;
        inBall = false;
        if(!bonkland){
            sounds[6].pitch = Random.Range(1f,1.4f);
            sounds[6].Play();
            uncurl = true;
        }else
        {
            sounds[5].Play();
            splat = true;
        }
        foreach(GameObject water in GameObject.FindGameObjectsWithTag("water")){
            water.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        bod.velocity = Vector2.zero;
        bonk = false;
        bonkland = false;
        bonkTime = 0;
        hand.parent = transform;
        hand.localPosition = new Vector3(-0.685f,1.375f,0);
        hand.localScale = Vector3.one;
        hand.localEulerAngles= Vector3.zero;
        transform.Find("Monkey").localEulerAngles=Vector3.zero;
        transform.Find("Shadow").localEulerAngles=Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        canSwat = true;
        transform.Find("CircleCollider").GetComponent<CircleCollider2D>().sharedMaterial.bounciness = 0.4f;
        transform.Find("BoxCollider").GetComponent<BoxCollider2D>().sharedMaterial.friction = 1f;
        stateAssign();
        animate();
        if(true){
            //if((transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point-new Vector2(transform.position.x,transform.position.y)).magnitude<.3f&&transform.Find("CricleCollider").GetComponent<ColliderEntry>().gdetect.transform.name=="Grid"){
                Debug.Log("gdetect.point was located at "+transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point.x+","+transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point.y);
                transform.position = new Vector3 (transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point.x,transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point.y+.55f,transform.position.z);
            //}
                
        }
        enableBox();
    }

    public void animate(){
        if(state=="walk"){
            animators[0].Play("walk");
            animators[1].Play("walk");
        }else if(uncurl){
            animators[0].Play("uncurl");
            animators[1].Play("uncurl");
        }else if(state=="stand"||state=="swatting"){
            animators[0].Play("walk",0,0);
            animators[1].Play("walk",0,0);
        }else if(state=="splat"){
            animators[0].Play("splat");
            animators[1].Play("splat");
        }else if(state=="bonk"){
            if(bonkTime<0.5f){
                animators[0].Play("bonk");
                animators[1].Play("bonk");
            }else{
                animators[0].Play("fall");
                animators[1].Play("fall");
            }
        }else if(state=="hit"){
            animators[0].Play("hit");
            animators[1].Play("hit");
        }else if(state=="fall"){
            animators[0].Play("fall");
            animators[1].Play("fall");
        }
    }

    public void gravity(){
        if(inBall){
            airTime+=Time.fixedDeltaTime;
            bod.velocity+=Vector2.down*(airTime)*2;
        }else{
            airTime = 0;
        }
    }

    public void move(){
        Vector2 moveVector = Vector2.zero;
        float width = GetComponentInChildren<BoxCollider2D>().size.x;
        RaycastHit2D hitBonkSlope = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y)+new Vector2(GetComponentInChildren<CircleCollider2D>().offset.x*direction,GetComponentInChildren<CircleCollider2D>().offset.y),Vector2.down,GetComponentInChildren<CircleCollider2D>().radius+extraLength,(1<<3));
        if(hitBonkSlope!=null){
            if(bonkland&&hitBonkSlope.normal==Vector2.up&&hitBonkSlope.transform.tag!="water"){
                if(!sounds[4].isPlaying)
                    sounds[4].Play();
                bod.AddForce(hitBonkSlope.normal*8);
            }
        }
        if(!gmanager.introCutscene&&(state=="stand"||state=="walk"||state=="splat"||state=="uncurl")&&!bonk){
            if(inputR){
                moveVector+=Vector2.right*3f;
                splat = false;
                uncurl = false;
            }
            if(inputL){
                moveVector-=Vector2.right*3f;
                splat = false;
                uncurl = false;
            }
        }
        if(moveVector.x<0){
            transform.localScale = new Vector3(-1,1,1);
            direction = -1;
            //transform.Find("Shadow").localScale = new Vector3(1,1,1);
        }else if(moveVector.x>0){
            transform.localScale = new Vector3(1,1,1);
            direction = 1;
            //transform.Find("Shadow").localScale = new Vector3(-1,1,1);
        }
        float verticality = 0;
        if(transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.normal!=Vector2.up){
            verticality = 0.2f;
        }
        RaycastHit2D hito = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y)+new Vector2(GetComponentInChildren<BoxCollider2D>().offset.x*direction,GetComponentInChildren<BoxCollider2D>().offset.y)+new Vector2(((width+.1f)*direction),0)+Vector2.down*GetComponentInChildren<BoxCollider2D>().size.y/2.2f,Vector2.down,extraLength,(1<<3));
        if (!gmanager.introCutscene&&(state=="stand"||state=="walk"||state =="splat"||state =="uncurl")&&!bonk&&hito.normal==Vector2.up&&hito!=null&&hito.transform.tag!="water"&&(Mathf.Abs(transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point.y-transform.position.y))<0.6f&&transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.normal==Vector2.up){
            bod.isKinematic = false;
            bod.velocity = moveVector;
        }else if(!hit&&!inBall){
            Debug.Log("the vertical distance from ground point is"+Mathf.Abs(transform.Find("CircleCollider").GetComponent<ColliderEntry>().gdetect.point.y-transform.position.y));
            bod.isKinematic = true;
            bod.velocity = Vector2.zero;
        }
        
        //Debug.Log("normal is "+hito.normal.x+","+hito.normal.y);

        
    }

    public void stateAssign(){
        if(hit&&!bonk){
            state = "hit";
        }else if (bonk){
            state = "bonk";
        }
        else if(true){
            if(swatting){
                state = "swatting";
            }
            else if(splat){
                state = "splat";
            }
            else if(uncurl){
                state = "uncurl";
            }
            else if(bod.velocity.magnitude<.1f){
                state = "stand";
            }else {
                if(!inBall&&!bonk){
                    state = "walk";
                }else{
                //state = "roll";
                }
            }
        }
        else if(!bonk){
            if(airTime>0.5f){
                state = "fall";
            }
        }
    }

    public void getControls(){
        if(Input.GetKey("left")){
            inputL = true;
        }else{
            inputL= false;
        }
        if(Input.GetKey("right")){
            inputR = true;
        }else{
            inputR= false;
        }
        if(Input.GetKey(KeyCode.Z)){
            inputZ = true;
        }else{
            inputZ= false;
        }
        if(Input.GetKey(KeyCode.X)){
            inputX = true;
        }else{
            inputX= false;
        }
    }

    public bool isGrounded(){
        float length;
        Collider2D collider;
        if(inBall){
            length = transform.Find("CircleCollider").GetComponent<CircleCollider2D>().radius;
            collider = transform.Find("CircleCollider").GetComponent<CircleCollider2D>();
        }
        else{
            length = transform.Find("BoxCollider").GetComponent<BoxCollider2D>().size.y/2;
            collider = transform.Find("BoxCollider").GetComponent<BoxCollider2D>();
            
       
        }
        int layerMask = (LayerMask.GetMask("Stage"));
        float bonkExtra = 0;
        if(bonk)
            bonkExtra = .3f;
        RaycastHit2D hittt = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y)+collider.offset+Vector2.down*length,Vector2.down,(.1f+bonkExtra),1<<3);
        if(hittt.collider!=null&&hittt!=null){
            if(hittt.normal==Vector2.up&&(state=="uncurl"||state=="stand"||state=="splat")&&hittt.transform.tag!="water")
                //transform.position = hittt.point-transform.Find("BoxCollider").GetComponent<BoxCollider2D>().offset+transform.Find("BoxCollider").GetComponent<BoxCollider2D>().size.y/1.9f*Vector2.up;
            //Debug.Log("we hit "+hit.transform.name+" with a length of "+length);
            if(!bonkland)
                airTime = 0;
            return true;
        }else{
            //Debug.Log("we hit nothing with a length of "+length);
            return false;
        }
    }
}

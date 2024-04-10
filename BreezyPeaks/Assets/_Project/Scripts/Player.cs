using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using UnityEngine.U2D;

public class Player : MonoBehaviour{
    public static Player INSTANCE;
    [SerializeField] Rigidbody2D rbObject;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask worldLayer;
    [SerializeField] public float deathCollisionThreshold = 32f;
    [SerializeField] public float deathCollisionThresholdMax = 64f;
    [SerializeField] public float velocityMagnitude = 11f;
    [SerializeField] Vector2 minMaxDistance = new Vector2(1f,3f);
    [SerializeField] bool velocityDoLimit = true;
    [SerializeField] VelocityLimitType velocityLimitType = VelocityLimitType.distTimesMagnitude;
    [ShowIf("@this.velocityLimitType==VelocityLimitType.constant")][SerializeField] float maxVelocity = 5f;
    [SerializeField] bool fly = false;
    [SerializeField] bool flyUpwards = false;
    [SerializeField] bool doTapPush = true;
    [SerializeField] bool doHoldPush = true;
    [SerializeField] bool offsetCamera = true;
    [SerializeField] float offsetCameraAmnt = 3f;
    [SerializeField] bool zoomoutCamera = true;
    [SerializeField] float zoomoutCameraAmnt = 0.1f;
    [SerializeField] float zoomoutCameraBase = 7f;
    [SerializeField] float velocityAddTime;
    [DisableInEditorMode][SerializeField] float velocityAddTimer;
    [SerializeField]List<GameObject> legsObjectsList;
    [DisableInEditorMode][SerializeField] public bool dead;
    [DisableInEditorMode][SerializeField] public bool canSetScore = true;
    [SerializeField] public float baseWindParticleSpeedMult = 0.3f;
    [SerializeField] float collisionSoundThresholdMin = 0.4f;
    [SerializeField] float collisionSoundThresholdMax = 6f;
    [SerializeField] float collisionSoundCooldown = 0.01f;
    [DisableInEditorMode][SerializeField] float collisionSoundCooldownTimer;
    [SerializeField] float deathCollisionSoundCooldown = 0.15f;
    [DisableInEditorMode][SerializeField] float deathCollisionSoundCooldownTimer;
    [DisableInEditorMode][SerializeField] Vector2 mousePosition;
    [DisableInEditorMode][SerializeField] Vector2 targetVelocity;
    [DisableInEditorMode][SerializeField] float currentBodyVelocityMagnitude;
    [DisableInEditorMode][SerializeField] float currentTotalVelocityMagnitude;
    [HideInEditorMode][SerializeField]List<GameObject> hitBodypartsList;
    [HideInEditorMode][SerializeField]bool easterEggTriggered = false;
    [HideInEditorMode][SerializeField]float easterEggTimer;
    [SerializeField]bool debugCollisions = false;
    CinemachineVirtualCamera  virtualCamera;
    Rigidbody2D rb;
    void Awake(){INSTANCE=this;}
    void Start(){
        rb = rbObject != null ? rbObject : gameObject.GetComponent<Rigidbody2D>();
        if(virtualCamera == null){virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();}
        canSetScore = true;
    }

    void FixedUpdate(){
        if(!GameManager.GlobalTimeIsPaused){
            // Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // mousePosition = AssetsManager.INSTANCE.ClampToWindow(_mousePosition);
            mousePosition = AssetsManager.INSTANCE.ClampToWindow(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // Debug.Log(_mousePosition+" | "+mousePosition);

            if(fly){
                Vector2 directionToMouse = mousePosition - (Vector2)GetPosition();
                Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

                float distance = directionToMouse.magnitude;
                float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);

                targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;

                AddForce(targetVelocity * Time.fixedDeltaTime);
            }
            if(flyUpwards){
                AddForce(Vector2.up * velocityMagnitude * Time.fixedDeltaTime);
            }
            
            if(velocityAddTimer>=0){
                AddForce(targetVelocity * Time.fixedDeltaTime);
                velocityAddTimer-=Time.fixedDeltaTime;
            }

            // Debug.Log("Velocity: "+rb.velocity+" | Mag: "+rb.velocity.magnitude);
            if(velocityDoLimit){
                switch(velocityLimitType){
                    case VelocityLimitType.maxDist:
                        rb.velocity = Vector3.ClampMagnitude(rb.velocity, minMaxDistance.y);
                    break;
                    case VelocityLimitType.distTimesMagnitude:
                        rb.velocity = Vector3.ClampMagnitude(rb.velocity, minMaxDistance.y * velocityMagnitude);
                    break;
                    default:
                        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
                    break;
                }
                // Debug.Log("Clamped: "+rb.velocity+" | Mag: "+rb.velocity.magnitude);
            }

            /// CAMERA
            if(rb.velocity.magnitude > 0.1f){
                if (offsetCamera){
                    virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(rb.velocity.x*offsetCameraAmnt, rb.velocity.y*offsetCameraAmnt,0f);
                }else{
                    if(virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset != Vector3.zero){
                        virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
                    }
                }
                if (zoomoutCamera){
                    virtualCamera.m_Lens.OrthographicSize=zoomoutCameraBase + (zoomoutCameraAmnt*rb.velocity.magnitude);
                }else{
                    virtualCamera.m_Lens.OrthographicSize=zoomoutCameraBase;
                }
            }
        }
    }

    void Update(){
        if(!GameManager.GlobalTimeIsPaused){
            if(!dead){
                // if((Input.GetKeyDown(KeyCode.Mouse0) && doTapPush)){
                if(Input.GetKeyDown(KeyCode.Mouse0) && (doTapPush || doHoldPush)){
                    Vector2 directionToMouse = mousePosition - (Vector2)GetPosition();
                    Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

                    float distance = directionToMouse.magnitude;
                    float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
                    // Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

                    velocityAddTimer = velocityAddTime;
                    PlayWindPushSound(distanceClamped);
                    MakeWindPushParticle(directionToMouse, distanceClamped);

                    if(doTapPush){
                        targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;
                        // velocityAddTimer = velocityAddTime;
                        // AudioManager.INSTANCE.Play("whoosh");
                    }
                }
                if((Input.GetKey(KeyCode.Mouse0) && doHoldPush)){
                    Vector2 directionToMouse = mousePosition - (Vector2)GetPosition();
                    Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

                    float distance = directionToMouse.magnitude;
                    float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
                    // Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

                    targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;
                    if(velocityAddTimer<=0){
                        velocityAddTimer = velocityAddTime;
                        PlayWindPushSound(distanceClamped);
                        MakeWindPushParticle(directionToMouse, distanceClamped);
                    }
                }
            }
            #if UNITY_EDITOR
                if(Input.GetKeyDown(KeyCode.Space)){
                    // fly=!fly;
                    flyUpwards=!flyUpwards;
                    if(dead){Respawn();}
                    return;
                }
                if(Input.GetKeyDown(KeyCode.F)){
                    SetPositionV2(Vector2.zero);
                    rb.velocity=Vector2.zero;
                    if(dead){Respawn();}
                    return;
                }
                if(Input.GetKeyDown(KeyCode.V)){if(dead){Respawn();}}
                if(Input.GetKeyDown(KeyCode.D)){Die();return;}
            #endif

            int _currentHeightScore=Mathf.RoundToInt(GetPosition().y*10);  // 40 is the amount you start at
            if(canSetScore && ((_currentHeightScore > 50 && _currentHeightScore>GameManager.INSTANCE.heightScore) || dead)){  // Only set score if above and dead or any for just goof
                GameManager.INSTANCE.heightScore = Mathf.Clamp(_currentHeightScore,0,int.MaxValue);
            }

            if(!dead){
                if(GetPosition().y<6){ // Balance legs on the ground, not in air
                    foreach(GameObject leg in legsObjectsList){leg.GetComponent<Balance>().force=80f;}
                }else{
                    foreach(GameObject leg in legsObjectsList){leg.GetComponent<Balance>().force=2f;}
                }
            }

            if(collisionSoundCooldownTimer>0){collisionSoundCooldownTimer-=Time.deltaTime;}
            if(deathCollisionSoundCooldownTimer>0){deathCollisionSoundCooldownTimer-=Time.deltaTime;}

            if(hitBodypartsList.Count>=transform.childCount){
                if(!easterEggTriggered){
                    Debug.LogWarning("EASTER EGG");
                    AudioManager.INSTANCE.Play("tomAndJerryGoofyAhhScream");
                    easterEggTriggered=true;
                    Time.timeScale=0.1f;
                    easterEggTimer = 12f;
                }
            }
            if(easterEggTimer>0){
                easterEggTimer-=Time.fixedDeltaTime;
            }else{
                if(easterEggTimer>-1){
                    Time.timeScale=1f;
                    easterEggTimer=-1f;
                }
            }

            currentBodyVelocityMagnitude = rb.velocity.magnitude;
            // currentTotalVelocityMagnitude=0;
            // foreach(){
            //     currentTotalVelocityMagnitude+=;
            // }
        }
        doHoldPush = SaveSerial.INSTANCE.settingsData.holdToFly;
        doTapPush = !SaveSerial.INSTANCE.settingsData.holdToFly;
    }

    void AddForce(Vector2 force){
        rb.AddForce(force);
        // foreach(Rigidbody2D _rb in GetComponentsInChildren<Rigidbody2D>()){_rb.AddForce(force);}
    }
    // void OnCollisionEnter2D(Collision2D other){
    //     // Debug.Log("Collision: "+other.relativeVelocity);
    //     Debug.Log(System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+collisionThreshold);
    //     if(other.relativeVelocity.magnitude > collisionThreshold){
    //         Die();
    //         AudioManager.INSTANCE.Play("meatyPunch");
    //     }else{
    //         if(other.relativeVelocity.magnitude > 0.1f){
    //             AudioManager.INSTANCE.Play("punch");
    //         }
    //     }
    // }
    public void DeathCollision(float velocity){
        if(deathCollisionSoundCooldownTimer<=0){
            deathCollisionSoundCooldownTimer = deathCollisionSoundCooldown;
            AudioManager.INSTANCE.PlayCustom("meatyPunch",Mathf.Clamp(AssetsManager.Normalize(velocity, deathCollisionThreshold, deathCollisionThresholdMax),0.5f,1));
            Die();
        }
    }
    public void RegularCollisionSound(float velocity){
        if(collisionSoundCooldownTimer<=0){
            collisionSoundCooldownTimer = collisionSoundCooldown;
            AudioManager.INSTANCE.PlayCustom("punch",Mathf.Clamp(AssetsManager.Normalize(velocity, collisionSoundThresholdMin-0.05f, collisionSoundThresholdMax),0,1));
        }
    }
    public void Die(){
        if(!dead){
            dead = true;
            fly = false;
            flyUpwards = false;
            // GetComponent<SpriteRenderer>().color = Color.red;
            // foreach(SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>()){spr.color = Color.red;}
            // rb.constraints = RigidbodyConstraints2D.None;
            foreach(Balance b in GetComponentsInChildren<Balance>()){b.enabled=false;}
            rb.mass = 1;
            Debug.LogWarning("You died.");
            GameManager.INSTANCE.SetCurrentScore();
            GameManager.INSTANCE.SaveHighscore();
        }
    }
    public void Respawn(){
        dead = false;
        // GetComponent<SpriteRenderer>().color = Color.white;
        foreach(SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>()){spr.color = Color.white;}
        transform.localEulerAngles = Vector2.zero;
        hitBodypartsList=new List<GameObject>();
        foreach(Balance b in GetComponentsInChildren<Balance>()){b.enabled=true;}
        rb.mass = 0.5f;
    }

    void PlayWindPushSound(float dist){
        AudioManager.INSTANCE.PlayCustom("whoosh",AssetsManager.Normalize(dist+0.1f, minMaxDistance.x, minMaxDistance.y));
    }
    GameObject MakeWindPushParticle(Vector2 dir, float dist){
        float angleToMouse = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        GameObject particle = AssetsManager.INSTANCE.VFX("windPush", mousePosition, 0.6f);
        if(particle!=null){
            particle.transform.localPosition = new Vector3(particle.transform.localPosition.x,particle.transform.localPosition.y, -10f);
            particle.transform.rotation = Quaternion.AngleAxis(180 - angleToMouse, Vector3.forward); // Flipped vertically

            var velocityModule = particle.GetComponent<ParticleSystem>().velocityOverLifetime;
            velocityModule.speedModifier = new ParticleSystem.MinMaxCurve(baseWindParticleSpeedMult*dist);
        }

        return particle;
    }
    public void AddBodypartToHitList(GameObject go){if(!hitBodypartsList.Contains(go))hitBodypartsList.Add(go);}

    public Vector2 GetVelocity(){
        return rb.velocity;
    }
    public Vector3 GetPosition(){
        // return rb.transform.position;
        return rb.transform.localPosition;
    }
    public void SetPosition(Vector3 pos){
        rb.transform.localPosition = pos;
    }
    public void SetPositionV2(Vector2 pos){
        rb.transform.localPosition = pos;
    }
    public float GetCollisionSoundThreshold(){return collisionSoundThresholdMin;}
    public bool GetDebugCollisions(){return debugCollisions;}
}

public enum VelocityLimitType{constant,maxDist,distTimesMagnitude}
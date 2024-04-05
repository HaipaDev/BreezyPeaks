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
    [SerializeField] public float collisionThreshold = 25f;
    [SerializeField] public float velocityMagnitude = 11f;
    [SerializeField] Vector2 minMaxDistance = new Vector2(1f,3f);
    [SerializeField] VelocityLimitType velocityLimitType = VelocityLimitType.distTimesMagnitude;
    [ShowIf("@this.velocityLimitType==VelocityLimitType.constant")][SerializeField] float maxVelocity = 5f;
    [SerializeField] bool fly = false;
    [SerializeField] bool flyUpwards = false;
    [SerializeField] bool doTapPush = true;
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
    [SerializeField] float collisionSoundCooldown = 0.1f;
    [DisableInEditorMode][SerializeField] float collisionSoundCooldownTimer;
    CinemachineVirtualCamera  virtualCamera;
    Rigidbody2D rb;
    Vector2 mousePosition;
    Vector2 targetVelocity;
    [HideInEditorMode][SerializeField]List<GameObject> hitBodypartsList;
    [HideInEditorMode][SerializeField]bool easterEggTriggered = false;
    [HideInEditorMode][SerializeField]float easterEggTimer;
    void Awake(){INSTANCE=this;}
    void Start(){
        rb = rbObject != null ? rbObject : gameObject.GetComponent<Rigidbody2D>();
        if(virtualCamera == null){virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();}
        canSetScore = true;
    }

    void FixedUpdate(){
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(fly){
            Vector2 directionToMouse = mousePosition - (Vector2)GetPosition();
            Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

            float distance = directionToMouse.magnitude;
            float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
            // Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

            targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;

            AddForce(targetVelocity * Time.fixedDeltaTime);
            // Debug.Log("Velocity: "+rb.velocity+" | Mag: "+rb.velocity.magnitude);
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
        if(flyUpwards){
            AddForce(Vector2.up * velocityMagnitude * Time.fixedDeltaTime);
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
                Vector2 movementDir = rb.velocity;
                virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(movementDir.x*offsetCameraAmnt, movementDir.y*offsetCameraAmnt,0f);
            }else{
                if(virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset != Vector3.zero){
                    virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
                }
            }
            if (zoomoutCamera){
                Vector2 movementDir = rb.velocity;
                virtualCamera.m_Lens.OrthographicSize=zoomoutCameraBase + (zoomoutCameraAmnt*movementDir.magnitude);
            }else{
                virtualCamera.m_Lens.OrthographicSize=zoomoutCameraBase;
            }
        }

        if(velocityAddTimer>=0){
            AddForce(targetVelocity * Time.fixedDeltaTime);
            // Debug.Log("Velocity: "+rb.velocity+" | Mag: "+rb.velocity.magnitude);
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

            velocityAddTimer-=Time.fixedDeltaTime;
        }


        // foreach(Rigidbody2D _rb in GetComponentsInChildren<Rigidbody2D>()){
        //     _rb.velocity = rb.velocity;
        // }
        // transform.position = rb.transform.localPosition;
        // rb.transform.localPosition = Vector2.zero;
        // transform.position = Vector2.MoveTowards(transform.position, rb.transform.localPosition, Time.fixedDeltaTime*100f);
        // rb.transform.localPosition = Vector2.MoveTowards(rb.transform.localPosition, Vector2.zero, Time.fixedDeltaTime*100f);
    }

    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Mouse0) && doTapPush && !dead){
            Vector2 directionToMouse = mousePosition - (Vector2)GetPosition();
            Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

            float distance = directionToMouse.magnitude;
            float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
            // Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

            targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;
            velocityAddTimer = velocityAddTime;
            AudioManager.INSTANCE.Play("whoosh");
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

        int _currentScore=Mathf.RoundToInt(GetPosition().y*10)-40;  // 40 is the amount you start at
        if(_currentScore>GameManager.INSTANCE.score && canSetScore){
            // Debug.Log(_currentScore+" | "+GameManager.INSTANCE.score);
            GameManager.INSTANCE.score=_currentScore;
        }

        if(!dead){
            if(GetPosition().y<6){ // Balance leg on the ground, not in air
                foreach(GameObject leg in legsObjectsList){leg.GetComponent<Balance>().force=80f;}
            }else{
                foreach(GameObject leg in legsObjectsList){leg.GetComponent<Balance>().force=2f;}
            }
        }

        if(collisionSoundCooldownTimer>0){
            collisionSoundCooldownTimer-=Time.deltaTime;
        }

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
    public void DeathCollision(){
        if(collisionSoundCooldownTimer<=0){
            collisionSoundCooldownTimer = collisionSoundCooldown;
            AudioManager.INSTANCE.Play("meatyPunch");
            Die();
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
}

public enum VelocityLimitType{constant,maxDist,distTimesMagnitude}
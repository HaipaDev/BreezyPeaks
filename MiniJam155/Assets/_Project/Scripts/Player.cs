using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using UnityEngine.U2D;

public class Player : MonoBehaviour{
    public static Player INSTANCE;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask worldLayer;
    [SerializeField] int healthMax = 1;
    [SerializeField] int healthStart = 1;
    [DisableInEditorMode][SerializeField] public int health = 1;
    [SerializeField] float collisionThreshold = 25f;
    [SerializeField] float velocityMagnitude = 11f;
    [SerializeField] Vector2 minMaxDistance = new Vector2(1f,3f);
    [SerializeField] VelocityLimitType velocityLimitType = VelocityLimitType.distTimesMagnitude;
    [ShowIf("@this.velocityLimitType==VelocityLimitType.constant")][SerializeField] float maxVelocity = 5f;
    [SerializeField] bool fly = false;
    [SerializeField] bool flyUpwards = false;
    [SerializeField] bool offsetCamera = true;
    [SerializeField] float offsetCameraAmnt = 3f;
    [SerializeField] bool zoomoutCamera = true;
    [SerializeField] float zoomoutCameraAmnt = 0.1f;
    [SerializeField] float zoomoutCameraBase = 7f;
    [SerializeField] float velocityAddTime;
    [DisableInEditorMode][SerializeField] float velocityAddTimer;
    CinemachineVirtualCamera  virtualCamera;
    Rigidbody2D rb;
    Vector2 mousePosition;
    Vector2 targetVelocity;
    void Awake(){INSTANCE=this;}
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        if(virtualCamera == null){virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();}

        health = healthStart;
    }

    void FixedUpdate(){
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(fly){
            Vector2 directionToMouse = mousePosition - (Vector2)transform.position;
            Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

            float distance = directionToMouse.magnitude;
            float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
            // Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

            targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;

            rb.velocity += targetVelocity * Time.fixedDeltaTime;
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
            rb.velocity += Vector2.up * velocityMagnitude * Time.fixedDeltaTime;
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
            rb.velocity += targetVelocity * Time.fixedDeltaTime;
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
    }

    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Mouse0) && health > 0){
            Vector2 directionToMouse = mousePosition - (Vector2)transform.position;
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
                if(health<=0){Respawn();}
                return;
            }
            if(Input.GetKeyDown(KeyCode.R)){
                transform.position=Vector2.zero;
                rb.velocity=Vector2.zero;
                if(health<=0){Respawn();}
                return;
            }
            if(Input.GetKeyDown(KeyCode.D)){Die();return;}
        #endif

        int _currentScore=Mathf.RoundToInt(transform.position.y*10);
        if(_currentScore>GameManager.INSTANCE.score){
            GameManager.INSTANCE.score=_currentScore;
        }
    }
    void OnCollisionEnter2D(Collision2D other){
        // Debug.Log("Collision: "+other.relativeVelocity);
        Debug.Log(System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+collisionThreshold);
        if(other.relativeVelocity.magnitude > collisionThreshold){
            Die();
            AudioManager.INSTANCE.Play("meatyPunch");
        }else{
            if(other.relativeVelocity.magnitude > 0.1f){
                AudioManager.INSTANCE.Play("punch");
            }
        }
    }
    public void Die(){
        health = 0;
        fly = false;
        flyUpwards = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.mass = 1;
        Debug.Log("You died.");
    }
    public void Respawn(){
        health = healthStart;
        GetComponent<SpriteRenderer>().color = Color.white;
        transform.localEulerAngles = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 0.5f;
    }
}

public enum VelocityLimitType{constant,maxDist,distTimesMagnitude}
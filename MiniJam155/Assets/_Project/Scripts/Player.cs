using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using UnityEngine.U2D;

public class Player : MonoBehaviour{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask worldLayer;
    [SerializeField] int healthMax = 1;
    [SerializeField] int healthStart = 1;
    [DisableInEditorMode][SerializeField] int health = 1;
    [SerializeField] float collisionThreshold = 25f;
    [SerializeField] float velocityMagnitude = 11f;
    [SerializeField] Vector2 minMaxDistance = new Vector2(1f,3f);
    [SerializeField] VelocityLimitType velocityLimitType = VelocityLimitType.distTimesMagnitude;
    [ShowIf("@this.velocityLimitType==VelocityLimitType.constant")][SerializeField] float maxVelocity = 5f;
    [SerializeField] bool fly = false;
    [SerializeField] bool offsetCamera = true;
    [SerializeField] float offsetCameraAmnt = 3f;
    CinemachineVirtualCamera  virtualCamera;
    Rigidbody2D rb;
    Vector2 mousePosition;
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        if(virtualCamera == null){virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();}

        health = healthStart;
    }

    void FixedUpdate(){
        if(fly){
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 directionToMouse = mousePosition - (Vector2)transform.position;
            Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

            float distance = directionToMouse.magnitude;
            float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
            // Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

            Vector2 targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;

            rb.velocity += targetVelocity * Time.deltaTime;
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

        /// CAMERA
        if (offsetCamera && rb.velocity.magnitude > 0.1f){
            Vector2 movementDir = rb.velocity;
            virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(movementDir.x*offsetCameraAmnt, movementDir.y*offsetCameraAmnt,0f);
        }else{
            if(virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset != Vector3.zero){
                virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
            }
        }
    }

    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            fly=!fly;
            if(health<=0){Respawn();}
            return;
        }
        if(Input.GetKeyDown(KeyCode.D)){Die();return;}
        if(Input.GetKeyDown(KeyCode.R)){transform.position=Vector2.zero;return;}
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision: "+collision.relativeVelocity);
        Debug.Log(System.Math.Round(collision.relativeVelocity.magnitude,2)+" / "+collisionThreshold);
        if (collision.relativeVelocity.magnitude > collisionThreshold){
            Die();
        }
    }
    public void Die(){
        health = 0;
        fly = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.mass = 1;
        Debug.Log("You died.");
    }
    public void Respawn(){
        health = healthStart;
        fly = true;
        GetComponent<SpriteRenderer>().color = Color.white;
        transform.localEulerAngles = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 0.5f;
    }
}

public enum VelocityLimitType{constant,maxDist,distTimesMagnitude}
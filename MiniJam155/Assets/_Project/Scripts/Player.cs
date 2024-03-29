using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;

public class Player : MonoBehaviour{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask worldLayer;
    [SerializeField] int healthMax = 1;
    [SerializeField] int healthStart = 1;
    [DisableInEditorMode][SerializeField] int health = 1;
    // [SerializeField] float collisionThreshold = 10f;
    [SerializeField] float collisionRayLength = 5.5f;
    [SerializeField] float rayOffset = 5.5f;
    [SerializeField] float velocityMagnitude = 11f;
    [SerializeField] Vector2 minMaxDistance = new Vector2(1f,3f);
    [SerializeField] VelocityLimitType velocityLimitType = VelocityLimitType.distTimesMagnitude;
    [ShowIf("@this.velocityLimitType==VelocityLimitType.constant")][SerializeField] float maxVelocity = 5f;
    [SerializeField] bool fly = false;
    [SerializeField] bool offsetCamera = true;
    [SerializeField] float offsetCameraAmnt = 3f;
    [SerializeField] CinemachineVirtualCamera  virtualCamera;
    Rigidbody2D rb;
    Vector2 mousePosition;
    void Start(){
        rb = GetComponent<Rigidbody2D>();

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


        /// COLLISION
        float playerHeight = GetComponent<Collider2D>().bounds.size.y;
        if (rb.velocity.magnitude > 0.1f) {
            Vector2 rayDirection = rb.velocity.normalized;
            float rayLength = rb.velocity.magnitude * Time.fixedDeltaTime * collisionRayLength;
            // Vector2 rayOrigin = (Vector2)transform.position;

            Bounds playerBounds = GetComponent<Collider2D>().bounds;

            Vector2 _rayOffset = new Vector2(playerBounds.extents.x * rayDirection.x, playerBounds.extents.y * rayDirection.y);
            Vector2 rayOrigin = (Vector2)transform.position + _rayOffset * rayOffset;
            // Vector2 rayOrigin = (Vector2)transform.position + rayDirection * rayOffset;
            // Debug.Log("   "+rayDirection);
            // Debug.Log("|| "+rayDirection*rayLength);
            // int allLayersExceptPlayer = Physics2D.DefaultRaycastLayers & ~(1 << playerLayer);

            // RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayLength, worldLayer);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, worldLayer);

            if(hit.collider != null && hit.collider.gameObject!=this.gameObject){
                Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);
                Debug.LogWarning("HIT");
            }
            if(hit.collider != null && hit.collider.CompareTag("World")){
                Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);
                Die();
            }
            if(hit.collider==null){
                Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.green);
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
    }

    // void OnCollisionEnter2D(Collision2D collision){
    //     if (collision.gameObject.CompareTag("World")){
    //         // Debug.Log(rb.velocity.magnitude+" / "+collisionThreshold);
    //         // if (Mathf.Abs(rb.velocity.magnitude) > collisionThreshold){
    //         //     Die();
    //         // }

    //         // Vector2 wallNormal = collision.contacts[0].normal;
    //         // Vector2 relativeVelocity = rb.velocity.normalized;

    //         // float dotProduct = Vector2.Dot(wallNormal, relativeVelocity);

    //         // Debug.Log(dotProduct+" / 0.5");
    //         // if (dotProduct < -0.5f) {
    //         //     Die();
    //         // }

    //         // Vector2 velocity = rb.velocity.normalized;

    //         // Vector2 collisionNormal = collision.GetContact(0).normal;

    //         // float dotProduct = Vector2.Dot(velocity, collisionNormal);
    //         // Debug.Log(dotProduct+" / -0.5");

    //         // if (dotProduct < -0.5f) {
    //         //     Die();
    //         // }
    //         Vector2 relativeVelocity = rb.velocity - collision.rigidbody.velocity;
    //         Debug.Log(relativeVelocity+" / "+collisionThreshold);
    //         float verticalVelocity = relativeVelocity.y;
    //         float horizontalVelocity = relativeVelocity.x;
    //         if (Mathf.Abs(verticalVelocity) > collisionThreshold || Mathf.Abs(horizontalVelocity) > collisionThreshold){
    //             Die();
    //         }
    //     }
    // }
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
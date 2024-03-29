using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;

public class Player : MonoBehaviour{
    [SerializeField] float velocityMagnitude = 11f;
    [SerializeField] Vector2 minMaxDistance = new Vector2(1f,3f);
    [SerializeField] VelocityLimitType velocityLimitType = VelocityLimitType.distTimesMagnitude;
    [ShowIf("@this.velocityLimitType==VelocityLimitType.constant")][SerializeField] float maxVelocity = 5f;
    [SerializeField] bool fly = false;
    [SerializeField] bool offsetCamera = true;
    [SerializeField] float offsetCameraAmnt = 3f;
    [SerializeField] bool oppositeOffset = false;
    [SerializeField] CinemachineVirtualCamera  virtualCamera;
    Rigidbody2D rb;
    Vector2 mousePosition;
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        if(fly){
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 directionToMouse = mousePosition - (Vector2)transform.position;
            Vector2 directionToMouseNormalizedOpposite = directionToMouse.normalized*-1;

            float distance = directionToMouse.magnitude;
            float distanceClamped = Mathf.Clamp(distance,minMaxDistance.x,minMaxDistance.y);
            Debug.Log("Distance: "+distance+" | Clamped: "+distanceClamped);

            Vector2 targetVelocity = directionToMouseNormalizedOpposite * distanceClamped * velocityMagnitude;

            rb.velocity += targetVelocity * Time.deltaTime;
            Debug.Log("Velocity: "+rb.velocity+" | Mag: "+rb.velocity.magnitude);
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
            Debug.Log("Clamped: "+rb.velocity+" | Mag: "+rb.velocity.magnitude);
        }
        if(offsetCamera){
            if (rb.velocity.magnitude > 0.1f) // Check if the velocity is significant
            {
                Vector2 movementDir = rb.velocity; // Get the normalized direction of the velocity
                if(oppositeOffset){movementDir*=-1;}
                // Quaternion rotation = Quaternion.LookRotation(Vector3.forward, movementDir); // Calculate the rotation to look towards the direction

                // Set the follow offset rotation of the CinemachineVirtualCamera
                // virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = (rotation * Vector3.forward) + new Vector3(0,0,virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z);
                // virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0f, 0f, virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z);
                // virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0f, 0f, -10f);
                // Vector2 _vector2=(Vector2)(rotation * Vector3.forward);
                // Debug.Log(_vector2);
                Debug.Log(movementDir);
                // virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(_vector2.x,_vector2.y,-10f);
                // virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(movementDir.x*offsetCameraAmnt, movementDir.y*offsetCameraAmnt,-10f);
                virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(movementDir.x*offsetCameraAmnt, movementDir.y*offsetCameraAmnt,0f);
            }
        }else{
            Vector3 _defaultCameraOffset=new Vector3(0, 0, -10f);
            // if(virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset != _defaultCameraOffset){
            //     virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _defaultCameraOffset;
            // }
        }
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){fly=!fly;return;}
    }
}

public enum VelocityLimitType{constant,maxDist,distTimesMagnitude}
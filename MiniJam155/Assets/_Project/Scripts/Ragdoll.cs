using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using Sirenix.OdinInspector;

public class Ragdoll : MonoBehaviour{
    // [SerializeField] Animator _anim;
    [SerializeField] List<Collider2D> _colliders;
    [SerializeField] List<HingeJoint2D> _joints;
    [SerializeField] List<Rigidbody2D> _rbs;
    [SerializeField] List<LimbSolver2D> _solvers;
    [SerializeField] bool startState;
    [DisableInEditorMode][SerializeField] bool isRagdollOn;
    void Start(){SetRagdoll(startState);}
    
    [Button("ToggleRagdoll")]
    public void ToggleRagdoll(){isRagdollOn=!isRagdollOn;SetRagdoll(isRagdollOn);}
    public void SetRagdoll(bool ragdollOn=true){
        isRagdollOn = ragdollOn;
        foreach(var col in _colliders){col.enabled=ragdollOn;}
        foreach(var joint in _joints){joint.enabled=ragdollOn;}
        foreach(var rb in _rbs){rb.simulated=ragdollOn;}
        foreach(var col in _colliders){col.enabled=ragdollOn;}
        foreach(var solver in _solvers){solver.weight=ragdollOn ? 0f : 1f;}
    }
}

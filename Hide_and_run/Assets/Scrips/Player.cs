using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnEndOfLevel;
    public float speed=7;
    public float smoothMoveTime=.2f;
    public float turnSpeed = 8f;

    new Rigidbody rigidbody;
    Vector3 velocity;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    bool disabled;
    
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
        Guard.GuardHasSpottedPlayer +=Disable;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled){
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagmitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagmitude, ref smoothMoveVelocity, smoothMoveTime); //Lerp

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z)*Mathf.Rad2Deg;

        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed*Time.deltaTime*inputMagmitude); //скорость поворота

        velocity= transform.forward*speed*smoothInputMagnitude;

        
    }

    void Disable(){
        disabled = true;
    }

    void FixedUpdate(){
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up*angle));
        rigidbody.MovePosition(rigidbody.position+velocity*Time.deltaTime);
        
    }

    void OnTriggerEnter(Collider hitCollider) {
        if (hitCollider.tag == "finish"){
            Disable();
            if(OnEndOfLevel!=null){
                OnEndOfLevel();
            }
        }
    }
    void OnDestroy(){
        Guard.GuardHasSpottedPlayer-=Disable;
    }
}

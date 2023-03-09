using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform PushHolder;

    public static event System.Action GuardHasSpottedPlayer;

    public float speed=5;
    public float waitTime=.3f;
    public float turnspeed = 90f;
    public float timeToSpotPlayer=0.5f;

    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    float TimeVisiblePlayer;

    Transform player;
    public LayerMask viewMask;

    Color originalSpotLightColor;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotLightColor = spotlight.color;

        Vector3[] waypoints = new Vector3[PushHolder.childCount];
        for(int i=0; i<waypoints.Length; i++){
            waypoints[i]=PushHolder.GetChild(i).position;
            waypoints[i]=new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    void Update() {
        if (CanSeePlayer()){
            TimeVisiblePlayer+=Time.deltaTime;
        }
        else{
            TimeVisiblePlayer-=Time.deltaTime;
        }
        TimeVisiblePlayer=Mathf.Clamp(TimeVisiblePlayer, 0 ,timeToSpotPlayer);
        spotlight.color=Color.Lerp(originalSpotLightColor, Color.red, TimeVisiblePlayer/timeToSpotPlayer);

        if(TimeVisiblePlayer>=timeToSpotPlayer){
            if(GuardHasSpottedPlayer != null){
                GuardHasSpottedPlayer();
            }
        }
    }
    bool CanSeePlayer(){
        if (Vector3.Distance(transform.position, player.position)<viewDistance){
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer<viewAngle/2f){
                if(!Physics.Linecast(transform.position, player.position, viewMask)){
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator TurnToFace(Vector3 lookTarget){
        Vector3 directionToLook = (lookTarget - transform.position).normalized;
        float angleToRotate = 90-Mathf.Atan2(directionToLook.z, directionToLook.x)*Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, angleToRotate))>0.05f){
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToRotate, turnspeed*Time.deltaTime);
            transform.eulerAngles = Vector3.up *angle;
            yield return null;
        }
    }


    IEnumerator FollowPath(Vector3[] waypoints){
        transform.position = waypoints[0];

        int targetIndex = 1;
        Vector3 targetPoint = waypoints[targetIndex];
        transform.LookAt(targetPoint);

        while(true){
            transform.position = Vector3.MoveTowards(transform.position , targetPoint, speed*Time.deltaTime);
            if(transform.position == targetPoint){
                targetIndex=(targetIndex+1)%waypoints.Length;
                targetPoint = waypoints[targetIndex];
                yield return new WaitForSeconds(waitTime); //возвращает в while
                yield return StartCoroutine(TurnToFace(targetPoint));
            }
            yield return null;
        }

    }


    void OnDrawGizmos(){

        Vector3 startPosition = PushHolder.GetChild(0).position;
        Vector3 previosPosition = startPosition;

        foreach(Transform waypoint in PushHolder){
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previosPosition, waypoint.position);
            previosPosition = waypoint.position;
        }
        Gizmos.DrawLine(previosPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward*viewDistance);
    }
}

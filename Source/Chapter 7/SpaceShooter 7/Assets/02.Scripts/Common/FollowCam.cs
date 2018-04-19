using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour 
{
    public Transform target; //추적할 대상
    public float moveDamping = 15.0f; //이동속도 계수
    public float rotateDamping = 10.0f; //회전속도 계수
    public float distance = 5.0f; //추적 대상과의 거리
    public float height = 4.0f; //추적 대상과의 높이
    public float targetOffset = 2.0f; //추적 좌표의 오프셋
    //CameraRig의 Transfrom 컴포넌트
    private Transform tr;

    void Start()
    {
        //CameraRig의 Transform 컴포넌트 추출
        tr = GetComponent<Transform>();
    }

    //주인공 캐릭터의 이동 로직이 완료된 후 처리하기 위해 LateUpdate에서 구현
    void LateUpdate()
    {
        //카메라의 높이와 거리를 계산
        var camPos = target.position 
                           - (target.forward * distance) 
                           + (target.up * height);
        
        //이동할 때의 속도 계수를 적용
        tr.position = Vector3.Slerp(tr.position
                                    , camPos
                                    , Time.deltaTime * moveDamping);
        
        //회전할 때의 속도 계수를 적용
        tr.rotation = Quaternion.Slerp(tr.rotation
                                       , target.rotation
                                       , Time.deltaTime * rotateDamping);
        
        //카메라를 추적 대상으로 Z축을 회전시킴
        tr.LookAt(target.position + (target.up * targetOffset));
    }

    //추적할 좌표를 시각적으로 표현
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //추적 및 시야를 맞출 위치를 표시
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f);
        //메인 카메라와 추적 지점 간의 선을 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position);
    }
}

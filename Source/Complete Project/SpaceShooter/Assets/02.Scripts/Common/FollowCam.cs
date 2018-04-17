using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;              //추적할 대상
    public float moveDamping    = 15.0f;  //이동속도 계수
    public float rotateDamping  = 10.0f;  //회전속도 계수
    public float distance       = 5.0f;   //추적 대상과의 거리
    public float height         = 4.0f;   //추적 대상과의 높이
    public float targetOffset   = 2.0f;   //추적 좌표의 오프셋

    [Header("Wall Obstacle Setting")]
    public float heightAboveWall = 8.0f;  //카메라가 올라갈 높이
    public float colliderRadius = 1.8f;   //충돌체의 반지름
    public float overDamping = 5.0f;      //이동속도 계수
    private float originHeight;           //최초 높이를 보관할 변수


    [Header("Etc Obstacle Setting")]
    //카메라가 올라갈 높이
    public float heightAboveObstacle = 12.0f; 
    //주인공에 투사할 래이케스트의 높이 오프셋
    public float castOffset = 1.0f;          

    //CameraRig의 Transfrom 컴포넌트
    private Transform tr;

    void Start()
    {
        //Transform 컴포넌트 추출
        tr = GetComponent<Transform>();
        //최초 카메라의 높이를 보관
        originHeight = height;
    }

	void Update()
	{
        //구체 형태의 충돌체로 충돌 여부를 검사
        if (Physics.CheckSphere(tr.position, colliderRadius))
        {
            //보간함수를 사용해 카메라의 높이를 부드럽게 상승시킴 
            height = Mathf.Lerp(height
                                , heightAboveWall
                                , Time.deltaTime * overDamping);
        }
        else
        {   //보간함수를 사용해 카메라의 높이를 부드럽게 하강시킴
            height = Mathf.Lerp(height
                                , originHeight
                                , Time.deltaTime * overDamping);
        }

        //주인공이 장애물에 가려졌는지를 판단할 래이케스트의 높낮이를 설정
        Vector3 castTarget = target.position + (target.up * castOffset);
        //castTarget 좌표로의 방향 벡터를 계산
        Vector3 castDir = (castTarget - tr.position).normalized;
        //충돌 정보를 반환받을 변수
        RaycastHit hit;

        //래이케스트를 투사해 장애물 여부를 검사
        if (Physics.Raycast(tr.position, castDir, out hit, Mathf.Infinity))
        {
            //주인공을 래이케스트에 맞지 않았을 경우
            if (!hit.collider.CompareTag("PLAYER"))
            {
                //보간함수를 사용해 카메라의 높이를 부드럽게 상승시킴
                height = Mathf.Lerp(height
                                    , heightAboveObstacle
                                    , Time.deltaTime * overDamping);
            }
            else
            {   
                //보간함수를 사용해 카메라의 높이를 부드럽게 하강시킴
                height = Mathf.Lerp(height
                                    , originHeight
                                    , Time.deltaTime * overDamping);
            }
        }
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

        //카메라의 충돌체를 표현하기 위한 구체를 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        //주인공 캐릭터가 장애물에 가려졌는지를 판단할 레이를 표시
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position + (target.up * castOffset), transform.position);
	}
}

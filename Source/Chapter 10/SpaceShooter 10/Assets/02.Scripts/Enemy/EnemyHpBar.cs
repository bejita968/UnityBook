using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    //Canvas를 렌더링하는 카메라
    private Camera uiCamera;
    //UI용 최상위 캔버스
    private Canvas canvas;
    //부모 RectTransform 컴포넌트
    private RectTransform rectParent;
    //자신 RectTransform 컴포넌트
    private RectTransform rectHp;
    //Hpbar 이미지의 위치를 조절할 오프셋
    [HideInInspector] public Vector3 offset = Vector3.zero;
    //추적할 대상의 Transform 컴포넌트
    [HideInInspector] public Transform targetTr;

    void Start()
    {
        //컴포넌트 추출 및 할당
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        //월드좌표를 스크린의 좌표로 변환
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        //카메라의 뒷쪽 영역(180도 회전)일 때 좌푯값 보정
        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }

        //RectTransform 좌푯값을 전달받을 변수
        var localPos = Vector2.zero;
        //스크린 좌표를 RectTransform 기준의 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent
                                                                , screenPos
                                                                , uiCamera
                                                                , out localPos);
        //생명 게이지 이미지의 위치를 변경
        rectHp.localPosition = localPos;
    }
}
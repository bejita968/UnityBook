using UnityEngine;
//Editor 클래스를 접근하기 위해 명시해야 하는 네임스페이스
using UnityEditor; 

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor 
{
	private void OnSceneGUI()
	{
        //EnemyFOV 클래스를 참조
        EnemyFOV fov = (EnemyFOV)target;

        //원주 위의 시작점의 좌표를 계산 (-주어진 각도의 1/2)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        //원의 색상을 흰색으로 지정
        Handles.color = Color.white;

        //외곽선만 표현하는 원반을 그림
        Handles.DrawWireDisc(fov.transform.position //원점 좌표
                             , Vector3.up           //노멀 벡터
                             , fov.viewRange);      //원의 반지름

        //부채꼭의 색성을 지정
        Handles.color = new Color(1, 1, 1, 0.2f);

        //채워진 부채꼴을 그림
        Handles.DrawSolidArc(fov.transform.position //원점 좌표
                             , Vector3.up           //노멀 벡터
                             , fromAnglePos         //부채꼴의 시작 좌표
                             , fov.viewAngle        //부채꼴의 각도
                             , fov.viewRange);      //부채꼴의 반지름

        //시야각의 텍스트를 표시
        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f)
                      , fov.viewAngle.ToString());

        //Handles.color = new Color(1, 0, 0, 1);
        //Handles.DrawLine(fov.transform.position, fov.transform.position + fromAngle * fov.viewRadius);
        //Handles.DrawLine(fov.transform.position, fov.transform.position + toAngle * fov.viewRadius);
	}
}

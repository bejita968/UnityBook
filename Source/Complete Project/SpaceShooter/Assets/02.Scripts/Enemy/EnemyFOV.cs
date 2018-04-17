using UnityEngine;

public class EnemyFOV : MonoBehaviour {

    //적 캐릭터의 추적 사정 거리의 범위
    public float viewRange = 15.0f;
    [Range(0,360)]
    //적 캐릭터의 시야각
    public float viewAngle = 120.0f;

    private Transform enemyTr;
    private Transform playerTr;
    private int playerLayer;
    private int obstacleLayer;
    private int layerMask;

    void Start()
	{
        //컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;

        //레이어 마스크 값 계산
        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
	}

    //주어진 각도에 의해 원주 위의 점의 좌푯값을 계산하는 함수
	public Vector3 CirclePoint(float angle)
    {
        //로컬 좌표계 기준으로 설정하기 위해 적 캐릭터의 Y 회전값을 더함
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad)
                           , 0
                           , Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool isTracePlayer()
    {
        bool isTrace = false;

        //추적 반경 범위안에서 주인공 캐릭터를 추출
        Collider[] colls = Physics.OverlapSphere(enemyTr.position
                                                 , viewRange
                                                 , 1 << playerLayer);

        //배열의 개수가 1일 때 주인공이 범위안에 있다고 판단
        if (colls.Length == 1)
        {
            //적 캐릭터와 주인공 사이의 방향 벡터를 계산
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;

            //적 캐릭터의 시야각에 들어왔는지를 판단
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }

        return isTrace;
    }

    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;

        //적 캐릭터와 주인공 사이의 방향 벡터를 계산
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        //래이케스트를 투사해서 장애물이 있는지 여부를 판단
        if (Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("PLAYER"));
        }
        return isView;
    }
}

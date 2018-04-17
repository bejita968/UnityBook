using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//UI항목을 접근하기 위해 선언하는 네임스페이스
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float initHp = 100.0f;
    public float currHp;
    //BloodScreen 텍스처를 저장하기 위한 변수
    public Image bloodScreen;
    //Hp Bar Image를 저장하기 위한 변수
    public Image hpBar;
    //생명 게이지의 처음 색상(녹색)
    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currColor;


    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        initHp = GameManager.instance.gameData.hp;
        currHp += GameManager.instance.gameData.hp - currHp;
    }

    void Start()
    {
        //불러온 데이터 값을 러hp 적용
        initHp = GameManager.instance.gameData.hp;
        currHp = initHp;

        //생명 게이지의 초기 색상을 설정
        hpBar.color = initColor;
        currColor = initColor;
    }

    //충돌한 Collider의 IsTrigger 옵션이 체크됐을 때 발생
    void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태그가 BULLET이면 Player의 currHp를 차감
        if (coll.tag == bulletTag)
        {
            //Destroy(coll.gameObject);
            coll.gameObject.SetActive(false);

            //혈흔 효과를 표현할 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());
            currHp -= 5.0f;
            //Debug.Log("Player HP = " + currHp.ToString());

            //생명 게이지의 색상 및 크기 변경 함수를 호출
            DisplayHpbar();

            //Player의 생명이 0 이하이면 사망 처리
            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }


    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
    }

    //Player의 사망 처리 루틴
    void PlayerDie()
    {
        OnPlayerDie();
        GameManager.instance.isGameOver = true;
        //Debug.Log("PlayerDie !");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
    }

    void DisplayHpbar()
    {
        //생명 수치가 50%일 때까지는 녹색에서 노란색으로 변경
        if ((currHp / initHp) > 0.5f)
            currColor.r = (1 - (currHp / initHp)) * 2.0f ;
        else //생명 수치가 0%일 때까지는 노란색에서 빨간색으로 변경
            currColor.g =(currHp / initHp) * 2.0f;

        //HpBar의 색상 변경
        hpBar.color = currColor;
        //HpBar의 크기 변경
        hpBar.fillAmount = (currHp / initHp);
    }
}

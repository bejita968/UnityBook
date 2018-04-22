using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Item 클래스에 접근하기 위해 명시한 네임스페이스
using DataInfo;

//에셋 메뉴를 등록하기 위한 어트리뷰트
[CreateAssetMenu(fileName = "GameDataSO"
                 , menuName = "Create GameData"
                 , order = 1)]
public class GameDataObject : ScriptableObject
{
    public int killCount = 0;                       //사망한 적 캐릭터의 수
    public float hp = 120.0f;                       //주인공의 초기 생명
    public float damage = 25.0f;                    //총알의 데미지
    public float speed = 6.0f;                      //이동 속도
    public List<Item> equipItem = new List<Item>(); //취득한 아이템
}
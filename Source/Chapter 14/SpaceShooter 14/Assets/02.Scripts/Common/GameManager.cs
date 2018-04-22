using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//GameData, Item 클래스가 담긴 네임스페이스 명시
using DataInfo;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    //적 캐릭터가 출현할 위치를 담을 배열
    public Transform[] points;
    //적 캐릭터 프리팹을 저장할 변수
    public GameObject enemy;
    //적 캐릭터를 생성할 주기
    public float createTime = 2.0f;
    //적 캐릭터의 최대 생성 개수
    public int maxEnemy = 10;
    //게임 종료 여부를 판단할 변수
    public bool isGameOver = false;

    //싱글턴에 접근하기 위한 Static 변수 선언
    public static GameManager instance = null;

    [Header("Object Pool")]
    //생성할 총알 프리팹
    public GameObject bulletPrefab;
    //오브젝트 풀에 생성할 개수
    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();

    //일시 정지 여부를 판단하는 변수
    private bool isPaused;
    //Inventory의 CanvasGroup 컴포넌트를 저장할 변수
    public CanvasGroup inventoryCG;

    //주인공이 죽인 적 캐릭터의 수
    //[HideInInspector] public int killCount; //주석 처리 또는 삭제
    [Header("GameData")]
    //적 캐릭터를 죽인 횟수를 표시할 텍스트 UI
    public Text killCountTxt;

    //DataManager를 저장할 변수
    private DataManager dataManager;
    //public GameData gameData; //삭제하거나 주석으로 처리한다.
    //ScriptableObject를 연결할 변수
    public GameDataObject gameData;

    //인벤토리의 아이템이 변경됐을 때 발생시킬 이벤트 정의
    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;

    //SlotList 게임오브젝트를 저장할 변수
    private GameObject slotList;
    //ItemList 하위에 있는 네 개의 아이템을 저장할 배열
    public GameObject[] itemObjects;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //instance에 할당된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미함
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        //다른 씬으로 넘어가더라도 삭제하지 않고 유지함
        DontDestroyOnLoad(this.gameObject);

        //DataManager를 추출해 저장
        dataManager = GetComponent<DataManager>();
        //DataManager 초기화
        dataManager.Initialize();

        //인벤토리에 추가된 아이템을 검색하기 위해 SlotList 게임오브젝트 추출
        slotList = inventoryCG.transform.Find("SlotList").gameObject;

        //게임의 초기 데이터 로드
        LoadGameData();

        //오브젝트 풀링 생성함수 호출
        CreatePooling();
    }

    //게임의 초기 데이터 로드
    void LoadGameData()
    {
        //DataManager를 통해 파일에 저장된 데이터 불러오기
        //GameData data = dataManager.Load();
        //gameData.hp = data.hp;
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.killCount = data.killCount;
        //gameData.equipItem = data.equipItem;

        //보유한 아이템이 있을 때만 호출
        if (gameData.equipItem.Count > 0)
        {
            InventorySetup();
        }

        //KILL_COUNT 키로 저장된 값을 로드함
        //killCount = PlayerPrefs.GetInt("KILL_COUNT", 0); //주석 처리 또는 삭제
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");
    }

    //게임 데이터를 저장
    void SaveGameData()
    {
        //dataManager.Save(gameData);
    }

    //로드한 데이터를 기준으로 인벤토리에 아이템을 추가하는 함수
    void InventorySetup()
    {
        //SlotList 하위에 있는 모든 Slot을 추출
        var slots = slotList.GetComponentsInChildren<Transform>();
        //보유한 아이템의 개수만큼 반복
        for (int i = 0; i < gameData.equipItem.Count; i++)
        {
            //인벤토리 UI에 있는 Slot 개수만큼 반복
            for (int j = 1; j < slots.Length; j++)
            {
                //Slot 하위에 다른 아이템이 있으면 다음 인덱스로 넘어감
                if (slots[j].childCount > 0) continue;
                //보유한 아이템의 종류에 따라 인덱스를 추출
                int itemIndex = (int)gameData.equipItem[i].itemType;
                //아이템의 부모를 Slot 게임오브젝트로 변경
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j]);
                //아이템의 ItemInfo 클래스의 itemData에 로드한 데이터 값을 저장
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];
                //아이템을 Slot에 추가하면 바깥 for 구문으로 빠져나감
                break;
            }
        }
    }

    //인벤토리에 아이템을 추가했을 때 데이터의 정보를 갱신하는 함수
    public void AddItem(Item item)
    {
        //보유 아이템에 같은 아이템이 있으면 추가하지 않고 빠져나감
        if (gameData.equipItem.Contains(item)) return;
        //아이템을 GameData.equipItem 배열에 추가
        gameData.equipItem.Add(item);

        //아이템의 종류에 따라 분기 처리
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                //아이템의 계산 방식에 따라 연산 처리
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp += item.value;
                else
                    gameData.hp += gameData.hp * item.value;
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage += gameData.damage * item.value;
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed += gameData.speed * item.value;
                break;
            case Item.ItemType.GRENADE:
                break;
        }

        //.asset 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);
        //아이템이 변경된 것을 실시간으로 반영하기 위해 이벤트를 발생시킴
        OnItemChange();
    }

    //인벤토리에서 아이템을 제거했을 때 데이터를 갱신하는 함수
    public void RemoveItem(Item item)
    {
        //아이템을 GameData.equipItem 배열에서 삭제
        gameData.equipItem.Remove(item);

        //아이템의 종류에 따라 분기 처리
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                //아이템의 계산 방식에 따라 연산 처리
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp -= item.value;
                else
                    gameData.hp = gameData.hp / (1.0f + item.value);
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage = gameData.damage / (1.0f + item.value);
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed = gameData.speed / (1.0f + item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
        }

        //.asset 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);
        //아이템이 변경된 것을 실시간으로 반영하기 위해 이벤트를 발생시킴
        OnItemChange();
    }

    // Use this for initialization
    void Start()
    {
        //처음 인벤토리를 비활성화
        OnInventoryOpen(false);

        //하이러키 뷰의 SpawnPointGroup을 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            StartCoroutine(this.CreateEnemy());
        }
    }

    //적 캐릭터를 생성하는 코루틴 함수
    IEnumerator CreateEnemy()
    {
        //게임 종료 시까지 무한 루프
        while (!isGameOver)
        {
            //현재 생성된 적 캐릭터의 개수 산출
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;
            //적 캐릭터의 최대 생성 개수보다 작을 때만 적 캐릭터를 생성
            if (enemyCount < maxEnemy)
            {
                //적 캐릭터의 생성 주기 시간만큼 대기
                yield return new WaitForSeconds(createTime);
                //불규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);
                //적 캐릭터의 동적 생성
                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    //오브젝트 풀에서 사용 가능한 총알을 가져오는 함수
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            //비활성화 여부로 사용 가능한 총알인지를 판단
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    //오브젝트 풀에 총알을 생성하는 함수
    public void CreatePooling()
    {
        //총알을 생성해 차일드화할 페어런트 게임오브젝트를 생성
        GameObject objectPools = new GameObject("ObjectPools");
        //풀링 개수만큼 미리 총알을 생성
        for (int i = 0; i < maxPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            //비활성화 시킴
            obj.SetActive(false);
            //리스트에 생성한 총알 추가
            bulletPool.Add(obj);
        }
    }

    //일시 정지 버튼 클릭 시 호출할 함수
    public void OnPauseClick()
    {
        //일시 정지 값을 토글시킴
        isPaused = !isPaused;
        //Time Scale이 0이면 정지, 1이면 정상 속도
        Time.timeScale = (isPaused) ? 0.0f : 1.0f;

        //주인공 객체를 추출
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가된 모든 스크립트를 추출함
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        //주인공 캐릭터의 모든 스크립트를 활성화/비활성화
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }

        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    //인벤토리를 활성화/비활성화하는 함수
    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }

    //적 캐릭터가 죽을 때마다 호출될 함수
    public void IncKillCount()
    {
        ++gameData.killCount;
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");
        //죽인 횟수를 저장
        //PlayerPrefs.SetInt("KILL_COUNT", killCount); //주석 처리 또는 삭제
    }

    void OnApplicationQuit()
    {
        //게임 종료 전 게임 데이터를 저장
        SaveGameData();
    }
}
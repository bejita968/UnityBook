using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {

    //폭발 효과 프리팹을 저장할 변수
    public GameObject expEffect;
    //찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;
    //드럼통의 텍스처를 저장할 배열
    public Texture[] textures;

    //총알이 맞은 횟수
    private int hitCount = 0;

    //Rigidbody 컴포넌트를 저장할 변수
    private Rigidbody rb;
    //MeshFilter 컴포넌트를 저장할 변수
    private MeshFilter meshFilter;

    //MeshRenderer 컴포넌트를 저장할 변수
    private MeshRenderer _renderer;

    //AudioSource 컴포넌트를 저장할 변수
    private AudioSource _audio;

    //폭발 반경
    public float expRadius = 10.0f;
    //폭발음 오디오 클립
    public AudioClip expSfx;

    //Shake 클래스를 저장할 변수
    public Shake shake;

    void Start()
    {
        //Rigidbody 컴포넌트를 추출해 저장
        rb = GetComponent<Rigidbody>();
        //MeshFilter 컴포넌트를 추출해 저장
        meshFilter = GetComponent<MeshFilter>();

        //MeshRenderer 컴포넌트를 추출해 저장
        _renderer = GetComponent<MeshRenderer>();

        //AudioSource 컴포넌트를 추출해 저장
        _audio = GetComponent<AudioSource>();

        //Shake 스크립트를 추출
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        //난수를 발생시켜 불규칙적인 텍스처를 적용
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
    }

    //충돌이 발생했을 때 한번 호출되는 콜백 함수
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 게임오브젝트의 태그를 비교
        if (coll.collider.CompareTag("BULLET"))
        {
            //총알의 충돌 횟수를 증가시키고 3발 이상 맞았는지 확인
            if (++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }
    //폭발 효과를 처리할 함수
    void ExpBarrel()
    {
        //폭발 효과 프리팹을 동적으로 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);

        //Rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함
        rb.mass = 1.0f;
        //위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * 1000.0f);

        //폭발력 생성
        IndirectDamage(transform.position);

        //난수를 발생
        int idx = Random.Range(0, meshes.Length);
        //찌그러진 메쉬를 적용
        meshFilter.sharedMesh = meshes[idx];
        GetComponent<MeshCollider>().sharedMesh = meshes[idx];

        //폭발음 발생
        _audio.PlayOneShot(expSfx, 1.0f);

        //셰이크 효과 호출
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));
    }

    //폭발력을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        //주변에 있는 드럼통을 모두 추출
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);

        foreach (var coll in colls)
        {
            //폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            var _rb = coll.GetComponent<Rigidbody>();
            //드럼통의 무게를 가볍게 함
            _rb.mass = 1.0f;
            //폭발력을 전달
            _rb.AddExplosionForce(1200.0f, pos, expRadius, 1000.0f);
        }
    }
}

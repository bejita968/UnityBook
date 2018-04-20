using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력
    public float damage = 20.0f;
    //총알 발사 속도
    public float speed = 1000.0f;

    //컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;
    void Awake()
    {
        //컴포넌트 할당
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
    }

    void OnDisable()
    {
        //재활용된 총알의 여러 효과값을 초기화
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
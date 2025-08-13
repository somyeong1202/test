using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rb;               // 리지드바디2D 컴포넌트 변수 (물리 이동용)
    public int nextMove;          // 다음 이동 방향 (-1: 왼쪽, 0: 정지, 1: 오른쪽)
    public float EnemySpeed;      // 몬스터 이동 속도
    Animator anim;                // 애니메이터 컴포넌트 변수
    SpriteRenderer spriteRenderer;  // 스프라이트 렌더러 변수 (방향 전환용)
    CapsuleCollider2D CuCollider;   // 이건 라핀 콜라이더
    BoxCollider2D bxCollider;       // 이건 늑대 콜라이더 
   
    private void Awake()
    {
        // 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CuCollider = GetComponent<CapsuleCollider2D>();
        bxCollider = GetComponent<BoxCollider2D>();
       

        // 5초 후 Think 함수 실행 (랜덤 이동 결정)
        //Invoke("Think", 5);  5초후에 실행 하면 너무 늦게 움직여서 바로 움직이게 변경 !!
        Think();
    }
   

    private void FixedUpdate()
    {
        // 이동 처리 : x 방향 속도 설정, y 방향은 기존 속도 유지
        rb.velocity = new Vector2(nextMove * EnemySpeed, rb.velocity.y);

        // 발 앞쪽 위치에서 아래로 Raycast 쏘기 (플랫폼 존재 여부 체크)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(frontVec, Vector3.down , new Color(0, 1, 0));  // 디버그용 초록 빔 그리기

        // 플랫폼 레이어에 닿는지 검사 (길 끝에서 방향 전환용)
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)  // 플랫폼이 없으면 (낭떠러지면)
        {
            Turn();                  // 방향 전환 함수 호출
        }

        // 애니메이션 파라미터 업데이트 (매 프레임)
        if (name == "Wolf")                          // 이름이 Wolf인 몬스터면
        {
            anim.SetInteger("WalkSpeed", nextMove);  // 정수형 파라미터에 이동 방향 넣기
        }
        else if (name == "EnemyLapin")               // 이름이 EnemyLapin인 몬스터면
        {
            anim.SetBool("isWalking", nextMove != 0);  // bool 파라미터로 걷는지 여부 설정
        }
    }

    // 몬스터의 다음 이동 방향을 정하는 함수 (재귀 호출)
    void Think()
    {
        // -1, 0, 1 중 랜덤으로 선택 (왼쪽, 정지, 오른쪽)
        nextMove = Random.Range(-1, 2);
       
        // 몬스터 이름에 따라 애니메이션 파라미터 설정
        if(name == "Wolf")
            anim.SetInteger("WalkSpeed", nextMove);  // int 타입 파라미터 설정

        else if (name == "EnemyLapin")
        {
            bool isWalking = (nextMove != 0);         // 0이 아니면 걷는 상태
            anim.SetBool("isWalking", isWalking);     // bool 타입 파라미터 설정
        }
        

        // 이동 방향에 따라 스프라이트 좌우 반전 처리
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == -1;  

        // 2~5초 사이 랜덤 시간 후 다시 Think 호출 (재귀)
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    // 방향 전환 함수 (앞에 길 없으면 호출)
    void Turn()
    {
        nextMove *= -1;                              // 이동 방향 반전
        spriteRenderer.flipX = nextMove == -1;        // 스프라이트 좌우 반전

        CancelInvoke();                              // 현재 예약된 Invoke 모두 취소
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);              // 랜덤 2~5초 후 Think 호출
    }
    public void OnDamaged()  //OnDamaged
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // Sprite Flip Y
        spriteRenderer.flipY = true;
        // Collider Disable
        if (name == "EnemyLapin")
            CuCollider.enabled = false;
        else if (name == "Wolf")
            bxCollider.enabled = false;
       

            // Die Effect Jump
            rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Destroy;
        Invoke("DeActive", 5);
    }
    void DeActive()
    {
        gameObject.SetActive(false);
    }
    
}

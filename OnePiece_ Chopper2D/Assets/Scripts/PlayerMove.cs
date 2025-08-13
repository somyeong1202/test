using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MaxSpeed;   // 최대속도!!
    public float JumpPower;  // 점프 !!
    Rigidbody2D rb;   // 이건 Rigidbody2D rb를 생성 
    SpriteRenderer SpriteRenderer;
    Animator anim;

    float rayDistance = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // rb 에 Rigidbody2D 기능을 집어 넣기
        SpriteRenderer =GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();    
    }

    private void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {     
            rb.velocity = new Vector2(rb.velocity.normalized.x * 0.5f, rb.velocity.y); //normalized  : 벡터 크기를 1로 만든 상태(단위 벡터)
        }

        // 방향 전환 !! Direction Sprite
        if(Input.GetButton("Horizontal"))
            SpriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //  Animation
        if (Mathf.Abs (rb.velocity.x) < 0.3)  //Mathf.Abs 절대값으로 만듬 !!
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    private void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");      //h= 좌우로 움직이는 기능 
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);   //
        // Max Speed
        if(rb.velocity.x > MaxSpeed)  // rb max speed;
              rb.velocity = new Vector2(MaxSpeed,rb.velocity.y);  
        else if(rb.velocity.x < MaxSpeed*(-1))
            rb.velocity = new Vector2(MaxSpeed * (-1), rb.velocity.y);

        bool isJumping = anim.GetBool("isJumping");
        // Landing Platform
        if (rb.velocity.y < 0)  // 캐릭터가 아래로 떨어지는 중일 때 (떨어질 때만 착지 체크)
        {
            float xOffset = 0f;  // 레이캐스트 시작 위치의 x축 오프셋 초기화

            if (isJumping)  // 점프 중일 때
            {
                if (h != 0)  // 이동 방향이 있을 때 (좌우 이동 중일 때)
                    xOffset = 0.5f * Mathf.Sign(h);  // 이동 방향에 따라 오른쪽(+0.5) 또는 왼쪽(-0.5)으로 오프셋 설정
                else
                    xOffset = SpriteRenderer.flipX ? -0.5f : 0.5f;  // 이동 중이 아니면 스프라이트가 뒤집혔는지 보고 방향 추정하여 오프셋 설정
            }
            else  // 점프 상태가 아닐 때 (걷거나 정지 상태)
            {
                xOffset = 0f;  // x축 오프셋 없이 캐릭터 중심에서 레이캐스트 시작
            }

            // 레이캐스트 시작 위치 = 캐릭터 위치 + x축 오프셋 + y축 -0.5f (캐릭터 발밑 정도)
            Vector2 rayStartPos = rb.position + new Vector2(xOffset, -0.5f);

            // 디버그용으로 레이캐스트 위치와 방향 시각화 (녹색 레이)
            Debug.DrawRay(rayStartPos, Vector2.down * rayDistance, Color.green);

            // rayStartPos 위치에서 아래 방향으로 rayDistance 거리만큼 레이캐스트 실행, "Platform" 레이어만 감지
            RaycastHit2D rayHit = Physics2D.Raycast(rayStartPos, Vector2.down, rayDistance, LayerMask.GetMask("Platform"));

            // 레이캐스트가 플랫폼과 충돌했고 충돌 거리가 1 이하라면(즉 바닥에 가까우면)
            if (rayHit.collider != null && rayHit.distance < 1f)
                anim.SetBool("isJumping", false);  // 점프 상태 해제 (착지)
            else if (!isJumping)
                anim.SetBool("isJumping", true);  // 바닥에 없으면 점프 상태로 유지 또는 전환
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 오브젝트 레퍼런스
        GameObject obj = collision.gameObject;

        // EnemyMove가 있는지 체크 (실제 몬스터인지 확인)
        EnemyMove enemyMove = obj.GetComponent<EnemyMove>();

        // -------------------------
        // 1. 플레이어가 몬스터와 충돌한 경우
        // Enemy 태그 + EnemyMove 존재 여부 확인
        // -------------------------
        if (obj.CompareTag("Enemy") && enemyMove != null)
        {
            // 플레이어가 몬스터 위에서 떨어지는 중이면 공격
            if (rb.velocity.y < 0 && transform.position.y > obj.transform.position.y)
            {
                OnAttack(obj.transform); // 몬스터 공격
            }
            else
            {
                OnDamaged(obj.transform.position); // 플레이어 피격
            }
        }
        // -------------------------
        // 2. 플레이어가 함정과 충돌한 경우
        // Enemy 태그이지만 EnemyMove 없음 → 함정
        // -------------------------
        else if (obj.CompareTag("Enemy") && enemyMove == null)
        {
            // 함정 피격 처리
            OnDamaged(obj.transform.position);
        }
        // -------------------------
        // 3. 그 외 (바닥, 아이템 등) → 아무 처리 안 함
        // -------------------------
        else
        {
            // Debug용
            // Debug.Log("충돌: " + obj.name + " (피격 없음)");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            // Point

            // Deactive Item
            collision.gameObject.SetActive(false);
        }
    }
    void OnAttack(Transform enemy)
    {
        //Point

        // Reaction Force
        rb.AddForce(Vector2.up * 5,ForceMode2D.Impulse);
        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        //Change Layer (Immortal Active)
        gameObject.layer = 12;

        SpriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rb.AddForce(new Vector2(dirc , 1) * 7, ForceMode2D.Impulse);

        // 3초후 무적해제
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 11;
        SpriteRenderer.color = new Color(1, 1, 1, 1);
    }
}

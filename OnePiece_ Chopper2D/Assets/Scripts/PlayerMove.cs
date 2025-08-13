using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MaxSpeed;   // �ִ�ӵ�!!
    public float JumpPower;  // ���� !!
    Rigidbody2D rb;   // �̰� Rigidbody2D rb�� ���� 
    SpriteRenderer SpriteRenderer;
    Animator anim;

    float rayDistance = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // rb �� Rigidbody2D ����� ���� �ֱ�
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
            rb.velocity = new Vector2(rb.velocity.normalized.x * 0.5f, rb.velocity.y); //normalized  : ���� ũ�⸦ 1�� ���� ����(���� ����)
        }

        // ���� ��ȯ !! Direction Sprite
        if(Input.GetButton("Horizontal"))
            SpriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //  Animation
        if (Mathf.Abs (rb.velocity.x) < 0.3)  //Mathf.Abs ���밪���� ���� !!
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    private void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");      //h= �¿�� �����̴� ��� 
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);   //
        // Max Speed
        if(rb.velocity.x > MaxSpeed)  // rb max speed;
              rb.velocity = new Vector2(MaxSpeed,rb.velocity.y);  
        else if(rb.velocity.x < MaxSpeed*(-1))
            rb.velocity = new Vector2(MaxSpeed * (-1), rb.velocity.y);

        bool isJumping = anim.GetBool("isJumping");
        // Landing Platform
        if (rb.velocity.y < 0)  // ĳ���Ͱ� �Ʒ��� �������� ���� �� (������ ���� ���� üũ)
        {
            float xOffset = 0f;  // ����ĳ��Ʈ ���� ��ġ�� x�� ������ �ʱ�ȭ

            if (isJumping)  // ���� ���� ��
            {
                if (h != 0)  // �̵� ������ ���� �� (�¿� �̵� ���� ��)
                    xOffset = 0.5f * Mathf.Sign(h);  // �̵� ���⿡ ���� ������(+0.5) �Ǵ� ����(-0.5)���� ������ ����
                else
                    xOffset = SpriteRenderer.flipX ? -0.5f : 0.5f;  // �̵� ���� �ƴϸ� ��������Ʈ�� ���������� ���� ���� �����Ͽ� ������ ����
            }
            else  // ���� ���°� �ƴ� �� (�Ȱų� ���� ����)
            {
                xOffset = 0f;  // x�� ������ ���� ĳ���� �߽ɿ��� ����ĳ��Ʈ ����
            }

            // ����ĳ��Ʈ ���� ��ġ = ĳ���� ��ġ + x�� ������ + y�� -0.5f (ĳ���� �߹� ����)
            Vector2 rayStartPos = rb.position + new Vector2(xOffset, -0.5f);

            // ����׿����� ����ĳ��Ʈ ��ġ�� ���� �ð�ȭ (��� ����)
            Debug.DrawRay(rayStartPos, Vector2.down * rayDistance, Color.green);

            // rayStartPos ��ġ���� �Ʒ� �������� rayDistance �Ÿ���ŭ ����ĳ��Ʈ ����, "Platform" ���̾ ����
            RaycastHit2D rayHit = Physics2D.Raycast(rayStartPos, Vector2.down, rayDistance, LayerMask.GetMask("Platform"));

            // ����ĳ��Ʈ�� �÷����� �浹�߰� �浹 �Ÿ��� 1 ���϶��(�� �ٴڿ� ������)
            if (rayHit.collider != null && rayHit.distance < 1f)
                anim.SetBool("isJumping", false);  // ���� ���� ���� (����)
            else if (!isJumping)
                anim.SetBool("isJumping", true);  // �ٴڿ� ������ ���� ���·� ���� �Ǵ� ��ȯ
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹 ������Ʈ ���۷���
        GameObject obj = collision.gameObject;

        // EnemyMove�� �ִ��� üũ (���� �������� Ȯ��)
        EnemyMove enemyMove = obj.GetComponent<EnemyMove>();

        // -------------------------
        // 1. �÷��̾ ���Ϳ� �浹�� ���
        // Enemy �±� + EnemyMove ���� ���� Ȯ��
        // -------------------------
        if (obj.CompareTag("Enemy") && enemyMove != null)
        {
            // �÷��̾ ���� ������ �������� ���̸� ����
            if (rb.velocity.y < 0 && transform.position.y > obj.transform.position.y)
            {
                OnAttack(obj.transform); // ���� ����
            }
            else
            {
                OnDamaged(obj.transform.position); // �÷��̾� �ǰ�
            }
        }
        // -------------------------
        // 2. �÷��̾ ������ �浹�� ���
        // Enemy �±������� EnemyMove ���� �� ����
        // -------------------------
        else if (obj.CompareTag("Enemy") && enemyMove == null)
        {
            // ���� �ǰ� ó��
            OnDamaged(obj.transform.position);
        }
        // -------------------------
        // 3. �� �� (�ٴ�, ������ ��) �� �ƹ� ó�� �� ��
        // -------------------------
        else
        {
            // Debug��
            // Debug.Log("�浹: " + obj.name + " (�ǰ� ����)");
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

        // 3���� ��������
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 11;
        SpriteRenderer.color = new Color(1, 1, 1, 1);
    }
}

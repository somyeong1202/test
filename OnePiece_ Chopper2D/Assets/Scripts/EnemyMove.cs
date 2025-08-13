using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rb;               // ������ٵ�2D ������Ʈ ���� (���� �̵���)
    public int nextMove;          // ���� �̵� ���� (-1: ����, 0: ����, 1: ������)
    public float EnemySpeed;      // ���� �̵� �ӵ�
    Animator anim;                // �ִϸ����� ������Ʈ ����
    SpriteRenderer spriteRenderer;  // ��������Ʈ ������ ���� (���� ��ȯ��)
    CapsuleCollider2D CuCollider;   // �̰� ���� �ݶ��̴�
    BoxCollider2D bxCollider;       // �̰� ���� �ݶ��̴� 
   
    private void Awake()
    {
        // ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CuCollider = GetComponent<CapsuleCollider2D>();
        bxCollider = GetComponent<BoxCollider2D>();
       

        // 5�� �� Think �Լ� ���� (���� �̵� ����)
        //Invoke("Think", 5);  5���Ŀ� ���� �ϸ� �ʹ� �ʰ� �������� �ٷ� �����̰� ���� !!
        Think();
    }
   

    private void FixedUpdate()
    {
        // �̵� ó�� : x ���� �ӵ� ����, y ������ ���� �ӵ� ����
        rb.velocity = new Vector2(nextMove * EnemySpeed, rb.velocity.y);

        // �� ���� ��ġ���� �Ʒ��� Raycast ��� (�÷��� ���� ���� üũ)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(frontVec, Vector3.down , new Color(0, 1, 0));  // ����׿� �ʷ� �� �׸���

        // �÷��� ���̾ ����� �˻� (�� ������ ���� ��ȯ��)
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)  // �÷����� ������ (����������)
        {
            Turn();                  // ���� ��ȯ �Լ� ȣ��
        }

        // �ִϸ��̼� �Ķ���� ������Ʈ (�� ������)
        if (name == "Wolf")                          // �̸��� Wolf�� ���͸�
        {
            anim.SetInteger("WalkSpeed", nextMove);  // ������ �Ķ���Ϳ� �̵� ���� �ֱ�
        }
        else if (name == "EnemyLapin")               // �̸��� EnemyLapin�� ���͸�
        {
            anim.SetBool("isWalking", nextMove != 0);  // bool �Ķ���ͷ� �ȴ��� ���� ����
        }
    }

    // ������ ���� �̵� ������ ���ϴ� �Լ� (��� ȣ��)
    void Think()
    {
        // -1, 0, 1 �� �������� ���� (����, ����, ������)
        nextMove = Random.Range(-1, 2);
       
        // ���� �̸��� ���� �ִϸ��̼� �Ķ���� ����
        if(name == "Wolf")
            anim.SetInteger("WalkSpeed", nextMove);  // int Ÿ�� �Ķ���� ����

        else if (name == "EnemyLapin")
        {
            bool isWalking = (nextMove != 0);         // 0�� �ƴϸ� �ȴ� ����
            anim.SetBool("isWalking", isWalking);     // bool Ÿ�� �Ķ���� ����
        }
        

        // �̵� ���⿡ ���� ��������Ʈ �¿� ���� ó��
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == -1;  

        // 2~5�� ���� ���� �ð� �� �ٽ� Think ȣ�� (���)
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    // ���� ��ȯ �Լ� (�տ� �� ������ ȣ��)
    void Turn()
    {
        nextMove *= -1;                              // �̵� ���� ����
        spriteRenderer.flipX = nextMove == -1;        // ��������Ʈ �¿� ����

        CancelInvoke();                              // ���� ����� Invoke ��� ���
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);              // ���� 2~5�� �� Think ȣ��
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

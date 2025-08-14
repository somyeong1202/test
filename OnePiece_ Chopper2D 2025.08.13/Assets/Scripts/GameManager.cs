using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ���� ���¸� �����ϴ� �Ŵ��� Ŭ����
public class GameManager : MonoBehaviour
{
    // ��ü ���� ����
    public int totalPoint;

    // ���� ������������ ȹ���� ����
    public int stagePoint;

    // ���� �������� �ε��� (0���� ����)
    public int stageIndex;

    public int health; // ĳ���� hp

    public PlayerMove Player;

    public GameObject[] Stages; 

    // ���� ���������� �Ѿ �� ȣ��Ǵ� �Լ�
    public void NextStage()
    { 
        //Change Stage
        if(stageIndex < Stages.Length -1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;           // �������� �ε����� 1 ����
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else
        {   // Game Clear

            // Player Contol Lock
            Time.timeScale = 0;

            // Result UI
            Debug.Log("���� Ŭ����!");
            //Restart Button UI;

        }

            //Calcuate Point
            totalPoint += stagePoint; // ���� �������� ������ ���� ������ ����
        stagePoint = 0;         // ���� �������� ������ �ʱ�ȭ
    }

    public void HealthDown()
    {
        if (health > 1)
            health--;
        else
        {
            //Player Die Effect
            Player.OnDie();
            // Result UI
            Debug.Log("�׾����ϴ�.!");
            // Retry Button UI
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Player Reposition  ���� �Ŵ����� ������ ĳ���� ��ġ �ʱ�ȭ
            if(health > 1)
            {
                PlayerReposition();
            }
            // Health Down
            HealthDown();
        }
    }
    void PlayerReposition()
    {
        Player.transform.position = new Vector3(-2.7f, 0, 0);
        Player.VelocityZero();
    }
}

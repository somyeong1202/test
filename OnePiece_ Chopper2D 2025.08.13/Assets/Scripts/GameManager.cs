using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임의 진행 상태를 관리하는 매니저 클래스
public class GameManager : MonoBehaviour
{
    // 전체 누적 점수
    public int totalPoint;

    // 현재 스테이지에서 획득한 점수
    public int stagePoint;

    // 현재 스테이지 인덱스 (0부터 시작)
    public int stageIndex;

    public int health; // 캐릭터 hp

    public PlayerMove Player;

    public GameObject[] Stages; 

    // 다음 스테이지로 넘어갈 때 호출되는 함수
    public void NextStage()
    { 
        //Change Stage
        if(stageIndex < Stages.Length -1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;           // 스테이지 인덱스를 1 증가
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else
        {   // Game Clear

            // Player Contol Lock
            Time.timeScale = 0;

            // Result UI
            Debug.Log("게임 클리어!");
            //Restart Button UI;

        }

            //Calcuate Point
            totalPoint += stagePoint; // 현재 스테이지 점수를 누적 점수에 더함
        stagePoint = 0;         // 현재 스테이지 점수를 초기화
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
            Debug.Log("죽었습니다.!");
            // Retry Button UI
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Player Reposition  게임 매니저에 닿으면 캐릭터 위치 초기화
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

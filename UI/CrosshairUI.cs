using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField]
    RectTransform[] cross_UD;
    [SerializeField]
    RectTransform[] cross_RL;

    public float plusPos = 1f; // 한발당 증가 수치
    float startPos = 20;  // 초기위치 20
    float sumPos;   // 최종수치

    public static CrosshairUI instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        for(int i=0; i<cross_RL.Length; i++)
        {
            if(i%2 == 0)
            {
                //cross_UD[i].anchoredPosition = Mathf.Lerp()

                //cross_UD[i].anchoredPosition = new Vector3(0, sumPos + startPos, 0);
                //cross_RL[i].anchoredPosition = new Vector3(sumPos + startPos, 0, 0);
            }
            else
            {
                cross_UD[i].anchoredPosition = new Vector3(0, -sumPos + -startPos, 0);
                cross_RL[i].anchoredPosition = new Vector3(-sumPos + -startPos, 0, 0);
            }
        }

        if (sumPos != 0f && sumPos >0)
        {
            DownPoint();
        }
    }

    // 크로스헤어 증가
    public void UpPoint()
    {
        for (int i = 0; i < cross_RL.Length; i++)
        {
            if (i % 2 == 0)
            {
                //cross_UD[i].anchoredPosition.y = Mathf.Lerp(cross_UD[i].anchoredPosition.y, cross_UD[i].anchoredPosition.y + sumPos, 0.1f);

                //cross_UD[i].anchoredPosition = new Vector3(0, sumPos + startPos, 0);
                //cross_RL[i].anchoredPosition = new Vector3(sumPos + startPos, 0, 0);
            }
            else
            {
                cross_UD[i].anchoredPosition = new Vector3(0, -sumPos + -startPos, 0);
                cross_RL[i].anchoredPosition = new Vector3(-sumPos + -startPos, 0, 0);
            }
        }


        //if(sumPos < 5f)
        //{
        //    sumPos += plusPos;
        //    if(sumPos + startPos >= 25)
        //    {
        //        sumPos = 5;
        //    }
        //}
    }

    // 크로스헤어 감소
    void DownPoint()
    {
        sumPos -= Time.deltaTime * 5f;
        if (sumPos + startPos <= 20)
        {
            sumPos = 0;
        }
    }
}

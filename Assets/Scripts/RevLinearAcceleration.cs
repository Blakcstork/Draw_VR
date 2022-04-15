using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevLinearAcceleration : MonoBehaviour
{

    private Vector3[] positionRegister;
    private float[] posTimeRegister;
    private int positionSamplesTaken = 0;

    public Vector3 LinearAccerlation(out Vector3 vector, Vector3 position, int samples)
    {
        Vector3 averageSpeedChange = Vector3.zero;
        Vector3 averageVelocity = Vector3.zero;
        vector = Vector3.zero;
        Vector3 deltaDistance;
        float deltaTime;
        Vector3 speedA = Vector3.zero;
        Vector3 speedB = Vector3.zero;

        // Clamp sample amount, 가속도 측정 위해선 적어도 2개 이상의 변화 필요
        // 적어도 3개의 포지션 샘플 필요
        if(samples < 3)
        {
            samples = 3;
        }


        // 초기화
        if (positionRegister == null)
        {
            positionRegister = new Vector3[samples];
            posTimeRegister = new float[samples];
        }

        // position과 time sample array
        //sample이 들어올대마다 왼쪽으로 밀어줌. 그러므로 첫번째 index는 항상 가장 오래된 샘플
        for(int i = 0; i<positionRegister.Length -1; i++)
        {
            positionRegister[i] = positionRegister[i + 1]; // 왼쪽으로 밀기
            posTimeRegister[i] = posTimeRegister[i + 1];
        }

        positionRegister[positionRegister.Length - 1] = position;
        posTimeRegister[posTimeRegister.Length - 1] = Time.time;

        positionSamplesTaken++;

        if(positionSamplesTaken >= samples)
        {
            // 평균 속도 변화값 측정
            for(int i = 0; i < positionRegister.Length - 2; i++)
            {
                deltaDistance = positionRegister[i + 1] - positionRegister[i];
                deltaTime = posTimeRegister[i + 1] - posTimeRegister[i];


                //deltatime이 0이면 output은 invalid
                if(deltaTime == 0)
                {
                    return Vector3.zero;
                }

                speedA = deltaDistance / deltaTime; // 속도 = 거리 / 시간
                deltaDistance = positionRegister[i + 2] - positionRegister[i + 1];
                deltaTime = posTimeRegister[i + 2] - posTimeRegister[i + 1]; // 시간, 거리 차이 갱신

                if(deltaTime == 0)
                {
                    return Vector3.zero;
                }

                speedB = deltaDistance / deltaTime; // speedB 산출

                averageSpeedChange += speedB - speedA;
                averageVelocity += speedB;
            }

            // 평균 구하기
            averageSpeedChange /= positionRegister.Length - 2;
            averageVelocity /= positionRegister.Length - 2;


            // 전체 시간 차이 구하기
            float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];


            // 가속도 계산
            vector = averageSpeedChange / deltaTimeTotal;

            // Vector3 curVelocity = (speedA + speedB) / 2.0f;

            return averageVelocity;
        }
        else
        {
            return Vector3.zero;
        }
    }
}

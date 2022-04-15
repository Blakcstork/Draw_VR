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

        // Clamp sample amount, ���ӵ� ���� ���ؼ� ��� 2�� �̻��� ��ȭ �ʿ�
        // ��� 3���� ������ ���� �ʿ�
        if(samples < 3)
        {
            samples = 3;
        }


        // �ʱ�ȭ
        if (positionRegister == null)
        {
            positionRegister = new Vector3[samples];
            posTimeRegister = new float[samples];
        }

        // position�� time sample array
        //sample�� ���ô븶�� �������� �о���. �׷��Ƿ� ù��° index�� �׻� ���� ������ ����
        for(int i = 0; i<positionRegister.Length -1; i++)
        {
            positionRegister[i] = positionRegister[i + 1]; // �������� �б�
            posTimeRegister[i] = posTimeRegister[i + 1];
        }

        positionRegister[positionRegister.Length - 1] = position;
        posTimeRegister[posTimeRegister.Length - 1] = Time.time;

        positionSamplesTaken++;

        if(positionSamplesTaken >= samples)
        {
            // ��� �ӵ� ��ȭ�� ����
            for(int i = 0; i < positionRegister.Length - 2; i++)
            {
                deltaDistance = positionRegister[i + 1] - positionRegister[i];
                deltaTime = posTimeRegister[i + 1] - posTimeRegister[i];


                //deltatime�� 0�̸� output�� invalid
                if(deltaTime == 0)
                {
                    return Vector3.zero;
                }

                speedA = deltaDistance / deltaTime; // �ӵ� = �Ÿ� / �ð�
                deltaDistance = positionRegister[i + 2] - positionRegister[i + 1];
                deltaTime = posTimeRegister[i + 2] - posTimeRegister[i + 1]; // �ð�, �Ÿ� ���� ����

                if(deltaTime == 0)
                {
                    return Vector3.zero;
                }

                speedB = deltaDistance / deltaTime; // speedB ����

                averageSpeedChange += speedB - speedA;
                averageVelocity += speedB;
            }

            // ��� ���ϱ�
            averageSpeedChange /= positionRegister.Length - 2;
            averageVelocity /= positionRegister.Length - 2;


            // ��ü �ð� ���� ���ϱ�
            float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];


            // ���ӵ� ���
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

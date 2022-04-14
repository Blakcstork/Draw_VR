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

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

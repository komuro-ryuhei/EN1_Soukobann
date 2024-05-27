using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    // �����܂łɂ����鎞��
    private float timeTaken = 0.2f;
    // �o�ߎ���
    private float timeErapsed;
    // �ړI�n
    private Vector3 destinatoin;
    // �o���n
    private Vector3 origin;

    public void MoveTo(Vector3 newDestination)
    {
        // �o�ߎ��Ԃ�������
        timeErapsed = 0;
        // �ړ����̉\��������̂ŁA���ݒn��position�ɑO��ړ��̖ړI�n����
        origin = destinatoin;
        transform.position = origin;
        // �V�����ړI�n����
        destinatoin = newDestination;
    }


    // Start is called before the first frame update
    private void Start()
    {
        // �ړI�n�E�o���n�����ݒn�ŏ�����
        destinatoin = transform.position;
        origin = destinatoin;
    }

    // Update is called once per frame
    private void Update()
    {
        // �ړI�n�ɓ��B���������珈�����Ȃ�
        if (origin == destinatoin) { return; }
        //�@���Ԍo�߂����Z
        timeErapsed += Time.deltaTime;
        // �o�ߎ��Ԃ��������Ԃ̉��������Z�o
        float timeRate = timeErapsed / timeTaken;
        // �������Ԃ𒴂���悤�ł���Ύ��s�������ԑ����Ɋۂ߂�
        if(timeRate > 1) { timeRate = 1; }
        // �C�[�W���O�p�v�Z(���j�A)
        float easing = timeRate;
        // ���W���Z�o
        Vector3 currentPosition = Vector3.Lerp(origin, destinatoin, easing);
        // �Z�o�������W��position�ɑ��
        transform.position = currentPosition;
    }
}

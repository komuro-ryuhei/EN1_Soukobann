using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{

    // ���ł���܂ł̎���
    private float lifeTime;
    // ���ł���܂ł̎c�莞��
    private float leftLiefTime;
    // �ړ���
    private Vector3 velocity;
    // ����Scale
    private Vector3 defaultScale;

    // Start is called before the first frame update
    void Start()
    {
        // ���ł���܂ł̎��Ԃ�0.3�b�Ƃ���
        lifeTime = 0.3f;
        // �c�莞�Ԃ�������
        leftLiefTime = lifeTime;
        // ���݂�Scale���L�^
        defaultScale = transform.localScale;
        // �����_���Ō��܂�ړ��ʂ̍ő�l
        float maxVelocity = 5;
        // �e�����փ����_���Ŕ�΂�
        velocity = new Vector3(
            Random.Range(-maxVelocity, maxVelocity),
            Random.Range(-maxVelocity, maxVelocity),
            0
            ); ;
    }

    // Update is called once per frame
    void Update()
    {
        // �c�莞�Ԃ��J�E���g�_�E��
        leftLiefTime -= Time.deltaTime;
        // ���g�̍��W���ړ�
        transform.position += velocity * Time.deltaTime;
        // �c�莞�Ԃɂ�菙�X��Scale������������
        transform.localScale = Vector3.Lerp(
            new Vector3(0,0,0),
            defaultScale,
            leftLiefTime / lifeTime
            );
        // �c�莞�Ԃ�0�ȉ��ɂȂ����玩�g�̃I�u�W�F�N�g������
        if (leftLiefTime <= 0) { Destroy(gameObject); }
    }
}

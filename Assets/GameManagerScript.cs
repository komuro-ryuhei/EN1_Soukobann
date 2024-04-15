using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    // �z��̐錾
    int[] map;

    // ��������o�͂��郁�\�b�h
    void PrintArray()
    {

        // ������̐錾�Ə�����
        string debugText = "";

        for (int i = 0; i < map.Length; i++)
        {
            // ������Ɍ���
            debugText += map[i].ToString() + ",";
        }
        // ������̏o��
        Debug.Log(debugText);
    }

    // 1�̒l���i�[����Ă���C���f�b�N�X���擾���鏈���̃��\�b�h
    int GetPlayerIndex()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// �ړ��̉ۂ𔻒f���郁�\�b�h
    /// </summary>
    /// <param name="number">����������</param>
    /// <param name="moveFrom">������̃C���f�b�N�X</param>
    /// <param name="moveTo">�����O�̃C���f�b�N�X</param>
    /// <returns></returns>
    bool MoveNumber(int number, int moveFrom, int moveTo)
    {
        if (moveTo < 0 || moveTo >= map.Length)
        {
            return false;
        }

        if (map[moveTo] == 2)
        {
            int velocity = moveTo - moveFrom;

            bool success = MoveNumber(2, moveTo, moveTo + velocity);

            if (!success) { return false; }
        }
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �z��̍쐬�Ə�����
        map = new int[] { 0, 0, 0, 1, 0, 2, 0, 0, 0 };
        PrintArray();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            // �E�ړ�����

            int playerIndex = GetPlayerIndex();

            MoveNumber(1, playerIndex, playerIndex + 1);

            PrintArray();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // ���ړ�����

            int playerIndex = GetPlayerIndex();

            MoveNumber(1, playerIndex, playerIndex - 1);

            PrintArray();
        }
    }
}
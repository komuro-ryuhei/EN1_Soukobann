using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerScript : MonoBehaviour
{
    /*/////////////////////////////////////
                �v���t�@�u
    /////////////////////////////////////*/
    // �v���C���[
    public GameObject playerPrefab;
    // �{�b�N�X
    public GameObject boxPrefab;
    // �S�[��
    public GameObject goalPrefab;
    // �p�[�e�B�N��
    public GameObject particlePrefab;
    // �N���A����
    public GameObject clearText;
    // ��
    public GameObject wallPrefab;


    // �{�b�N�X���ړ�������ۂ�SE
    public AudioSource boxMoveSound;
    // �N���A�����ۂ�SE
    public AudioSource clearSound;

    // �X�e�[�W�֘A
    private int[][,] maps;
    private int currentStageIndex = 0;
    private bool isCleared;

    // �z��̐錾
    int[,] map;
    int[,] initialMap;
    // �Q�[���Ǘ��p�̔z��
    GameObject[,] field;

    // �N���A�������ɏ������̃��X�g
    List<GameObject> objectsToRemove = new List<GameObject>();


    // �}�b�v�̏�����
    void InitializeMap()
    {
        // ��dfor���œ񎟌��z��̏����o��
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                    objectsToRemove.Add(field[y, x]);
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                    objectsToRemove.Add(field[y, x]);
                }
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                        goalPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                    objectsToRemove.Add(field[y, x]);
                }
                if (map[y, x] == 4)
                {
                    field[y, x] = Instantiate(
                        wallPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                    objectsToRemove.Add(field[y, x]);
                }
            }
        }
    }

    // 1�̒l���i�[����Ă���C���f�b�N�X���擾���鏈���̃��\�b�h
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// �ړ��̉ۂ𔻒f���郁�\�b�h
    /// </summary>
    /// <param name="number">����������</param>
    /// <param name="moveFrom">������̃C���f�b�N�X</param>
    /// <param name="moveTo">�����O�̃C���f�b�N�X</param>
    /// <returns></returns>
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }

        if (field[moveTo.y, moveTo.x] != null)
        {
            if (field[moveTo.y, moveTo.x].tag == "Box")
            {
                Vector2Int nextMove = moveTo + (moveTo - moveFrom);
                Vector2Int velocity = moveTo - moveFrom;

                // �ړ���ɂ���Ƀ{�b�N�X������ꍇ�͈ړ��𒆎~
                if (field[nextMove.y, nextMove.x] != null && field[nextMove.y, nextMove.x].tag == "Box")
                {
                    return false;
                }

                bool success = MoveNumber("Box", moveTo, moveTo + velocity);

              
                    boxMoveSound.Play();
                

                if (!success) { return false; }
            }
            else if (field[moveTo.y, moveTo.x].tag == "Wall")
            {
                return false; // �ǂ�����ꏊ�ɂ͈ړ��ł��Ȃ�
            }
        }

        // �p�[�e�B�N���𐶐�����
        GenerateParticles(IndexToPosition(moveFrom));

        Vector3 moveToPosition = IndexToPosition(moveTo);
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        return true;
    }

    // �p�[�e�B�N���̊֐�
    void GenerateParticles(Vector3 position)
    {
        // �p�[�e�B�N���̐����w��
        int particleCount = 5;
        for (int i = 0; i < particleCount; i++)
        {
            Instantiate(particlePrefab, position, Quaternion.identity);
        }
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(
            index.x - map.GetLength(1) / 2 + 0.5f,
            -index.y + map.GetLength(0) / 2,
            0
            );
    }

    // �N���A���鏈��
    bool IsCleard()
    {
        // Vector2Int�^�̉ϒ��z��̍쐬
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // �i�[�ꏊ���ۂ��𔻒f
                if (map[y, x] == 3)
                {
                    // �i�[�ꏊ�̃C���f�b�N�X���T���Ă���
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        // �v�f����goals.Count�Ŏ擾
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // 1�ł������Ȃ�������������B��
                return false;
            }
        }

        // �������B���łȂ���Ώ����B��
        return true;
    }

    // ���݂̃t�B�[���h���N���A���鏈��
    void ClearField()
    {
        // ���݂̃t�B�[���h�ɂ���I�u�W�F�N�g���폜
        foreach (GameObject obj in objectsToRemove)
        {
            //obj.SetActive(false);
            Destroy(obj);
        }
        objectsToRemove.Clear();
        // �t�B�[���h�z������Z�b�g
        //field = new GameObject[map.GetLength(0), map.GetLength(1)];
    }

    // �Q�[�������Z�b�g���鏈��
    void ResetGame()
    {
        // ���݂̃t�B�[���h���N���A
        ClearField();

        // �����}�b�v���R�s�[
        map = (int[,])initialMap.Clone();

        // �t�B�[���h���č\�z
        InitializeMap();
        // �N���A�e�L�X�g���\���ɂ���
        clearText.SetActive(false);
        // �N���A���Ă��Ȃ���Ԃ�
        isCleared = false;
    }

    // �X�e�[�W�̓ǂݍ���
    void LoadStage(int stageIndex)
    {

        // ���݂̃t�B�[���h���N���A
        ClearField();

        // �}�b�v�����[�h
        map = (int[,])maps[stageIndex].Clone();
        initialMap = (int[,])map.Clone();

        // �t�B�[���h������
        field = new GameObject[map.GetLength(0), map.GetLength(1)];
        // �}�b�v������
        InitializeMap();

        // �N���A��Ԃ����Z�b�g
        isCleared = false;
        clearText.SetActive(false);
        currentStageIndex = stageIndex;
    }


    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        // �X�e�[�W�̃}�b�v���`
        maps = new int[][,]
        {
            // �X�e�[�W1
            new int[,] {
                // �}�b�v�f�[�^�������ɏ���
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0 ,0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 2, 3, 0, 0, 4},
            { 4, 0, 1, 0, 0, 2, 3, 0, 0, 4},
            { 4, 0, 0, 0, 0, 2, 3, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            },
            // �X�e�[�W2
            new int[,] {
                // �}�b�v�f�[�^�������ɏ���
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0 ,0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 2, 3, 0, 4},
            { 4, 0, 0, 1, 0, 0, 2, 3, 0, 4},
            { 4, 0, 0, 0, 0, 0, 2, 3, 0, 4},
            { 4, 0, 0, 0, 0, 0, 2, 3, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            },
            // �X�e�[�W3
            new int[,] {
                // �}�b�v�f�[�^�������ɏ���
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 3, 0, 3, 0, 0, 0, 4},
            { 4, 0, 0, 2, 3 ,2, 0, 0, 0, 4},
            { 4, 0, 3, 2, 2, 2, 3, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 1, 0, 0, 0, 0, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            },
        };

        // �ŏ��̃X�e�[�W��ǂݍ���
        LoadStage(currentStageIndex);
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsCleard())
        {

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {

                // �E�ړ�����

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                // ���ړ�����

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                // ��ړ�����

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                // ���ړ�����

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));

            }
        }

        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] != null)
                {
                    objectsToRemove.Add(field[y, x]);
                }
            }
        }

        // �X�e�[�W�؂�ւ�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadStage(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadStage(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadStage(2);
        }

        // �����N���A���Ă�����
        if (IsCleard() && !isCleared)
        {
            // �Q�[�����N���A
            isCleared = true;

            // �Q�[���I�u�W�F�N�g��SetActive���\�b�h���g���L����
            clearText.SetActive(true);

            // SE���Đ��i�����d�Ȃ�Ȃ��悤�ɂ��邽�߁A���łɍĐ����łȂ����Ƃ��m�F�j
            if (!clearSound.isPlaying)
            {
                clearSound.Play();
            }

            // ���X�g���̃I�u�W�F�N�g���폜
            // ���݂̃t�B�[���h���N���Ax
            ClearField();
        }

        // R�L�[�������ꂽ�烊�Z�b�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }
}
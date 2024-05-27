using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerScript : MonoBehaviour
{
    /*/////////////////////////////////////
                プレファブ
    /////////////////////////////////////*/
    // プレイヤー
    public GameObject playerPrefab;
    // ボックス
    public GameObject boxPrefab;
    // ゴール
    public GameObject goalPrefab;
    // パーティクル
    public GameObject particlePrefab;
    // クリア判定
    public GameObject clearText;
    // 壁
    public GameObject wallPrefab;


    // ボックスを移動させる際のSE
    public AudioSource boxMoveSound;
    // クリアした際のSE
    public AudioSource clearSound;

    // ステージ関連
    private int[][,] maps;
    private int currentStageIndex = 0;
    private bool isCleared;

    // 配列の宣言
    int[,] map;
    int[,] initialMap;
    // ゲーム管理用の配列
    GameObject[,] field;

    // クリアした時に消すものリスト
    List<GameObject> objectsToRemove = new List<GameObject>();


    // マップの初期化
    void InitializeMap()
    {
        // 二重for文で二次元配列の情報を出力
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

    // 1の値が格納されているインデックスを取得する処理のメソッド
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
    /// 移動の可否を判断するメソッド
    /// </summary>
    /// <param name="number">動かす数字</param>
    /// <param name="moveFrom">動く先のインデックス</param>
    /// <param name="moveTo">動く前のインデックス</param>
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

                // 移動先にさらにボックスがある場合は移動を中止
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
                return false; // 壁がある場所には移動できない
            }
        }

        // パーティクルを生成する
        GenerateParticles(IndexToPosition(moveFrom));

        Vector3 moveToPosition = IndexToPosition(moveTo);
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        return true;
    }

    // パーティクルの関数
    void GenerateParticles(Vector3 position)
    {
        // パーティクルの数を指定
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

    // クリアする処理
    bool IsCleard()
    {
        // Vector2Int型の可変長配列の作成
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // 格納場所か否かを判断
                if (map[y, x] == 3)
                {
                    // 格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        // 要素数はgoals.Countで取得
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // 1つでも箱がなかったら条件未達成
                return false;
            }
        }

        // 条件未達成でなければ条件達成
        return true;
    }

    // 現在のフィールドをクリアする処理
    void ClearField()
    {
        // 現在のフィールドにあるオブジェクトを削除
        foreach (GameObject obj in objectsToRemove)
        {
            //obj.SetActive(false);
            Destroy(obj);
        }
        objectsToRemove.Clear();
        // フィールド配列をリセット
        //field = new GameObject[map.GetLength(0), map.GetLength(1)];
    }

    // ゲームをリセットする処理
    void ResetGame()
    {
        // 現在のフィールドをクリア
        ClearField();

        // 初期マップをコピー
        map = (int[,])initialMap.Clone();

        // フィールドを再構築
        InitializeMap();
        // クリアテキストを非表示にする
        clearText.SetActive(false);
        // クリアしていない状態に
        isCleared = false;
    }

    // ステージの読み込み
    void LoadStage(int stageIndex)
    {

        // 現在のフィールドをクリア
        ClearField();

        // マップをロード
        map = (int[,])maps[stageIndex].Clone();
        initialMap = (int[,])map.Clone();

        // フィールド初期化
        field = new GameObject[map.GetLength(0), map.GetLength(1)];
        // マップ初期化
        InitializeMap();

        // クリア状態をリセット
        isCleared = false;
        clearText.SetActive(false);
        currentStageIndex = stageIndex;
    }


    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        // ステージのマップを定義
        maps = new int[][,]
        {
            // ステージ1
            new int[,] {
                // マップデータをここに書く
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
            // ステージ2
            new int[,] {
                // マップデータをここに書く
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
            // ステージ3
            new int[,] {
                // マップデータをここに書く
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

        // 最初のステージを読み込む
        LoadStage(currentStageIndex);
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsCleard())
        {

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {

                // 右移動処理

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                // 左移動処理

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                // 上移動処理

                Vector2Int playerIndex = GetPlayerIndex();

                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                // 下移動処理

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

        // ステージ切り替え
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

        // もしクリアしていたら
        if (IsCleard() && !isCleared)
        {
            // ゲームをクリア
            isCleared = true;

            // ゲームオブジェクトのSetActiveメソッドを使い有効化
            clearText.SetActive(true);

            // SEを再生（音が重ならないようにするため、すでに再生中でないことを確認）
            if (!clearSound.isPlaying)
            {
                clearSound.Play();
            }

            // リスト内のオブジェクトを削除
            // 現在のフィールドをクリアx
            ClearField();
        }

        // Rキーが押されたらリセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }
}
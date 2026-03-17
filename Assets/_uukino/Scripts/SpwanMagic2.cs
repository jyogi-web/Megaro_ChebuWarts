using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer; // ★ PDollarの機能を使うための宣言
using System.IO;

public class SpwanMagic2 : MonoBehaviour
{
    [SerializeField] GameObject[] magics;
    [SerializeField] GameObject worldControllerPos; 
    [SerializeField] LineRenderer circle;
    
    // ▼ 追加：ジェスチャー判定用の変数
    public TextAsset[] gestureTemplates; // インスペクターから登録する正解図形データ(XML)
    public float minScore = 0.8f;        // 魔法が発動する最低一致率 (80%)
    private List<Gesture> trainingSet = new List<Gesture>(); // 読み込んだ図形データを格納するリスト

    List<Vector3> points = new List<Vector3>();
    Vector3 lastPos;
    bool hasLastPos = false;
    Vector3 center;
    public int magicNumber;

    [SerializeField] InputController2 inputController;

    void Start()
    {

        // ★ 追加：ゲーム開始時に、登録した図形のテンプレート(XML)を読み込む
        if (gestureTemplates != null)
        {
            foreach (TextAsset xml in gestureTemplates)
            {
                trainingSet.Add(GestureIO.ReadGestureFromXML(xml.text));
            }
        }
    }

    void Update()
    {
        // 軌跡の記録と描画（元のコードのまま）
        if (inputController.rtrigger) {
            // ★変更：コントローラーのワールド座標を直接取得する
            Vector3 pos = worldControllerPos.transform.position;
            if (!hasLastPos || Vector3.Distance(lastPos, pos) > 0.02f) {
                points.Add(pos);
                lastPos = pos;
                hasLastPos = true;
            }
        }

        circle.positionCount = points.Count;
        circle.SetPositions(points.ToArray());
        
        center = Vector3.zero;
        foreach (var p in points) {
            center += p;
        }
        if (points.Count > 0) center /= points.Count;

        // ★ 変更：トリガーを離した瞬間の処理
        if (!inputController.rtrigger && points.Count > 0 && hasLastPos) {
            
            RecognizeAndSpawnMagic(); // 判定と魔法生成を実行

            hasLastPos = false;
            Invoke("DeleteMagicCircle", 5f);
        }   
    }

    // ★ 追加：軌跡を判定して、結果に応じた魔法を出すメソッド
    void RecognizeAndSpawnMagic()
    {
        // 誤作動防止：描いた点が少なすぎる（ただのトリガーのクリック等）場合は無視
        if (points.Count < 10) return;

        // 1. 3D座標の軌跡を、PDollar用の2Dデータ(Point配列)に変換する
        Point[] pointArray = new Point[points.Count];
        Transform cam = Camera.main.transform; // プレイヤーの視点を基準にする
        
        for (int i = 0; i < points.Count; i++)
        {
            // 空間に描いた3D座標を、プレイヤーのカメラから見た相対的な平面(2D)座標に変換
            Vector3 localPoint = cam.InverseTransformPoint(points[i]);
            pointArray[i] = new Point(localPoint.x, localPoint.y, 0); // 最後の0はストロークID(一筆書き)
        }

        // 2. アルゴリズムで図形判定を実行
        Gesture candidate = new Gesture(pointArray);
        Result result = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

        Debug.Log("判定結果: " + result.GestureClass + " / スコア: " + result.Score);

        // 3. 判定スコアが基準値以上なら、種類に応じて魔法を切り替えて生成
        if (result.Score >= minScore)
        {
            // XMLのファイル名（ジェスチャー名）に応じて magicNumber を変更
            if (result.GestureClass == "Circle") {
                magicNumber = 0; // 丸なら magics[0]
            } else if (result.GestureClass == "Star") {
                magicNumber = 1; // 星なら magics[1]
            } else if (result.GestureClass == "Triangle") {
                magicNumber = 2; // 三角なら magics[2]
            }

            // 魔法オブジェクトを生成
            if (magicNumber < magics.Length) 
            {
                GameObject magic = Instantiate(magics[magicNumber], center, Quaternion.identity);
                Destroy(magic, 5f);
            }
        }
        else
        {
            Debug.Log("判定失敗：形が崩れています");
        }
    }

    void DeleteMagicCircle() {
        points.Clear();
        circle.positionCount = 0;
    }
}
---
name: unicli
description: このスキルは、ユーザーが「unicliを使って」「Unityをターミナルから操作」「コンパイルして」「テストを実行」「GameObjectを検索」「シーンを開く」「パッケージを追加」「unicli exec」「unicli eval」などを要求したときに使用する。UniCliを使ってUnity Editorをターミナル・AIエージェントから制御するための操作ガイド。
version: 0.1.0
---

# UniCli 操作スキル

UniCliはターミナルからUnity Editorを制御するCLIツール。AIエージェントと人間の両方がUnityを操作できるように設計されている。

## 基本コマンド構成

```
unicli <サブコマンド> [オプション]
```

| サブコマンド | 用途 |
|---|---|
| `check` | インストール確認・エディタ接続チェック |
| `install` | UniCli Unityパッケージをプロジェクトにインストール |
| `exec <Command>` | Unity Editor上でコマンド実行 |
| `eval '<C#コード>'` | 任意のC#コードを動的実行 |
| `commands` | 利用可能なコマンド一覧表示 |
| `status` | 接続状態とプロジェクト情報表示 |

## 接続と確認

```bash
# 接続確認（最初に必ず実行）
unicli check

# 利用可能なコマンド一覧
unicli commands --json
```

Unity Editorが開いた状態で、Unityプロジェクトのディレクトリ（またはサブディレクトリ）から実行する。`Assets`フォルダを持つディレクトリを自動検出する。

## exec コマンド

`unicli exec <Command>` でUnity Editor上のコマンドを実行する。

### パラメータ渡し方

```bash
# --key value 形式（推奨）
unicli exec GameObject.Find --name "Main Camera"

# JSON文字列形式
unicli exec GameObject.Find '{"name":"Main Camera"}'

# boolean フラグ（値なしで渡す）
unicli exec GameObject.Find --includeInactive

# 配列パラメータ（同じフラグを繰り返す）
unicli exec BuildPlayer.Build --options Development --options ConnectWithProfiler
```

### 共通オプション

| オプション | 説明 |
|---|---|
| `--json` | JSON形式で出力 |
| `--timeout <ms>` | タイムアウト設定（ミリ秒） |
| `--no-focus` | Unity Editorをフォアグラウンドにしない |
| `--help` | コマンドのパラメータ詳細表示 |

## よく使うコマンド例

### コンパイル・ビルド

```bash
# スクリプトのコンパイル
unicli exec Compile --json
unicli exec Compile --timeout 30000

# プレイヤービルド
unicli exec BuildPlayer.Build --locationPathName "Builds/Game.app"
unicli exec BuildPlayer.Build --locationPathName "Builds/Game.apk" --target Android --options Development
```

### テスト実行

```bash
# EditModeテスト
unicli exec TestRunner.RunEditMode --json
unicli exec TestRunner.RunEditMode --testNameFilter MyTest

# PlayModeテスト
unicli exec TestRunner.RunPlayMode --json
unicli exec TestRunner.RunPlayMode --testNameFilter MyTest --stackTraceLines 3
```

### シーン操作

```bash
# シーンを開く
unicli exec Scene.Open --path "Assets/Scenes/Level1.unity"

# 全シーン保存
unicli exec Scene.Save --all

# アクティブシーン情報
unicli exec Scene.GetActive --json
```

### GameObject操作

```bash
# GameObjectを検索
unicli exec GameObject.Find --name "Main Camera" --json
unicli exec GameObject.GetHierarchy --json

# コンポーネント取得
unicli exec GameObject.GetComponents --instanceId 1234 --json

# GameObject作成・変形
unicli exec GameObject.Create --name "Enemy"
unicli exec GameObject.SetTransform --path "Enemy" --position 1,2,3 --rotation 0,90,0

# コンポーネント追加
unicli exec GameObject.AddComponent --path "Enemy" --typeName BoxCollider
```

### パッケージ管理

```bash
# インストール済みパッケージ一覧
unicli exec PackageManager.List --json

# パッケージ追加
unicli exec PackageManager.Add --packageId "com.unity.cinemachine"

# パッケージ削除
unicli exec PackageManager.Remove --packageId "com.unity.cinemachine"
```

### コンソールログ

```bash
# エラーと警告のログ取得
unicli exec Console.GetLog --logType "Warning,Error" --json

# 全ログ取得
unicli exec Console.GetLog --json
```

## eval コマンド（C#動的実行）

Unity APIに完全アクセスできる任意のC#コードを実行する。

```bash
# 基本的な使い方
unicli eval 'return Application.unityVersion;' --json

# 複数行コード（ヒアドキュメント使用）
unicli eval "$(cat <<'EOF'
var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
var objects = GameObject.FindObjectsOfType<GameObject>(true);
return $"{scene.name}: {objects.Length} objects";
EOF
)" --json

# カスタム型定義を使った構造化レスポンス
unicli eval "$(cat <<'EOF'
var stats = new SceneStats();
stats.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
stats.objectCount = GameObject.FindObjectsOfType<GameObject>().Length;
return stats;
EOF
)" --declarations "$(cat <<'EOF'
[System.Serializable]
public class SceneStats
{
    public string sceneName;
    public int objectCount;
}
EOF
)" --json

# PlayerSettings変更（exec経由では変更不可のため evalを使う）
unicli eval 'PlayerSettings.companyName = "MyCompany";' --json
```

### eval オプション

| オプション | 説明 |
|---|---|
| `--json` | JSON形式で出力 |
| `--declarations '<C#型定義>'` | 追加の型定義（クラス・構造体・enum） |
| `--timeout <ms>` | タイムアウト設定 |

`[Serializable]`な型を返す場合は`JsonUtility`でシリアライズされる。

## プロジェクト検出

`Assets`フォルダを持つ親ディレクトリを自動検出。プロジェクト外から実行する場合は環境変数を使用：

```bash
UNICLI_PROJECT=/path/to/unity-project unicli exec Compile --json
```

## エラー時の対処

1. **接続エラー** → `unicli check`でUnity Editorが開いているか確認
2. **コンパイルエラー** → `unicli exec Compile --json`でエラー詳細を確認
3. **コマンド不明** → `unicli commands --json`で利用可能なコマンドを確認
4. **タイムアウト** → `--timeout 60000`でタイムアウトを延長

## 追加リソース

詳細なコマンドリファレンスは以下を参照：
- **`references/commands.md`** - 全80+コマンドの詳細一覧
- **`references/custom-commands.md`** - カスタムコマンドの作成方法
- **`examples/eval-examples.sh`** - evalコマンドの実用例
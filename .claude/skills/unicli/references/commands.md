# UniCli 全コマンドリファレンス

## コンパイル・ビルド

| コマンド | 説明 |
|---|---|
| `Compile` | スクリプトをコンパイル |
| `BuildPlayer.Build` | プレイヤーをビルド |
| `BuildPlayer.Compile` | プレイヤー向けにコンパイル（ビルドなし） |

### BuildPlayer.Build パラメータ

```bash
unicli exec BuildPlayer.Build \
  --locationPathName "Builds/Game.app" \
  --target iOS \
  --options Development \
  --options ConnectWithProfiler
```

## テストランナー

| コマンド | 説明 |
|---|---|
| `TestRunner.RunEditMode` | EditModeテスト実行 |
| `TestRunner.RunPlayMode` | PlayModeテスト実行 |
| `TestRunner.List` | テスト一覧取得 |

```bash
unicli exec TestRunner.RunEditMode --testNameFilter MyTest --stackTraceLines 3 --resultFilter all
```

## シーン

| コマンド | 説明 |
|---|---|
| `Scene.Open` | シーンを開く |
| `Scene.Save` | シーンを保存 |
| `Scene.GetActive` | アクティブシーン情報取得 |
| `Scene.GetLoaded` | ロード済みシーン一覧 |
| `Scene.Create` | 新規シーン作成 |
| `Scene.Close` | シーンを閉じる |

## GameObject・コンポーネント

| コマンド | 説明 |
|---|---|
| `GameObject.Find` | 名前でGameObject検索 |
| `GameObject.GetHierarchy` | シーン階層取得 |
| `GameObject.GetComponents` | コンポーネント一覧取得 |
| `GameObject.Create` | GameObject作成 |
| `GameObject.Delete` | GameObject削除 |
| `GameObject.SetActive` | アクティブ状態変更 |
| `GameObject.SetTransform` | Transform設定 |
| `GameObject.AddComponent` | コンポーネント追加 |
| `GameObject.RemoveComponent` | コンポーネント削除 |
| `Component.SetProperty` | コンポーネントプロパティ設定 |
| `Component.GetProperty` | コンポーネントプロパティ取得 |

```bash
# instanceIdで検索してコンポーネント取得
unicli exec GameObject.GetComponents --instanceId 1234 --json

# パスでTransform設定
unicli exec GameObject.SetTransform --path "Player/Weapon" --position 0,1,0

# コンポーネントプロパティ変更
unicli exec Component.SetProperty \
  --componentInstanceId 1234 \
  --propertyPath "m_IsKinematic" \
  --value "true"
```

## アセット管理

| コマンド | 説明 |
|---|---|
| `AssetDatabase.Find` | アセット検索 |
| `AssetDatabase.Refresh` | アセットデータベース更新 |
| `AssetDatabase.Import` | アセットインポート |
| `Prefab.Instantiate` | プレハブをインスタンス化 |
| `Prefab.Apply` | プレハブ変更を適用 |
| `Material.SetProperty` | マテリアルプロパティ設定 |

```bash
# アセット検索
unicli exec AssetDatabase.Find --filter "t:Texture2D" --json
unicli exec AssetDatabase.Find --filter "l:MyLabel" --json

# アセットデータベース更新
unicli exec AssetDatabase.Refresh --json
```

## パッケージマネージャー

| コマンド | 説明 |
|---|---|
| `PackageManager.List` | インストール済みパッケージ一覧 |
| `PackageManager.Add` | パッケージ追加 |
| `PackageManager.Remove` | パッケージ削除 |
| `PackageManager.Search` | パッケージ検索 |
| `PackageManager.Update` | パッケージ更新 |

```bash
unicli exec PackageManager.Add --packageId "com.unity.cinemachine@2.9.0"
unicli exec PackageManager.Search --query "cinemachine" --json
```

## プレイモード

| コマンド | 説明 |
|---|---|
| `PlayMode.Enter` | プレイモード開始 |
| `PlayMode.Exit` | プレイモード終了 |
| `PlayMode.GetState` | プレイモード状態取得 |

## メニュー実行

```bash
# Unity Editorのメニュー項目を実行
unicli exec Menu.Execute --menuItem "Assets/Reimport All"
unicli exec Menu.Execute --menuItem "Edit/Clear All PlayerPrefs"
```

## コンソール

| コマンド | 説明 |
|---|---|
| `Console.GetLog` | コンソールログ取得 |
| `Console.Clear` | コンソールクリア |

```bash
# ログタイプでフィルタリング
unicli exec Console.GetLog --logType "Error" --json
unicli exec Console.GetLog --logType "Warning,Error" --count 50 --json
```

## 設定Inspect

| コマンド | 説明 |
|---|---|
| `PlayerSettings.Inspect` | PlayerSettings取得 |
| `EditorSettings.Inspect` | EditorSettings取得 |
| `QualitySettings.Inspect` | QualitySettings取得 |
| `PhysicsSettings.Inspect` | PhysicsSettings取得 |
| `GraphicsSettings.Inspect` | GraphicsSettings取得 |

設定の**変更**は `unicli eval` を使う（Inspectコマンドは読み取り専用）:
```bash
unicli eval 'PlayerSettings.bundleVersion = "1.2.3";'
```

## アニメーション

| コマンド | 説明 |
|---|---|
| `Animation.GetClips` | AnimationClip一覧取得 |
| `Animation.Inspect` | AnimatorController詳細取得 |

## プロファイラー

| コマンド | 説明 |
|---|---|
| `Profiler.Inspect` | プロファイラー状態・メモリ統計 |
| `Profiler.StartRecording` | プロファイル記録開始 |
| `Profiler.StopRecording` | プロファイル記録停止 |
| `Profiler.SaveProfile` | プロファイルデータ保存 |
| `Profiler.GetFrameData` | 特定フレームのCPUサンプルデータ取得 |
| `Profiler.FindSpikes` | スパイクフレーム検出 |
| `Profiler.TakeSnapshot` | メモリスナップショット取得 |

## スクリーンショット・録画

| コマンド | 説明 |
|---|---|
| `Screenshot.Capture` | ゲームビューをPNGキャプチャ（Play Mode必要） |
| `Recorder.StartRecording` | 動画録画開始（Play Mode必要） |
| `Recorder.StopRecording` | 録画停止 |
| `Recorder.Status` | 録画状態取得 |

## モジュール管理

| コマンド | 説明 |
|---|---|
| `Module.List` | モジュール一覧と有効状態 |
| `Module.Enable` | モジュール有効化 |
| `Module.Disable` | モジュール無効化 |

利用可能なモジュール: `Scene`, `GameObject`, `Assets`, `Profiler`, `Animation`, `Remote`, `Recorder`, `Search`, `NuGet`, `BuildMagic`

## リモートデバッグ（デバイス）

開発ビルドに接続してコマンド実行。`UNICLI_REMOTE`スクリプティングシンボル設定とDevelopment Buildが必要。

| コマンド | 説明 |
|---|---|
| `Remote.List` | 接続デバイスのデバッグコマンド一覧 |
| `Remote.Invoke` | デバイスのデバッグコマンド実行 |

```bash
unicli exec Remote.List --json
unicli exec Remote.Invoke '{"command":"Debug.Stats"}'
unicli exec Remote.Invoke '{"command":"Debug.GetLogs","data":"{\"limit\":20}"}'
```

### 組み込みデバッグコマンド

| コマンド | 説明 |
|---|---|
| `Debug.SystemInfo` | デバイス・OS・CPU・GPU・メモリ情報 |
| `Debug.Stats` | FPS・フレームタイム・メモリ使用量 |
| `Debug.GetLogs` | ログバッファ取得 |
| `Debug.GetHierarchy` | シーン階層取得 |
| `Debug.FindGameObjects` | GameObject検索 |
| `Debug.GetScenes` | ロード済みシーン一覧 |
| `Debug.GetPlayerPref` | PlayerPrefs値取得 |

## オプションパッケージ対応コマンド

### Search（Unity Search API）
```bash
unicli exec Search --query "t:Prefab Player" --json
```

### NuGet（NuGetForUnity必要）
```bash
unicli exec NuGet.List --json
unicli exec NuGet.Install --packageId "Newtonsoft.Json" --version "13.0.3"
```

### BuildMagic（jp.co.cyberagent.buildmagic必要）
```bash
unicli exec BuildMagic.List --json
unicli exec BuildMagic.Apply --scheme "Release"
```
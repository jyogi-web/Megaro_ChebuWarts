#!/bin/bash
# UniCli eval コマンド実用例

# Unityバージョン確認
unicli eval 'return Application.unityVersion;' --json

# アクティブシーンのオブジェクト数
unicli eval "$(cat <<'EOF'
var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
var objects = GameObject.FindObjectsOfType<GameObject>(true);
return $"{scene.name}: {objects.Length} objects";
EOF
)" --json

# 構造化データ返却（SceneStats型定義）
unicli eval "$(cat <<'EOF'
var stats = new SceneStats();
stats.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
stats.objectCount = GameObject.FindObjectsOfType<GameObject>().Length;
stats.unityVersion = Application.unityVersion;
return stats;
EOF
)" --declarations "$(cat <<'EOF'
[System.Serializable]
public class SceneStats
{
    public string sceneName;
    public int objectCount;
    public string unityVersion;
}
EOF
)" --json

# PlayerSettings変更（exec経由では変更できないためevalを使う）
unicli eval 'PlayerSettings.companyName = "MyCompany";'
unicli eval 'PlayerSettings.bundleVersion = "1.0.0";'
unicli eval 'PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);'

# EditorPrefs操作
unicli eval 'EditorPrefs.SetBool("MyKey", true);'
unicli eval 'return EditorPrefs.GetBool("MyKey");' --json

# アセット検索と処理
unicli eval "$(cat <<'EOF'
var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Textures" });
return $"Found {guids.Length} textures";
EOF
)" --json

# Scriptable Object値変更
unicli eval "$(cat <<'EOF'
var path = "Assets/Data/GameConfig.asset";
var config = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
var serialized = new SerializedObject(config);
var prop = serialized.FindProperty("maxHealth");
if (prop != null)
{
    prop.intValue = 100;
    serialized.ApplyModifiedProperties();
    AssetDatabase.SaveAssets();
    return "maxHealth updated";
}
return "property not found";
EOF
)" --json

# タイムアウト付き（長時間処理用）
unicli eval 'await System.Threading.Tasks.Task.Delay(5000, cancellationToken); return "done";' \
  --timeout 10000 --json
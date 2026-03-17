# UniCli カスタムコマンド作成ガイド

## 概要

C#クラス1つでカスタムコマンドを追加できる。コマンドはリフレクションで自動検出される。

## 基本構造

```csharp
using System;
using UniCli;

public sealed class MyCommand : UniCliCommand<MyCommand.Request, MyCommand.Response>
{
    public override string CommandName => "MyCommand.DoSomething";
    public override string Description => "コマンドの説明";

    protected override Response Execute(Request request)
    {
        // Unityの処理
        return new Response { result = "success" };
    }

    [Serializable]
    public class Request
    {
        public string targetName;
        public int count;
    }

    [Serializable]
    public class Response
    {
        public string result;
        public int processedCount;
    }
}
```

**重要**: Request/Responseクラスは`[Serializable]`属性と**パブリックフィールド**が必要（`JsonUtility`要件）。

## テキストフォーマット

`UniCliTextWriter`で書式付き出力が可能：

```csharp
protected override Response Execute(Request request)
{
    Writer.Header("処理結果");
    Writer.Table(new[]
    {
        new[] { "名前", "値" },
        new[] { "Status", "OK" },
        new[] { "Count", "42" },
    });
    Writer.Divider();

    return new Response { success = true };
}
```

## 非同期コマンド

```csharp
using System.Threading;
using System.Threading.Tasks;

public sealed class AsyncCommand : UniCliCommand<AsyncCommand.Request, AsyncCommand.Response>
{
    public override string CommandName => "MyCommand.AsyncOp";

    protected override async Task<Response> ExecuteAsync(
        Request request,
        CancellationToken cancellationToken)
    {
        // クライアント切断でキャンセルされる
        await Task.Delay(1000, cancellationToken);
        return new Response { done = true };
    }

    [Serializable] public class Request { }
    [Serializable] public class Response { public bool done; }
}
```

## エラーハンドリング

```csharp
protected override Response Execute(Request request)
{
    if (string.IsNullOrEmpty(request.targetName))
    {
        throw new UniCliCommandException("targetName は必須です");
    }

    var obj = GameObject.Find(request.targetName);
    if (obj == null)
    {
        throw new UniCliCommandException($"GameObject '{request.targetName}' が見つかりません");
    }

    return new Response { instanceId = obj.GetInstanceID() };
}
```

## リモートデバッグコマンド（デバイス側）

```csharp
using UniCli.Remote;

public sealed class ToggleHitboxesCommand : DebugCommand<ToggleHitboxesCommand.Request, ToggleHitboxesCommand.Response>
{
    public override string CommandName => "Debug.ToggleHitboxes";
    public override string Description => "ヒットボックス表示切替";

    protected override Response ExecuteCommand(Request request)
    {
        HitboxVisualizer.Enabled = request.enabled;
        return new Response { enabled = HitboxVisualizer.Enabled };
    }

    [Serializable] public class Request { public bool enabled; }
    [Serializable] public class Response { public bool enabled; }
}
```

入出力不要の場合は `Unit` を使用：

```csharp
public sealed class ResetStateCommand : DebugCommand<Unit, Unit>
{
    public override string CommandName => "Debug.ResetState";
    protected override Unit ExecuteCommand(Unit request)
    {
        GameManager.ResetAll();
        return Unit.Value;
    }
}
```

## コマンド確認

作成後、利用可能なコマンド一覧で確認：
```bash
unicli commands --json | grep "MyCommand"
```
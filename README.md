# ChebuWarts (チェブウォーツ)

> VR × スマートフォンの非対称マルチプレイヤー魔法バトルゲーム

[![itch.io](https://img.shields.io/badge/itch.io-Play%20Now-fa5c5c?style=for-the-badge&logo=itchdotio)](https://kou050223.itch.io/chebu-warts)

## プロダクト概要

ChebuWarts は、**VRプレイヤー**と**スマートフォンプレイヤー**が非対称な役割で対戦するマルチプレイヤーゲームです。

- **VRプレイヤー** : Meta Quest などの VR デバイスを装着し、五大属性（地・水・火・風・空）の魔法を詠唱してモンスターを倒す
- **スマートフォンプレイヤー** : スマートフォンのミニマップ UI を操作し、VR プレイヤーの周囲にモンスターを召喚して妨害する

制限時間内に VR 側が規定数のモンスターを倒せばVR側の勝利、タイムアップになればスマートフォン側の勝利となります。

**関連リンク:**
- [解説ページ (Topaz)](https://topaz.dev/projects/9ae51df3f2987dd2d0b5)
- [配布ページ (itch.io)](https://kou050223.itch.io/chebu-warts)
- [設計ドキュメント (Figma)](https://www.figma.com/board/tXrwssYSDdPiM16LxjsQ8G/)

---

## ゲームの流れ

```
タイトル画面 (VR)
  └─ 両手コントローラーを広げると魔法陣が召喚される
  └─ 魔法陣中央の星を掴むとゲーム開始

ゲームシーン
  ├─ VR側: 魔法を選択・発射してモンスターを倒す
  │       10体倒す → VR側の勝利
  └─ スマホ側: ミニマップからモンスターを召喚
              制限時間 (120秒) タイムアップ → スマホ側の勝利
```

---

## 技術スタック

| カテゴリ | 技術 |
|---|---|
| エンジン | Unity (URP) |
| VR SDK | Meta XR SDK / XR Interaction Toolkit |
| ネットワーク | Unity Netcode for GameObjects |
| マッチメイキング | Unity Gaming Services (Lobby / Relay) |
| プラットフォーム | Android (Meta Quest) / Android スマートフォン |
| CI/CD | butler (itch.io) / unicli |

---

## アーキテクチャ

### ネットワーク構成

```
Host (VRプレイヤー)
  ├─ GameFlowController  ... タイマー・KillCount・勝敗判定を管理 (Server Authority)
  ├─ MonsterSpawner      ... Client からのスポーン要求を受け付け、レート制限付きで処理
  └─ NetworkMonsterBase  ... 各モンスターの HP・死亡をネットワーク同期

Client (スマホプレイヤー)
  └─ ミニマップ UI → RequestSpawnMonsterServerRpc → Host 側でモンスターをスポーン
```

### 魔法システム

五大属性 (`MagicElement`: Earth / Water / Fire / Wind / Sky) を定義し、属性ごとにエフェクト・サウンド・ダメージを `SpellEffectCatalog` で管理。

```
SpellEffectManager (オブジェクトプール)
  ├─ 属性ごとに GameObject をプールし、最大 5 個まで同時発火
  ├─ 上限超過時は最古のエフェクトを強制回収
  └─ SpellProjectile → MonsterBase.TakeDamage() → OnDeath() → GameFlowController.OnMonsterDefeated()
```

### タイトル演出 (VR)

```
MagicCircleController
  ├─ 両手コントローラーの移動距離を検出
  ├─ 閾値超過で魔法陣をアニメーション召喚 (EaseOut)
  ├─ ガイダンスパーティクルをプレイヤー→魔法陣中央に配置
  └─ 中央の星をグリップ → SceneManager.LoadScene("GameScene")
```

---

## ディレクトリ構成

```
Assets/
├─ _chebuo/          # chebuo 担当: 入力制御・魔法発射・敵システム・GameManager
├─ _misamisa/        # misamisa 担当: タイトル演出・魔法陣・スクロール・スターフィールド
├─ _uukino/          # uukino 担当: 入力制御V2・魔法コントローラー・落下判定・PDollarジェスチャー認識
├─ _kou/             # kou 担当: エフェクト・デプロイ (deploy.sh)
└─ _Shared/          # 共有コード
    ├─ Magic/        # 魔法属性定義・SpellEffect管理
    ├─ Monster/      # モンスター基底クラス
    └─ Multiplayer/  # ネットワーク (Lobby / Relay / GameFlow / MonsterSpawner)
```

---

## 開発者と役割分担

| メンバー | 主な担当領域 |
|---|---|
| **chebuo** | VR 入力制御、魔法発射システム、敵 AI、GameManager |
| **misamisa** | タイトルシーン演出、魔法陣召喚 UI、スターフィールド、スクロール |
| **uukino** | VR 入力制御 (V2)、PDollar ジェスチャー認識、落下判定 |
| **kou** | エフェクト実装、itch.io デプロイ基盤 (`deploy.sh`) |

---

## ビルド & デプロイ

### 必要ツール

- [Unity Hub](https://unity.com/ja/unity-hub)
- [unicli](https://github.com/kou050223/unicli) (Unity CLI ラッパー)
- [butler](https://itch.io/docs/butler/) (itch.io 配布ツール)

### ビルド → デプロイ

```bash
# Unity ビルド + itch.io デプロイ
./deploy.sh --build

# 既存 APK のみデプロイ
./deploy.sh
```

配布先: `https://kou050223.itch.io/chebu-warts` (Android APK)

---

## ライセンス

本リポジトリのコードはチーム内利用を目的としています。外部アセットについてはそれぞれのライセンスに従ってください。

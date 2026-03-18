#!/bin/bash
set -e

# itch.io デプロイ設定
ITCH_USER="kou050223"
ITCH_PROJECT="chebu-warts"
ITCH_CHANNEL="android"
BUILD_DIR="build"
APK_PATH="$BUILD_DIR/Android.apk"

echo "=== ChebuWarts itch.io デプロイ ==="
echo "対象: https://$ITCH_USER.itch.io/$ITCH_PROJECT"
echo "チャネル: $ITCH_CHANNEL"
echo ""

# butler コマンドの確認
if ! command -v butler &> /dev/null; then
    echo "エラー: butler が見つかりません"
    echo "インストール: https://itch.io/docs/butler/"
    exit 1
fi

# APK の存在確認
if [ ! -f "$APK_PATH" ]; then
    echo "エラー: APK が見つかりません: $APK_PATH"
    echo "先に Unity でビルドを実行してください"
    exit 1
fi

echo "APK: $APK_PATH ($(du -sh "$APK_PATH" | cut -f1))"
echo ""

# butler push
echo "butler push 開始..."
butler push "$APK_PATH" "$ITCH_USER/$ITCH_PROJECT:$ITCH_CHANNEL"

echo ""
echo "デプロイ完了!"
echo "確認: https://$ITCH_USER.itch.io/$ITCH_PROJECT"

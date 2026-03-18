#!/bin/bash
set -e

# itch.io デプロイ設定
ITCH_USER="kou050223"
ITCH_PROJECT="chebu-warts"
ITCH_CHANNEL="android"
BUILD_DIR="build"
APK_PATH="$BUILD_DIR/Android.apk"
DO_BUILD=false

# オプション解析
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --build|-b) DO_BUILD=true ;;
        *) echo "不明なオプション: $1"; echo "使い方: ./deploy.sh [--build|-b]"; exit 1 ;;
    esac
    shift
done

echo "=== ChebuWarts itch.io デプロイ ==="
echo "対象: https://$ITCH_USER.itch.io/$ITCH_PROJECT"
echo "チャネル: $ITCH_CHANNEL"
echo ""

# Unity ビルド（--build オプション指定時のみ）
if [ "$DO_BUILD" = true ]; then
    if ! command -v unicli &> /dev/null; then
        echo "エラー: unicli が見つかりません"
        exit 1
    fi
    echo "Unity ビルド開始..."
    unicli exec BuildPlayer.Build --locationPathName "$APK_PATH"
    echo "Unity ビルド完了"
    echo ""
fi

# butler コマンドの確認
if ! command -v butler &> /dev/null; then
    echo "エラー: butler が見つかりません"
    echo "インストール: https://itch.io/docs/butler/"
    exit 1
fi

# APK の存在確認
if [ ! -f "$APK_PATH" ]; then
    echo "エラー: APK が見つかりません: $APK_PATH"
    echo "ビルドするには --build オプションを使用してください"
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

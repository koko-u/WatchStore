#!/bin/sh

# pgAdmin の servers.json テンプレートファイル
TEMPLATE_FILE="/pgadmin4/servers.json.template" # コンテナ内のパス
OUTPUT_DIR="/var/lib/pgadmin/servers"          # 永続ボリューム内の出力ディレクトリ
OUTPUT_FILE="$OUTPUT_DIR/servers.json"         # 生成される servers.json の最終パス

# 環境変数をチェック
if [ -z "$DATABASE__NAME" ]; then
  echo "Error: DATABASE__NAME environment variable is not set."
  exit 1
fi
if [ -z "$DATABASE__USER" ]; then
  echo "Error: DATABASE__USER environment variable is not set."
  exit 1
fi

# 出力ディレクトリが存在しない場合は作成
mkdir -p "$OUTPUT_DIR"

echo "Generating servers.json from template..."

# sed を使ってテンプレートのプレースホルダーを環境変数で置換
sed -e "s/__DATABASE_NAME__/${DATABASE__NAME}/g" \
    -e "s/__DATABASE_USER__/${DATABASE__USER}/g" \
    "$TEMPLATE_FILE" > "$OUTPUT_FILE"

echo "servers.json generated successfully at $OUTPUT_FILE."

# pgAdmin のオリジナルのエントリーポイントを実行
exec /entrypoint.sh "$@"

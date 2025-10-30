#!/bin/bash

echo "🔄 Parando containers existentes..."
docker compose down

echo "🗑️  Removendo imagens antigas..."
docker rmi bookstore-frontend bookstore-backend 2>/dev/null || true

echo "🏗️  Reconstruindo imagens..."
docker compose build --no-cache

echo "🚀 Iniciando containers..."
docker compose up -d

echo ""
echo "✅ Containers reconstruídos e iniciados!"
echo ""
echo "📋 Status dos containers:"
docker compose ps

echo ""
echo "🌐 URLs disponíveis:"
echo "   Frontend: http://localhost:3000"
echo "   Backend:  http://localhost:8080/swagger"
echo "   Health:   http://localhost:3000/health (após fazer login)"

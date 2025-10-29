# 🪵 Sistema de Logging Estruturado

Este projeto implementa **logging estruturado profissional** usando **Serilog** para rastreamento completo de operações e troubleshooting.

## 📋 O que é Logged

### ✅ **Eventos de Aplicação**
- 🚀 **Startup**: Inicialização da API
- ✅ **Sucesso**: Operações bem-sucedidas
- ⚠️ **Warnings**: Validações e regras de negócio
- ❌ **Erros**: Exceções e falhas
- 🛑 **Shutdown**: Encerramento da aplicação

### 📘 **Criação de Livros**
Quando um livro é criado, são logados:
- Título, editora e ano
- Quantidade de autores, assuntos e preços
- Usuário que criou
- Timestamp da operação
- Erros de validação (se houver)

**Exemplo de log:**
```
[10:15:32 INF] 📘 Iniciando criação de novo livro | Título: "Clean Code" | Editora: "Pearson" | Ano: "2008"
[10:15:32 INF] ✅ Livro criado com sucesso! | ID: 42 | Título: "Clean Code" | Autores: 1 | Assuntos: 2 | Preços: 3 | Usuario: "admin"
```

### 📄 **Geração de Relatórios PDF**
Para cada relatório gerado:
- Número de autores e livros no relatório
- Tamanho do arquivo em KB
- Tempo de geração em ms

**Exemplo de log:**
```
[10:20:15 INF] 📄 Iniciando geração de relatório PDF de Livros por Autor...
[10:20:16 INF] ✅ Relatório PDF gerado com sucesso! | Autores: 15 | Livros: 42 | Tamanho: 127.53 KB | Tempo: 1234.56 ms
```

### 🌐 **Requisições HTTP**
Todas as requisições HTTP são logadas com:
- Método (GET, POST, PUT, DELETE)
- Path (/api/livros, /api/relatorios, etc.)
- Status code (200, 201, 404, 500, etc.)
- Tempo de resposta em ms
- IP do cliente
- User-Agent

**Exemplo de log:**
```
[10:25:00 INF] HTTP POST /api/livros responded 201 in 156.7890 ms
```

### ❌ **Tratamento de Erros**
Todos os erros são logados com contexto completo:
- Tipo de exceção
- Mensagem de erro
- Stack trace
- Path da requisição
- Método HTTP
- IP do cliente
- User-Agent

**Exemplo de log:**
```
[10:30:00 ERR] ❌ Exceção não tratada capturada | Tipo: "NotFoundException" | Mensagem: "Livro com ID 999 não encontrado" | Path: "/api/livros/999" | Method: "GET" | IP: "192.168.1.100" | UserAgent: "Mozilla/5.0..."
```

## 📁 Onde os Logs são Salvos

### 1. **Console** (Terminal)
Logs aparecem em tempo real no terminal onde a aplicação está rodando.

**Formato:** `[HH:mm:ss LEVEL] Mensagem {Properties}`

### 2. **Arquivos** (`backend/logs/`)
Logs são salvos em arquivos rotativos:
- **Nome**: `bookstore-YYYY-MM-DD.log`
- **Rotação**: Diária (um arquivo por dia)
- **Retenção**: 30 dias (arquivos mais antigos são deletados automaticamente)
- **Formato**: Timestamp completo + Level + Context + Mensagem

**Localização:**
```
backend/
└── logs/
    ├── bookstore-2025-10-29.log
    ├── bookstore-2025-10-28.log
    └── bookstore-2025-10-27.log
```

### 3. **Seq** (Opcional - Visualização Web)
Logs também são enviados para **Seq** (se instalado) na porta `http://localhost:5341`.

Seq fornece uma interface web poderosa para:
- ✅ Busca e filtros avançados
- ✅ Gráficos e dashboards
- ✅ Alertas
- ✅ Análise de performance

## 🔍 Ferramentas de Visualização de Logs

### **Opção 1: Seq (Recomendado)** 🏆

**O que é**: Ferramenta web moderna para visualização de logs estruturados.

**Instalação com Docker:**
```bash
docker run -d \
  --name seq \
  -p 5341:80 \
  -p 5342:5342 \
  -e ACCEPT_EULA=Y \
  -e SEQ_FIRSTRUN_ADMINPASSWORD=admin \
  -v seq-data:/data \
  datalust/seq:latest
```

**Acesso**: http://localhost:5341
**Login**: `admin` / `admin`

**Recursos:**
- ✅ Interface web intuitiva
- ✅ Busca full-text e filtros
- ✅ Queries SQL-like
- ✅ Gráficos em tempo real
- ✅ Alertas por email/Slack
- ✅ Dashboards personalizados
- ✅ Gratuito para uso local

**Exemplo de query:**
```sql
WHERE ExceptionType = "NotFoundException"
AND RequestPath LIKE "/api/livros%"
ORDER BY @Timestamp DESC
```

### **Opção 2: Grafana + Loki**

**O que é**: Stack de observabilidade completa.

**Instalação com Docker Compose:**
```yaml
version: '3.8'
services:
  loki:
    image: grafana/loki:latest
    ports:
      - "3100:3100"
    command: -config.file=/etc/loki/local-config.yaml

  grafana:
    image: grafana/grafana:latest
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
```

**Acesso Grafana**: http://localhost:3000 (admin/admin)

**Configuração no Serilog** (adicionar ao `Program.cs`):
```csharp
.WriteTo.GrafanaLoki(
    "http://localhost:3100",
    labels: new[] { new LokiLabel { Key = "app", Value = "bookstore-api" } })
```

### **Opção 3: ElasticSearch + Kibana (ELK Stack)**

**O que é**: Stack completo para logs em grande escala.

**Mais complexo**, mas poderoso para ambientes de produção.

### **Opção 4: Ver Logs Diretamente** (Mais Simples)

Se não quiser instalar nada, pode ver os logs diretamente:

**No terminal (em tempo real):**
```bash
cd backend/src/BookStore.API
dotnet run
# Logs aparecerão aqui em tempo real
```

**Em arquivos:**
```bash
# Ver último arquivo de log
cat backend/logs/bookstore-$(date +%Y-%m-%d).log

# Ver logs em tempo real (tail -f)
tail -f backend/logs/bookstore-$(date +%Y-%m-%d).log

# Buscar erros
grep "ERR" backend/logs/bookstore-*.log

# Buscar criação de livros
grep "Livro criado" backend/logs/bookstore-*.log
```

## 🎯 Exemplos de Uso

### Buscar todos os erros de hoje
```bash
grep "\[ERR\]" backend/logs/bookstore-$(date +%Y-%m-%d).log
```

### Ver todos os livros criados
```bash
grep "Livro criado" backend/logs/bookstore-*.log
```

### Ver relatórios PDF gerados
```bash
grep "Relatório PDF gerado" backend/logs/bookstore-*.log
```

### Ver erros de validação
```bash
grep "Falha na validação" backend/logs/bookstore-*.log
```

### Ver requisições lentas (>1000ms)
```bash
grep "responded [0-9]* in [1-9][0-9][0-9][0-9]" backend/logs/bookstore-*.log
```

## 🔧 Configuração Avançada

### Níveis de Log

Configurados no `Program.cs`:

```csharp
.MinimumLevel.Information()                    // Nível mínimo global
.MinimumLevel.Override("Microsoft", Warning)   // Reduzir logs do framework
.MinimumLevel.Override("System", Warning)      // Reduzir logs do sistema
```

**Hierarquia de níveis** (do mais específico ao mais abrangente):
1. **Verbose** - Tudo (muito verboso)
2. **Debug** - Informações de debug
3. **Information** - Operações normais ← **Padrão**
4. **Warning** - Avisos
5. **Error** - Erros
6. **Fatal** - Erros fatais

### Mudar Nível de Log

Para desenvolvimento local, pode ser útil ativar Debug:

```csharp
// Em appsettings.Development.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"  // Ativa logs de debug
    }
  }
}
```

## 📊 Métricas Úteis

Com Seq ou Grafana, você pode criar dashboards para:

- 📈 Requisições por minuto
- ⚡ Tempo médio de resposta
- ❌ Taxa de erros
- 📘 Livros criados por hora
- 📄 Relatórios gerados por dia
- 👥 Usuários mais ativos

## 🚀 Próximos Passos

1. **Instale Seq** para ter uma interface web de logs
2. **Teste criando livros** e veja os logs aparecerem
3. **Gere relatórios PDF** e veja as métricas de performance
4. **Force alguns erros** (IDs inválidos) para ver o tracking de exceções

## 📚 Recursos

- [Serilog Documentation](https://serilog.net/)
- [Seq Documentation](https://docs.datalust.co/docs)
- [Structured Logging Best Practices](https://nblumhardt.com/2016/06/structured-logging-concepts-in-net-series-1/)

---

**🎓 Este sistema de logging demonstra:**
- ✅ Logging estruturado profissional
- ✅ Tratamento completo de exceções
- ✅ Rastreabilidade de operações críticas
- ✅ Métricas de performance
- ✅ Troubleshooting facilitado
- ✅ Compliance e auditoria


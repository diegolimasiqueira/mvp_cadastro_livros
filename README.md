# 📚 Sistema de Cadastro de Livros

Sistema completo de gerenciamento de livros desenvolvido com **.NET 9** e **Angular v20**, seguindo os princípios de **Clean Architecture** e boas práticas de desenvolvimento.

## 🏗️ Arquitetura

### Backend (.NET 9)
- **Clean Architecture** com separação em camadas:
  - **Domain**: Entidades e interfaces de domínio
  - **Application**: DTOs, validações e serviços de aplicação
  - **Infrastructure**: Acesso a dados, repositórios e migrations
  - **API**: Controllers e configurações da API REST

- **Tecnologias**:
  - Entity Framework Core 9.0 com PostgreSQL
  - FluentValidation para validação de DTOs
  - JWT para autenticação
  - Swagger/OpenAPI para documentação da API
  - QuestPDF para geração de relatórios em PDF
  - EPPlus para geração de relatórios em Excel

### Frontend (Angular v20)
- **Standalone Components** com Signals para reatividade
- **ng-bootstrap** para componentes de UI
- **Interceptors** para autenticação JWT
- **Guards** para proteção de rotas
- Layout responsivo com menu colapsável

## 📋 Funcionalidades

### Gestão de Entidades
- ✅ **Livros**: CRUD completo com múltiplos autores, assuntos e preços por forma de compra
- ✅ **Autores**: Cadastro e gerenciamento de autores
- ✅ **Assuntos**: Categorização de livros por assunto
- ✅ **Formas de Compra**: Definição de canais de venda (Internet, Balcão, etc.)

### Relatórios
- ✅ **Relatório de Livros por Autor**:
  - Visualização HTML com agrupamento por autor
  - Paginação em dois níveis (autores e livros)
  - Filtros dinâmicos por todos os campos
  - Exportação para PDF e Excel
  - VIEW otimizada no PostgreSQL para performance

### Validações
- ✅ Validação completa de dados no backend e frontend
- ✅ Feedback visual imediato para erros de validação
- ✅ Mensagens de erro claras e objetivas

## 🚀 Executando o Projeto

### Pré-requisitos
- Docker e Docker Compose
- (Opcional para execução local) .NET 9 SDK, Node.js 22+, PostgreSQL 16+

### Com Docker (Recomendado)

1. **Clone o repositório**:
```bash
git clone <url-do-repositorio>
cd mvp_cadastro_livros
```

2. **Inicie todos os serviços**:
```bash
docker compose up -d
```

3. **Acesse a aplicação**:
   - Frontend: http://localhost:3000
   - Backend (Swagger): http://localhost:8080/swagger
   - PostgreSQL: localhost:5433 (mapeado do container 5432)

4. **Credenciais padrão**:
   - Usuário: `admin`
   - Senha: `Admin@123`

5. **Autenticação no Swagger**:
   - Acesse http://localhost:8080/swagger
   - Use o endpoint `/api/Auth/login` com as credenciais acima para obter o token JWT
   - Clique no botão **Authorize** (cadeado verde no topo da página)
   - Cole o token no campo (não precisa adicionar "Bearer", será adicionado automaticamente)
   - Clique em **Authorize** e depois **Close**
   - Agora você pode testar todos os endpoints protegidos

6. **Parar os serviços**:
```bash
docker compose down
```

7. **Conectar ao PostgreSQL com pgAdmin ou outro cliente**:
   - **Host**: `localhost`
   - **Porta**: `5433`
   - **Database**: `bookstoredb`
   - **Usuário**: `postgres`
   - **Senha**: `postgres123`

8. **Resetar o ambiente (limpar banco de dados)**:
```bash
docker compose down -v
docker compose up -d
```

### Execução Local (Sem Docker)

#### Backend

**Pré-requisitos**: .NET 9 SDK, PostgreSQL 16+

1. **Instale o PostgreSQL 16+** (se ainda não tiver):
```bash
# Ubuntu/Debian
sudo apt install postgresql-16

# macOS
brew install postgresql@16

# Windows
# Baixe o instalador em: https://www.postgresql.org/download/windows/
```

2. **Crie o banco de dados**:
```bash
# Método 1: Via psql
psql -U postgres
CREATE DATABASE bookstoredb;
\q

# Método 2: Via createdb
createdb -U postgres bookstoredb

# Método 3: Via pgAdmin
# Abra o pgAdmin, clique com botão direito em "Databases" → "Create" → "Database"
# Nome: bookstoredb
```

3. **Configure suas credenciais locais**:
```bash
cd backend/src/BookStore.API

# Edite o arquivo appsettings.Development.json
# Ajuste APENAS a senha na ConnectionString para sua senha local do PostgreSQL
nano appsettings.Development.json  # ou use seu editor preferido (VS Code, vim, etc.)
```

Exemplo do conteúdo do `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=SUA_SENHA_AQUI"
  },
  ...
}
```

> ⚠️ **IMPORTANTE - Segurança**: 
> - **NUNCA commit o `appsettings.Development.json` com sua senha pessoal!**
> - Antes de fazer commit, **reverta a senha para `postgres123`** (senha padrão do Docker).
> - Este arquivo está configurado com a senha padrão do repositório para facilitar execução com Docker.
> - Para seu ambiente local, você ajusta a senha temporariamente, mas **não deve committá-la**.

4. **Instale a ferramenta EF Core CLI** (se não tiver):
```bash
dotnet tool install --global dotnet-ef
# Ou atualize se já tiver: dotnet tool update --global dotnet-ef
```

5. **Execute as migrations** (cria todas as tabelas e VIEW automaticamente):
```bash
cd backend

# Passe a connection string diretamente (mais confiável que appsettings)
# Substitua "SUA_SENHA_AQUI" pela senha do seu PostgreSQL local
dotnet ef database update \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API \
  --connection "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=SUA_SENHA_AQUI"

# Exemplo com senha "Master@123":
# dotnet ef database update --project src/BookStore.Infrastructure --startup-project src/BookStore.API --connection "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=Master@123"
```

**O que a migration faz automaticamente:**
- ✅ Criar todas as tabelas (Livro, Autor, Assunto, FormaCompra)
- ✅ Criar os relacionamentos (Livro_Autor, Livro_Assunto, LivroPreco)
- ✅ Criar a VIEW (vw_livros_por_autor) para relatórios
- ✅ Inserir dados iniciais (usuário admin com senha Admin@123)

6. **Execute a API**:
```bash
cd backend/src/BookStore.API

# A API usará automaticamente o appsettings.Development.json
# (com a senha que você configurou no passo 3)
dotnet run
```

A API estará disponível em **http://localhost:8080** (a porta pode variar, verifique o console)
- **Swagger UI**: http://localhost:8080/swagger (ou a porta exibida no console)
- **Health Check**: http://localhost:8080/health

**Comandos úteis para gerenciar o banco de dados local**:

> **💡 Dica:** Para todos os comandos abaixo, você pode precisar passar o parâmetro `--connection` com sua connection string, caso o `dotnet ef` CLI não carregue automaticamente o `appsettings.Development.json`.

```bash
cd backend

# Verificar migrations pendentes
dotnet ef migrations list \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API

# Criar uma nova migration (se necessário)
dotnet ef migrations add NomeDaMigration \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API

# Reverter a última migration (passe --connection se necessário)
dotnet ef database update NomeMigrationAnterior \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API

# Remover a última migration (se ainda não foi aplicada)
dotnet ef migrations remove \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API

# Resetar o banco de dados (DROP + CREATE + Migrations)
dotnet ef database drop --force \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API \
  --connection "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=SUA_SENHA"

dotnet ef database update \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API \
  --connection "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=SUA_SENHA"
```

#### Frontend

**Pré-requisitos**: Node.js 22+, npm

1. **Instale o Node.js 22+** (se ainda não tiver):
```bash
# Ubuntu/Debian (usando nvm - recomendado)
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
nvm install 22
nvm use 22

# macOS
brew install node@22

# Windows
# Baixe o instalador em: https://nodejs.org/
```

2. **Instale as dependências**:
```bash
cd frontend
npm install --legacy-peer-deps
```

3. **Configure a URL da API** (se necessário):
```typescript
// frontend/src/app/services/api.service.ts
// A URL padrão já está configurada para http://localhost:5000/api
// Altere apenas se sua API estiver em outra porta

private API_URL = 'http://localhost:5000/api';
```

4. **Execute o frontend**:
```bash
npm start

# Ou em modo de desenvolvimento com reload automático:
ng serve
```

O frontend estará disponível em http://localhost:4200

**Nota**: Certifique-se de que o backend está rodando antes de iniciar o frontend.

## 🧪 Testes e Cobertura

### Backend

O projeto possui testes unitários e de integração com cobertura mínima de 80%.

#### Executar todos os testes:
```bash
cd backend
dotnet test
```

#### Gerar relatório de cobertura:
```bash
cd backend

# 1. Executar testes com cobertura
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./TestResults

# 2. Gerar relatório HTML
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" \
                 -targetdir:"TestResults/CoverageReport" \
                 -reporttypes:"Html;Cobertura"

# 3. Visualizar relatório
cd TestResults/CoverageReport
python3 -m http.server 8081
# Acesse: http://localhost:8081
```

#### Executar apenas testes unitários:
```bash
cd backend
dotnet test tests/BookStore.UnitTests/BookStore.UnitTests.csproj
```

#### Executar apenas testes de integração:
```bash
cd backend
dotnet test tests/BookStore.IntegrationTests/BookStore.IntegrationTests.csproj
```

### Frontend

O projeto possui testes com cobertura mínima de 70%.

#### Executar todos os testes:
```bash
cd frontend
npm test
```

#### Gerar relatório de cobertura:
```bash
cd frontend

# 1. Executar testes com cobertura
npm run test:coverage

# 2. Visualizar relatório
cd coverage/frontend
python3 -m http.server 8082
# Acesse: http://localhost:8082
```

#### Executar testes em modo watch (desenvolvimento):
```bash
cd frontend
npm test -- --watch
```

## 🔌 Conectando ao Banco de Dados

### Usando pgAdmin

1. **Abra o pgAdmin** e crie uma nova conexão
2. **Configurações da conexão**:
   ```
   General:
     Name: BookStore Docker
   
   Connection:
     Host: localhost
     Port: 5433
     Maintenance database: bookstoredb
     Username: postgres
     Password: postgres123
   
   SSL:
     SSL mode: Prefer
   ```

3. **Salve e conecte**

### Usando DBeaver, DataGrip ou outra ferramenta

Use as mesmas configurações:
- **Driver**: PostgreSQL
- **Host**: localhost
- **Port**: 5433
- **Database**: bookstoredb
- **User**: postgres
- **Password**: postgres123

### Via linha de comando (psql)

```bash
# Com psql instalado localmente
psql -h localhost -p 5433 -U postgres -d bookstoredb

# Ou usando o container Docker
docker exec -it bookstore-postgres psql -U postgres -d bookstoredb
```

## 🗄️ Estrutura do Banco de Dados

### 📊 Diagrama Entidade-Relacionamento

Para visualizar o diagrama completo da modelagem de dados, acesse: **[DATABASE_SCHEMA.md](DATABASE_SCHEMA.md)**

O diagrama Mermaid será renderizado automaticamente no GitHub com todas as tabelas, relacionamentos e cardinalidades.

### Tabelas Principais
- **Livro**: Informações básicas dos livros
- **Autor**: Cadastro de autores
- **Assunto**: Categorias de assuntos
- **FormaCompra**: Canais de venda
- **Livro_Autor**: Relacionamento N:N entre livros e autores
- **Livro_Assunto**: Relacionamento N:N entre livros e assuntos
- **LivroPreco**: Preços por forma de compra

### VIEW para Relatórios
- **vw_livros_por_autor**: VIEW otimizada com dados agregados para geração de relatórios

## 📊 Estrutura de Diretórios

```
mvp_cadastro_livros/
├── backend/
│   ├── src/
│   │   ├── BookStore.API/          # Controllers e Program.cs
│   │   ├── BookStore.Application/  # DTOs, Services, Validators
│   │   ├── BookStore.Domain/       # Entities e Interfaces
│   │   └── BookStore.Infrastructure/# Repositories e Data
│   ├── tests/
│   │   ├── BookStore.UnitTests/    # Testes unitários
│   │   └── BookStore.IntegrationTests/ # Testes de integração
│   ├── Dockerfile
│   ├── coverlet.runsettings        # Configuração de cobertura
│   └── BookStore.sln
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/         # Componentes da aplicação
│   │   │   ├── services/           # Serviços (API, Auth)
│   │   │   ├── guards/             # Guards de rota
│   │   │   ├── interceptors/       # HTTP Interceptors
│   │   │   └── models/             # Interfaces TypeScript
│   │   ├── main.ts
│   │   └── styles.scss
│   ├── Dockerfile
│   ├── nginx.conf
│   └── package.json
├── docker-compose.yml
├── .gitignore
└── README.md
```

## 🔧 Tecnologias Utilizadas

### Backend
- .NET 9.0
- Entity Framework Core 9.0
- PostgreSQL 16
- FluentValidation 11.x
- Swashbuckle (Swagger) 9.x
- xUnit + Moq + FluentAssertions
- Testcontainers
- Coverlet
- QuestPDF 2024.10
- EPPlus 7.5

### Frontend
- Angular 20
- TypeScript 5.7
- ng-bootstrap 19
- Bootstrap 5.3
- RxJS 7.8
- Jasmine + Karma

### DevOps
- Docker & Docker Compose
- Multi-stage builds
- Health checks

## 📝 Endpoints da API

### Autenticação
- `POST /api/auth/login` - Login de usuário

### Livros
- `GET /api/livros` - Listar livros (com paginação)
- `GET /api/livros/{id}` - Buscar livro por ID
- `POST /api/livros` - Criar livro
- `PUT /api/livros/{id}` - Atualizar livro
- `DELETE /api/livros/{id}` - Deletar livro

### Autores
- `GET /api/autores` - Listar autores (com paginação)
- `GET /api/autores/{id}` - Buscar autor por ID
- `POST /api/autores` - Criar autor
- `PUT /api/autores/{id}` - Atualizar autor
- `DELETE /api/autores/{id}` - Deletar autor

### Assuntos
- `GET /api/assuntos` - Listar assuntos (com paginação)
- `GET /api/assuntos/{id}` - Buscar assunto por ID
- `POST /api/assuntos` - Criar assunto
- `PUT /api/assuntos/{id}` - Atualizar assunto
- `DELETE /api/assuntos/{id}` - Deletar assunto

### Formas de Compra
- `GET /api/formascompra` - Listar formas de compra (com paginação)
- `GET /api/formascompra/{id}` - Buscar forma de compra por ID
- `POST /api/formascompra` - Criar forma de compra
- `PUT /api/formascompra/{id}` - Atualizar forma de compra
- `DELETE /api/formascompra/{id}` - Deletar forma de compra

### Relatórios
- `GET /api/reports/books-by-author` - Obter dados do relatório (JSON)
- `GET /api/reports/books-by-author/pdf` - Baixar relatório em PDF
- `GET /api/reports/books-by-author/excel` - Baixar relatório em Excel

## 🛡️ Segurança

- Autenticação via JWT
- Proteção de rotas no frontend via Guards
- Interceptor HTTP para anexar token automaticamente
- Validação de dados no backend e frontend
- Tratamento global de exceções

### ⚠️ Nota Importante sobre Credenciais

**Este é um projeto de demonstração/desafio técnico.** Por isso, as credenciais e secrets estão **hardcoded** nos arquivos `appsettings.json` para facilitar a avaliação e execução do projeto.

**Em um ambiente de produção, NUNCA faça isso!** As boas práticas incluem:

#### ✅ **O que fazer em produção:**

1. **Usar Gerenciadores de Secrets**:
   - Azure Key Vault
   - AWS Secrets Manager
   - HashiCorp Vault
   - Google Cloud Secret Manager

2. **Variáveis de Ambiente**:
   ```bash
   # Exemplo no docker-compose.yml
   environment:
     - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
     - JwtSettings__SecretKey=${JWT_SECRET_KEY}
   ```

3. **Docker Secrets** (Docker Swarm):
   ```yaml
   secrets:
     db_password:
       external: true
   ```

4. **Kubernetes Secrets**:
   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: db-credentials
   type: Opaque
   data:
     password: <base64-encoded>
   ```

5. **Adicionar ao `.gitignore`**:
   ```gitignore
   appsettings.json
   appsettings.*.json
   !appsettings.Development.json.example
   ```

#### ❌ **O que NÃO fazer (mas fizemos aqui para facilitar):**
- ❌ Commitar connection strings
- ❌ Commitar JWT secret keys
- ❌ Commitar senhas no código
- ❌ Usar credenciais padrão em produção

#### 📝 **Arquivos com credenciais neste projeto:**
- `backend/src/BookStore.API/appsettings.json` - Connection string (Docker) e JWT secret
- `backend/src/BookStore.API/appsettings.Development.json` - Connection string (Local com senha padrão `postgres123`)
- `docker-compose.yml` - Senha do PostgreSQL

**Estes arquivos foram commitados propositalmente apenas para facilitar a execução e avaliação deste desafio técnico.**

> 💡 **Para desenvolvimento local**: O `appsettings.Development.json` está configurado com a senha padrão `postgres123`. Se você alterar para sua senha local, **lembre-se de reverter antes de fazer commit!**

## 📦 Build para Produção

### Docker (Recomendado)
```bash
# Build e deploy com Docker Compose
docker compose up -d --build
```

### Manual

#### Backend
```bash
cd backend/src/BookStore.API
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
cd frontend
npm run build
# Os arquivos estarão em frontend/dist/frontend/browser/
```

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.

## 👨‍💻 Autor

Desenvolvido para o desafio de cadastro de livros.

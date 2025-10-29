# 📚 Stored Procedures para População do Banco de Dados

Este documento explica como usar as **Stored Procedures** criadas em PL/pgSQL para popular o banco de dados do sistema de cadastro de livros.

## 🎯 Objetivo

Demonstrar conhecimento em **Store Procedures (SP)** e **PL/pgSQL** através de procedures robustas que:
- ✅ Verificam duplicatas antes de inserir
- ✅ Fornecem feedback detalhado via `RAISE NOTICE`
- ✅ Tratam exceções apropriadamente
- ✅ Permitem inserção individual ou em lote
- ✅ Facilitam manutenção e testes

## 📝 Procedures Disponíveis

### 1. `sp_inserir_assunto(p_descricao)`
Insere um assunto verificando se já existe.

```sql
CALL sp_inserir_assunto('Terror');
```

### 2. `sp_inserir_autor(p_nome)`
Insere um autor verificando se já existe.

```sql
CALL sp_inserir_autor('Stephen Hawking');
```

### 3. `sp_inserir_forma_compra(p_descricao)`
Insere uma forma de compra verificando se já existe.

```sql
CALL sp_inserir_forma_compra('Empréstimo');
```

### 4. `sp_inserir_livro_completo(...)`
Insere um livro completo com todos os relacionamentos.

**Parâmetros:**
- `p_titulo`: Título do livro (VARCHAR 40)
- `p_editora`: Editora (VARCHAR 40)
- `p_edicao`: Número da edição (INTEGER)
- `p_ano_publicacao`: Ano (VARCHAR 4)
- `p_autores`: Array de IDs de autores (INTEGER[])
- `p_assuntos`: Array de IDs de assuntos (INTEGER[])
- `p_formas_compra`: Array de IDs de formas de compra (INTEGER[])
- `p_precos`: Array de preços correspondentes (DECIMAL[])

**Exemplo:**
```sql
CALL sp_inserir_livro_completo(
    'O Senhor dos Anéis',           -- título
    'HarperCollins',                -- editora
    1,                              -- edição
    '1954',                         -- ano
    ARRAY[1, 2],                    -- IDs dos autores
    ARRAY[12, 10],                  -- IDs dos assuntos (Fantasia, Aventura)
    ARRAY[1, 3],                    -- IDs das formas de compra (Físico Novo, E-book)
    ARRAY[79.90, 39.90]            -- preços correspondentes
);
```

### 5. `sp_popular_banco_completo()`
⭐ **Procedure Master** - Popula o banco inteiro com dados de exemplo.

```sql
CALL sp_popular_banco_completo();
```

**O que esta procedure faz:**
- ✅ Insere 12 assuntos variados
- ✅ Insere 15 autores (brasileiros e internacionais)
- ✅ Insere 6 formas de compra
- ✅ Insere 8 livros completos com todos os relacionamentos
- ✅ Fornece feedback detalhado de cada operação
- ✅ Exibe relatório final com totais

### 6. `sp_limpar_banco()`
Limpa todos os dados e reseta as sequências.

```sql
CALL sp_limpar_banco();
```

## 🚀 Como Usar

### Opção 1: Popular Banco Completo (Recomendado)

```bash
# 1. Conecte no banco via psql
psql -h localhost -p 5432 -U postgres -d bookstoredb

# 2. Execute o script de criação das procedures
\i seed_procedures.sql

# 3. Execute a procedure master
CALL sp_popular_banco_completo();

# 4. Verifique os dados
SELECT * FROM vw_livros_por_autor ORDER BY "AutorNome", "LivroTitulo";
```

### Opção 2: Inserir Dados Individuais

```sql
-- Conecte no banco
psql -h localhost -p 5432 -U postgres -d bookstoredb

-- Carregue as procedures
\i seed_procedures.sql

-- Insira dados individuais
CALL sp_inserir_assunto('Terror');
CALL sp_inserir_autor('Stephen Hawking');
CALL sp_inserir_forma_compra('Kindle Unlimited');

-- Insira um livro completo
CALL sp_inserir_livro_completo(
    'Uma Breve História do Tempo',
    'Intrínseca',
    1,
    '1988',
    ARRAY[16],              -- ID do autor (usar SELECT para verificar)
    ARRAY[4, 8],            -- Tecnologia, Filosofia
    ARRAY[1, 3, 4],         -- Físico, E-book, Audiobook
    ARRAY[54.90, 29.90, 34.90]
);
```

### Opção 3: Resetar e Repopular

```sql
-- Limpar tudo
CALL sp_limpar_banco();

-- Repopular
CALL sp_popular_banco_completo();
```

## 📊 Dados Populados

Ao executar `sp_popular_banco_completo()`, você terá:

### Assuntos (12)
- Ficção Científica, Romance, Suspense, Tecnologia
- História, Autoajuda, Biografia, Filosofia
- Programação, Aventura, Drama, Fantasia

### Autores (15)
- **Brasileiros**: Machado de Assis, Clarice Lispector, Paulo Coelho, Graciliano Ramos, Fernando Pessoa
- **Internacionais**: J.K. Rowling, George Orwell, Isaac Asimov, Agatha Christie, Stephen King, Dan Brown
- **Técnicos**: Yuval Noah Harari, Robert C. Martin, Martin Fowler, Eric Evans

### Formas de Compra (6)
- Físico - Novo
- Físico - Usado
- E-book
- Audiobook
- Assinatura Digital
- Aluguel

### Livros (8)
1. **Dom Casmurro** - Machado de Assis (Romance, Drama)
2. **1984** - George Orwell (Ficção Científica, Suspense)
3. **Harry Potter e a Pedra Filosofal** - J.K. Rowling (Fantasia, Aventura)
4. **O Alquimista** - Paulo Coelho (Autoajuda, Filosofia)
5. **Fundação** - Isaac Asimov (Ficção Científica)
6. **Clean Code** - Robert C. Martin (Tecnologia, Programação)
7. **Sapiens** - Yuval Noah Harari (História, Filosofia)
8. **A Hora da Estrela** - Clarice Lispector (Romance, Drama)

## 💡 Vantagens das Stored Procedures

✅ **Reutilização**: Uma vez criadas, podem ser chamadas múltiplas vezes  
✅ **Performance**: Execução otimizada no servidor de banco de dados  
✅ **Segurança**: Controle de acesso granular  
✅ **Manutenibilidade**: Lógica centralizada no banco  
✅ **Validação**: Verificações de duplicatas e integridade  
✅ **Feedback**: Mensagens detalhadas via RAISE NOTICE  
✅ **Transações**: Operações atômicas e consistentes  

## 🔧 Troubleshooting

### Erro: "relation does not exist"
```sql
-- Certifique-se de que as migrations foram executadas
cd backend
dotnet ef database update \
  --project src/BookStore.Infrastructure \
  --startup-project src/BookStore.API \
  --connection "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=SUA_SENHA"
```

### Erro: "foreign key constraint"
```sql
-- Execute as procedures na ordem correta ou use sp_popular_banco_completo()
-- Esta procedure já insere na ordem correta automaticamente
```

### Limpar e recomeçar
```sql
CALL sp_limpar_banco();
CALL sp_popular_banco_completo();
```

## 📚 Recursos Adicionais

- [PostgreSQL PL/pgSQL Documentation](https://www.postgresql.org/docs/current/plpgsql.html)
- [Stored Procedures Best Practices](https://wiki.postgresql.org/wiki/PL/pgSQL)

---

**🎓 Este conjunto de procedures demonstra:**
- ✅ Conhecimento avançado em PL/pgSQL
- ✅ Design de APIs de banco de dados robustas
- ✅ Tratamento de exceções
- ✅ Validação de dados
- ✅ Feedback adequado ao usuário
- ✅ Manutenibilidade e reutilização de código


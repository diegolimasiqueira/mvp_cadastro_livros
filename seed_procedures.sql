-- ============================================
-- STORED PROCEDURES - Sistema de Cadastro de Livros
-- Demonstração de conhecimento em PL/pgSQL e Store Procedures
-- ============================================

-- ===========================================
-- 1. PROCEDURE: Inserir Assunto (com verificação de duplicata)
-- ===========================================
CREATE OR REPLACE PROCEDURE sp_inserir_assunto(
    p_descricao VARCHAR(20)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_count INTEGER;
BEGIN
    -- Verificar se já existe
    SELECT COUNT(*) INTO v_count 
    FROM "Assunto" 
    WHERE "Descricao" = p_descricao;
    
    IF v_count = 0 THEN
        INSERT INTO "Assunto" ("Descricao") VALUES (p_descricao);
        RAISE NOTICE 'Assunto "%" inserido com sucesso.', p_descricao;
    ELSE
        RAISE NOTICE 'Assunto "%" já existe. Ignorado.', p_descricao;
    END IF;
END;
$$;

-- ===========================================
-- 2. PROCEDURE: Inserir Autor (com verificação de duplicata)
-- ===========================================
CREATE OR REPLACE PROCEDURE sp_inserir_autor(
    p_nome VARCHAR(40)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_count INTEGER;
BEGIN
    -- Verificar se já existe
    SELECT COUNT(*) INTO v_count 
    FROM "Autor" 
    WHERE "Nome" = p_nome;
    
    IF v_count = 0 THEN
        INSERT INTO "Autor" ("Nome") VALUES (p_nome);
        RAISE NOTICE 'Autor "%" inserido com sucesso.', p_nome;
    ELSE
        RAISE NOTICE 'Autor "%" já existe. Ignorado.', p_nome;
    END IF;
END;
$$;

-- ===========================================
-- 3. PROCEDURE: Inserir Forma de Compra (com verificação)
-- ===========================================
CREATE OR REPLACE PROCEDURE sp_inserir_forma_compra(
    p_descricao VARCHAR(20)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO v_count 
    FROM "FormaCompra" 
    WHERE "Descricao" = p_descricao;
    
    IF v_count = 0 THEN
        INSERT INTO "FormaCompra" ("Descricao") VALUES (p_descricao);
        RAISE NOTICE 'Forma de Compra "%" inserida com sucesso.', p_descricao;
    ELSE
        RAISE NOTICE 'Forma de Compra "%" já existe. Ignorado.', p_descricao;
    END IF;
END;
$$;

-- ===========================================
-- 4. PROCEDURE: Inserir Livro Completo (com relacionamentos)
-- ===========================================
CREATE OR REPLACE PROCEDURE sp_inserir_livro_completo(
    p_titulo VARCHAR(40),
    p_editora VARCHAR(40),
    p_edicao INTEGER,
    p_ano_publicacao VARCHAR(4),
    p_autores INTEGER[],           -- Array de IDs de autores
    p_assuntos INTEGER[],          -- Array de IDs de assuntos
    p_formas_compra INTEGER[],     -- Array de IDs de formas de compra
    p_precos DECIMAL[]             -- Array de preços correspondentes
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_livro_id INTEGER;
    v_autor_id INTEGER;
    v_assunto_id INTEGER;
    v_forma_id INTEGER;
    v_preco DECIMAL;
    v_idx INTEGER;
BEGIN
    -- 1. Inserir o livro
    INSERT INTO "Livro" ("Titulo", "Editora", "Edicao", "AnoPublicacao")
    VALUES (p_titulo, p_editora, p_edicao, p_ano_publicacao)
    RETURNING "CodI" INTO v_livro_id;
    
    RAISE NOTICE 'Livro "%" inserido com ID: %', p_titulo, v_livro_id;
    
    -- 2. Associar autores
    IF p_autores IS NOT NULL THEN
        FOREACH v_autor_id IN ARRAY p_autores
        LOOP
            INSERT INTO "Livro_Autor" ("Livro_CodI", "Autor_CodAu")
            VALUES (v_livro_id, v_autor_id);
            RAISE NOTICE '  → Autor ID % associado', v_autor_id;
        END LOOP;
    END IF;
    
    -- 3. Associar assuntos
    IF p_assuntos IS NOT NULL THEN
        FOREACH v_assunto_id IN ARRAY p_assuntos
        LOOP
            INSERT INTO "Livro_Assunto" ("Livro_CodI", "Assunto_CodAs")
            VALUES (v_livro_id, v_assunto_id);
            RAISE NOTICE '  → Assunto ID % associado', v_assunto_id;
        END LOOP;
    END IF;
    
    -- 4. Associar preços
    IF p_formas_compra IS NOT NULL AND p_precos IS NOT NULL THEN
        v_idx := 1;
        FOREACH v_forma_id IN ARRAY p_formas_compra
        LOOP
            v_preco := p_precos[v_idx];
            INSERT INTO "LivroPreco" ("Livro_CodI", "FormaCompra_CodFc", "Valor")
            VALUES (v_livro_id, v_forma_id, v_preco);
            RAISE NOTICE '  → Forma de Compra ID %: R$ %', v_forma_id, v_preco;
            v_idx := v_idx + 1;
        END LOOP;
    END IF;
    
    RAISE NOTICE '✅ Livro completo inserido com sucesso!';
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Erro ao inserir livro: %', SQLERRM;
END;
$$;

-- ===========================================
-- 5. PROCEDURE MASTER: Popular Banco Completo
-- ===========================================
CREATE OR REPLACE PROCEDURE sp_popular_banco_completo()
LANGUAGE plpgsql
AS $$
BEGIN
    RAISE NOTICE '🚀 Iniciando população do banco de dados...';
    RAISE NOTICE '';
    
    -- ========================================
    -- INSERIR ASSUNTOS
    -- ========================================
    RAISE NOTICE '📚 Inserindo Assuntos...';
    CALL sp_inserir_assunto('Ficção Científica');
    CALL sp_inserir_assunto('Romance');
    CALL sp_inserir_assunto('Suspense');
    CALL sp_inserir_assunto('Tecnologia');
    CALL sp_inserir_assunto('História');
    CALL sp_inserir_assunto('Autoajuda');
    CALL sp_inserir_assunto('Biografia');
    CALL sp_inserir_assunto('Filosofia');
    CALL sp_inserir_assunto('Programação');
    CALL sp_inserir_assunto('Aventura');
    CALL sp_inserir_assunto('Drama');
    CALL sp_inserir_assunto('Fantasia');
    RAISE NOTICE '';
    
    -- ========================================
    -- INSERIR AUTORES
    -- ========================================
    RAISE NOTICE '👤 Inserindo Autores...';
    CALL sp_inserir_autor('Machado de Assis');
    CALL sp_inserir_autor('Clarice Lispector');
    CALL sp_inserir_autor('Paulo Coelho');
    CALL sp_inserir_autor('J.K. Rowling');
    CALL sp_inserir_autor('George Orwell');
    CALL sp_inserir_autor('Isaac Asimov');
    CALL sp_inserir_autor('Agatha Christie');
    CALL sp_inserir_autor('Stephen King');
    CALL sp_inserir_autor('Dan Brown');
    CALL sp_inserir_autor('Yuval Noah Harari');
    CALL sp_inserir_autor('Robert C. Martin');
    CALL sp_inserir_autor('Martin Fowler');
    CALL sp_inserir_autor('Eric Evans');
    CALL sp_inserir_autor('Graciliano Ramos');
    CALL sp_inserir_autor('Fernando Pessoa');
    RAISE NOTICE '';
    
    -- ========================================
    -- INSERIR FORMAS DE COMPRA
    -- ========================================
    RAISE NOTICE '💳 Inserindo Formas de Compra...';
    CALL sp_inserir_forma_compra('Físico - Novo');
    CALL sp_inserir_forma_compra('Físico - Usado');
    CALL sp_inserir_forma_compra('E-book');
    CALL sp_inserir_forma_compra('Audiobook');
    CALL sp_inserir_forma_compra('Assinatura Digital');
    CALL sp_inserir_forma_compra('Aluguel');
    RAISE NOTICE '';
    
    -- ========================================
    -- INSERIR LIVROS COMPLETOS
    -- ========================================
    RAISE NOTICE '📖 Inserindo Livros Completos...';
    RAISE NOTICE '';
    
    -- Livro 1: Dom Casmurro
    CALL sp_inserir_livro_completo(
        'Dom Casmurro',
        'Editora Nova Fronteira',
        1,
        '1899',
        ARRAY[1],              -- Machado de Assis
        ARRAY[2, 11],          -- Romance, Drama
        ARRAY[1, 2, 3],        -- Físico Novo, Usado, E-book
        ARRAY[35.90, 18.50, 12.90]
    );
    RAISE NOTICE '';
    
    -- Livro 2: 1984
    CALL sp_inserir_livro_completo(
        '1984',
        'Companhia das Letras',
        2,
        '1949',
        ARRAY[5],              -- George Orwell
        ARRAY[1, 3],           -- Ficção Científica, Suspense
        ARRAY[1, 3, 4],        -- Físico Novo, E-book, Audiobook
        ARRAY[42.90, 19.90, 24.90]
    );
    RAISE NOTICE '';
    
    -- Livro 3: Harry Potter e a Pedra Filosofal
    CALL sp_inserir_livro_completo(
        'Harry Potter e a Pedra Filosofal',
        'Rocco',
        1,
        '1997',
        ARRAY[4],              -- J.K. Rowling
        ARRAY[12, 10],         -- Fantasia, Aventura
        ARRAY[1, 3, 4],
        ARRAY[54.90, 29.90, 39.90]
    );
    RAISE NOTICE '';
    
    -- Livro 4: O Alquimista
    CALL sp_inserir_livro_completo(
        'O Alquimista',
        'HarperCollins',
        3,
        '1988',
        ARRAY[3],              -- Paulo Coelho
        ARRAY[6, 8],           -- Autoajuda, Filosofia
        ARRAY[1, 2, 3],
        ARRAY[38.90, 22.50, 16.90]
    );
    RAISE NOTICE '';
    
    -- Livro 5: Fundação
    CALL sp_inserir_livro_completo(
        'Fundação',
        'Aleph',
        1,
        '1951',
        ARRAY[6],              -- Isaac Asimov
        ARRAY[1],              -- Ficção Científica
        ARRAY[1, 3],
        ARRAY[49.90, 27.90]
    );
    RAISE NOTICE '';
    
    -- Livro 6: Clean Code
    CALL sp_inserir_livro_completo(
        'Clean Code',
        'Pearson',
        1,
        '2008',
        ARRAY[11],             -- Robert C. Martin
        ARRAY[4, 9],           -- Tecnologia, Programação
        ARRAY[1, 3, 5],        -- Físico, E-book, Assinatura
        ARRAY[89.90, 49.90, 39.90]
    );
    RAISE NOTICE '';
    
    -- Livro 7: Sapiens
    CALL sp_inserir_livro_completo(
        'Sapiens',
        'L&PM Editores',
        1,
        '2011',
        ARRAY[10],             -- Yuval Noah Harari
        ARRAY[5, 8],           -- História, Filosofia
        ARRAY[1, 3, 4],
        ARRAY[59.90, 34.90, 44.90]
    );
    RAISE NOTICE '';
    
    -- Livro 8: A Hora da Estrela
    CALL sp_inserir_livro_completo(
        'A Hora da Estrela',
        'Rocco',
        1,
        '1977',
        ARRAY[2],              -- Clarice Lispector
        ARRAY[2, 11],          -- Romance, Drama
        ARRAY[1, 2, 3],
        ARRAY[32.90, 16.50, 14.90]
    );
    RAISE NOTICE '';
    
    -- ========================================
    -- RELATÓRIO FINAL
    -- ========================================
    RAISE NOTICE '✅ População concluída com sucesso!';
    RAISE NOTICE '';
    RAISE NOTICE '📊 RESUMO:';
    RAISE NOTICE '  → % Assuntos', (SELECT COUNT(*) FROM "Assunto");
    RAISE NOTICE '  → % Autores', (SELECT COUNT(*) FROM "Autor");
    RAISE NOTICE '  → % Formas de Compra', (SELECT COUNT(*) FROM "FormaCompra");
    RAISE NOTICE '  → % Livros', (SELECT COUNT(*) FROM "Livro");
    RAISE NOTICE '  → % Relações Livro-Autor', (SELECT COUNT(*) FROM "Livro_Autor");
    RAISE NOTICE '  → % Relações Livro-Assunto', (SELECT COUNT(*) FROM "Livro_Assunto");
    RAISE NOTICE '  → % Preços cadastrados', (SELECT COUNT(*) FROM "LivroPreco");
    RAISE NOTICE '';
    
END;
$$;

-- ===========================================
-- 6. PROCEDURE: Limpar todos os dados
-- ===========================================
CREATE OR REPLACE PROCEDURE sp_limpar_banco()
LANGUAGE plpgsql
AS $$
BEGIN
    RAISE NOTICE '🗑️  Limpando banco de dados...';
    
    DELETE FROM "LivroPreco";
    DELETE FROM "Livro_Assunto";
    DELETE FROM "Livro_Autor";
    DELETE FROM "Livro";
    DELETE FROM "FormaCompra";
    DELETE FROM "Assunto";
    DELETE FROM "Autor";
    
    -- Resetar sequências
    ALTER SEQUENCE "Livro_CodI_seq" RESTART WITH 1;
    ALTER SEQUENCE "Autor_CodAu_seq" RESTART WITH 1;
    ALTER SEQUENCE "Assunto_CodAs_seq" RESTART WITH 1;
    ALTER SEQUENCE "FormaCompra_CodFc_seq" RESTART WITH 1;
    ALTER SEQUENCE "LivroPreco_Id_seq" RESTART WITH 1;
    
    RAISE NOTICE '✅ Banco de dados limpo com sucesso!';
END;
$$;

-- ============================================
-- INSTRUÇÕES DE USO
-- ============================================

/*
-- Para popular o banco completo (recomendado):
CALL sp_popular_banco_completo();

-- Para inserir dados individuais:
CALL sp_inserir_assunto('Terror');
CALL sp_inserir_autor('Stephen Hawking');
CALL sp_inserir_forma_compra('Empréstimo');

-- Para inserir um livro completo manualmente:
CALL sp_inserir_livro_completo(
    'O Senhor dos Anéis',           -- título
    'HarperCollins',                -- editora
    1,                              -- edição
    '1954',                         -- ano
    ARRAY[1, 2],                    -- IDs dos autores
    ARRAY[12, 10],                  -- IDs dos assuntos
    ARRAY[1, 3],                    -- IDs das formas de compra
    ARRAY[79.90, 39.90]            -- preços correspondentes
);

-- Para limpar tudo e recomeçar:
CALL sp_limpar_banco();
CALL sp_popular_banco_completo();

-- Para verificar os dados:
SELECT * FROM vw_livros_por_autor ORDER BY "AutorNome", "LivroTitulo";
*/

-- ============================================
-- ✅ PROCEDURES CRIADAS COM SUCESSO!
-- Execute: CALL sp_popular_banco_completo();
-- ============================================


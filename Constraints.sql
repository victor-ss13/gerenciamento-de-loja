-- ============================================
-- CONSTRAINTS PARA VALORES MONETÁRIOS
-- ============================================

-- Produtos: Preço deve ser positivo
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Produtos_Preco')
    ALTER TABLE Produtos 
    ADD CONSTRAINT CK_Produtos_Preco CHECK (Preco > 0);

-- ItemPedido: Preço deve ser positivo
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ItemPedido_Preco')
    ALTER TABLE ItemPedido 
    ADD CONSTRAINT CK_ItemPedido_Preco CHECK (Preco > 0);

-- ItemPedido: Quantidade deve ser positiva
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ItemPedido_Quantidade')
    ALTER TABLE ItemPedido 
    ADD CONSTRAINT CK_ItemPedido_Quantidade CHECK (Quantidade > 0);

-- Pagamentos: Valor deve ser positivo
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Pagamentos_Valor')
    ALTER TABLE Pagamentos 
    ADD CONSTRAINT CK_Pagamentos_Valor CHECK (Valor > 0);


-- ============================================
-- CONSTRAINTS PARA ESTOQUE
-- ============================================

-- Produtos: Estoque não pode ser negativo
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Produtos_Estoque')
    ALTER TABLE Produtos 
    ADD CONSTRAINT CK_Produtos_Estoque CHECK (Estoque >= 0);


-- ============================================
-- CONSTRAINTS PARA SITUAÇÃO (Valores permitidos)
-- ============================================

-- Categorias
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Categorias_Situacao')
    ALTER TABLE Categorias 
    ADD CONSTRAINT CK_Categorias_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'));

-- Clientes
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Clientes_Situacao')
    ALTER TABLE Clientes 
    ADD CONSTRAINT CK_Clientes_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'));

-- Produtos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Produtos_Situacao')
    ALTER TABLE Produtos 
    ADD CONSTRAINT CK_Produtos_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'));

-- Pedidos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Pedidos_Situacao')
    ALTER TABLE Pedidos 
    ADD CONSTRAINT CK_Pedidos_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'));

-- ItemPedido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ItemPedido_Situacao')
    ALTER TABLE ItemPedido 
    ADD CONSTRAINT CK_ItemPedido_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'));

-- Pagamentos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Pagamentos_Situacao')
    ALTER TABLE Pagamentos 
    ADD CONSTRAINT CK_Pagamentos_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'));


-- ============================================
-- CONSTRAINTS PARA CAMPOS OBRIGATÓRIOS
-- ============================================

-- Categorias: Nome obrigatório
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Categorias_Nome')
    ALTER TABLE Categorias 
    ADD CONSTRAINT CK_Categorias_Nome CHECK (Nome IS NOT NULL AND LEN(TRIM(Nome)) > 0);

-- Clientes: Nome obrigatório
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Clientes_Nome')
    ALTER TABLE Clientes 
    ADD CONSTRAINT CK_Clientes_Nome CHECK (Nome IS NOT NULL AND LEN(TRIM(Nome)) > 0);

-- Produtos: Nome obrigatório
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Produtos_Nome')
    ALTER TABLE Produtos 
    ADD CONSTRAINT CK_Produtos_Nome CHECK (Nome IS NOT NULL AND LEN(TRIM(Nome)) > 0);

-- Pagamentos: Método obrigatório
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Pagamentos_Metodo')
    ALTER TABLE Pagamentos 
    ADD CONSTRAINT CK_Pagamentos_Metodo CHECK (Metodo IS NOT NULL AND LEN(TRIM(Metodo)) > 0);
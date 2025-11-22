-- ============================================
-- CRIAR BANCO DE DADOS
-- ============================================

-- Verificar se o banco existe e deletar (CUIDADO: isso apaga tudo!)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'LojaGerenciamento')
BEGIN
    ALTER DATABASE LojaGerenciamento SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LojaGerenciamento;
END
GO

-- Criar o banco de dados
CREATE DATABASE LojaGerenciamento;
GO

-- Usar o banco
USE LojaGerenciamento;
GO

-- ============================================
-- CRIAR TABELAS
-- ============================================

-- Tabela: Categorias
CREATE TABLE Categorias (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Situacao NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    
    CONSTRAINT CK_Categorias_Nome CHECK (LEN(TRIM(Nome)) > 0),
    CONSTRAINT CK_Categorias_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'))
);

-- Tabela: Clientes
CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NULL,
    Telefone NVARCHAR(20) NULL,
    Situacao NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    
    CONSTRAINT CK_Clientes_Nome CHECK (LEN(TRIM(Nome)) > 0),
    CONSTRAINT CK_Clientes_Situacao CHECK (Situacao IN ('Ativo', 'Excluido'))
);

-- Tabela: Produtos
CREATE TABLE Produtos (
    IdProduto INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Preco DECIMAL(18,2) NOT NULL,
    Estoque INT NOT NULL DEFAULT 0,
    IdCategoria INT NOT NULL,
    Situacao NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    
    CONSTRAINT CK_Produtos_Nome CHECK (LEN(TRIM(Nome)) > 0),
    CONSTRAINT CK_Produtos_Preco CHECK (Preco > 0),
    CONSTRAINT CK_Produtos_Estoque CHECK (Estoque >= 0),
    CONSTRAINT CK_Produtos_Situacao CHECK (Situacao IN ('Ativo', 'Excluido')),
    CONSTRAINT FK_Produtos_Categorias FOREIGN KEY (IdCategoria) 
        REFERENCES Categorias(IdCategoria)
);

-- Tabela: Pedidos
CREATE TABLE Pedidos (
    IdPedido INT IDENTITY(1,1) PRIMARY KEY,
    Data DATETIME NOT NULL DEFAULT GETDATE(),
    IdCliente INT NOT NULL,
    Situacao NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    
    CONSTRAINT CK_Pedidos_Situacao CHECK (Situacao IN ('Ativo', 'Excluido')),
    CONSTRAINT FK_Pedidos_Clientes FOREIGN KEY (IdCliente) 
        REFERENCES Clientes(IdCliente)
);

-- Tabela: ItemPedido
CREATE TABLE ItemPedido (
    IdItemPedido INT IDENTITY(1,1) PRIMARY KEY,
    IdPedido INT NOT NULL,
    IdProduto INT NOT NULL,
    Quantidade INT NOT NULL,
    Preco DECIMAL(18,2) NOT NULL,
    Situacao NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    
    CONSTRAINT CK_ItemPedido_Quantidade CHECK (Quantidade > 0),
    CONSTRAINT CK_ItemPedido_Preco CHECK (Preco > 0),
    CONSTRAINT CK_ItemPedido_Situacao CHECK (Situacao IN ('Ativo', 'Excluido')),
    CONSTRAINT FK_ItemPedido_Pedidos FOREIGN KEY (IdPedido) 
        REFERENCES Pedidos(IdPedido) ON DELETE CASCADE,
    CONSTRAINT FK_ItemPedido_Produtos FOREIGN KEY (IdProduto) 
        REFERENCES Produtos(IdProduto)
);

-- Tabela: Pagamentos
CREATE TABLE Pagamentos (
    IdPagamento INT IDENTITY(1,1) PRIMARY KEY,
    IdPedido INT NOT NULL,
    Valor DECIMAL(18,2) NOT NULL,
    DataPagamento DATETIME NOT NULL DEFAULT GETDATE(),
    Metodo NVARCHAR(50) NOT NULL,
    Situacao NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    
    CONSTRAINT CK_Pagamentos_Valor CHECK (Valor > 0),
    CONSTRAINT CK_Pagamentos_Metodo CHECK (LEN(TRIM(Metodo)) > 0),
    CONSTRAINT CK_Pagamentos_Situacao CHECK (Situacao IN ('Ativo', 'Excluido')),
    CONSTRAINT FK_Pagamentos_Pedidos FOREIGN KEY (IdPedido) 
        REFERENCES Pedidos(IdPedido) ON DELETE CASCADE
);

GO

-- ============================================
-- CRIAR ÍNDICES PARA FOREIGN KEYS
-- ============================================

CREATE INDEX IX_Produtos_IdCategoria ON Produtos(IdCategoria);
CREATE INDEX IX_Pedidos_IdCliente ON Pedidos(IdCliente);
CREATE INDEX IX_ItemPedido_IdPedido ON ItemPedido(IdPedido);
CREATE INDEX IX_ItemPedido_IdProduto ON ItemPedido(IdProduto);
CREATE INDEX IX_Pagamentos_IdPedido ON Pagamentos(IdPedido);

GO

-- ============================================
-- CRIAR ÍNDICES PARA FILTROS POR SITUAÇÃO
-- ============================================

CREATE INDEX IX_Categorias_Situacao ON Categorias(Situacao);
CREATE INDEX IX_Clientes_Situacao ON Clientes(Situacao);
CREATE INDEX IX_Produtos_Situacao ON Produtos(Situacao);
CREATE INDEX IX_Pedidos_Situacao ON Pedidos(Situacao);
CREATE INDEX IX_ItemPedido_Situacao ON ItemPedido(Situacao);
CREATE INDEX IX_Pagamentos_Situacao ON Pagamentos(Situacao);

GO

-- ============================================
-- CRIAR ÍNDICES PARA FILTROS POR DATA
-- ============================================

CREATE INDEX IX_Pedidos_Data ON Pedidos(Data);
CREATE INDEX IX_Pagamentos_DataPagamento ON Pagamentos(DataPagamento);

GO

-- ============================================
-- CRIAR ÍNDICES COMPOSTOS
-- ============================================

CREATE INDEX IX_Pedidos_IdCliente_Situacao ON Pedidos(IdCliente, Situacao);
CREATE INDEX IX_Produtos_IdCategoria_Situacao ON Produtos(IdCategoria, Situacao);
CREATE INDEX IX_ItemPedido_IdPedido_Situacao ON ItemPedido(IdPedido, Situacao);
CREATE INDEX IX_Pagamentos_IdPedido_Situacao ON Pagamentos(IdPedido, Situacao);

GO

-- ============================================
-- VERIFICAÇÃO FINAL
-- ============================================

-- Listar todas as tabelas criadas
SELECT 
    name AS Tabela,
    create_date AS DataCriacao
FROM 
    sys.tables
ORDER BY 
    name;

-- Listar todas as Foreign Keys
SELECT 
    fk.name AS 'Foreign_Key',
    OBJECT_NAME(fk.parent_object_id) AS 'Tabela',
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 'Coluna',
    OBJECT_NAME(fk.referenced_object_id) AS 'Tabela_Referenciada',
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS 'Coluna_Referenciada'
FROM 
    sys.foreign_keys AS fk
INNER JOIN 
    sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY 
    OBJECT_NAME(fk.parent_object_id);

-- Listar todos os Índices
SELECT 
    t.name AS Tabela,
    i.name AS Indice,
    i.type_desc AS Tipo
FROM 
    sys.indexes i
INNER JOIN 
    sys.tables t ON i.object_id = t.object_id
WHERE 
    i.name IS NOT NULL
ORDER BY 
    t.name, i.name;

-- Listar todas as Constraints
SELECT 
    t.name AS Tabela,
    cc.name AS Constraint,
    cc.definition AS Definicao
FROM 
    sys.check_constraints cc
INNER JOIN 
    sys.tables t ON cc.parent_object_id = t.object_id
ORDER BY 
    t.name, cc.name;

GO

PRINT ' Banco de dados LojaGerenciamento criado com sucesso!';
PRINT ' 6 Tabelas criadas: Categorias, Clientes, Produtos, Pedidos, ItemPedido, Pagamentos';
PRINT ' 5 Foreign Keys configuradas';
PRINT ' 16 Índices criados';
PRINT ' 15 Constraints de validação aplicadas';
GO
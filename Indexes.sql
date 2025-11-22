-- ============================================
-- ÍNDICES PARA FOREIGN KEYS
-- ============================================

-- Produtos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_IdCategoria')
    CREATE INDEX IX_Produtos_IdCategoria ON Produtos(IdCategoria);

-- Pedidos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedidos_IdCliente')
    CREATE INDEX IX_Pedidos_IdCliente ON Pedidos(IdCliente);

-- ItemPedido
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ItemPedido_IdPedido')
    CREATE INDEX IX_ItemPedido_IdPedido ON ItemPedido(IdPedido);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ItemPedido_IdProduto')
    CREATE INDEX IX_ItemPedido_IdProduto ON ItemPedido(IdProduto);

-- Pagamentos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pagamentos_IdPedido')
    CREATE INDEX IX_Pagamentos_IdPedido ON Pagamentos(IdPedido);


-- ============================================
-- ÍNDICES PARA FILTROS POR SITUAÇÃO
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categorias_Situacao')
    CREATE INDEX IX_Categorias_Situacao ON Categorias(Situacao);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clientes_Situacao')
    CREATE INDEX IX_Clientes_Situacao ON Clientes(Situacao);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_Situacao')
    CREATE INDEX IX_Produtos_Situacao ON Produtos(Situacao);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedidos_Situacao')
    CREATE INDEX IX_Pedidos_Situacao ON Pedidos(Situacao);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ItemPedido_Situacao')
    CREATE INDEX IX_ItemPedido_Situacao ON ItemPedido(Situacao);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pagamentos_Situacao')
    CREATE INDEX IX_Pagamentos_Situacao ON Pagamentos(Situacao);


-- ============================================
-- ÍNDICES PARA FILTROS POR DATA
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedidos_Data')
    CREATE INDEX IX_Pedidos_Data ON Pedidos(Data);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pagamentos_DataPagamento')
    CREATE INDEX IX_Pagamentos_DataPagamento ON Pagamentos(DataPagamento);


-- ============================================
-- ÍNDICES COMPOSTOS (para queries mais complexas)
-- ============================================

-- Pedidos ativos por cliente
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedidos_IdCliente_Situacao')
    CREATE INDEX IX_Pedidos_IdCliente_Situacao ON Pedidos(IdCliente, Situacao);

-- Produtos ativos por categoria
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_IdCategoria_Situacao')
    CREATE INDEX IX_Produtos_IdCategoria_Situacao ON Produtos(IdCategoria, Situacao);

-- Itens ativos por pedido
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ItemPedido_IdPedido_Situacao')
    CREATE INDEX IX_ItemPedido_IdPedido_Situacao ON ItemPedido(IdPedido, Situacao);

-- Pagamentos ativos por pedido
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pagamentos_IdPedido_Situacao')
    CREATE INDEX IX_Pagamentos_IdPedido_Situacao ON Pagamentos(IdPedido, Situacao);
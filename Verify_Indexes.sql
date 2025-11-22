SELECT 
    t.name AS Tabela,
    i.name AS Indice,
    i.type_desc AS Tipo
FROM 
    sys.indexes i
INNER JOIN 
    sys.tables t ON i.object_id = t.object_id
WHERE 
    t.name IN ('Categorias', 'Clientes', 'Produtos', 'Pedidos', 'ItemPedido', 'Pagamentos')
    AND i.name IS NOT NULL
ORDER BY 
    t.name, i.name;
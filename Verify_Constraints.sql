SELECT 
    t.name AS Tabela,
    cc.name AS Constraint,
    cc.definition AS Definicao
FROM 
    sys.check_constraints cc
INNER JOIN 
    sys.tables t ON cc.parent_object_id = t.object_id
WHERE 
    t.name IN ('Categorias', 'Clientes', 'Produtos', 'Pedidos', 'ItemPedido', 'Pagamentos')
ORDER BY 
    t.name, cc.name;
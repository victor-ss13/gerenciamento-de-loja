using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.ItemPedido;
using LojaGerenciamento.Application.Models.Pagamento;
using LojaGerenciamento.Application.Models.Pedido;
using LojaGerenciamento.Domain.Classes;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LojaGerenciamento.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly LojaContext _context;

        public PedidoService(LojaContext context)
        {
            _context = context;
        }

        #region CRUD Básico

        public async Task<Response<PedidoResponseModel>> ObterPorIdAsync(int id)
        {
            var retorno = new Response<PedidoResponseModel>
            {
                Data = new PedidoResponseModel()
            };

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == id && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            retorno.Data.IdPedido = pedido.IdPedido;
            retorno.Data.Data = pedido.Data;
            retorno.Data.IdCliente = pedido.IdCliente;
            retorno.Data.Cliente = pedido.Cliente.Nome;
            retorno.Data.Situacao = pedido.Situacao;
            retorno.Data.ValorTotal = pedido.ObterValorTotal();
            retorno.Data.ValorPago = pedido.ObterValorPago();
            retorno.Data.ValorPendente = pedido.ObterValorTotal() - pedido.ObterValorPago();
            retorno.Data.EstaQuitado = pedido.EstaQuitado();
            retorno.Data.QuantidadeItens = pedido.Itens.Count(i => i.Situacao == "Ativo");

            retorno.Data.Itens = pedido.Itens
                .Where(i => i.Situacao == "Ativo")
                .Select(i => new ItemPedidoResponseModel
                {
                    IdItemPedido = i.IdItemPedido,
                    IdPedido = i.IdPedido,
                    Pedido = $"Pedido #{i.Pedido.IdPedido} - {i.Pedido.Data:dd/MM/yyyy}",
                    IdProduto = i.IdProduto,
                    Produto = i.Produto.Nome,
                    Quantidade = i.Quantidade,
                    Preco = i.Preco
                }).ToList();

            retorno.Data.Pagamentos = pedido.Pagamentos
                .Where(p => p.Situacao == "Ativo")
                .Select(p => new PagamentoResponseModel
                {
                    IdPagamento = p.IdPagamento,
                    IdPedido = p.IdPedido,
                    Pedido = $"Pedido #{p.Pedido.IdPedido} - {p.Pedido.Data:dd/MM/yyyy}",
                    Valor = p.Valor,
                    DataPagamento = p.DataPagamento,
                    Metodo = p.Metodo,
                    Situacao = p.Situacao
                }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Pedido => {id}";
            return retorno;
        }

        public async Task<Response<ListarPedidoResponseModel>> ListarPedidosAsync(FiltroPedidoRequestModel request)
        {
            var retorno = new Response<ListarPedidoResponseModel>
            {
                Data = new ListarPedidoResponseModel()
            };

            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Pagamentos)
                .Where(p => p.Situacao == "Ativo")
                .AsQueryable();

            // Filtro por ID do Pedido
            if (request.IdPedido.HasValue && request.IdPedido.Value > 0)
            {
                query = query.Where(p => p.IdPedido == request.IdPedido.Value);
            }

            // Filtro por ID do Cliente
            if (request.IdCliente.HasValue && request.IdCliente.Value > 0)
            {
                query = query.Where(p => p.IdCliente == request.IdCliente.Value);
            }

            // Filtro por Data - Início
            if (request.DataInicio.HasValue)
            {
                query = query.Where(p => p.Data.Date >= request.DataInicio.Value.Date);
            }

            // Filtro por Data - Fim
            if (request.DataFim.HasValue)
            {
                var dataFinalComHora = request.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.Data <= dataFinalComHora);
            }

            // Filtro por Valor Total - Mínimo
            if (request.ValorTotalMinimo.HasValue)
            {
                query = query.Where(p => p.Itens.Where(i => i.Situacao == "Ativo").Sum(i => i.Preco * i.Quantidade) >= request.ValorTotalMinimo.Value);
            }

            // Filtro por Valor Total - Máximo
            if (request.ValorTotalMaximo.HasValue)
            {
                query = query.Where(p => p.Itens.Where(i => i.Situacao == "Ativo").Sum(i => i.Preco * i.Quantidade) <= request.ValorTotalMaximo.Value);
            }

            var pedidos = await query.OrderByDescending(p => p.Data).ToListAsync();

            // Filtros que dependem de cálculos (aplica após carregar do banco)
            if (request.ApenasQuitados.HasValue && request.ApenasQuitados.Value)
            {
                pedidos = pedidos.Where(p => p.EstaQuitado()).ToList();
            }

            if (request.ApenasPendentes.HasValue && request.ApenasPendentes.Value)
            {
                pedidos = pedidos.Where(p => !p.EstaQuitado()).ToList();
            }

            foreach (var pedido in pedidos)
            {
                retorno.Data.Lista.Add(new PedidoResponseModel
                {
                    IdPedido = pedido.IdPedido,
                    Data = pedido.Data,
                    IdCliente = pedido.IdCliente,
                    Cliente = pedido.Cliente.Nome,
                    Situacao = pedido.Situacao,
                    ValorTotal = pedido.ObterValorTotal(),
                    ValorPago = pedido.ObterValorPago(),
                    ValorPendente = pedido.ObterValorTotal() - pedido.ObterValorPago(),
                    EstaQuitado = pedido.EstaQuitado(),
                    QuantidadeItens = pedido.Itens.Count(i => i.Situacao == "Ativo"),
                    Itens = pedido.Itens
                        .Where(i => i.Situacao == "Ativo")
                        .Select(i => new ItemPedidoResponseModel
                        {
                            IdItemPedido = i.IdItemPedido,
                            IdPedido = i.IdPedido,
                            Pedido = $"Pedido #{i.Pedido.IdPedido} - {i.Pedido.Data:dd/MM/yyyy}",
                            IdProduto = i.IdProduto,
                            Produto = i.Produto.Nome,
                            Quantidade = i.Quantidade,
                            Preco = i.Preco
                        }).ToList(),
                    Pagamentos = pedido.Pagamentos
                        .Where(p => p.Situacao == "Ativo")
                        .Select(p => new PagamentoResponseModel
                        {
                            IdPagamento = p.IdPagamento,
                            IdPedido = p.IdPedido,
                            Pedido = $"Pedido #{p.Pedido.IdPedido} - {p.Pedido.Data:dd/MM/yyyy}",
                            Valor = p.Valor,
                            DataPagamento = p.DataPagamento,
                            Metodo = p.Metodo,
                            Situacao = p.Situacao
                        }).ToList()
                });
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Lista de pedidos";
            return retorno;
        }

        public async Task<Response<PedidoResponseModel>> CriarPedidoAsync(CriarPedidoRequestModel request)
        {
            var retorno = new Response<PedidoResponseModel>();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == request.IdCliente  && c.Situacao == "Ativo");

            if (cliente == null)
                throw new Exception("Cliente não encontrado");

            var pedido = new Pedido(cliente);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pedido criado com sucesso";
            return retorno;
        }

        public async Task<Response<PedidoResponseModel>> AtualizarPedidoAsync(AtualizarPedidoRequestModel request)
        {
            var retorno = new Response<PedidoResponseModel>();

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == request.IdCliente && c.Situacao == "Ativo");

            if (cliente == null)
                throw new Exception("Cliente não encontrado");

            pedido.Alterar(cliente);

            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pedido atualizado com sucesso";
            return retorno;
        }

        public async Task<Response<PedidoResponseModel>> ExcluirPedidoAsync(int id)
        {
            var retorno = new Response<PedidoResponseModel>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == id && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            // Devolve os itens ao estoque antes de excluir
            foreach (var item in pedido.Itens.Where(i => i.Situacao == "Ativo"))
            {
                item.Produto.AdicionarEstoque(item.Quantidade);
                _context.Produtos.Update(item.Produto);
            }

            pedido.Excluir();

            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pedido excluído com sucesso";
            return retorno;
        }

        #endregion

        #region Gerenciamento de Itens

        public async Task<Response<PedidoResponseModel>> AdicionarItemAsync(AdicionarItemRequestModel request)
        {
            var retorno = new Response<PedidoResponseModel>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto && p.Situacao == "Ativo");

            if (produto == null)
                throw new Exception("Produto não encontrado");

            // Verifica estoque
            if (produto.Estoque < request.Quantidade)
                throw new Exception($"Estoque insuficiente. Disponível: {produto.Estoque}");

            var item = new ItemPedido(pedido, produto, request.Quantidade, request.Preco);

            pedido.AdicionarItem(item);
            produto.RemoverEstoque(request.Quantidade);

            _context.Pedidos.Update(pedido);
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Item adicionado ao pedido com sucesso";
            return retorno;
        }

        public async Task<Response<PedidoResponseModel>> RemoverItemAsync(RemoverItemRequestModel request)
        {
            var retorno = new Response<PedidoResponseModel>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var item = pedido.Itens.FirstOrDefault(i => i.IdItemPedido == request.IdItemPedido && i.Situacao == "Ativo");

            if (item == null)
                throw new Exception("Item não encontrado no pedido");

            // Devolve ao estoque
            item.Produto.AdicionarEstoque(item.Quantidade);

            pedido.RemoverItem(request.IdItemPedido);

            _context.Pedidos.Update(pedido);
            _context.Produtos.Update(item.Produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Item removido do pedido com sucesso";
            return retorno;
        }

        public async Task<Response<PedidoResponseModel>> AtualizarQuantidadeItemAsync(AtualizarQuantidadeItemRequestModel request)
        {
            var retorno = new Response<PedidoResponseModel>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var item = pedido.Itens.FirstOrDefault(i => i.IdItemPedido == request.IdItemPedido && i.Situacao == "Ativo");

            if (item == null)
                throw new Exception("Item não encontrado no pedido");

            var quantidadeAnterior = item.Quantidade;
            var diferenca = request.NovaQuantidade - quantidadeAnterior;

            if (diferenca > 0)
            {
                // Aumentou - remove do estoque
                if (item.Produto.Estoque < diferenca)
                    throw new Exception($"Estoque insuficiente. Disponível: {item.Produto.Estoque}");

                item.Produto.RemoverEstoque(diferenca);
            }
            else if (diferenca < 0)
            {
                // Diminuiu - devolve ao estoque
                item.Produto.AdicionarEstoque(Math.Abs(diferenca));
            }

            pedido.AtualizarQuantidadeItem(request.IdItemPedido, request.NovaQuantidade);

            _context.Pedidos.Update(pedido);
            _context.Produtos.Update(item.Produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Quantidade do item atualizada com sucesso";
            return retorno;
        }

        #endregion

        #region Gerenciamento de Pagamentos

        public async Task<Response<PedidoResponseModel>> AdicionarPagamentoAsync(AdicionarPagamentoRequestModel request)
        {
            var retorno = new Response<PedidoResponseModel>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            // Verifica se não ultrapassa o valor total
            var valorTotal = pedido.ObterValorTotal();
            var valorPago = pedido.ObterValorPago();
            var valorRestante = valorTotal - valorPago;

            if (request.Valor > valorRestante)
                throw new Exception($"Valor do pagamento (R$ {request.Valor:N2}) ultrapassa o valor restante (R$ {valorRestante:N2})");

            var pagamento = new Pagamento(pedido, request.Valor, request.Metodo);

            pedido.AdicionarPagamento(pagamento);

            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pagamento adicionado ao pedido com sucesso";
            return retorno;
        }

        #endregion

        #region Consultas e Cálculos

        public async Task<Response<decimal>> ObterValorTotalAsync(int idPedido)
        {
            var retorno = new Response<decimal>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            retorno.Data = pedido.ObterValorTotal();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Valor total do pedido => {idPedido}";
            return retorno;
        }

        public async Task<Response<decimal>> ObterValorPagoAsync(int idPedido)
        {
            var retorno = new Response<decimal>();

            var pedido = await _context.Pedidos
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            retorno.Data = pedido.ObterValorPago();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Valor pago do pedido => {idPedido}";
            return retorno;
        }

        public async Task<Response<bool>> VerificarSeEstaQuitadoAsync(int idPedido)
        {
            var retorno = new Response<bool>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            retorno.Data = pedido.EstaQuitado();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Status de quitação do pedido => {idPedido}";
            return retorno;
        }

        #endregion
    }
}

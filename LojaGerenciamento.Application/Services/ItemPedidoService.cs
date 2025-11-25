using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.ItemPedido;
using LojaGerenciamento.Domain.Classes;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LojaGerenciamento.Application.Services
{
    public class ItemPedidoService  :IItemPedidoService
    {
        private readonly LojaContext _context;

        public ItemPedidoService(LojaContext context)
        {
            _context = context;
        }

        public async Task<Response<ItemPedidoResponseModel>> ObterPorIdAsync(int id)
        {
            var retorno = new Response<ItemPedidoResponseModel>
            {
                Data = new ItemPedidoResponseModel()
            };

            var item = await _context.ItensPedidos
                .Include(x => x.Pedido)
                .Include(x => x.Produto)
                .FirstOrDefaultAsync(x => x.IdItemPedido == id);

            if (item == null)
                throw new Exception("Item do pedido não encontrado");

            retorno.Data.IdItemPedido = item.IdItemPedido;
            retorno.Data.IdPedido = item.IdPedido;
            retorno.Data.Pedido = $"Pedido #{item.Pedido.IdPedido} - {item.Pedido.Data:dd/MM/yyyy}";
            retorno.Data.IdProduto = item.IdProduto;
            retorno.Data.Produto = item.Produto.Nome;
            retorno.Data.Quantidade = item.Quantidade;
            retorno.Data.Preco = item.Preco;

            retorno.Sucesso = true;
            retorno.Mensagem = $"Item do pedido => {id}";
            return retorno;
        }

        public async Task<Response<ListarItemPedidoResponseModel>> ListarItensAsync(FiltroItemPedidoRequestModel request)
        {
            var retorno = new Response<ListarItemPedidoResponseModel>
            {
                Data = new ListarItemPedidoResponseModel()
            };

            var query = _context.ItensPedidos
                .Include(x => x.Pedido)
                .Include(x => x.Produto)
                .Where(x => x.Situacao == "Ativo")
                .AsQueryable();

            var itens = await query.OrderByDescending(x => x.Pedido.Data).ToListAsync();

            foreach (var item in itens)
            {
                retorno.Data.Lista.Add(new ItemPedidoResponseModel
                {
                    IdItemPedido = item.IdItemPedido,
                    IdPedido = item.IdPedido,
                    Pedido = $"Pedido #{item.Pedido.IdPedido} - {item.Pedido.Data:dd/MM/yyyy}",
                    IdProduto = item.IdProduto,
                    Produto = item.Produto.Nome,
                    Quantidade = item.Quantidade,
                    Preco = item.Preco
                });
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Lista de itens de pedidos";
            return retorno;
        }

        public async Task<Response<ItemPedidoResponseModel>> CriarItemAsync(CriarItemPedidoRequestModel request)
        {
            var retorno = new Response<ItemPedidoResponseModel>();

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido);

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            // Verifica se há estoque suficiente
            if (produto.Estoque < request.Quantidade)
                throw new Exception("Estoque insuficiente");

            var item = new ItemPedido(pedido, produto, request.Quantidade, request.Preco);

            // Remove do estoque
            produto.RemoverEstoque(request.Quantidade);

            _context.ItensPedidos.Add(item);
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Item adicionado ao pedido com sucesso";
            return retorno;
        }

        public async Task<Response<ItemPedidoResponseModel>> AtualizarItemAsync(AtualizarItemPedidoRequestModel request)
        {
            var retorno = new Response<ItemPedidoResponseModel>();

            var item = await _context.ItensPedidos
                .Include(ip => ip.Produto)
                .FirstOrDefaultAsync(ip => ip.IdItemPedido == request.IdItemPedido);

            if (item == null)
                throw new Exception("Item do pedido não encontrado");

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido);

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            // Devolve quantidade antiga ao estoque
            item.Produto.AdicionarEstoque(item.Quantidade);

            // Verifica se há estoque para a nova quantidade
            if (produto.Estoque < request.Quantidade)
                throw new Exception("Estoque insuficiente");

            // Remove nova quantidade do estoque
            produto.RemoverEstoque(request.Quantidade);

            item.Alterar(pedido, produto, request.Quantidade, request.Preco);

            _context.ItensPedidos.Update(item);
            _context.Produtos.Update(item.Produto);
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Item do pedido atualizado com sucesso";
            return retorno;
        }

        public async Task<Response<ItemPedidoResponseModel>> AtualizarQuantidadeAsync(int idItem, int novaQuantidade)
        {
            var retorno = new Response<ItemPedidoResponseModel>();

            var item = await _context.ItensPedidos
                .Include(ip => ip.Produto)
                .FirstOrDefaultAsync(ip => ip.IdItemPedido == idItem);

            if (item == null)
                throw new Exception("Item do pedido não encontrado");

            var quantidadeAnterior = item.Quantidade;
            var diferenca = novaQuantidade - quantidadeAnterior;

            if (diferenca > 0)
            {
                // Aumentou quantidade - precisa remover do estoque
                if (item.Produto.Estoque < diferenca)
                    throw new Exception("Estoque insuficiente");

                item.Produto.RemoverEstoque(diferenca);
            }
            else if (diferenca < 0)
            {
                // Diminuiu quantidade - devolve ao estoque
                item.Produto.AdicionarEstoque(Math.Abs(diferenca));
            }

            item.AlterarQuantidade(novaQuantidade);

            _context.ItensPedidos.Update(item);
            _context.Produtos.Update(item.Produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Quantidade do item atualizada com sucesso";
            return retorno;
        }

        public async Task<Response<ItemPedidoResponseModel>> ExcluirItemAsync(int id)
        {
            var retorno = new Response<ItemPedidoResponseModel>();

            var item = await _context.ItensPedidos
                .Include(ip => ip.Produto)
                .FirstOrDefaultAsync(ip => ip.IdItemPedido == id);

            if (item == null)
                throw new Exception("Item do pedido não encontrado");

            // Devolve ao estoque
            item.Produto.AdicionarEstoque(item.Quantidade);

            item.Excluir();

            _context.ItensPedidos.Update(item);
            _context.Produtos.Update(item.Produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Item do pedido excluído com sucesso";
            return retorno;
        }

        public async Task<Response<List<ItemPedidoResponseModel>>> ListarItensPorPedidoAsync(int idPedido)
        {
            var retorno = new Response<List<ItemPedidoResponseModel>>
            {
                Data = new List<ItemPedidoResponseModel>()
            };

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var itens = await _context.ItensPedidos
                .Include(ip => ip.Pedido)
                .Include(ip => ip.Produto)
                .Where(ip => ip.IdPedido == idPedido && ip.Situacao == "Ativo")
                .ToListAsync();

            retorno.Data = itens.Select(item => new ItemPedidoResponseModel
            {
                IdItemPedido = item.IdItemPedido,
                IdPedido = item.IdPedido,
                Pedido = $"Pedido #{item.Pedido.IdPedido} - {item.Pedido.Data:dd/MM/yyyy}",
                IdProduto = item.IdProduto,
                Produto = item.Produto.Nome,
                Quantidade = item.Quantidade,
                Preco = item.Preco
            }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Itens do pedido => {idPedido}";
            return retorno;
        }
    }
}

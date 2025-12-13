using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Produto;
using LojaGerenciamento.Domain.Classes;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LojaGerenciamento.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly LojaContext _context;

        public ProdutoService(LojaContext context)
        {
            _context = context;
        }

        #region CRUD Básico

        public async Task<Response<ProdutoResponseModel>> ObterPorIdAsync(int id)
        {
            var retorno = new Response<ProdutoResponseModel>
            {
                Data = new ProdutoResponseModel()
            };

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            retorno.Data.IdProduto = produto.IdProduto;
            retorno.Data.Nome = produto.Nome;
            retorno.Data.Preco = produto.Preco;
            retorno.Data.Estoque = produto.Estoque;
            retorno.Data.IdCategoria = produto.IdCategoria;
            retorno.Data.Categoria = produto.Categoria.Nome;
            retorno.Data.Situacao = produto.Situacao;
            retorno.Data.TemEstoque = produto.Estoque > 0;

            retorno.Sucesso = true;
            retorno.Mensagem = $"Produto => {id}";
            return retorno;
        }

        public async Task<Response<ListarProdutoResponseModel>> ListarProdutosAsync(FiltroProdutoRequestModel request)
        {
            var retorno = new Response<ListarProdutoResponseModel>
            {
                Data = new ListarProdutoResponseModel()
            };

            var query = _context.Produtos
                .Include(p => p.Categoria)
                .AsQueryable();

            // Filtro por ID do Produto
            if (request.IdProduto.HasValue && request.IdProduto.Value > 0)
            {
                query = query.Where(p => p.IdProduto == request.IdProduto.Value);
            }

            // Filtro por Nome
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                query = query.Where(p => p.Nome.Contains(request.Nome));
            }

            // Filtro por Categoria
            if (request.IdCategoria.HasValue && request.IdCategoria.Value > 0)
            {
                query = query.Where(p => p.IdCategoria == request.IdCategoria.Value);
            }

            // Filtro por Preço Mínimo
            if (request.PrecoMinimo.HasValue)
            {
                query = query.Where(p => p.Preco >= request.PrecoMinimo.Value);
            }

            // Filtro por Preço Máximo
            if (request.PrecoMaximo.HasValue)
            {
                query = query.Where(p => p.Preco <= request.PrecoMaximo.Value);
            }

            // Filtro por Estoque Mínimo
            if (request.EstoqueMinimo.HasValue)
            {
                query = query.Where(p => p.Estoque >= request.EstoqueMinimo.Value);
            }

            // Filtro por Estoque Máximo
            if (request.EstoqueMaximo.HasValue)
            {
                query = query.Where(p => p.Estoque <= request.EstoqueMaximo.Value);
            }

            // Filtro: Apenas com estoque
            if (request.ApenasComEstoque.HasValue && request.ApenasComEstoque.Value)
            {
                query = query.Where(p => p.Estoque > 0);
            }

            // Filtro: Apenas sem estoque
            if (request.ApenasSemEstoque.HasValue && request.ApenasSemEstoque.Value)
            {
                query = query.Where(p => p.Estoque == 0);
            }

            var produtos = await query.OrderBy(p => p.Nome).ToListAsync();

            foreach (var item in produtos)
            {
                retorno.Data.Lista.Add(new ProdutoResponseModel
                {
                    IdProduto = item.IdProduto,
                    Nome = item.Nome,
                    Preco = item.Preco,
                    Estoque = item.Estoque,
                    IdCategoria = item.IdCategoria,
                    Categoria = item.Categoria.Nome,
                    Situacao = item.Situacao,
                    TemEstoque = item.Estoque > 0
                });
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Lista de produtos";
            return retorno;
        }

        public async Task<Response<ProdutoResponseModel>> CriarProdutoAsync(CriarProdutoRequestModel request)
        {
            var retorno = new Response<ProdutoResponseModel>();

            // Verifica se já existe produto com o mesmo nome
            var produtoExistente = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Nome.ToLower() == request.Nome.ToLower());

            if (produtoExistente != null)
                throw new Exception("Já existe um produto com este nome");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);

            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            var produto = new Produto(request.Nome, request.Preco, request.Estoque, categoria);

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Produto criado com sucesso";
            return retorno;
        }

        public async Task<Response<ProdutoResponseModel>> AtualizarProdutoAsync(AtualizarProdutoRequestModel request)
        {
            var retorno = new Response<ProdutoResponseModel>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            // Verifica se já existe outro produto com o mesmo nome
            var produtoExistente = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Nome.ToLower() == request.Nome.ToLower()
                                       && p.IdProduto != request.IdProduto);

            if (produtoExistente != null)
                throw new Exception("Já existe outro produto com este nome");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);

            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            produto.Alterar(request.Nome, request.Preco, request.Estoque, categoria);

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Produto atualizado com sucesso";
            return retorno;
        }

        public async Task<Response<ProdutoResponseModel>> ExcluirProdutoAsync(int id)
        {
            var retorno = new Response<ProdutoResponseModel>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            produto.Excluir(); // Valida se tem estoque (não pode excluir com estoque)

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Produto excluído com sucesso";
            return retorno;
        }

        #endregion

        #region Gerenciamento de Preço

        public async Task<Response<ProdutoResponseModel>> AtualizarPrecoAsync(AtualizarPrecoProdutoRequestModel request)
        {
            var retorno = new Response<ProdutoResponseModel>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            produto.AlterarPreco(request.NovoPreco);

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Preço do produto atualizado com sucesso";
            return retorno;
        }

        #endregion

        #region Gerenciamento de Estoque

        public async Task<Response<ProdutoResponseModel>> AdicionarEstoqueAsync(AdicionarEstoqueProdutoRequestModel request)
        {
            var retorno = new Response<ProdutoResponseModel>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            produto.AdicionarEstoque(request.Quantidade);

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Estoque adicionado com sucesso. Estoque atual: {produto.Estoque}";
            return retorno;
        }

        public async Task<Response<ProdutoResponseModel>> RemoverEstoqueAsync(RemoverEstoqueProdutoRequestModel request)
        {
            var retorno = new Response<ProdutoResponseModel>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            produto.RemoverEstoque(request.Quantidade);

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Estoque removido com sucesso. Estoque atual: {produto.Estoque}";
            return retorno;
        }

        public async Task<Response<ProdutoResponseModel>> AtualizarEstoqueAsync(AtualizarEstoqueProdutoRequestModel request)
        {
            var retorno = new Response<ProdutoResponseModel>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            produto.AtualizarEstoque(request.NovoEstoque);

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Estoque atualizado com sucesso. Estoque atual: {produto.Estoque}";
            return retorno;
        }

        #endregion

        #region Consultas

        public async Task<Response<bool>> VerificarEstoqueDisponivelAsync(int idProduto, int quantidade)
        {
            var retorno = new Response<bool>();

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == idProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado");

            retorno.Data = produto.TemEstoqueDisponivel(quantidade);

            retorno.Sucesso = true;
            retorno.Mensagem = retorno.Data
                ? $"Estoque suficiente. Disponível: {produto.Estoque}"
                : $"Estoque insuficiente. Disponível: {produto.Estoque}, Necessário: {quantidade}";
            return retorno;
        }

        public async Task<Response<List<ProdutoResponseModel>>> ListarProdutosPorCategoriaAsync(int idCategoria)
        {
            var retorno = new Response<List<ProdutoResponseModel>>
            {
                Data = new List<ProdutoResponseModel>()
            };

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.IdCategoria == idCategoria && p.Situacao == "Ativo")
                .OrderBy(p => p.Nome)
                .ToListAsync();

            retorno.Data = produtos.Select(item => new ProdutoResponseModel
            {
                IdProduto = item.IdProduto,
                Nome = item.Nome,
                Preco = item.Preco,
                Estoque = item.Estoque,
                IdCategoria = item.IdCategoria,
                Categoria = item.Categoria.Nome,
                Situacao = item.Situacao,
                TemEstoque = item.Estoque > 0
            }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Produtos da categoria => {idCategoria}";
            return retorno;
        }

        #endregion
    }
}

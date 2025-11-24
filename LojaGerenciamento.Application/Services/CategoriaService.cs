using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Categoria;
using LojaGerenciamento.Domain.Classes;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LojaGerenciamento.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly LojaContext _context;

        public CategoriaService(LojaContext context)
        {
            _context = context;
        }

        public async Task<Response<CategoriaResponseModel>> ObterPorIdAsync(int id)
        {
            var retorno = new Response<CategoriaResponseModel>
            {
                Data = new CategoriaResponseModel()
            };

            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == id);
            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            retorno.Data.IdCategoria = categoria.IdCategoria;
            retorno.Data.Nome = categoria.Nome;
            retorno.Data.Situacao = categoria.Situacao;
            retorno.Data.QuantidadeProdutos = categoria.Produtos.Count;
            retorno.Data.Produtos = categoria.Produtos.Select(p => new ProdutoResumoResponseModel
            {
                IdProduto = p.IdProduto,
                Nome = p.Nome,
                Preco = p.Preco,
                Estoque = p.Estoque
            }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Categoria => {id}";
            return retorno;
        }

        public async Task<Response<ListarCategoriaResponseModel>> ListarCategoriasAsync(FiltroCategoriaRequestModel request)
        {
            var retorno = new Response<ListarCategoriaResponseModel>()
            {
                Data = new ListarCategoriaResponseModel()
            };

            var query = _context.Categorias
                .Include(c => c.Produtos)
                .AsQueryable();

            // Filtro por ID
            if (request.IdCategoria.HasValue && request.IdCategoria > 0)
            {
                query = query.Where(c => c.IdCategoria == request.IdCategoria.Value);
            }

            // Filtro por Nome
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                query = query.Where(c => c.Nome.Contains(request.Nome));
            }

            // Filtro: Apenas categorias sem produtos
            if (request.ApenasCategoriasSemProdutos.HasValue && request.ApenasCategoriasSemProdutos.Value)
            {
                query = query.Where(c => !c.Produtos.Any());
            }

            var categorias = await query.OrderBy(c => c.Nome).ToListAsync();

            foreach(var item in categorias)
            {
                retorno.Data.Lista.Add(new CategoriaResponseModel
                {
                    IdCategoria = item.IdCategoria,
                    Nome = item.Nome,
                    Situacao = item.Situacao,
                    QuantidadeProdutos = item.Produtos.Count,
                    Produtos = item.Produtos.Select(p => new ProdutoResumoResponseModel
                    {
                        IdProduto = p.IdProduto,
                        Nome = p.Nome,
                        Preco = p.Preco,
                        Estoque = p.Estoque
                    }).ToList()
                });
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Lista de categorias";
            return retorno;
        }

        public async Task<Response<CategoriaResponseModel>> CriarCategoriaAsync(CriarCategoriaRequestModel request)
        {
            var retorno = new Response<CategoriaResponseModel>();

            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == request.Nome.ToLower());
            if (categoriaExistente != null)
                throw new Exception("Já existe uma categoria com este nome");

            var categoria = new Categoria(request.Nome);

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Categoria criada com sucesso";
            return retorno;
        }

        public async Task<Response<CategoriaResponseModel>> AtualizarCategoriaAsync(AtualizarCategoriaRequestModel request)
        {
            var retorno = new Response<CategoriaResponseModel>();

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);
            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == request.Nome.ToLower() && c.IdCategoria != request.IdCategoria);
            if (categoriaExistente != null)
                throw new Exception("Já existe outra categoria com este nome");

            categoria.Alterar(request.Nome);

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Categoria atualizada com sucesso";
            return retorno;
        }

        public async Task<Response<CategoriaResponseModel>> ExcluirCategoriaAsync(int id)
        {
            var retorno = new Response<CategoriaResponseModel>();

            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            categoria.Excluir();

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Categoria excluída com sucesso";
            return retorno;
        }

        public async Task<Response<CategoriaResponseModel>> AdicionarProdutoAsync(AdicionarProdutoCategoriaRequestModel request)
        {
            var retorno = new Response<CategoriaResponseModel>();

            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);
            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == request.IdProduto);
            if (produto == null)
                throw new Exception("Produto não encontrado");

            categoria.AdicionarProduto(produto);

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Produto adicionado à categoria com sucesso";
            return retorno;
        }

        public async Task<Response<CategoriaResponseModel>> RemoverProdutoAsync(RemoverProdutoCategoriaRequestModel request)
        {
            var retorno = new Response<CategoriaResponseModel>();

            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);
            if (categoria == null)
                throw new Exception("Categoria não encontrada");

            categoria.RemoverProduto(request.IdProduto);

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Produto removido da categoria com sucesso";
            return retorno;
        }
    }
}

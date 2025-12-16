using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Cliente;
using LojaGerenciamento.Domain.Classes;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LojaGerenciamento.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly LojaContext _context;

        public ClienteService(LojaContext context)
        {
            _context = context;
        }

        public async Task<Response<ClienteResponseModel>> ObterPorIdAsync(int id)
        {
            var retorno = new Response<ClienteResponseModel>
            {
                Data = new ClienteResponseModel()
            };

            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Itens)
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Pagamentos)
                .FirstOrDefaultAsync(c => c.IdCliente == id && c.Situacao == "Ativo");

            if (cliente == null)
                throw new Exception("Cliente não encontrado");

            retorno.Data.IdCliente = cliente.IdCliente;
            retorno.Data.Nome = cliente.Nome;
            retorno.Data.Email = cliente.Email;
            retorno.Data.Telefone = cliente.Telefone;
            retorno.Data.Situacao = cliente.Situacao;
            retorno.Data.QuantidadePedidos = cliente.ObterQuantidadePedidos();
            retorno.Data.ValorTotalPedidos = cliente.ObterValorTotalPedidos();
            retorno.Data.Pedidos = cliente.Pedidos
                .Where(p => p.Situacao == "Ativo")
                .Select(p => new PedidoResumoClienteResponseModel
                {
                    IdPedido = p.IdPedido,
                    Data = p.Data,
                    ValorTotal = p.ObterValorTotal(),
                    EstaQuitado = p.EstaQuitado(),
                    Situacao = p.Situacao
                }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Cliente => {id}";
            return retorno;
        }

        public async Task<Response<ListarClienteResponseModel>> ListarClientesAsync(FiltroClienteRequestModel request)
        {
            var retorno = new Response<ListarClienteResponseModel>
            {
                Data = new ListarClienteResponseModel()
            };

            var query = _context.Clientes
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Itens)
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Pagamentos)
                .Where(x => x.Situacao == "Ativo")
                .AsQueryable();

            // Filtro por ID
            if (request.IdCliente.HasValue && request.IdCliente.Value > 0)
            {
                query = query.Where(c => c.IdCliente == request.IdCliente.Value);
            }

            // Filtro por Nome
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                query = query.Where(x => x.Nome.Contains(request.Nome));
            }

            // Filtro: Apenas clientes com pedidos
            if (request.ApenasComPedidos.HasValue && request.ApenasComPedidos.Value)
            {
                query = query.Where(x => x.Pedidos.Any(p => p.Situacao == "Ativo"));
            }

            // Filtro: Apenas clientes sem pedidos
            if (request.ApenasSemPedidos.HasValue && request.ApenasSemPedidos.Value)
            {
                query = query.Where(x => !x.Pedidos.Any(p => p.Situacao == "Ativo"));
            }

            var clientes = await query.OrderBy(x => x.Nome).ToListAsync();


            foreach (var item in clientes)
            {
                retorno.Data.Lista.Add(new ClienteResponseModel
                {
                    IdCliente = item.IdCliente,
                    Nome = item.Nome,
                    Email = item.Email,
                    Telefone = item.Telefone,
                    Situacao = item.Situacao,
                    QuantidadePedidos = item.ObterQuantidadePedidos(),
                    ValorTotalPedidos = item.ObterValorTotalPedidos(),
                    Pedidos = item.Pedidos
                        .Where(p => p.Situacao == "Ativo")
                        .Select(p => new PedidoResumoClienteResponseModel
                        {
                            IdPedido = p.IdPedido,
                            Data = p.Data,
                            ValorTotal = p.ObterValorTotal(),
                            EstaQuitado = p.EstaQuitado(),
                            Situacao = p.Situacao
                        }).ToList()
                });
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Lista de clientes";
            return retorno;
        }

        public async Task<Response<ClienteResponseModel>> CriarClienteAsync(CriarClienteRequestModel request)
        {
            var retorno = new Response<ClienteResponseModel>();

            // Verifica se já existe cliente com o mesmo nome
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == request.Nome.ToLower() && c.Situacao == "Ativo");

            if (clienteExistente != null)
                throw new Exception("Já existe um cliente com este nome");

            var cliente = new Cliente(request.Nome, request.Email, request.Telefone);

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Cliente criado com sucesso";
            return retorno;
        }

        public async Task<Response<ClienteResponseModel>> AtualizarClienteAsync(AtualizarClienteRequestModel request)
        {
            var retorno = new Response<ClienteResponseModel>();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == request.IdCliente && c.Situacao == "Ativo");

            if (cliente == null)
                throw new Exception("Cliente não encontrado");

            // Verifica se já existe outro cliente com o mesmo nome
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == request.Nome.ToLower()
                                       && c.IdCliente != request.IdCliente
                                       && c.Situacao == "Ativo");

            if (clienteExistente != null)
                throw new Exception("Já existe outro cliente com este nome");

            cliente.Alterar(request.Nome, request.Email, request.Telefone);

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Cliente atualizado com sucesso";
            return retorno;
        }

        public async Task<Response<ClienteResponseModel>> ExcluirClienteAsync(int id)
        {
            var retorno = new Response<ClienteResponseModel>();

            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                .FirstOrDefaultAsync(c => c.IdCliente == id && c.Situacao == "Ativo");

            if (cliente == null)
                throw new Exception("Cliente não encontrado");

            cliente.Excluir(); // Valida se pode excluir (verifica pedidos ativos)

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Cliente excluído com sucesso";
            return retorno;
        }

        public async Task<Response<List<PedidoResumoClienteResponseModel>>> ObterPedidosDoClienteAsync(int idCliente)
        {
            var retorno = new Response<List<PedidoResumoClienteResponseModel>>
            {
                Data = new List<PedidoResumoClienteResponseModel>()
            };

            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Itens)
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Pagamentos)
                .FirstOrDefaultAsync(c => c.IdCliente == idCliente && c.Situacao == "Ativo");

            if (cliente == null)
                throw new Exception("Cliente não encontrado");

            retorno.Data = cliente.Pedidos
                .Where(p => p.Situacao == "Ativo")
                .OrderByDescending(p => p.Data)
                .Select(p => new PedidoResumoClienteResponseModel
                {
                    IdPedido = p.IdPedido,
                    Data = p.Data,
                    ValorTotal = p.ObterValorTotal(),
                    EstaQuitado = p.EstaQuitado(),
                    Situacao = p.Situacao
                }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Pedidos do cliente => {idCliente}";
            return retorno;
        }
    }
}

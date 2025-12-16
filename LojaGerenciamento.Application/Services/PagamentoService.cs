using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Pagamento;
using LojaGerenciamento.Domain.Classes;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LojaGerenciamento.Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly LojaContext _context;

        public PagamentoService(LojaContext context)
        {
            _context = context;
        }

        public async Task<Response<PagamentoResponseModel>> ObterPorIdAsync(int id)
        {
            var retorno = new Response<PagamentoResponseModel>
            {
                Data = new PagamentoResponseModel()
            };

            var pagamento = await _context.Pagamentos
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.IdPagamento == id && p.Situacao == "Ativo");
            if (pagamento == null)
                throw new Exception("Pagamento não encontrado");

            retorno.Data.IdPagamento = pagamento.IdPagamento;
            retorno.Data.IdPedido = pagamento.IdPedido;
            retorno.Data.Pedido = $"Pedido #{pagamento.Pedido.IdPedido} - {pagamento.Pedido.Data:dd/MM/yyyy}";
            retorno.Data.Valor = pagamento.Valor;
            retorno.Data.DataPagamento = pagamento.DataPagamento;
            retorno.Data.Metodo = pagamento.Metodo;
            retorno.Data.Situacao = pagamento.Situacao;

            retorno.Sucesso = true;
            retorno.Mensagem = $"Pagamento => {id}";
            return retorno;
        }

        public async Task<Response<ListarPagamentoResponseModel>> ListarPagamentosAsync(FiltroPagamentoRequestModel request)
        {
            var retorno = new Response<ListarPagamentoResponseModel>
            {
                Data = new ListarPagamentoResponseModel()
            };

            var query = _context.Pagamentos
                .Include(p => p.Pedido)
                .Where(p => p.Situacao == "Ativo")
                .AsQueryable();

            // Filtro por ID do Pagamento
            if (request.IdPagamento.HasValue && request.IdPagamento.Value > 0)
            {
                query = query.Where(p => p.IdPagamento == request.IdPagamento.Value);
            }

            // Filtro por ID do Pedido
            if (request.IdPedido.HasValue && request.IdPedido.Value > 0)
            {
                query = query.Where(p => p.IdPedido == request.IdPedido.Value);
            }

            // Filtro por Valor Mínimo
            if (request.ValorMinimo.HasValue)
            {
                query = query.Where(p => p.Valor >= request.ValorMinimo.Value);
            }

            // Filtro por Valor Máximo
            if (request.ValorMaximo.HasValue)
            {
                query = query.Where(p => p.Valor <= request.ValorMaximo.Value);
            }

            // Filtro por Data de Pagamento - Início
            if (request.DataPagamentoInicio.HasValue)
            {
                query = query.Where(p => p.DataPagamento.Date >= request.DataPagamentoInicio.Value.Date);
            }

            // Filtro por Data de Pagamento - Fim
            if (request.DataPagamentoFim.HasValue)
            {
                var dataFinalComHora = request.DataPagamentoFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.DataPagamento <= dataFinalComHora);
            }

            // Filtro por Método
            if (!string.IsNullOrWhiteSpace(request.Metodo))
            {
                query = query.Where(p => p.Metodo.Contains(request.Metodo));
            }

            var pagamentos = await query
                .OrderByDescending(p => p.DataPagamento)
                .ToListAsync();

            foreach (var item in pagamentos)
            {
                retorno.Data.Lista.Add(new PagamentoResponseModel
                {
                    IdPagamento = item.IdPagamento,
                    IdPedido = item.IdPedido,
                    Pedido = $"Pedido #{item.Pedido.IdPedido} - {item.Pedido.Data:dd/MM/yyyy}",
                    Valor = item.Valor,
                    DataPagamento = item.DataPagamento,
                    Metodo = item.Metodo,
                    Situacao = item.Situacao
                });
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Lista de pagamentos";
            return retorno;
        }

        public async Task<Response<PagamentoResponseModel>> CriarPagamentoAsync(CriarPagamentoRequestModel request)
        {
            var retorno = new Response<PagamentoResponseModel>();

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            // Verifica se o pedido está ativo
            if (pedido.Situacao != "Ativo")
                throw new Exception("Não é possível adicionar pagamento a um pedido inativo");

            // Verifica se o valor do pagamento não ultrapassa o valor total do pedido
            var valorTotalPedido = pedido.ObterValorTotal();
            var valorJaPago = pedido.ObterValorPago();
            var valorRestante = valorTotalPedido - valorJaPago;

            if (request.Valor > valorRestante)
                throw new Exception($"Valor do pagamento (R$ {request.Valor:N2}) ultrapassa o valor restante do pedido (R$ {valorRestante:N2})");

            var pagamento = new Pagamento(pedido, request.Valor, request.Metodo);

            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pagamento registrado com sucesso";
            return retorno;
        }

        public async Task<Response<PagamentoResponseModel>> AtualizarPagamentoAsync(AtualizarPagamentoRequestModel request)
        {
            var retorno = new Response<PagamentoResponseModel>();

            var pagamento = await _context.Pagamentos
                .FirstOrDefaultAsync(p => p.IdPagamento == request.IdPagamento && p.Situacao == "Ativo");

            if (pagamento == null)
                throw new Exception("Pagamento não encontrado");

            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            // Verifica se o novo valor não ultrapassa o total do pedido
            var valorTotalPedido = pedido.ObterValorTotal();
            var valorJaPago = pedido.ObterValorPago() - pagamento.Valor; // Desconta o valor atual
            var valorRestante = valorTotalPedido - valorJaPago;

            if (request.Valor > valorRestante)
                throw new Exception($"Valor do pagamento (R$ {request.Valor:N2}) ultrapassa o valor restante do pedido (R$ {valorRestante:N2})");

            pagamento.Alterar(pedido, request.Valor, request.Metodo);

            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pagamento atualizado com sucesso";
            return retorno;
        }

        public async Task<Response<PagamentoResponseModel>> ExcluirPagamentoAsync(int id)
        {
            var retorno = new Response<PagamentoResponseModel>();

            var pagamento = await _context.Pagamentos
                .FirstOrDefaultAsync(p => p.IdPagamento == id && p.Situacao == "Ativo");

            if (pagamento == null)
                throw new Exception("Pagamento não encontrado");

            pagamento.Excluir(); // Valida data (não pode excluir pagamentos passados)

            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Pagamento excluído com sucesso";
            return retorno;
        }

        public async Task<Response<List<PagamentoResponseModel>>> ListarPagamentosPorPedidoAsync(int idPedido)
        {
            var retorno = new Response<List<PagamentoResponseModel>>
            {
                Data = new List<PagamentoResponseModel>()
            };

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            var pagamentos = await _context.Pagamentos
                .Include(p => p.Pedido)
                .Where(p => p.IdPedido == idPedido && p.Situacao == "Ativo")
                .OrderByDescending(p => p.DataPagamento)
                .ToListAsync();

            retorno.Data = pagamentos.Select(item => new PagamentoResponseModel
            {
                IdPagamento = item.IdPagamento,
                IdPedido = item.IdPedido,
                Pedido = $"Pedido #{item.Pedido.IdPedido} - {item.Pedido.Data:dd/MM/yyyy}",
                Valor = item.Valor,
                DataPagamento = item.DataPagamento,
                Metodo = item.Metodo,
                Situacao = item.Situacao
            }).ToList();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Pagamentos do pedido => {idPedido}";
            return retorno;
        }

        public async Task<Response<decimal>> ObterTotalPagoPorPedidoAsync(int idPedido)
        {
            var retorno = new Response<decimal>();

            var pedido = await _context.Pedidos
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido && p.Situacao == "Ativo");

            if (pedido == null)
                throw new Exception("Pedido não encontrado");

            retorno.Data = pedido.ObterValorPago();

            retorno.Sucesso = true;
            retorno.Mensagem = $"Total pago do pedido => {idPedido}";
            return retorno;
        }
    }
}

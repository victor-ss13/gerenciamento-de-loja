using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Pagamento;
using Microsoft.Extensions.DependencyInjection;

namespace LojaGerenciamento.ConsoleApp.Menus
{
    public static class MenuPagamentos
    {
        public static async Task ExibirAsync(IServiceProvider serviceProvider)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("      GERENCIAR PAGAMENTOS");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("1. Listar todos os pagamentos");
                Console.WriteLine("2. Buscar pagamento por ID");
                Console.WriteLine("3. Criar novo pagamento");
                Console.WriteLine("4. Atualizar pagamento");
                Console.WriteLine("5. Excluir pagamento");
                Console.WriteLine("6. Listar pagamentos por pedido");
                Console.WriteLine("7. Ver total pago de um pedido");
                Console.WriteLine("0. Voltar ao menu principal");
                Console.WriteLine();
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await ListarPagamentosAsync(serviceProvider);
                        break;
                    case "2":
                        await BuscarPagamentoPorIdAsync(serviceProvider);
                        break;
                    case "3":
                        await CriarPagamentoAsync(serviceProvider);
                        break;
                    case "4":
                        await AtualizarPagamentoAsync(serviceProvider);
                        break;
                    case "5":
                        await ExcluirPagamentoAsync(serviceProvider);
                        break;
                    case "6":
                        await ListarPagamentosPorPedidoAsync(serviceProvider);
                        break;
                    case "7":
                        await VerTotalPagoPorPedidoAsync(serviceProvider);
                        break;
                    case "0":
                        voltarMenuPrincipal = true;
                        break;
                    default:
                        Console.WriteLine("\n❌ Opção inválida!");
                        Console.WriteLine("Pressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static async Task ListarPagamentosAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      LISTAR PAGAMENTOS");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

            try
            {
                Console.WriteLine("Filtros (deixe em branco para listar todos):");
                Console.WriteLine();

                Console.Write("ID do Pedido: ");
                var idPedidoStr = Console.ReadLine();
                int? idPedido = string.IsNullOrWhiteSpace(idPedidoStr) ? null : int.Parse(idPedidoStr);

                Console.Write("Método de pagamento (busca parcial): ");
                var metodo = Console.ReadLine();

                Console.Write("Valor mínimo: R$ ");
                var valorMinimoStr = Console.ReadLine();
                decimal? valorMinimo = string.IsNullOrWhiteSpace(valorMinimoStr) ? null : decimal.Parse(valorMinimoStr);

                Console.Write("Valor máximo: R$ ");
                var valorMaximoStr = Console.ReadLine();
                decimal? valorMaximo = string.IsNullOrWhiteSpace(valorMaximoStr) ? null : decimal.Parse(valorMaximoStr);

                var filtro = new FiltroPagamentoRequestModel
                {
                    IdPedido = idPedido,
                    Metodo = string.IsNullOrWhiteSpace(metodo) ? null : metodo,
                    ValorMinimo = valorMinimo,
                    ValorMaximo = valorMaximo
                };

                var resultado = await pagamentoService.ListarPagamentosAsync(filtro);

                Console.WriteLine();
                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Lista.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Lista.Count} pagamento(s) encontrado(s):\n");

                    decimal totalGeral = 0;

                    foreach (var pag in resultado.Data.Lista)
                    {
                        Console.WriteLine($"ID: {pag.IdPagamento}");
                        Console.WriteLine($"Pedido: {pag.Pedido}");
                        Console.WriteLine($"Valor: R$ {pag.Valor:N2}");
                        Console.WriteLine($"Data: {pag.DataPagamento:dd/MM/yyyy HH:mm}");
                        Console.WriteLine($"Método: {pag.Metodo}");
                        Console.WriteLine($"Situação: {pag.Situacao}");
                        Console.WriteLine("------------------------------------");

                        totalGeral += pag.Valor;
                    }

                    Console.WriteLine($"\n💰 Total Geral: R$ {totalGeral:N2}");
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhum pagamento encontrado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task BuscarPagamentoPorIdAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("    BUSCAR PAGAMENTO POR ID");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

            try
            {
                Console.Write("Digite o ID do pagamento: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await pagamentoService.ObterPorIdAsync(id);

                if (resultado.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");
                    Console.WriteLine($"ID: {resultado.Data.IdPagamento}");
                    Console.WriteLine($"Pedido: {resultado.Data.Pedido}");
                    Console.WriteLine($"Valor: R$ {resultado.Data.Valor:N2}");
                    Console.WriteLine($"Data: {resultado.Data.DataPagamento:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"Método: {resultado.Data.Metodo}");
                    Console.WriteLine($"Situação: {resultado.Data.Situacao}");
                    Console.WriteLine("====================================");
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task CriarPagamentoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      CRIAR NOVO PAGAMENTO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                // Mostra informações do pedido
                var pedido = await pedidoService.ObterPorIdAsync(idPedido);
                if (pedido.Sucesso)
                {
                    Console.WriteLine($"\nPedido #{pedido.Data.IdPedido} - Cliente: {pedido.Data.Cliente}");
                    Console.WriteLine($"Valor Total: R$ {pedido.Data.ValorTotal:N2}");
                    Console.WriteLine($"Valor Pago: R$ {pedido.Data.ValorPago:N2}");
                    Console.WriteLine($"Valor Pendente: R$ {pedido.Data.ValorPendente:N2}");
                    Console.WriteLine($"Status: {(pedido.Data.EstaQuitado ? "✅ Quitado" : "⏳ Pendente")}");
                    Console.WriteLine();
                }

                Console.Write("Valor do pagamento: R$ ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
                {
                    Console.WriteLine("\n❌ Valor inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Método de pagamento (ex: Dinheiro, Cartão, PIX): ");
                var metodo = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(metodo))
                {
                    Console.WriteLine("\n❌ Método de pagamento é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new CriarPagamentoRequestModel
                {
                    IdPedido = idPedido,
                    Valor = valor,
                    Metodo = metodo
                };

                var resultado = await pagamentoService.CriarPagamentoAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task AtualizarPagamentoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      ATUALIZAR PAGAMENTO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

            try
            {
                Console.Write("Digite o ID do pagamento: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var pagamentoAtual = await pagamentoService.ObterPorIdAsync(id);
                if (!pagamentoAtual.Sucesso)
                {
                    Console.WriteLine("\n❌ Pagamento não encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nPagamento atual:");
                Console.WriteLine($"Pedido: {pagamentoAtual.Data.Pedido}");
                Console.WriteLine($"Valor: R$ {pagamentoAtual.Data.Valor:N2}");
                Console.WriteLine($"Método: {pagamentoAtual.Data.Metodo}");
                Console.WriteLine();

                Console.Write("Novo ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Novo valor: R$ ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
                {
                    Console.WriteLine("\n❌ Valor inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Novo método: ");
                var metodo = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(metodo))
                {
                    Console.WriteLine("\n❌ Método de pagamento é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AtualizarPagamentoRequestModel
                {
                    IdPagamento = id,
                    IdPedido = idPedido,
                    Valor = valor,
                    Metodo = metodo
                };

                var resultado = await pagamentoService.AtualizarPagamentoAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task ExcluirPagamentoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      EXCLUIR PAGAMENTO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

            try
            {
                Console.Write("Digite o ID do pagamento: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var pagamento = await pagamentoService.ObterPorIdAsync(id);
                if (!pagamento.Sucesso)
                {
                    Console.WriteLine("\n❌ Pagamento não encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nPagamento: {pagamento.Data.Pedido}");
                Console.WriteLine($"Valor: R$ {pagamento.Data.Valor:N2}");
                Console.WriteLine($"Data: {pagamento.Data.DataPagamento:dd/MM/yyyy}");
                Console.WriteLine();
                Console.WriteLine("⚠️  ATENÇÃO: Só é possível excluir pagamentos de datas futuras!");
                Console.WriteLine();
                Console.Write("Confirma a exclusão? (S/N): ");

                if (Console.ReadLine()?.ToUpper() != "S")
                {
                    Console.WriteLine("\n⚠️ Operação cancelada.");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await pagamentoService.ExcluirPagamentoAsync(id);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task ListarPagamentosPorPedidoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("  LISTAR PAGAMENTOS POR PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("Digite o ID do pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                // Mostra informações do pedido
                var pedido = await pedidoService.ObterPorIdAsync(idPedido);
                if (pedido.Sucesso)
                {
                    Console.WriteLine($"\nPedido #{pedido.Data.IdPedido} - Cliente: {pedido.Data.Cliente}");
                    Console.WriteLine($"Valor Total: R$ {pedido.Data.ValorTotal:N2}");
                    Console.WriteLine($"Valor Pago: R$ {pedido.Data.ValorPago:N2}");
                    Console.WriteLine($"Valor Pendente: R$ {pedido.Data.ValorPendente:N2}");
                    Console.WriteLine($"Status: {(pedido.Data.EstaQuitado ? "✅ Quitado" : "⏳ Pendente")}");
                    Console.WriteLine();
                }

                var resultado = await pagamentoService.ListarPagamentosPorPedidoAsync(idPedido);

                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Count} pagamento(s) encontrado(s):\n");

                    foreach (var pag in resultado.Data)
                    {
                        Console.WriteLine($"ID: {pag.IdPagamento}");
                        Console.WriteLine($"Valor: R$ {pag.Valor:N2}");
                        Console.WriteLine($"Data: {pag.DataPagamento:dd/MM/yyyy HH:mm}");
                        Console.WriteLine($"Método: {pag.Metodo}");
                        Console.WriteLine($"Situação: {pag.Situacao}");
                        Console.WriteLine("------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhum pagamento encontrado para este pedido.");
                }

                Console.WriteLine($"\n✅ {resultado.Mensagem}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task VerTotalPagoPorPedidoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("   VER TOTAL PAGO DE UM PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("Digite o ID do pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var pedido = await pedidoService.ObterPorIdAsync(idPedido);
                var totalPago = await pagamentoService.ObterTotalPagoPorPedidoAsync(idPedido);

                if (pedido.Sucesso && totalPago.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");
                    Console.WriteLine($"Pedido #{pedido.Data.IdPedido}");
                    Console.WriteLine($"Cliente: {pedido.Data.Cliente}");
                    Console.WriteLine();
                    Console.WriteLine($"💰 Valor Total do Pedido: R$ {pedido.Data.ValorTotal:N2}");
                    Console.WriteLine($"✅ Total Pago: R$ {totalPago.Data:N2}");
                    Console.WriteLine($"⏳ Valor Pendente: R$ {pedido.Data.ValorPendente:N2}");
                    Console.WriteLine();
                    Console.WriteLine($"Status: {(pedido.Data.EstaQuitado ? "✅ QUITADO" : "⏳ PENDENTE DE PAGAMENTO")}");
                    Console.WriteLine("====================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }
    }
}

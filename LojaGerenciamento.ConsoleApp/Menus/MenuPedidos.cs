using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.ItemPedido;
using LojaGerenciamento.Application.Models.Pedido;
using LojaGerenciamento.Application.Models.Pagamento; // Caso precise de referências para AdicionarPagamento
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace LojaGerenciamento.ConsoleApp.Menus
{
    public static class MenuPedidos
    {
        public static async Task ExibirAsync(IServiceProvider serviceProvider)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("        GERENCIAR PEDIDOS");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("--- Pedidos ---");
                Console.WriteLine("1. Listar pedidos (com filtros)");
                Console.WriteLine("2. Buscar pedido por ID (Detalhes completos)");
                Console.WriteLine("3. Criar novo pedido");
                Console.WriteLine("4. Atualizar cliente do pedido");
                Console.WriteLine("5. Excluir pedido");
                Console.WriteLine();
                Console.WriteLine("--- Itens do Pedido ---");
                Console.WriteLine("6. Adicionar item ao pedido");
                Console.WriteLine("7. Atualizar quantidade de um item");
                Console.WriteLine("8. Remover item do pedido");
                Console.WriteLine();
                Console.WriteLine("--- Financeiro Rápido ---");
                Console.WriteLine("9. Adicionar pagamento rápido ao pedido");
                Console.WriteLine();
                Console.WriteLine("0. Voltar ao menu principal");
                Console.WriteLine();
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await ListarPedidosAsync(serviceProvider);
                        break;
                    case "2":
                        await BuscarPedidoPorIdAsync(serviceProvider);
                        break;
                    case "3":
                        await CriarPedidoAsync(serviceProvider);
                        break;
                    case "4":
                        await AtualizarPedidoAsync(serviceProvider);
                        break;
                    case "5":
                        await ExcluirPedidoAsync(serviceProvider);
                        break;
                    case "6":
                        await AdicionarItemAsync(serviceProvider);
                        break;
                    case "7":
                        await AtualizarQuantidadeItemAsync(serviceProvider);
                        break;
                    case "8":
                        await RemoverItemAsync(serviceProvider);
                        break;
                    case "9":
                        await AdicionarPagamentoRapidoAsync(serviceProvider);
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

        private static async Task ListarPedidosAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         LISTAR PEDIDOS");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.WriteLine("Filtros (deixe em branco para ignorar):");
                Console.WriteLine();

                Console.Write("ID do Pedido: ");
                var idPedidoStr = Console.ReadLine();
                int? idPedido = string.IsNullOrWhiteSpace(idPedidoStr) ? null : int.Parse(idPedidoStr);

                Console.Write("ID do Cliente: ");
                var idClienteStr = Console.ReadLine();
                int? idCliente = string.IsNullOrWhiteSpace(idClienteStr) ? null : int.Parse(idClienteStr);

                Console.Write("Data Início (dd/MM/yyyy): ");
                var dataInicioStr = Console.ReadLine();
                DateTime? dataInicio = string.IsNullOrWhiteSpace(dataInicioStr) ? null : DateTime.Parse(dataInicioStr);

                Console.Write("Data Fim (dd/MM/yyyy): ");
                var dataFimStr = Console.ReadLine();
                DateTime? dataFim = string.IsNullOrWhiteSpace(dataFimStr) ? null : DateTime.Parse(dataFimStr);

                Console.WriteLine("Filtrar por Situação:");
                Console.WriteLine("1 - Apenas Quitados");
                Console.WriteLine("2 - Apenas Pendentes");
                Console.WriteLine("Enter - Todos");
                Console.Write("Opção: ");
                var opcaoSituacao = Console.ReadLine();

                bool? apenasQuitados = opcaoSituacao == "1" ? true : null;
                bool? apenasPendentes = opcaoSituacao == "2" ? true : null;

                var filtro = new FiltroPedidoRequestModel
                {
                    IdPedido = idPedido,
                    IdCliente = idCliente,
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    ApenasQuitados = apenasQuitados,
                    ApenasPendentes = apenasPendentes
                };

                var resultado = await pedidoService.ListarPedidosAsync(filtro);

                Console.WriteLine();
                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Lista.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Lista.Count} pedido(s) encontrado(s):\n");

                    foreach (var ped in resultado.Data.Lista)
                    {
                        Console.WriteLine($"Pedido #{ped.IdPedido} | Data: {ped.Data:dd/MM/yyyy}");
                        Console.WriteLine($"Cliente: {ped.Cliente} (ID: {ped.IdCliente})");
                        Console.WriteLine($"Itens: {ped.QuantidadeItens} | Status: {(ped.EstaQuitado ? "✅ Quitado" : "⏳ Pendente")}");
                        Console.WriteLine($"Total: R$ {ped.ValorTotal:N2} | Pago: R$ {ped.ValorPago:N2} | Pendente: R$ {ped.ValorPendente:N2}");
                        Console.WriteLine("------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhum pedido encontrado com esses filtros.");
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

        private static async Task BuscarPedidoPorIdAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      DETALHES DO PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("Digite o ID do pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.ReadKey();
                    return;
                }

                var resultado = await pedidoService.ObterPorIdAsync(id);

                if (resultado.Sucesso)
                {
                    var p = resultado.Data;
                    Console.WriteLine();
                    Console.WriteLine($"🧾 PEDIDO #{p.IdPedido}");
                    Console.WriteLine($"📅 Data: {p.Data:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"👤 Cliente: {p.Cliente} (ID: {p.IdCliente})");
                    Console.WriteLine($"📊 Situação: {p.Situacao}");
                    Console.WriteLine($"💰 Status Pagamento: {(p.EstaQuitado ? "✅ QUITADO" : "⏳ PENDENTE")}");

                    Console.WriteLine("\n--- 📦 ITENS ---");
                    if (p.Itens.Any())
                    {
                        foreach (var item in p.Itens)
                        {
                            Console.WriteLine($"   [ID Item: {item.IdItemPedido}] {item.Produto} (ID Prod: {item.IdProduto})");
                            Console.WriteLine($"   {item.Quantidade}x R$ {item.Preco:N2} = R$ {(item.Quantidade * item.Preco):N2}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   (Nenhum item adicionado)");
                    }

                    Console.WriteLine("\n--- 💲 PAGAMENTOS ---");
                    if (p.Pagamentos.Any())
                    {
                        foreach (var pag in p.Pagamentos)
                        {
                            Console.WriteLine($"   [ID: {pag.IdPagamento}] {pag.DataPagamento:dd/MM} - {pag.Metodo}: R$ {pag.Valor:N2}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   (Nenhum pagamento registrado)");
                    }

                    Console.WriteLine("\n------------------------------------");
                    Console.WriteLine($"TOTAL DO PEDIDO:  R$ {p.ValorTotal:N2}");
                    Console.WriteLine($"TOTAL PAGO:       R$ {p.ValorPago:N2}");
                    Console.WriteLine($"VALOR PENDENTE:   R$ {p.ValorPendente:N2}");
                    Console.WriteLine("------------------------------------");
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

        private static async Task CriarPedidoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("        CRIAR NOVO PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Cliente: ");
                if (!int.TryParse(Console.ReadLine(), out int idCliente))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.ReadKey();
                    return;
                }

                var request = new CriarPedidoRequestModel
                {
                    IdCliente = idCliente
                };

                var resultado = await pedidoService.CriarPedidoAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro ao criar pedido: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        private static async Task AtualizarPedidoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("    ATUALIZAR CLIENTE DO PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.ReadKey();
                    return;
                }

                // Busca pedido atual para mostrar info
                var pedidoAtual = await pedidoService.ObterPorIdAsync(idPedido);
                Console.WriteLine($"Pedido atual pertence a: {pedidoAtual.Data.Cliente}");
                Console.WriteLine();

                Console.Write("Novo ID do Cliente: ");
                if (!int.TryParse(Console.ReadLine(), out int idCliente))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.ReadKey();
                    return;
                }

                var request = new AtualizarPedidoRequestModel
                {
                    IdPedido = idPedido,
                    IdCliente = idCliente
                };

                var resultado = await pedidoService.AtualizarPedidoAsync(request);

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

        private static async Task ExcluirPedidoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         EXCLUIR PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("Digite o ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.ReadKey();
                    return;
                }

                var pedido = await pedidoService.ObterPorIdAsync(id);
                if (pedido.Sucesso)
                {
                    Console.WriteLine($"\nPedido #{pedido.Data.IdPedido} - Cliente: {pedido.Data.Cliente}");
                    Console.WriteLine($"Valor: R$ {pedido.Data.ValorTotal:N2} | Itens: {pedido.Data.QuantidadeItens}");
                    Console.WriteLine("\n⚠️  ATENÇÃO: A exclusão devolverá os itens ao estoque e cancelará pagamentos!");
                    Console.Write("Confirma a exclusão? (S/N): ");

                    if (Console.ReadLine()?.ToUpper() != "S")
                    {
                        Console.WriteLine("\n⚠️ Operação cancelada.");
                        Console.ReadKey();
                        return;
                    }

                    var resultado = await pedidoService.ExcluirPedidoAsync(id);

                    if (resultado.Sucesso)
                    {
                        Console.WriteLine($"\n✅ {resultado.Mensagem}");
                    }
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

        #region Gerenciamento de Itens

        private static async Task AdicionarItemAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      ADICIONAR ITEM AO PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido)) return;

                // Mostra resumo do pedido
                var ped = await pedidoService.ObterPorIdAsync(idPedido);
                Console.WriteLine($"\nCliente: {ped.Data.Cliente} | Total Atual: R$ {ped.Data.ValorTotal:N2}\n");

                Console.Write("ID do Produto: ");
                if (!int.TryParse(Console.ReadLine(), out int idProduto)) return;

                Console.Write("Quantidade: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade)) return;

                Console.Write("Preço Unitário (R$): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal preco)) return;

                var request = new AdicionarItemRequestModel
                {
                    IdPedido = idPedido,
                    IdProduto = idProduto,
                    Quantidade = quantidade,
                    Preco = preco
                };

                var resultado = await pedidoService.AdicionarItemAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.ReadKey();
        }

        private static async Task AtualizarQuantidadeItemAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     ATUALIZAR QUANTIDADE ITEM");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido)) return;

                // Lista itens para ajudar o usuário
                var pedido = await pedidoService.ObterPorIdAsync(idPedido);
                Console.WriteLine("\n--- Itens Atuais ---");
                foreach (var item in pedido.Data.Itens)
                {
                    Console.WriteLine($"ID Item: {item.IdItemPedido} | Produto: {item.Produto} | Qtd: {item.Quantidade}");
                }
                Console.WriteLine("--------------------\n");

                Console.Write("ID do Item (IdItemPedido): ");
                if (!int.TryParse(Console.ReadLine(), out int idItem)) return;

                Console.Write("Nova Quantidade: ");
                if (!int.TryParse(Console.ReadLine(), out int novaQtd)) return;

                var request = new AtualizarQuantidadeItemRequestModel
                {
                    IdPedido = idPedido,
                    IdItemPedido = idItem,
                    NovaQuantidade = novaQtd
                };

                var resultado = await pedidoService.AtualizarQuantidadeItemAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.ReadKey();
        }

        private static async Task RemoverItemAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("      REMOVER ITEM DO PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido)) return;

                // Lista itens
                var pedido = await pedidoService.ObterPorIdAsync(idPedido);
                Console.WriteLine("\n--- Itens Atuais ---");
                foreach (var item in pedido.Data.Itens)
                {
                    Console.WriteLine($"ID Item: {item.IdItemPedido} | Produto: {item.Produto} | Qtd: {item.Quantidade}");
                }
                Console.WriteLine("--------------------\n");

                Console.Write("ID do Item a remover (IdItemPedido): ");
                if (!int.TryParse(Console.ReadLine(), out int idItem)) return;

                var request = new RemoverItemRequestModel
                {
                    IdPedido = idPedido,
                    IdItemPedido = idItem
                };

                var resultado = await pedidoService.RemoverItemAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.ReadKey();
        }

        #endregion

        #region Helpers

        private static async Task AdicionarPagamentoRapidoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     PAGAMENTO RÁPIDO PEDIDO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

            try
            {
                Console.Write("ID do Pedido: ");
                if (!int.TryParse(Console.ReadLine(), out int idPedido)) return;

                var pedido = await pedidoService.ObterPorIdAsync(idPedido);
                Console.WriteLine($"\nTotal: R$ {pedido.Data.ValorTotal:N2}");
                Console.WriteLine($"Falta Pagar: R$ {pedido.Data.ValorPendente:N2}\n");

                if (pedido.Data.EstaQuitado)
                {
                    Console.WriteLine("✅ Este pedido já está quitado!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Valor a pagar (R$): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal valor)) return;

                Console.Write("Método (Ex: PIX, Dinheiro): ");
                var metodo = Console.ReadLine();

                var request = new AdicionarPagamentoRequestModel
                {
                    IdPedido = idPedido,
                    Valor = valor,
                    Metodo = metodo
                };

                var resultado = await pedidoService.AdicionarPagamentoAsync(request);

                if (resultado.Sucesso)
                {
                    Console.WriteLine($"\n✅ {resultado.Mensagem}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        #endregion
    }
}
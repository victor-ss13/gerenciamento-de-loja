using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Cliente;
using Microsoft.Extensions.DependencyInjection;

namespace LojaGerenciamento.ConsoleApp.Menus
{
    public static class MenuClientes
    {
        public static async Task ExibirAsync(IServiceProvider serviceProvider)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("       GERENCIAR CLIENTES");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("1. Listar todos os clientes");
                Console.WriteLine("2. Buscar cliente por ID");
                Console.WriteLine("3. Criar novo cliente");
                Console.WriteLine("4. Atualizar cliente");
                Console.WriteLine("5. Excluir cliente");
                Console.WriteLine("6. Ver pedidos do cliente");
                Console.WriteLine("0. Voltar ao menu principal");
                Console.WriteLine();
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await ListarClientesAsync(serviceProvider);
                        break;
                    case "2":
                        await BuscarClientePorIdAsync(serviceProvider);
                        break;
                    case "3":
                        await CriarClienteAsync(serviceProvider);
                        break;
                    case "4":
                        await AtualizarClienteAsync(serviceProvider);
                        break;
                    case "5":
                        await ExcluirClienteAsync(serviceProvider);
                        break;
                    case "6":
                        await VerPedidosDoClienteAsync(serviceProvider);
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

        private static async Task ListarClientesAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       LISTAR CLIENTES");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var clienteService = scope.ServiceProvider.GetRequiredService<IClienteService>();

            try
            {
                Console.WriteLine("Filtros (deixe em branco para listar todos):");
                Console.WriteLine();

                Console.Write("Nome (busca parcial): ");
                var nome = Console.ReadLine();

                Console.Write("Apenas com pedidos? (S/N): ");
                var comPedidos = Console.ReadLine()?.ToUpper() == "S";

                Console.Write("Apenas sem pedidos? (S/N): ");
                var semPedidos = Console.ReadLine()?.ToUpper() == "S";

                var filtro = new FiltroClienteRequestModel
                {
                    Nome = string.IsNullOrWhiteSpace(nome) ? null : nome,
                    ApenasComPedidos = comPedidos ? true : null,
                    ApenasSemPedidos = semPedidos ? true : null
                };

                var resultado = await clienteService.ListarClientesAsync(filtro);

                Console.WriteLine();
                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Lista.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Lista.Count} cliente(s) encontrado(s):\n");

                    foreach (var cli in resultado.Data.Lista)
                    {
                        Console.WriteLine($"ID: {cli.IdCliente}");
                        Console.WriteLine($"Nome: {cli.Nome}");
                        Console.WriteLine($"Email: {cli.Email ?? "Não informado"}");
                        Console.WriteLine($"Telefone: {cli.Telefone ?? "Não informado"}");
                        Console.WriteLine($"Situação: {cli.Situacao}");
                        Console.WriteLine($"Quantidade de Pedidos: {cli.QuantidadePedidos}");
                        Console.WriteLine($"Valor Total em Pedidos: R$ {cli.ValorTotalPedidos:N2}");

                        if (cli.Pedidos.Any())
                        {
                            Console.WriteLine("Pedidos:");
                            foreach (var ped in cli.Pedidos)
                            {
                                Console.WriteLine($"  - Pedido #{ped.IdPedido} | {ped.Data:dd/MM/yyyy} | R$ {ped.ValorTotal:N2} | {(ped.EstaQuitado ? "✅ Quitado" : "⏳ Pendente")}");
                            }
                        }

                        Console.WriteLine("------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhum cliente encontrado.");
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

        private static async Task BuscarClientePorIdAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     BUSCAR CLIENTE POR ID");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var clienteService = scope.ServiceProvider.GetRequiredService<IClienteService>();

            try
            {
                Console.Write("Digite o ID do cliente: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await clienteService.ObterPorIdAsync(id);

                if (resultado.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");
                    Console.WriteLine($"ID: {resultado.Data.IdCliente}");
                    Console.WriteLine($"Nome: {resultado.Data.Nome}");
                    Console.WriteLine($"Email: {resultado.Data.Email ?? "Não informado"}");
                    Console.WriteLine($"Telefone: {resultado.Data.Telefone ?? "Não informado"}");
                    Console.WriteLine($"Situação: {resultado.Data.Situacao}");
                    Console.WriteLine($"Quantidade de Pedidos: {resultado.Data.QuantidadePedidos}");
                    Console.WriteLine($"Valor Total em Pedidos: R$ {resultado.Data.ValorTotalPedidos:N2}");

                    if (resultado.Data.Pedidos.Any())
                    {
                        Console.WriteLine("\nPedidos:");
                        foreach (var ped in resultado.Data.Pedidos)
                        {
                            Console.WriteLine($"  - Pedido #{ped.IdPedido}");
                            Console.WriteLine($"    Data: {ped.Data:dd/MM/yyyy HH:mm}");
                            Console.WriteLine($"    Valor Total: R$ {ped.ValorTotal:N2}");
                            Console.WriteLine($"    Status: {(ped.EstaQuitado ? "✅ Quitado" : "⏳ Pendente de pagamento")}");
                            Console.WriteLine($"    Situação: {ped.Situacao}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n⚠️ Cliente não possui pedidos.");
                    }

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

        private static async Task CriarClienteAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       CRIAR NOVO CLIENTE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var clienteService = scope.ServiceProvider.GetRequiredService<IClienteService>();

            try
            {
                Console.Write("Nome do cliente: ");
                var nome = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nome))
                {
                    Console.WriteLine("\n❌ Nome é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Email (opcional): ");
                var email = Console.ReadLine();

                Console.Write("Telefone (opcional): ");
                var telefone = Console.ReadLine();

                var request = new CriarClienteRequestModel
                {
                    Nome = nome,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone
                };

                var resultado = await clienteService.CriarClienteAsync(request);

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

        private static async Task AtualizarClienteAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       ATUALIZAR CLIENTE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var clienteService = scope.ServiceProvider.GetRequiredService<IClienteService>();

            try
            {
                Console.Write("Digite o ID do cliente: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var clienteAtual = await clienteService.ObterPorIdAsync(id);
                if (!clienteAtual.Sucesso)
                {
                    Console.WriteLine("\n❌ Cliente não encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nCliente atual:");
                Console.WriteLine($"Nome: {clienteAtual.Data.Nome}");
                Console.WriteLine($"Email: {clienteAtual.Data.Email ?? "Não informado"}");
                Console.WriteLine($"Telefone: {clienteAtual.Data.Telefone ?? "Não informado"}");
                Console.WriteLine();

                Console.Write("Novo nome: ");
                var nome = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nome))
                {
                    Console.WriteLine("\n❌ Nome é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Novo email (opcional): ");
                var email = Console.ReadLine();

                Console.Write("Novo telefone (opcional): ");
                var telefone = Console.ReadLine();

                var request = new AtualizarClienteRequestModel
                {
                    IdCliente = id,
                    Nome = nome,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone
                };

                var resultado = await clienteService.AtualizarClienteAsync(request);

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

        private static async Task ExcluirClienteAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       EXCLUIR CLIENTE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var clienteService = scope.ServiceProvider.GetRequiredService<IClienteService>();

            try
            {
                Console.Write("Digite o ID do cliente: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var cliente = await clienteService.ObterPorIdAsync(id);
                if (!cliente.Sucesso)
                {
                    Console.WriteLine("\n❌ Cliente não encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nCliente: {cliente.Data.Nome}");
                Console.WriteLine($"Pedidos ativos: {cliente.Data.QuantidadePedidos}");
                Console.WriteLine();
                Console.Write("Confirma a exclusão? (S/N): ");

                if (Console.ReadLine()?.ToUpper() != "S")
                {
                    Console.WriteLine("\n⚠️ Operação cancelada.");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await clienteService.ExcluirClienteAsync(id);

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

        private static async Task VerPedidosDoClienteAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     VER PEDIDOS DO CLIENTE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var clienteService = scope.ServiceProvider.GetRequiredService<IClienteService>();

            try
            {
                Console.Write("Digite o ID do cliente: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await clienteService.ObterPedidosDoClienteAsync(id);

                if (resultado.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");

                    if (resultado.Data.Any())
                    {
                        Console.WriteLine($"\n✅ {resultado.Data.Count} pedido(s) encontrado(s):\n");

                        foreach (var ped in resultado.Data)
                        {
                            Console.WriteLine($"Pedido #{ped.IdPedido}");
                            Console.WriteLine($"  Data: {ped.Data:dd/MM/yyyy HH:mm}");
                            Console.WriteLine($"  Valor Total: R$ {ped.ValorTotal:N2}");
                            Console.WriteLine($"  Status Pagamento: {(ped.EstaQuitado ? "✅ Quitado" : "⏳ Pendente")}");
                            Console.WriteLine($"  Situação: {ped.Situacao}");
                            Console.WriteLine("------------------------------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n⚠️ Cliente não possui pedidos ativos.");
                    }

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
    }
}
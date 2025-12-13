using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Services;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ============================================
// CONFIGURAÇÃO
// ============================================

var builder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = builder.Build();

// ============================================
// DEPENDENCY INJECTION
// ============================================

var services = new ServiceCollection();

// DbContext
services.AddDbContext<LojaContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Services
services.AddScoped<ICategoriaService, CategoriaService>();
services.AddScoped<IClienteService, ClienteService>();
services.AddScoped<IProdutoService, ProdutoService>();
services.AddScoped<IPedidoService, PedidoService>();
services.AddScoped<IItemPedidoService, ItemPedidoService>();
services.AddScoped<IPagamentoService, PagamentoService>();

var serviceProvider = services.BuildServiceProvider();

// ============================================
// TESTE DE CONEXÃO
// ============================================

Console.WriteLine("====================================");
Console.WriteLine("  SISTEMA DE GERENCIAMENTO DE LOJA");
Console.WriteLine("====================================");
Console.WriteLine();

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LojaContext>();

    try
    {
        Console.Write("Testando conexão com o banco de dados... ");
        await context.Database.CanConnectAsync();
        Console.WriteLine("✅ OK!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ ERRO!");
        Console.WriteLine($"Detalhes: {ex.Message}");
        Console.WriteLine();
        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
        return;
    }
}

Console.WriteLine();
Console.WriteLine("Todos os services foram registrados com sucesso!");
Console.WriteLine();

// ============================================
// MENU PRINCIPAL (exemplo simples por enquanto)
// ============================================

bool executando = true;

while (executando)
{
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("  SISTEMA DE GERENCIAMENTO DE LOJA");
    Console.WriteLine("====================================");
    Console.WriteLine();
    Console.WriteLine("1. Gerenciar Categorias");
    Console.WriteLine("2. Gerenciar Produtos");
    Console.WriteLine("3. Gerenciar Clientes");
    Console.WriteLine("4. Gerenciar Pedidos");
    Console.WriteLine("5. Gerenciar Pagamentos");
    Console.WriteLine("0. Sair");
    Console.WriteLine();
    Console.Write("Escolha uma opção: ");

    var opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            await LojaGerenciamento.ConsoleApp.Menus.MenuCategorias.ExibirAsync(serviceProvider);
            break;

        case "2":
            await LojaGerenciamento.ConsoleApp.Menus.MenuProdutos.ExibirAsync(serviceProvider);
            break;

        case "3":
            await LojaGerenciamento.ConsoleApp.Menus.MenuClientes.ExibirAsync(serviceProvider);
            break;

        case "4":
            await LojaGerenciamento.ConsoleApp.Menus.MenuPedidos.ExibirAsync(serviceProvider);
            break;

        case "5":
            await LojaGerenciamento.ConsoleApp.Menus.MenuPagamentos.ExibirAsync(serviceProvider);
            break;

        case "0":
            executando = false;
            Console.WriteLine("\nEncerrando sistema...");
            break;

        default:
            Console.WriteLine("\nOpção inválida!");
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

Console.WriteLine("Sistema encerrado. Até logo!");
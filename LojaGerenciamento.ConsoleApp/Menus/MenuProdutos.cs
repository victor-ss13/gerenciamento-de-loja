using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Produto;
using Microsoft.Extensions.DependencyInjection;

namespace LojaGerenciamento.ConsoleApp.Menus
{
    public static class MenuProdutos
    {
        public static async Task ExibirAsync(IServiceProvider serviceProvider)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("       GERENCIAR PRODUTOS");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("1. Listar todos os produtos");
                Console.WriteLine("2. Buscar produto por ID");
                Console.WriteLine("3. Criar novo produto");
                Console.WriteLine("4. Atualizar produto");
                Console.WriteLine("5. Excluir produto");
                Console.WriteLine();
                Console.WriteLine("--- GERENCIAR PREÇO ---");
                Console.WriteLine("6. Atualizar preço");
                Console.WriteLine();
                Console.WriteLine("--- GERENCIAR ESTOQUE ---");
                Console.WriteLine("7. Adicionar estoque");
                Console.WriteLine("8. Remover estoque");
                Console.WriteLine("9. Atualizar estoque");
                Console.WriteLine("10. Verificar disponibilidade");
                Console.WriteLine();
                Console.WriteLine("11. Listar produtos por categoria");
                Console.WriteLine("0. Voltar ao menu principal");
                Console.WriteLine();
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await ListarProdutosAsync(serviceProvider);
                        break;
                    case "2":
                        await BuscarProdutoPorIdAsync(serviceProvider);
                        break;
                    case "3":
                        await CriarProdutoAsync(serviceProvider);
                        break;
                    case "4":
                        await AtualizarProdutoAsync(serviceProvider);
                        break;
                    case "5":
                        await ExcluirProdutoAsync(serviceProvider);
                        break;
                    case "6":
                        await AtualizarPrecoAsync(serviceProvider);
                        break;
                    case "7":
                        await AdicionarEstoqueAsync(serviceProvider);
                        break;
                    case "8":
                        await RemoverEstoqueAsync(serviceProvider);
                        break;
                    case "9":
                        await AtualizarEstoqueAsync(serviceProvider);
                        break;
                    case "10":
                        await VerificarDisponibilidadeAsync(serviceProvider);
                        break;
                    case "11":
                        await ListarPorCategoriaAsync(serviceProvider);
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

        private static async Task ListarProdutosAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       LISTAR PRODUTOS");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.WriteLine("Filtros (deixe em branco para listar todos):");
                Console.WriteLine();

                Console.Write("Nome (busca parcial): ");
                var nome = Console.ReadLine();

                Console.Write("ID da Categoria: ");
                var idCategoriaStr = Console.ReadLine();
                int? idCategoria = string.IsNullOrWhiteSpace(idCategoriaStr) ? null : int.Parse(idCategoriaStr);

                Console.Write("Apenas com estoque? (S/N): ");
                var comEstoque = Console.ReadLine()?.ToUpper() == "S";

                Console.Write("Apenas sem estoque? (S/N): ");
                var semEstoque = Console.ReadLine()?.ToUpper() == "S";

                var filtro = new FiltroProdutoRequestModel
                {
                    Nome = string.IsNullOrWhiteSpace(nome) ? null : nome,
                    IdCategoria = idCategoria,
                    ApenasComEstoque = comEstoque ? true : null,
                    ApenasSemEstoque = semEstoque ? true : null
                };

                var resultado = await produtoService.ListarProdutosAsync(filtro);

                Console.WriteLine();
                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Lista.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Lista.Count} produto(s) encontrado(s):\n");

                    foreach (var prod in resultado.Data.Lista)
                    {
                        Console.WriteLine($"ID: {prod.IdProduto}");
                        Console.WriteLine($"Nome: {prod.Nome}");
                        Console.WriteLine($"Preço: R$ {prod.Preco:N2}");
                        Console.WriteLine($"Estoque: {prod.Estoque} {(prod.TemEstoque ? "✅" : "❌")}");
                        Console.WriteLine($"Categoria: {prod.Categoria} (ID: {prod.IdCategoria})");
                        Console.WriteLine($"Situação: {prod.Situacao}");
                        Console.WriteLine("------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhum produto encontrado.");
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

        private static async Task BuscarProdutoPorIdAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     BUSCAR PRODUTO POR ID");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await produtoService.ObterPorIdAsync(id);

                if (resultado.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");
                    Console.WriteLine($"ID: {resultado.Data.IdProduto}");
                    Console.WriteLine($"Nome: {resultado.Data.Nome}");
                    Console.WriteLine($"Preço: R$ {resultado.Data.Preco:N2}");
                    Console.WriteLine($"Estoque: {resultado.Data.Estoque} {(resultado.Data.TemEstoque ? "✅ Disponível" : "❌ Indisponível")}");
                    Console.WriteLine($"Categoria: {resultado.Data.Categoria} (ID: {resultado.Data.IdCategoria})");
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

        private static async Task CriarProdutoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       CRIAR NOVO PRODUTO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                // Lista categorias disponíveis
                var categorias = await categoriaService.ListarCategoriasAsync(new());
                if (categorias.Sucesso && categorias.Data.Lista.Any())
                {
                    Console.WriteLine("Categorias disponíveis:");
                    foreach (var cat in categorias.Data.Lista.Where(c => c.Situacao == "Ativo"))
                    {
                        Console.WriteLine($"  [{cat.IdCategoria}] {cat.Nome}");
                    }
                    Console.WriteLine();
                }

                Console.Write("Nome do produto: ");
                var nome = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nome))
                {
                    Console.WriteLine("\n❌ Nome é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Preço: R$ ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal preco))
                {
                    Console.WriteLine("\n❌ Preço inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Estoque inicial: ");
                if (!int.TryParse(Console.ReadLine(), out int estoque))
                {
                    Console.WriteLine("\n❌ Estoque inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("ID da Categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int idCategoria))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new CriarProdutoRequestModel
                {
                    Nome = nome,
                    Preco = preco,
                    Estoque = estoque,
                    IdCategoria = idCategoria
                };

                var resultado = await produtoService.CriarProdutoAsync(request);

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

        private static async Task AtualizarProdutoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       ATUALIZAR PRODUTO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var produtoAtual = await produtoService.ObterPorIdAsync(id);
                if (!produtoAtual.Sucesso)
                {
                    Console.WriteLine("\n❌ Produto não encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nProduto atual:");
                Console.WriteLine($"Nome: {produtoAtual.Data.Nome}");
                Console.WriteLine($"Preço: R$ {produtoAtual.Data.Preco:N2}");
                Console.WriteLine($"Estoque: {produtoAtual.Data.Estoque}");
                Console.WriteLine($"Categoria: {produtoAtual.Data.Categoria} (ID: {produtoAtual.Data.IdCategoria})");
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

                Console.Write("Novo preço: R$ ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal preco))
                {
                    Console.WriteLine("\n❌ Preço inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Novo estoque: ");
                if (!int.TryParse(Console.ReadLine(), out int estoque))
                {
                    Console.WriteLine("\n❌ Estoque inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Novo ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int idCategoria))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AtualizarProdutoRequestModel
                {
                    IdProduto = id,
                    Nome = nome,
                    Preco = preco,
                    Estoque = estoque,
                    IdCategoria = idCategoria
                };

                var resultado = await produtoService.AtualizarProdutoAsync(request);

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

        private static async Task ExcluirProdutoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       EXCLUIR PRODUTO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var produto = await produtoService.ObterPorIdAsync(id);
                if (!produto.Sucesso)
                {
                    Console.WriteLine("\n❌ Produto não encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nProduto: {produto.Data.Nome}");
                Console.WriteLine($"Estoque: {produto.Data.Estoque}");
                Console.WriteLine();
                Console.Write("Confirma a exclusão? (S/N): ");

                if (Console.ReadLine()?.ToUpper() != "S")
                {
                    Console.WriteLine("\n⚠️ Operação cancelada.");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await produtoService.ExcluirProdutoAsync(id);

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

        private static async Task AtualizarPrecoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       ATUALIZAR PREÇO");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var produtoAtual = await produtoService.ObterPorIdAsync(id);
                if (produtoAtual.Sucesso)
                {
                    Console.WriteLine($"\nProduto: {produtoAtual.Data.Nome}");
                    Console.WriteLine($"Preço atual: R$ {produtoAtual.Data.Preco:N2}");
                    Console.WriteLine();
                }

                Console.Write("Novo preço: R$ ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal novoPreco))
                {
                    Console.WriteLine("\n❌ Preço inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AtualizarPrecoProdutoRequestModel
                {
                    IdProduto = id,
                    NovoPreco = novoPreco
                };

                var resultado = await produtoService.AtualizarPrecoAsync(request);

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

        private static async Task AdicionarEstoqueAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       ADICIONAR ESTOQUE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var produtoAtual = await produtoService.ObterPorIdAsync(id);
                if (produtoAtual.Sucesso)
                {
                    Console.WriteLine($"\nProduto: {produtoAtual.Data.Nome}");
                    Console.WriteLine($"Estoque atual: {produtoAtual.Data.Estoque}");
                    Console.WriteLine();
                }

                Console.Write("Quantidade a adicionar: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade))
                {
                    Console.WriteLine("\n❌ Quantidade inválida!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AdicionarEstoqueProdutoRequestModel
                {
                    IdProduto = id,
                    Quantidade = quantidade
                };

                var resultado = await produtoService.AdicionarEstoqueAsync(request);

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

        private static async Task RemoverEstoqueAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       REMOVER ESTOQUE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var produtoAtual = await produtoService.ObterPorIdAsync(id);
                if (produtoAtual.Sucesso)
                {
                    Console.WriteLine($"\nProduto: {produtoAtual.Data.Nome}");
                    Console.WriteLine($"Estoque atual: {produtoAtual.Data.Estoque}");
                    Console.WriteLine();
                }

                Console.Write("Quantidade a remover: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade))
                {
                    Console.WriteLine("\n❌ Quantidade inválida!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new RemoverEstoqueProdutoRequestModel
                {
                    IdProduto = id,
                    Quantidade = quantidade
                };

                var resultado = await produtoService.RemoverEstoqueAsync(request);

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

        private static async Task AtualizarEstoqueAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       ATUALIZAR ESTOQUE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var produtoAtual = await produtoService.ObterPorIdAsync(id);
                if (produtoAtual.Sucesso)
                {
                    Console.WriteLine($"\nProduto: {produtoAtual.Data.Nome}");
                    Console.WriteLine($"Estoque atual: {produtoAtual.Data.Estoque}");
                    Console.WriteLine();
                }

                Console.Write("Novo estoque: ");
                if (!int.TryParse(Console.ReadLine(), out int novoEstoque))
                {
                    Console.WriteLine("\n❌ Estoque inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AtualizarEstoqueProdutoRequestModel
                {
                    IdProduto = id,
                    NovoEstoque = novoEstoque
                };

                var resultado = await produtoService.AtualizarEstoqueAsync(request);

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

        private static async Task VerificarDisponibilidadeAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("   VERIFICAR DISPONIBILIDADE");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

            try
            {
                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Quantidade necessária: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade))
                {
                    Console.WriteLine("\n❌ Quantidade inválida!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await produtoService.VerificarEstoqueDisponivelAsync(id, quantidade);

                if (resultado.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");
                    if (resultado.Data)
                    {
                        Console.WriteLine($"✅ {resultado.Mensagem}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ {resultado.Mensagem}");
                    }
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

        private static async Task ListarPorCategoriaAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("   LISTAR PRODUTOS POR CATEGORIA");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                // Lista categorias disponíveis
                var categorias = await categoriaService.ListarCategoriasAsync(new());
                if (categorias.Sucesso && categorias.Data.Lista.Any())
                {
                    Console.WriteLine("Categorias disponíveis:");
                    foreach (var cat in categorias.Data.Lista.Where(c => c.Situacao == "Ativo"))
                    {
                        Console.WriteLine($"  [{cat.IdCategoria}] {cat.Nome}");
                    }
                    Console.WriteLine();
                }

                Console.Write("Digite o ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await produtoService.ListarProdutosPorCategoriaAsync(id);

                Console.WriteLine();
                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Count} produto(s) encontrado(s):\n");

                    foreach (var prod in resultado.Data)
                    {
                        Console.WriteLine($"ID: {prod.IdProduto}");
                        Console.WriteLine($"Nome: {prod.Nome}");
                        Console.WriteLine($"Preço: R$ {prod.Preco:N2}");
                        Console.WriteLine($"Estoque: {prod.Estoque} {(prod.TemEstoque ? "✅" : "❌")}");
                        Console.WriteLine("------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhum produto encontrado nesta categoria.");
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
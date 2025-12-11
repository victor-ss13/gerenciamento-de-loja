using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Categoria;
using Microsoft.Extensions.DependencyInjection;

namespace LojaGerenciamento.ConsoleApp.Menus
{
    public static class MenuCategorias
    {
        public static async Task ExibirAsync(IServiceProvider serviceProvider)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("     GERENCIAR CATEGORIAS");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("1. Listar todas as categorias");
                Console.WriteLine("2. Buscar categoria por ID");
                Console.WriteLine("3. Criar nova categoria");
                Console.WriteLine("4. Atualizar categoria");
                Console.WriteLine("5. Excluir categoria");
                Console.WriteLine("6. Adicionar produto à categoria");
                Console.WriteLine("7. Remover produto da categoria");
                Console.WriteLine("0. Voltar ao menu principal");
                Console.WriteLine();
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await ListarCategoriasAsync(serviceProvider);
                        break;
                    case "2":
                        await BuscarCategoriaPorIdAsync(serviceProvider);
                        break;
                    case "3":
                        await CriarCategoriaAsync(serviceProvider);
                        break;
                    case "4":
                        await AtualizarCategoriaAsync(serviceProvider);
                        break;
                    case "5":
                        await ExcluirCategoriaAsync(serviceProvider);
                        break;
                    case "6":
                        await AdicionarProdutoAsync(serviceProvider);
                        break;
                    case "7":
                        await RemoverProdutoAsync(serviceProvider);
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

        private static async Task ListarCategoriasAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     LISTAR CATEGORIAS");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.WriteLine("Filtros (deixe em branco para listar todas):");
                Console.WriteLine();

                Console.Write("Nome (busca parcial): ");
                var nome = Console.ReadLine();

                Console.Write("Apenas sem produtos? (S/N): ");
                var semProdutos = Console.ReadLine()?.ToUpper() == "S";

                var filtro = new FiltroCategoriaRequestModel
                {
                    Nome = string.IsNullOrWhiteSpace(nome) ? null : nome,
                    ApenasCategoriasSemProdutos = semProdutos ? true : null
                };

                var resultado = await categoriaService.ListarCategoriasAsync(filtro);

                Console.WriteLine();
                Console.WriteLine("====================================");

                if (resultado.Sucesso && resultado.Data.Lista.Any())
                {
                    Console.WriteLine($"\n✅ {resultado.Data.Lista.Count} categoria(s) encontrada(s):\n");

                    foreach (var cat in resultado.Data.Lista)
                    {
                        Console.WriteLine($"ID: {cat.IdCategoria}");
                        Console.WriteLine($"Nome: {cat.Nome}");
                        Console.WriteLine($"Situação: {cat.Situacao}");
                        Console.WriteLine($"Quantidade de Produtos: {cat.QuantidadeProdutos}");

                        if (cat.Produtos.Any())
                        {
                            Console.WriteLine("Produtos:");
                            foreach (var prod in cat.Produtos)
                            {
                                Console.WriteLine($"  - {prod.Nome} (R$ {prod.Preco:N2}) - Estoque: {prod.Estoque}");
                            }
                        }

                        Console.WriteLine("------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\n⚠️ Nenhuma categoria encontrada.");
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

        private static async Task BuscarCategoriaPorIdAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     BUSCAR CATEGORIA POR ID");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.Write("Digite o ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await categoriaService.ObterPorIdAsync(id);

                if (resultado.Sucesso)
                {
                    Console.WriteLine();
                    Console.WriteLine("====================================");
                    Console.WriteLine($"ID: {resultado.Data.IdCategoria}");
                    Console.WriteLine($"Nome: {resultado.Data.Nome}");
                    Console.WriteLine($"Situação: {resultado.Data.Situacao}");
                    Console.WriteLine($"Quantidade de Produtos: {resultado.Data.QuantidadeProdutos}");

                    if (resultado.Data.Produtos.Any())
                    {
                        Console.WriteLine("\nProdutos:");
                        foreach (var prod in resultado.Data.Produtos)
                        {
                            Console.WriteLine($"  - [{prod.IdProduto}] {prod.Nome} (R$ {prod.Preco:N2}) - Estoque: {prod.Estoque}");
                        }
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

        private static async Task CriarCategoriaAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     CRIAR NOVA CATEGORIA");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.Write("Nome da categoria: ");
                var nome = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nome))
                {
                    Console.WriteLine("\n❌ Nome é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new CriarCategoriaRequestModel
                {
                    Nome = nome
                };

                var resultado = await categoriaService.CriarCategoriaAsync(request);

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

        private static async Task AtualizarCategoriaAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     ATUALIZAR CATEGORIA");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.Write("Digite o ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                // Busca a categoria atual
                var categoriaAtual = await categoriaService.ObterPorIdAsync(id);
                if (!categoriaAtual.Sucesso)
                {
                    Console.WriteLine("\n❌ Categoria não encontrada!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nCategoria atual: {categoriaAtual.Data.Nome}");
                Console.WriteLine();
                Console.Write("Novo nome: ");
                var novoNome = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(novoNome))
                {
                    Console.WriteLine("\n❌ Nome é obrigatório!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AtualizarCategoriaRequestModel
                {
                    IdCategoria = id,
                    Nome = novoNome
                };

                var resultado = await categoriaService.AtualizarCategoriaAsync(request);

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

        private static async Task ExcluirCategoriaAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     EXCLUIR CATEGORIA");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.Write("Digite o ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                // Busca a categoria
                var categoria = await categoriaService.ObterPorIdAsync(id);
                if (!categoria.Sucesso)
                {
                    Console.WriteLine("\n❌ Categoria não encontrada!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nCategoria: {categoria.Data.Nome}");
                Console.WriteLine($"Produtos vinculados: {categoria.Data.QuantidadeProdutos}");
                Console.WriteLine();
                Console.Write("Confirma a exclusão? (S/N): ");

                if (Console.ReadLine()?.ToUpper() != "S")
                {
                    Console.WriteLine("\n⚠️ Operação cancelada.");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var resultado = await categoriaService.ExcluirCategoriaAsync(id);

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

        private static async Task AdicionarProdutoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("  ADICIONAR PRODUTO À CATEGORIA");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.Write("Digite o ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int idCategoria))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Digite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int idProduto))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new AdicionarProdutoCategoriaRequestModel
                {
                    IdCategoria = idCategoria,
                    IdProduto = idProduto
                };

                var resultado = await categoriaService.AdicionarProdutoAsync(request);

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

        private static async Task RemoverProdutoAsync(IServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("  REMOVER PRODUTO DA CATEGORIA");
            Console.WriteLine("====================================");
            Console.WriteLine();

            using var scope = serviceProvider.CreateScope();
            var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();

            try
            {
                Console.Write("Digite o ID da categoria: ");
                if (!int.TryParse(Console.ReadLine(), out int idCategoria))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                // Mostra os produtos da categoria
                var categoria = await categoriaService.ObterPorIdAsync(idCategoria);
                if (categoria.Sucesso && categoria.Data.Produtos.Any())
                {
                    Console.WriteLine("\nProdutos na categoria:");
                    foreach (var prod in categoria.Data.Produtos)
                    {
                        Console.WriteLine($"  - [{prod.IdProduto}] {prod.Nome}");
                    }
                    Console.WriteLine();
                }

                Console.Write("Digite o ID do produto a remover: ");
                if (!int.TryParse(Console.ReadLine(), out int idProduto))
                {
                    Console.WriteLine("\n❌ ID inválido!");
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    return;
                }

                var request = new RemoverProdutoCategoriaRequestModel
                {
                    IdCategoria = idCategoria,
                    IdProduto = idProduto
                };

                var resultado = await categoriaService.RemoverProdutoAsync(request);

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
    }
}
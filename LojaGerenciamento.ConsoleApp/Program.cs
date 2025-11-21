using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = builder.Build();

var services = new ServiceCollection();
services.AddDbContext<LojaContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

var serviceProvider = services.BuildServiceProvider();
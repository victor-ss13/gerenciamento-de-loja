using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Services;
using LojaGerenciamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados
builder.Services.AddDbContext<LojaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Injeção de Dependência (Copiado do seu ConsoleApp)
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IItemPedidoService, ItemPedidoService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();

// 3. Configuração do CORS (Para permitir que o Angular acesse a API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // URL padrão do Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 4. Serviços padrão da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================
// PIPELINE DE REQUISIÇÃO
// ============================================

// Swagger (Documentação visual)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplica o CORS antes de mapear os controllers
app.UseCors("PermitirAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
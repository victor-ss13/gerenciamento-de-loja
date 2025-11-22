# **LojaOnline – Sistema de Gerenciamento de Loja**

Sistema de gerenciamento de loja desenvolvido em **C# (.NET)** com **SQL Server** e **Entity Framework Core**, estruturado em camadas e operando via **Console**.  
O objetivo é construir uma solução completa que envolva modelagem de dados, lógica de domínio, acesso a banco e interface por linha de comando.

---

## 📂 **Estrutura do Projeto**

```
LojaOnline/
│
├── Domain/ # Entidades (modelo de domínio)
│ ├── Cliente.cs
│ ├── Produto.cs
│ ├── Categoria.cs
│ ├── Pedido.cs
│ ├── ItemPedido.cs
│ └── Pagamento.cs
│
├── Infrastructure/ # DbContext e configuração do EF Core
│ └── LojaContext.cs
│
├── Application/ # Repositórios e serviços (CRUD)
│ ├── Repositories/
│ └── Services/
│
├── ConsoleUI/ # Interface de console (menus)
│ └── Menus.cs
│
├── Program.cs
└── appsettings.json
```


---

## 🧭 **Etapas de Desenvolvimento**

### **1. Estrutura inicial**
Criação da arquitetura base do projeto, com divisão clara entre Domain, Application, Infrastructure e ConsoleUI.

---

### **2. Criação do Domain (Entidades)**
Modelagem das classes que representam o núcleo do sistema:

- Cliente  
- Categoria  
- Produto  
- Pedido  
- ItemPedido  
- Pagamento  

Cada entidade possui propriedades essenciais e relacionamentos definidos conforme o DER.

---

### **3. Configuração do SQL Server**
Instalação, configuração e criação do banco para o projeto usando SQL Server Management Studio (SSMS).

---

### **4. Configuração do Entity Framework Core**
- Criação do `DbContext`
- Registro das entidades via `DbSet`
- Inserção da connection string no `appsettings.json`
- Configuração da Injeção de Dependência (quando necessário)

---

### **5. Migrations**
Uso aprofundado das migrations para versionar o esquema do banco:

```
dotnet ef migrations add NomeDaMigration
dotnet ef database update
dotnet ef migrations remove
```


Inclui:
- Como criar
- Como aplicar
- Como desfazer
- Como versionar corretamente

---

### **6. Repositórios (CRUD com EF Core)**
Implementação de operações fundamentais:

- Criar
- Ler
- Atualizar
- Excluir

Sempre dentro da camada `Application`, respeitando os princípios de organização.

---

### **7. Menus do Console**
Criação da interface textual que permite:

- Gerenciar clientes
- Gerenciar produtos
- Gerenciar categorias
- Criar pedidos
- Listar pedidos
- Registrar pagamentos

---

## 💾 **Tecnologias Utilizadas**

- **C# / .NET**
- **Entity Framework Core**
- **SQL Server**
- Aplicação em **Console**
- Arquitetura em camadas

---

## 🎯 **Objetivo do Projeto**

Este projeto tem foco em:

- Estruturar corretamente as camadas de um software
- Modelar bancos relacionais
- Usar EF Core de forma prática
- Aplicar migrations
- Criar um CRUD completo com repositórios
- Construir um sistema funcional via console

---

## 🚀 **Como Executar**

1. Configure o SQL Server e ajuste a `connection string` no `appsettings.json`.
2. Aplique as migrations:

```
dotnet ef database update
```

3. Rode o projeto:

```
dotnet run
```

4. Use o menu do console para navegar pelas funcionalidades.

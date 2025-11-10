# Tough Service (Projeto Tough Service) 🚀

![Tough Service Logo]([httpsCaminho/Para/Sua/Logo.png](https://www.econodata.com.br/consulta-empresa/28185366000100-tough-service-extintores-equipamentos-e-assessoria-de-seguranca-ltda)) 
Projeto de aplicação web completa para a "Tough Service", uma empresa de equipamentos e serviços de segurança contra incêndio. A plataforma combina um **E-commerce** para venda de produtos (extintores, etc.) com um **Sistema de Orçamentos e Chamados** para serviços de manutenção.

Este projeto foi desenvolvido como um portfólio de ponta-a-ponta, utilizando a stack .NET (ASP.NET Core MVC, Entity Framework Core) e MySQL.

**Status do Projeto:** 🚧 Em Desenvolvimento 🚧

---

## ✨ Funcionalidades Principais

A plataforma é dividida em várias áreas-chave para atender tanto os clientes quanto os administradores.

### 🛍️ Para Clientes (E-commerce)
* **Loja de Produtos:** Visualização de produtos (extintores, etc.) com detalhes, imagens e preços.
* **Carrinho de Compras Híbrido:**
    * **Visitantes (Anónimos):** O carrinho é guardado na **Sessão** (Session).
    * **Utilizadores Logados:** O carrinho é guardado na **Base de Dados** (MySQL).
    * **Migração Automática:** Ao fazer login, o carrinho da sessão é automaticamente transferido e "juntado" (merged) com o carrinho da base de dados.
* **Navegação e Detalhes:** Páginas de detalhes dos produtos com itens relacionados.

### 🔧 Para Clientes (Serviços)
* **Catálogo de Serviços:** Página de serviços (Manutenção, Reparo, Instalação) com descrições.
* **Sistema de Orçamento (Chamados):**
    * Um formulário modal permite aos utilizadores logados solicitar orçamentos detalhados.
    * O formulário inclui integração com a API **ViaCEP** para preenchimento automático de endereço.
    * A solicitação é guardada na tabela `Chamados` na base de dados.

### 👤 Sistema de Contas (Identity)
* **Registo de Utilizador:** Sistema de registo completo com validação e **Google reCAPTCHA** para prevenção de bots.
* **Login e Logout:** Autenticação segura usando **ASP.NET Identity Framework**.
* **Perfil do Utilizador:** Um menu dropdown de perfil que dá acesso ao perfil do utilizador, histórico de pedidos (a implementar) e botão de "Sair".

### 🔒 Área Administrativa (Admin)
* **Gestão de Produtos:** Uma área restrita onde o admin pode **Adicionar, Editar e Remover** produtos da loja.
* **Gestão de Chamados:** Visualização de todos os orçamentos e chamados de serviço solicitados pelos clientes.

---

## 🔧 Tecnologias Utilizadas

* **Backend:** C# com .NET 8 (ASP.NET Core MVC)
* **Base de Dados:** MySQL
* **ORM:** Entity Framework Core 8
* **Autenticação:** ASP.NET Core Identity
* **Frontend:** HTML5, CSS3, JavaScript (ES6)
* **Design:** Estilo Minimalista (CSS customizado)
* **APIs:** Google reCAPTCHA, ViaCEP

---

## 🚀 Como Executar o Projeto

1.  **Clone o Repositório:**
    ```bash
    git clone [https://github.com/Rick-0110/ProjetoThoughServiceMvc.git](https://github.com/Rick-0110/ProjetoThoughServiceMvc.git)
    cd ProjetoThoughServiceMvc
    ```

2.  **Configure as Credenciais:**
    * Abra o ficheiro `appsettings.json`.
    * Adicione a sua "Connection String" do MySQL.
    * Adicione as suas chaves "SiteKey" e "SecretKey" do Google reCAPTCHA.

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=thoughservice;User=root;Password=sua_senha;"
      },
      "Captcha": {
        "SiteKey": "SUA_SITE_KEY_AQUI",
        "SecretKey": "SUA_SECRET_KEY_AQUI"
      }
    }
    ```

3.  **Aplique as Migrações (Migrations):**
    * Certifique-se de que a base de dados (`thoughservice`) existe no seu MySQL.
    * Execute os comandos do EF Core para criar as tabelas:
    ```bash
    dotnet ef database update
    ```

4.  **Execute a Aplicação:**
    ```bash
    dotnet run
    ```
    * A aplicação estará a correr em `https://localhost:7004` (ou numa porta semelhante).

---

## 👨‍💻 Autor

Feito por **[Seu Nome Aqui]** (Rick-0110).

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Rick-0110)

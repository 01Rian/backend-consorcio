# API de Gerenciamento de Consórcios

Esta é uma API construída com ASP.NET Core para gerenciar consórcios, cotas e usuários, seguindo princípios de arquitetura limpa e boas práticas de desenvolvimento.

## Índice

- [Visão Geral da Arquitetura](#-visão-geral-da-arquitetura)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Como Executar o Projeto](#-como-executar-o-projeto)
  - [Pré-requisitos](#pré-requisitos)
  - [Opção 1: Executando com Docker (Recomendado)](#opção-1-executando-com-docker-recomendado)
  - [Opção 2: Executando Localmente](#opção-2-executando-localmente)
- [Executando os Testes](#-executando-os-testes)
- [Documentação da API](#-documentação-da-api)
  - [Exemplos de Uso](#exemplos-de-uso)
  - [Endpoints](#endpoints)

---

## 🏛️ Visão Geral da Arquitetura

O projeto utiliza uma **Arquitetura em Camadas**, refletida na estrutura de pastas, para garantir a separação de responsabilidades, manutenibilidade e testabilidade do código.

```
└── Src/
    ├── Controllers/      # Camada de Apresentação (API Endpoints)
    │   ├── Consorcio/
    │   ├── Cotas/
    │   └── Usuario/
    ├── Data/             # Camada de Acesso a Dados
    │   ├── Context/
    │   ├── Migrations/
    │   └── Repositories/
    ├── Models/           # Entidades do Domínio
    │   └── Entities/
    └── Services/         # Camada de Lógica de Negócio
        ├── Implementations/
        └── Interfaces/
```

-   **Controllers**: Responsáveis por receber as requisições HTTP, validar os DTOs e retornar as respostas.
-   **Services**: Contêm a lógica de negócio da aplicação, orquestrando as operações e interagindo com os repositórios.
-   **Repositories**: Abstraem o acesso ao banco de dados, centralizando as operações de persistência.
-   **Models/Entities**: Representam as estruturas de dados e tabelas do banco de dados.

---

## ✨ Tecnologias Utilizadas

-   **Framework**: [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
-   **Web API**: ASP.NET Core 8
-   **ORM**: Entity Framework Core 8
-   **Banco de Dados**: SQL Server
-   **Autenticação**: JWT (JSON Web Tokens)
-   **Documentação**: Swagger
-   **Testes**: xUnit, Moq, Bogus
-   **Containerização**: Docker
-   **Variáveis de Ambiente**: DotNetEnv

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Docker](https://www.docker.com/products/docker-desktop/) (para a Opção 1)
-   Um cliente de API, como [Postman](https://www.postman.com/) ou a extensão [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) do VS Code.

### Opção 1: Executando com Docker (Recomendado)

Esta abordagem utiliza o Docker para executar o banco de dados SQL Server, simplificando a configuração do ambiente.

1.  **Clone o repositório:**
    ```bash
    git clone <URL>
    cd rian-p01-back
    ```

2.  **Crie o arquivo de ambiente:**
    Copie o arquivo de exemplo `.env.example` para um novo arquivo chamado `.env`.
    ```bash
    # No Windows (Command Prompt)
    copy .env.example .env

    # No Linux ou Windows (PowerShell/Git Bash)
    cp .env.example .env
    ```
    *Certifique-se de que a variável `DB_SERVER` no arquivo `.env` está como `localhost` e revise a senha `DB_PASSWORD` se necessário.*

3.  **Inicie o container do banco de dados:**
    Execute o comando abaixo para iniciar o container do SQL Server em segundo plano.
    ```bash
    docker-compose up -d
    ```

4.  **Restaure as dependências e aplique as migrations:**
    ```bash
    dotnet restore
    dotnet ef database update
    ```

5.  **Execute a aplicação:**
    ```bash
    dotnet run
    ```
    A API estará disponível em `https://localhost:7103` ou `http://localhost:5297`.

### Opção 2: Executando Localmente

Esta abordagem requer uma instância do SQL Server rodando em sua máquina local.

1.  **Clone o repositório** (se ainda não o fez).

2.  **Configure o Banco de Dados:**
    - Abra o arquivo `appsettings.Development.json`.
    - Altere a string de conexão `DefaultConnection` para apontar para a sua instância local do SQL Server.

3.  **Aplique as Migrations:**
    ```bash
    dotnet ef database update
    ```

4.  **Restaure as dependências e execute:**
    ```bash
    dotnet restore
    dotnet run
    ```

---

## ✅ Executando os Testes

Para executar a suíte de testes unitários do projeto, utilize o seguinte comando na raiz do repositório:

```bash
dotnet test
```

---

## 📖 Documentação da API

A documentação completa dos endpoints está disponível via Swagger quando a aplicação está em execução, no endpoint `/swagger`.

### Exemplos de Uso

A seguir, alguns exemplos de requisições baseados nos arquivos `.http` do projeto.

#### Criar um novo usuário

**`POST /api/Usuario`**

**Request Body:**
```json
{
  "nome": "Carlos Pereira",
  "email": "carlos.pereira@exemplo.com",
  "senha": "senhaSegura789",
  "cpf": "321.654.987-00",
  "telefone": "(31) 5678-1234",
  "celular": "(31) 98765-4321",
  "dataNascimento": "1988-08-20",
  "endereco": {
    "logradouro": "Rua das Palmeiras, 123",
    "numero": "123",
    "complemento": "Apto 202",
    "bairro": "Centro",
    "cidade": "Belo Horizonte",
    "estado": "MG",
    "cep": "30110-050"
  },
  "ativo": true
}
```

#### Autenticar um usuário

**`POST /api/Usuario/login`**

**Request Body:**
```json
{
  "email": "carlos.pereira@exemplo.com",
  "senha": "senhaSegura789"
}
```

#### Criar um novo consórcio

**`POST /api/Consorcio`** (Requer autenticação)

**Request Body:**
```json
{
  "nome": "Consórcio Imóvel Premium",
  "codigo": "IMP2024",
  "valorBem": 250000.00,
  "quantidadeCotas": 1,
  "prazoMeses": 60,
  "taxaAdministracao": 15.00,
  "fundoReserva": 2500.00,
  "dataInicio": "2025-01-01T00:00:00"
}
```

### Endpoints

A seguir estão os endpoints disponíveis na API. A maioria das rotas requer autenticação via token JWT Bearer.

---

#### 🏛️ **ConsorcioController** (`api/Consorcio`)

| Método HTTP | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/` | Cria um novo consórcio. |
| `GET` | `/` | Obtém todos os consórcios (ativos e inativos). |
| `GET` | `/{id}` | Obtém um consórcio pelo ID. |
| `GET` | `/{id}/with-cotas` | Obtém um consórcio com suas cotas. |
| `GET` | `/active` | Obtém todos os consórcios ativos. |
| `GET` | `/{id}/available-cotas` | Obtém as cotas disponíveis de um consórcio. |
| `POST` | `/{id}/cotas` | Adiciona novas cotas a um consórcio. |
| `POST` | `/assign-cota` | Atribui uma cota a um usuário. |
| `PUT` | `/{id}` | Atualiza um consórcio existente. |
| `DELETE` | `/{id}` | Deleta um consórcio. |

---

#### 📑 **CotasController** (`api/Cotas`)

| Método HTTP | Rota | Descrição |
| :--- | :--- | :--- |
| `GET` | `/{id}` | Obtém uma cota pelo ID. |
| `GET` | `/{id}/detailed` | Obtém os detalhes de uma cota. |
| `GET` | `/by-consorcio/{consorcioId}` | Obtém todas as cotas de um consórcio específico. |
| `GET` | `/by-user/{usuarioId}` | Obtém todas as cotas de um usuário específico. |
| `GET` | `/active` | Obtém todas as cotas ativas. |
| `GET` | `/contemplated` | Obtém todas as cotas contempladas. |
| `POST` | `/{id}/contemplate` | Marca uma cota como contemplada. |
| `POST` | `/{id}/register-payment` | Registra um pagamento para uma cota. |
| `PUT` | `/{id}/update-status` | Atualiza o status de uma cota (ativa, suspensa, etc.). |
| `DELETE` | `/{id}/remove-user` | Remove o usuário associado a uma cota. |

---

#### 👤 **UsuarioController** (`api/Usuario`)

| Método HTTP | Rota | Descrição | Autenticação |
| :--- | :--- | :--- | :--- |
| `POST` | `/` | Cria um novo usuário. | Não requer |
| `POST` | `/login` | Autentica um usuário e retorna um token. | Não requer |
| `GET` | `/` | Obtém todos os usuários. | Requer |
| `GET` | `/{id}` | Obtém um usuário pelo ID. | Requer |
| `GET` | `/email/{email}` | Obtém um usuário pelo e-mail. | Requer |
| `GET` | `/cpf/{cpf}` | Obtém um usuário pelo CPF. | Requer |
| `PUT` | `/{id}` | Atualiza um usuário existente. | Requer |
| `DELETE` | `/{id}` | Deleta um usuário. | Requer |
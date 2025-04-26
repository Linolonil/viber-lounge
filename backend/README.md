# Viber Lounge - Backend

Backend da aplicação Viber Lounge, desenvolvido com Node.js, TypeScript e Express.

## 🚀 Tecnologias

- Node.js
- TypeScript
- Express
- SQLite
- Multer (Upload de arquivos)
- Sharp (Processamento de imagens)

## 📁 Estrutura do Projeto

```
backend/
├── src/
│   ├── controllers/     # Controladores da aplicação
│   ├── core/           # Lógica de negócio central
│   ├── infrastructure/ # Configurações de infraestrutura
│   ├── middlewares/    # Middlewares da aplicação
│   ├── presentation/   # Camada de apresentação
│   ├── repositories/   # Repositórios de dados
│   ├── routes/         # Rotas da API
│   ├── services/       # Serviços da aplicação
│   ├── types/          # Definições de tipos
│   ├── utils/          # Utilitários
│   └── index.ts        # Ponto de entrada
├── data/               # Banco de dados SQLite
├── uploads/           # Armazenamento de imagens
└── package.json       # Dependências e scripts
```

## 🔧 Configuração

1. Instale as dependências:
```bash
npm install
```

2. Configure as variáveis de ambiente:
```bash
cp .env.example .env
```

3. Inicie o servidor:
```bash
npm run dev
```

## 📝 Funcionalidades Principais

### Produtos
- CRUD completo de produtos
- Upload e processamento de imagens
- Validação de dados
- Tratamento de erros

### Vendas
- Registro de vendas
- Histórico de vendas
- Relatórios por período

### Período de Trabalho
- Controle de períodos de trabalho
- Início e encerramento de períodos
- Relatórios de vendas por período

## 🔒 Segurança

- Autenticação JWT
- Validação de dados
- Sanitização de inputs
- Tratamento de erros
- Proteção contra uploads maliciosos

## 🛠️ Desenvolvimento

### Scripts Disponíveis

- `npm run dev`: Inicia o servidor em modo desenvolvimento
- `npm run build`: Compila o projeto
- `npm start`: Inicia o servidor em produção
- `npm run lint`: Executa o linter
- `npm run test`: Executa os testes

### Convenções

- TypeScript para tipagem estática
- Arquitetura limpa e modular
- Padrão Repository para acesso a dados
- Serviços para lógica de negócio
- Controllers para manipulação de requisições

## 📦 Deploy

O projeto pode ser containerizado usando Docker:

```bash
docker-compose up -d
```

## 📄 Licença

Este projeto está sob a licença MIT.

## 📡 Endpoints da API

### Produtos

#### Listar Produtos
```http
GET /api/produtos
```
**Resposta:**
```json
[
  {
    "id": "string",
    "nome": "string",
    "preco": number,
    "quantidade": number,
    "imagemUrl": "string"
  }
]
```

#### Buscar Produto por ID
```http
GET /api/produtos/:id
```
**Resposta:**
```json
{
  "id": "string",
  "nome": "string",
  "preco": number,
  "quantidade": number,
  "imagemUrl": "string"
}
```

#### Criar Produto
```http
POST /api/produtos
```
**Body (FormData):**
- `nome`: string (obrigatório)
- `preco`: number (obrigatório)
- `quantidade`: number (obrigatório)
- `imagem`: File (obrigatório)

**Resposta:**
```json
{
  "id": "string",
  "nome": "string",
  "preco": number,
  "quantidade": number,
  "imagemUrl": "string"
}
```

#### Atualizar Produto
```http
PUT /api/produtos/:id
```
**Body (FormData):**
- `nome`: string (obrigatório)
- `preco`: number (obrigatório)
- `quantidade`: number (obrigatório)
- `imagem`: File (opcional)

**Resposta:**
```json
{
  "id": "string",
  "nome": "string",
  "preco": number,
  "quantidade": number,
  "imagemUrl": "string"
}
```

#### Excluir Produto
```http
DELETE /api/produtos/:id
```
**Resposta:** 204 No Content

### Vendas

#### Listar Vendas
```http
GET /api/vendas
```
**Resposta:**
```json
[
  {
    "id": "string",
    "data": "string",
    "itens": [
      {
        "produto": {
          "id": "string",
          "nome": "string",
          "preco": number
        },
        "quantidade": number
      }
    ],
    "total": number,
    "formaPagamento": "string",
    "status": "string"
  }
]
```

#### Criar Venda
```http
POST /api/vendas
```
**Body:**
```json
{
  "itens": [
    {
      "produtoId": "string",
      "quantidade": number
    }
  ],
  "formaPagamento": "string"
}
```
**Resposta:**
```json
{
  "id": "string",
  "data": "string",
  "itens": [
    {
      "produto": {
        "id": "string",
        "nome": "string",
        "preco": number
      },
      "quantidade": number
    }
  ],
  "total": number,
  "formaPagamento": "string",
  "status": "string"
}
```

#### Cancelar Venda
```http
DELETE /api/vendas/:id
```
**Resposta:** 204 No Content

### Período de Trabalho

#### Iniciar Período
```http
POST /api/periodo-trabalho/iniciar
```
**Body:**
```json
{
  "usuarioId": "string",
  "usuarioNome": "string"
}
```
**Resposta:**
```json
{
  "id": "string",
  "inicio": "string",
  "fim": null,
  "usuarioId": "string",
  "usuarioNome": "string",
  "status": "ativo"
}
```

#### Encerrar Período
```http
POST /api/periodo-trabalho/encerrar/:id
```
**Resposta:**
```json
{
  "id": "string",
  "inicio": "string",
  "fim": "string",
  "usuarioId": "string",
  "usuarioNome": "string",
  "status": "encerrado"
}
```

#### Buscar Período Atual
```http
GET /api/periodo-trabalho/atual/:usuarioId
```
**Resposta:**
```json
{
  "id": "string",
  "inicio": "string",
  "fim": "string",
  "usuarioId": "string",
  "usuarioNome": "string",
  "status": "string"
}
```

### Autenticação

#### Login
```http
POST /api/auth/login
```
**Body:**
```json
{
  "email": "string",
  "senha": "string"
}
```
**Resposta:**
```json
{
  "token": "string",
  "usuario": {
    "id": "string",
    "nome": "string",
    "email": "string",
    "role": "string"
  }
}
``` 
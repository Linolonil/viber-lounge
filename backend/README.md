# Backend do Sistema de Vendas

Este é o backend do sistema de vendas, desenvolvido em TypeScript com Express.js.

## Estrutura do Projeto

```
backend/
├── src/
│   ├── controllers/     # Controladores da aplicação
│   ├── middlewares/     # Middlewares (validação, tratamento de erros)
│   ├── repositories/    # Repositórios para acesso aos dados
│   ├── routes/         # Definição das rotas
│   ├── services/       # Lógica de negócio
│   ├── types.ts        # Definição de tipos
│   ├── utils/          # Utilitários
│   └── index.ts        # Ponto de entrada da aplicação
├── data/               # Armazenamento de dados (JSON)
└── package.json        # Dependências e scripts
```

## Funcionalidades

### Vendas

#### Criar Venda
```http
POST /api/vendas
```
Body:
```json
{
  "data": "2024-03-20T10:00:00.000Z",
  "itens": [
    {
      "produto": {
        "id": "produto1",
        "nome": "Produto 1",
        "preco": 50.50,
        "imagem": "url_ou_base64",
        "quantidade": 10
      },
      "quantidade": 2
    }
  ],
  "cliente": "Nome do Cliente",
  "formaPagamento": "pix",
  "total": 101.00
}
```

#### Listar Todas as Vendas
```http
GET /api/vendas
```

#### Buscar Venda por ID
```http
GET /api/vendas/:id
```

#### Buscar Vendas por Data
```http
GET /api/vendas/data/:date
```
Exemplo: `/api/vendas/data/2024-03-20`

#### Cancelar Venda
```http
DELETE /api/vendas/:id
```

#### Cancelar Item da Venda
```http
DELETE /api/vendas/:id/itens/:itemId
```
Body:
```json
{
  "quantidade": 2
}
```

### Validações

- **Criação de Venda**:
  - Data é obrigatória e deve ser válida
  - Pelo menos um item é obrigatório
  - Cliente é obrigatório
  - Forma de pagamento é obrigatória
  - Estoque deve ser suficiente
  - Total deve corresponder à soma dos itens

- **Cancelamento de Item**:
  - Quantidade deve ser maior que 0
  - Quantidade não pode ser maior que a quantidade atual
  - Item deve existir na venda

### Gestão de Estoque

- Estoque é atualizado automaticamente:
  - Diminui ao criar venda
  - Aumenta ao cancelar venda
  - Aumenta ao cancelar item

### Status da Venda

- **ativa**: Venda criada normalmente
- **cancelada**: Venda cancelada (totalmente ou sem itens)

## Instalação

1. Instale as dependências:
```bash
npm install
```

2. Inicie o servidor:
```bash
npm start
```

O servidor estará disponível em `http://localhost:3000`.

## Tecnologias Utilizadas

- TypeScript
- Express.js
- Node.js
- JSON (armazenamento de dados) 
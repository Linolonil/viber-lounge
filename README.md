# Viber Lounge - Sistema de Gestão

Sistema de gestão para o Viber Lounge, um bar/restaurante, desenvolvido com React, TypeScript, Node.js e Express.

## 🚀 Tecnologias

### Backend
- Node.js
- Express
- TypeScript
- Multer (para upload de imagens)
- JSON como banco de dados
- dotenv (para variáveis de ambiente)

### Frontend
- React
- TypeScript
- Tailwind CSS
- Shadcn/ui
- React Query
- React Router
- Lucide Icons
- Vite (para variáveis de ambiente)

## 📦 Instalação

### Backend
```bash
cd backend
npm install
# Copie o arquivo .env.example para .env e ajuste as variáveis
cp .env.example .env
npm run dev
```

### Frontend
```bash
cd frontend
npm install
# Copie o arquivo .env.example para .env e ajuste as variáveis
cp .env.example .env
npm run dev
```

## 🔧 Configuração

### Variáveis de Ambiente

#### Backend (.env)
```env
# Configurações do Servidor
PORT=3001
NODE_ENV=development

# Configurações de Upload
UPLOAD_DIR=uploads
MAX_FILE_SIZE=10485760 # 10MB em bytes

# Configurações de Banco de Dados
DATA_DIR=data
PRODUTOS_FILE=produtos.json
VENDAS_FILE=vendas.json

# Configurações de CORS
CORS_ORIGIN=http://localhost:3000
```

#### Frontend (.env)
```env
# Configurações da API
VITE_API_URL=http://localhost:3001/api
VITE_API_IMG_URL=http://localhost:3001/api/images
```

O backend roda na porta 3001 e o frontend na porta 3000 por padrão.

## 📚 Documentação da API

### Produtos

#### Listar todos os produtos
```http
GET /api/produtos
```

#### Buscar produto por ID
```http
GET /api/produtos/:id
```

#### Criar produto
```http
POST /api/produtos
Content-Type: multipart/form-data

{
  "nome": string,
  "preco": number,
  "quantidade": number,
  "imagem": File
}
```

#### Atualizar produto
```http
PUT /api/produtos/:id
Content-Type: multipart/form-data

{
  "nome": string,
  "preco": number,
  "quantidade": number,
  "imagem": File (opcional)
}
```

#### Deletar produto
```http
DELETE /api/produtos/:id
```

### Vendas

#### Listar todas as vendas
```http
GET /api/vendas
```

#### Buscar venda por ID
```http
GET /api/vendas/:id
```

#### Criar venda
```http
POST /api/vendas
Content-Type: application/json

{
  "cliente": string,
  "itens": [
    {
      "produto": {
        "id": string,
        "nome": string,
        "preco": number
      },
      "quantidade": number
    }
  ],
  "formaPagamento": "pix" | "credito" | "debito" | "dinheiro"
}
```

#### Buscar vendas por data
```http
GET /api/vendas/data/:date
```

#### Cancelar venda
```http
DELETE /api/vendas/:id
```

#### Cancelar item da venda
```http
DELETE /api/vendas/:id/itens/:itemId
Content-Type: application/json

{
  "quantidade": number
}
```

### Dashboard

#### Buscar dados do dashboard por data
```http
GET /api/dashboard/:date
```

#### Buscar rastreamento por data
```http
GET /api/trace/:date
```

## 📁 Estrutura do Frontend

### Páginas
- `/` - Dashboard
- `/produtos` - Lista de produtos
- `/produtos/novo` - Cadastro de produto
- `/vendas` - Lista de vendas
- `/vendas/nova` - Nova venda
- `/historico` - Histórico de vendas

### Componentes
- `Card` - Card reutilizável
- `Button` - Botão personalizado
- `Input` - Campo de entrada
- `Select` - Campo de seleção
- `Table` - Tabela de dados
- `Toast` - Notificações

### Serviços
- `api.ts` - Serviço centralizado de API
- `ProdutoService` - Serviço de produtos
- `VendaService` - Serviço de vendas
- `DashboardService` - Serviço do dashboard

### Tipos
- `Produto` - Interface do produto
- `Venda` - Interface da venda
- `ItemVenda` - Interface do item de venda
- `FormaPagamento` - Tipo de forma de pagamento
- `DadosGrafico` - Interface dos dados do gráfico

## 🔒 Validações

### Produto
- Nome é obrigatório
- Preço deve ser maior que zero
- Quantidade deve ser não negativa
- Imagem é obrigatória no cadastro
- Imagem deve ser do tipo jpeg, png, gif ou webp
- Tamanho máximo da imagem: 10MB

### Venda
- Cliente é obrigatório
- Pelo menos um item é obrigatório
- Quantidade do item deve ser maior que zero
- Forma de pagamento é obrigatória
- Forma de pagamento deve ser válida (pix, credito, debito, dinheiro)

## 🎨 Estilização

O projeto utiliza Tailwind CSS para estilização e shadcn/ui para componentes base. As cores principais são:

- Fundo: `bg-zinc-800`
- Borda: `border-zinc-700`
- Texto: `text-white`
- Destaque: `bg-viber-gold`

## 📝 Notas

- O backend salva as imagens na pasta `uploads`
- O banco de dados é um arquivo JSON na pasta `data`
- As imagens são servidas através do endpoint `/api/images`
- O sistema suporta múltiplas formas de pagamento
- O dashboard mostra dados por data
- O histórico de vendas permite filtros por data e forma de pagamento
- As variáveis de ambiente devem ser configuradas antes de iniciar o projeto
- O frontend usa o prefixo `VITE_` para as variáveis de ambiente
- O backend usa o pacote `dotenv` para carregar as variáveis de ambiente

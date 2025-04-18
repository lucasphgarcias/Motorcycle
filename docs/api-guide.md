# API Guide - Sistema de Gestão de Aluguel de Motocicletas

Este documento fornece informações detalhadas sobre como consumir a API REST do Sistema de Gestão de Aluguel de Motocicletas.

## Visão Geral

A API fornece endpoints para gerenciar motocicletas, entregadores, aluguéis e autenticação de usuários. Todas as requisições (exceto de autenticação) exigem um token JWT válido no cabeçalho de autorização.

## Base URL

```
http://localhost:5000/swagger/index.html
```

## Autenticação

A API utiliza autenticação baseada em JWT (JSON Web Token).

### Obter Token

```http
POST /auth/login
```

**Request Body:**

```json
{
  "name": "admin@example.com",
  "password": "senha123"
}
```

**Response (200 OK):**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Admin User",
    "email": "admin@example.com",
    "role": "admin"
  }
}
```

### Utilização do Token

Inclua o token no cabeçalho de autorização em todas as requisições subsequentes:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Endpoints

### Motocicletas

#### Listar Todas as Motocicletas

```http
GET /motorcycles
```

**Parâmetros de Query:**
- `page` (opcional): Número da página (default: 1)
- `pageSize` (opcional): Tamanho da página (default: 10)
- `model` (opcional): Filtro por modelo
- `year` (opcional): Filtro por ano
- `status` (opcional): Filtro por status (Available, Rented, Maintenance)

**Response (200 OK):**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "model": "Honda CG 160",
      "year": 2022,
      "licensePlate": "ABC1D23",
      "status": "Available",
      "dailyRentalRate": {
        "amount": 50.00,
        "currency": "BRL"
      }
    }
  ],
  "totalCount": 1,
  "pageSize": 10,
  "currentPage": 1,
  "totalPages": 1
}
```

#### Obter Motocicleta por ID

```http
GET /motorcycles/{id}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "model": "Honda CG 160",
  "year": 2022,
  "licensePlate": "ABC1D23",
  "status": "Available",
  "dailyRentalRate": {
    "amount": 50.00,
    "currency": "BRL"
  }
}
```

#### Criar Motocicleta

```http
POST /motorcycles
```

**Request Body:**

```json
{
  "model": "Honda CG 160",
  "year": 2022,
  "licensePlate": "ABC1D23",
  "dailyRentalRate": {
    "amount": 50.00,
    "currency": "BRL"
  }
}
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "model": "Honda CG 160",
  "year": 2022,
  "licensePlate": "ABC1D23",
  "status": "Available",
  "dailyRentalRate": {
    "amount": 50.00,
    "currency": "BRL"
  }
}
```

#### Atualizar Motocicleta

```http
PUT /motorcycles/{id}
```

**Request Body:**

```json
{
  "model": "Honda CG 160",
  "year": 2022,
  "licensePlate": "ABC1D23",
  "status": "Maintenance",
  "dailyRentalRate": {
    "amount": 55.00,
    "currency": "BRL"
  }
}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "model": "Honda CG 160",
  "year": 2022,
  "licensePlate": "ABC1D23",
  "status": "Maintenance",
  "dailyRentalRate": {
    "amount": 55.00,
    "currency": "BRL"
  }
}
```

#### Excluir Motocicleta

```http
DELETE /motorcycles/{id}
```

**Response (204 No Content)**

### Entregadores

#### Listar Todos os Entregadores

```http
GET /deliverypersons
```

**Parâmetros de Query:**
- `page` (opcional): Número da página (default: 1)
- `pageSize` (opcional): Tamanho da página (default: 10)
- `name` (opcional): Filtro por nome
- `cnpj` (opcional): Filtro por CNPJ

**Response (200 OK):**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "João Silva",
      "cnpj": "04.252.011/0001-10",
      "email": "joao.silva@example.com",
      "phoneNumber": "(11) 91234-5678",
      "driverLicense": {
        "number": "12345678901",
        "category": "A",
        "expirationDate": "2025-12-31"
      }
    }
  ],
  "totalCount": 1,
  "pageSize": 10,
  "currentPage": 1,
  "totalPages": 1
}
```

#### Obter Entregador por ID

```http
GET /deliverypersons/{id}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "cnpj": "04.252.011/0001-10",
  "email": "joao.silva@example.com",
  "phoneNumber": "(11) 91234-5678",
  "driverLicense": {
    "number": "12345678901",
    "category": "A",
    "expirationDate": "2025-12-31"
  }
}
```

#### Criar Entregador

```http
POST /deliverypersons
```

**Request Body:**

```json
{
  "name": "João Silva",
  "cnpj": "04.252.011/0001-10",
  "email": "joao.silva@example.com",
  "phoneNumber": "(11) 91234-5678",
  "driverLicense": {
    "number": "12345678901",
    "category": "A",
    "expirationDate": "2025-12-31"
  }
}
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "cnpj": "04.252.011/0001-10",
  "email": "joao.silva@example.com",
  "phoneNumber": "(11) 91234-5678",
  "driverLicense": {
    "number": "12345678901",
    "category": "A",
    "expirationDate": "2025-12-31"
  }
}
```

#### Atualizar Entregador

```http
PUT /deliverypersons/{id}
```

**Request Body:**

```json
{
  "name": "João Silva",
  "cnpj": "04.252.011/0001-10",
  "email": "joao.silva@example.com",
  "phoneNumber": "(11) 98765-4321",
  "driverLicense": {
    "number": "12345678901",
    "category": "A",
    "expirationDate": "2025-12-31"
  }
}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "cnpj": "04.252.011/0001-10",
  "email": "joao.silva@example.com",
  "phoneNumber": "(11) 98765-4321",
  "driverLicense": {
    "number": "12345678901",
    "category": "A",
    "expirationDate": "2025-12-31"
  }
}
```

#### Excluir Entregador

```http
DELETE /deliverypersons/{id}
```

**Response (204 No Content)**

### Aluguéis

#### Listar Todos os Aluguéis

```http
GET /rentals
```

**Parâmetros de Query:**
- `page` (opcional): Número da página (default: 1)
- `pageSize` (opcional): Tamanho da página (default: 10)
- `deliveryPersonId` (opcional): Filtro por ID do entregador
- `motorcycleId` (opcional): Filtro por ID da motocicleta
- `status` (opcional): Filtro por status (Active, Completed, Cancelled)

**Response (200 OK):**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "startDate": "2023-05-10",
      "endDate": "2023-05-17",
      "returnDate": null,
      "status": "Active",
      "planType": "Seven",
      "totalAmount": {
        "amount": 350.00,
        "currency": "BRL"
      },
      "motorcycle": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "model": "Honda CG 160",
        "licensePlate": "ABC1D23"
      },
      "deliveryPerson": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "João Silva",
        "cnpj": "04.252.011/0001-10"
      }
    }
  ],
  "totalCount": 1,
  "pageSize": 10,
  "currentPage": 1,
  "totalPages": 1
}
```

#### Obter Aluguel por ID

```http
GET /rentals/{id}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2023-05-10",
  "endDate": "2023-05-17",
  "returnDate": null,
  "status": "Active",
  "planType": "Seven",
  "totalAmount": {
    "amount": 350.00,
    "currency": "BRL"
  },
  "motorcycle": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "model": "Honda CG 160",
    "licensePlate": "ABC1D23"
  },
  "deliveryPerson": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "cnpj": "04.252.011/0001-10"
  }
}
```

#### Criar Aluguel

```http
POST /rentals
```

**Request Body:**

```json
{
  "startDate": "2023-05-10",
  "planType": "Seven",
  "motorcycleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "deliveryPersonId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2023-05-10",
  "endDate": "2023-05-17",
  "returnDate": null,
  "status": "Active",
  "planType": "Seven",
  "totalAmount": {
    "amount": 350.00,
    "currency": "BRL"
  },
  "motorcycle": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "model": "Honda CG 160",
    "licensePlate": "ABC1D23"
  },
  "deliveryPerson": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "cnpj": "04.252.011/0001-10"
  }
}
```

#### Registrar Devolução da Motocicleta

```http
PATCH /rentals/{id}/return
```

**Request Body:**

```json
{
  "returnDate": "2023-05-15"
}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2023-05-10",
  "endDate": "2023-05-17",
  "returnDate": "2023-05-15",
  "status": "Completed",
  "planType": "Seven",
  "totalAmount": {
    "amount": 350.00,
    "currency": "BRL"
  },
  "motorcycle": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "model": "Honda CG 160",
    "licensePlate": "ABC1D23"
  },
  "deliveryPerson": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "cnpj": "04.252.011/0001-10"
  }
}
```

#### Cancelar Aluguel

```http
PATCH /rentals/{id}/cancel
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2023-05-10",
  "endDate": "2023-05-17",
  "returnDate": null,
  "status": "Cancelled",
  "planType": "Seven",
  "totalAmount": {
    "amount": 0.00,
    "currency": "BRL"
  },
  "motorcycle": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "model": "Honda CG 160",
    "licensePlate": "ABC1D23"
  },
  "deliveryPerson": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "cnpj": "04.252.011/0001-10"
  }
}
```

## Erros e Validações

### Formato de Erro

Quando ocorre um erro, a API retorna um objeto com a seguinte estrutura:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Erro de Validação",
  "status": 400,
  "traceId": "00-f528ca4fbfe0a94e89fd7a6a414a3fb0-b58f635643d98b4e-00",
  "errors": {
    "propertyName": [
      "Mensagem de erro para a propriedade"
    ]
  }
}
```

### Códigos de Status HTTP

- `200 OK`: Requisição bem-sucedida
- `201 Created`: Recurso criado com sucesso
- `204 No Content`: Requisição bem-sucedida sem conteúdo a ser retornado
- `400 Bad Request`: Dados inválidos ou faltando
- `401 Unauthorized`: Autenticação necessária
- `403 Forbidden`: Acesso negado
- `404 Not Found`: Recurso não encontrado
- `409 Conflict`: Conflito com o estado atual do recurso
- `422 Unprocessable Entity`: Erro de validação de negócio
- `500 Internal Server Error`: Erro interno do servidor

## Regras de Negócio

### Planos de Aluguel

| Plano   | Duração (dias) | Valor Diário (BRL) |
|---------|----------------|---------------------|
| Seven   | 7              | 50,00               |
| Fifteen | 15             | 45,00               |
| Thirty  | 30             | 40,00               |
| FortyFive | 45           | 35,00               |
| Ninety  | 90             | 30,00               |

### Devolução Antecipada

- Se a motocicleta for devolvida antes da data de término prevista, será cobrado:
  - O valor integral se já tiver passado mais de 90% do período
  - 90% do valor se estiver entre 70% e 90% do período
  - 70% do valor se estiver entre 50% e 70% do período
  - 50% do valor se estiver abaixo de 50% do período

### Devolução Tardia

- Se a motocicleta for devolvida após a data de término prevista, será cobrado o valor diário normal acrescido de 20% por dia de atraso.

## Exemplos de Uso

### Autenticação e Listagem de Motocicletas com cURL

```bash
# Autenticação
curl -X POST https://api.motorcyclerental.com/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"senha123"}'

# Listagem de Motocicletas (com o token obtido)
curl -X GET https://api.motorcyclerental.com/api/v1/motorcycles \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Criação de Aluguel com JavaScript

```javascript
// Função para autenticação
async function login(email, password) {
  const response = await fetch('https://api.motorcyclerental.com/api/v1/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  
  if (!response.ok) {
    throw new Error('Falha na autenticação');
  }
  
  return await response.json();
}

// Função para criar um aluguel
async function createRental(token, rentalData) {
  const response = await fetch('https://api.motorcyclerental.com/api/v1/rentals', {
    method: 'POST',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(rentalData)
  });
  
  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(`Falha ao criar aluguel: ${JSON.stringify(errorData)}`);
  }
  
  return await response.json();
}

// Exemplo de uso
async function main() {
  try {
    // Login
    const authResult = await login('admin@example.com', 'senha123');
    const token = authResult.token;
    
    // Dados do aluguel
    const rentalData = {
      startDate: "2023-05-10",
      planType: "Seven",
      motorcycleId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      deliveryPersonId: "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    };
    
    // Criar aluguel
    const rental = await createRental(token, rentalData);
    console.log('Aluguel criado com sucesso:', rental);
  } catch (error) {
    console.error('Erro:', error.message);
  }
}

main();
```

## Limitações de Taxa

A API possui limitações de taxa para evitar abusos:

- 100 requisições por minuto por IP
- 1000 requisições por hora por usuário autenticado

Ao exceder esses limites, a API retornará o status HTTP 429 (Too Many Requests).

## Versão da API

A versão atual da API é v1. Versões futuras serão disponibilizadas em endpoints separados, como `/api/v2/*`.

## Suporte

Para suporte técnico ou dúvidas relacionadas à API, entre em contato:

- Email: api-support@motorcyclerental.com
- Telefone: (11) 1234-5678
- Horário de atendimento: Segunda a Sexta, das 9h às 18h 
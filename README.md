# Sistema de Gestão de Aluguel de Motocicletas

## Visão Geral do Sistema

O Sistema de Gestão de Aluguel de Motocicletas é uma aplicação completa para gerenciar o aluguel de motocicletas para entregadores. A aplicação foi desenvolvida seguindo os princípios da Arquitetura Limpa (Clean Architecture) e Domain-Driven Design (DDD), oferecendo uma alta manutenibilidade e escalabilidade.

### Principais Funcionalidades

- Cadastro e gestão de motocicletas
- Cadastro e gestão de entregadores
- Criação e gestão de contratos de aluguel
- Diferentes planos de aluguel (7, 15, 30, 45 e 50 dias)
- Cálculo de valores de aluguel e multas
- Autenticação e autorização de usuários

## Arquitetura do Sistema

O sistema é dividido em camadas claramente separadas, seguindo os princípios da Arquitetura Limpa:

### Camadas da Aplicação

1. **Camada de Domínio** (`Motorcycle.Domain`): Contém as entidades de negócio, value objects, enumerações, exceções e interfaces de repositório.

2. **Camada de Aplicação** (`Motorcycle.Application`): Implementa os casos de uso da aplicação, contendo serviços, DTOs, mapeamentos e validações.

3. **Camada de Infraestrutura** (`Motorcycle.Infrastructure`): Responsável por implementar os repositórios, acesso a banco de dados, serviços externos e mensageria.

4. **Camada de API** (`Motorcycle.API`): Expõe os endpoints HTTP para consumo externo, gerencia autenticação e autorização.

## Requisitos do Sistema

- .NET 8.0 SDK ou superior
- SQL Server (ou alternativa compatível com Entity Framework)
- Docker (opcional, para contêineres)

## Instalação e Configuração

### Instalação Manual

1. Clone o repositório:
   ```
   git clone https://seu-repositorio/motorcycle.git
   cd motorcycle
   ```

2. Restaure os pacotes e compile o projeto:
   ```
   dotnet restore
   dotnet build
   ```

3. Configure a conexão com o banco de dados no arquivo `appsettings.json` na pasta `Motorcycle.API`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=MotorcycleDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
   }
   ```

4. Execute as migrações para criar o banco de dados:
   ```
   cd src/Motorcycle.API
   dotnet ef database update
   ```

5. Execute a aplicação:
   ```
   dotnet run
   ```

### Utilizando Docker

1. Certifique-se de ter o Docker e Docker Compose instalados.

2. Execute o seguinte comando na raiz do projeto:
   ```
   docker-compose up -d
   ```

3. Acesse a aplicação em `http://localhost:5000/swagger/index.html` ou `https://localhost:5001/swagger/index.html`.

4. Acessar o PgAdmin em `http://localhost:5050/browser/` 
    ```
      Use as credencias : 
        (Configurar no PgAmdin com as informações do Postgres)
          usuário: MotocycleDb 
          Senha: MotocycleDb 
          Nome do servidor (Docker) : Nome do container
        (Acessar o PgAdmin)
        E-mail: motorcycle@motorcycle.com
        Senha: motorcycle@2025#
    ```

5. Acessar o MiniO em `http://localhost:9001/browser` 
    ```
      Use as credencias : 
        usuário: minio 
        Senha: minio@2025# 
        
    ```


## Documentação da API

A API RESTful expõe os seguintes recursos principais:

### Autenticação

#### Login de Usuário

```
POST /api/auth/login
```

**Corpo da Requisição:**
```json
{
  "email": "admin@example.com",
  "password": "senha123"
}
```

**Resposta de Sucesso:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2023-12-01T12:00:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Administrador",
    "email": "admin@example.com"
  }
}
```

### Motocicletas

#### Obter Todas as Motocicletas

```
GET /api/motorcycles
```

**Parâmetros de Consulta:**
- `page` (opcional): Número da página (padrão: 1)
- `pageSize` (opcional): Tamanho da página (padrão: 10)
- `search` (opcional): Termo de busca para filtrar por modelo ou placa

**Resposta de Sucesso:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "model": "Honda CG 160",
      "year": 2023,
      "licensePlate": "ABC1234",
      "isAvailable": true
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### Cadastrar Nova Motocicleta

```
POST /api/motorcycles
```

**Corpo da Requisição:**
```json
{
  "model": "Honda CG 160",
  "year": 2023,
  "licensePlate": "ABC1234"
}
```

**Resposta de Sucesso:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "model": "Honda CG 160",
  "year": 2023,
  "licensePlate": "ABC1234",
  "isAvailable": true
}
```

#### Atualizar Placa da Motocicleta

```
PATCH /api/motorcycles/{id}/license-plate
```

**Corpo da Requisição:**
```json
{
  "licensePlate": "XYZ5678"
}
```

**Resposta de Sucesso:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "model": "Honda CG 160",
  "year": 2023,
  "licensePlate": "XYZ5678",
  "isAvailable": true
}
```

### Entregadores

#### Obter Todos os Entregadores

```
GET /api/delivery-persons
```

**Resposta de Sucesso:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "João da Silva",
      "cnpj": "04.252.011/0001-10",
      "birthDate": "1990-01-01",
      "licenseNumber": "12345678901",
      "licenseType": "A",
      "hasActiveRental": false
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### Cadastrar Novo Entregador

```
POST /api/delivery-persons
```

**Corpo da Requisição:**
```json
{
  "name": "João da Silva",
  "cnpj": "04.252.011/0001-10",
  "birthDate": "1990-01-01",
  "licenseNumber": "12345678901",
  "licenseType": "A"
}
```

**Resposta de Sucesso:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João da Silva",
  "cnpj": "04.252.011/0001-10",
  "birthDate": "1990-01-01",
  "licenseNumber": "12345678901",
  "licenseType": "A",
  "hasActiveRental": false
}
```

### Aluguéis

#### Criar Novo Aluguel

```
POST /api/rentals
```

**Corpo da Requisição:**
```json
{
  "motorcycleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "deliveryPersonId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2023-12-01",
  "planType": "SevenDays"
}
```

**Resposta de Sucesso:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "motorcycleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "motorcycleModel": "Honda CG 160",
  "motorcycleLicensePlate": "ABC1234",
  "deliveryPersonId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "deliveryPersonName": "João da Silva",
  "startDate": "2023-12-01",
  "expectedEndDate": "2023-12-07",
  "actualEndDate": null,
  "planType": "SevenDays",
  "dailyRate": 30.00,
  "totalAmount": null,
  "status": "Active"
}
```

#### Registrar Devolução de Motocicleta

```
POST /api/rentals/{id}/return
```

**Corpo da Requisição:**
```json
{
  "returnDate": "2023-12-05"
}
```

**Resposta de Sucesso:**
```json
{
  "totalAmount": 150.00,
  "rentalDays": 5,
  "dailyRate": 30.00,
  "baseAmount": 150.00,
  "extraAmount": 0,
  "discount": 0,
  "penalty": 0,
  "details": "Devolução antecipada sem penalidade."
}
```

#### Calcular Valor de Aluguel (Simulação)

```
GET /api/rentals/{id}/calculate?returnDate=2023-12-05
```

**Resposta de Sucesso:**
```json
{
  "totalAmount": 150.00,
  "rentalDays": 5,
  "dailyRate": 30.00,
  "baseAmount": 150.00,
  "extraAmount": 0,
  "discount": 0,
  "penalty": 0,
  "details": "Simulação de devolução antecipada sem penalidade."
}
```

## Tabela de Valores e Planos

O sistema oferece diferentes planos de aluguel com valores variados:

| Plano | Duração | Valor Diário | Penalidade por Devolução Antecipada |
|-------|---------|--------------|-------------------------------------|
| SevenDays | 7 dias | R$ 30,00 | 20% |
| FifteenDays | 15 dias | R$ 28,00 | 40% |
| ThirtyDays | 30 dias | R$ 22,00 | 0% |
| FortyFiveDays | 45 dias | R$ 20,00 | 0% |
| FiftyDays | 50 dias | R$ 18,00 | 0% |

**Regras de Negócio:**
- Devolução antecipada: Paga-se pelos dias utilizados + penalidade sobre os dias não utilizados (quando aplicável)
- Devolução tardia: Paga-se pelos dias do plano + R$ 50,00 por dia adicional

## Executando Testes

O projeto contém testes unitários para todas as camadas da aplicação. Para executar todos os testes:

```
dotnet test
```

Para executar testes de uma camada específica:

```
dotnet test tests/Motorcycle.Domain.Tests/
dotnet test tests/Motorcycle.Application.Tests/
dotnet test tests/Motorcycle.Infrastructure.Tests/
```

Para executar testes com relatório de cobertura:

```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

## Troubleshooting

### Problemas Comuns e Soluções

1. **Erro de conexão com o banco de dados**
   - Verifique se o SQL Server está em execução
   - Verifique se a string de conexão está correta
   - Certifique-se de que o usuário tem as permissões necessárias

2. **Autenticação falha**
   - Verifique se o JWT Secret está configurado corretamente
   - Certifique-se de que as credenciais são válidas

3. **Erro ao criar aluguel**
   - Verifique se o entregador possui CNH categoria A
   - Verifique se a data de início é posterior à data atual
   - Certifique-se de que o entregador não possui aluguéis ativos
   - Verifique se a motocicleta está disponível

## Ambientes

O sistema pode ser configurado para diferentes ambientes através do uso de variáveis de ambiente ou do arquivo `appsettings.json`:

- **Development**: Ambiente de desenvolvimento com dados fictícios para teste
- **Staging**: Ambiente de homologação que simula o ambiente de produção
- **Production**: Ambiente de produção com todas as funcionalidades ativas

### Variáveis de Ambiente

As principais variáveis de ambiente que podem ser configuradas são:

- `ASPNETCORE_ENVIRONMENT`: Define o ambiente (Development, Staging, Production)
- `ConnectionStrings__DefaultConnection`: String de conexão com o banco de dados
- `JwtSettings__Secret`: Chave secreta para geração de tokens JWT
- `JwtSettings__ExpirationHours`: Tempo de expiração dos tokens em horas
- `EmailSettings__SendGridKey`: Chave da API SendGrid para envio de e-mails

## Contribuindo

Para contribuir com o projeto:

1. Faça um fork do repositório
2. Crie uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. Implemente suas alterações
4. Execute os testes para garantir que nada foi quebrado
5. Faça commit das alterações (`git commit -m 'Adiciona nova funcionalidade'`)
6. Envie para o branch (`git push origin feature/nova-funcionalidade`)
7. Abra um Pull Request

## Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE para mais detalhes.


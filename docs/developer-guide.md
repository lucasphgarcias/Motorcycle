# Guia do Desenvolvedor - Sistema de Gestão de Aluguel de Motocicletas

Este documento é destinado a desenvolvedores que irão trabalhar no projeto e fornece informações detalhadas sobre a arquitetura, padrões de código e fluxos de desenvolvimento.

## Arquitetura Detalhada

### Camada de Domínio (Motorcycle.Domain)

A camada de domínio implementa o núcleo de negócio da aplicação e segue os princípios do Domain-Driven Design:

#### Componentes Principais

- **Entidades**: Classes que representam os objetos de negócio com identidade própria.
  - `Entity`: Classe base abstrata com Id e métodos comuns.
  - `MotorcycleEntity`: Representa uma motocicleta no sistema.
  - `DeliveryPersonEntity`: Representa um entregador.
  - `RentalEntity`: Representa um contrato de aluguel.
  - `UserEntity`: Representa um usuário do sistema.

- **Value Objects**: Objetos imutáveis que representam conceitos de domínio.
  - `LicensePlate`: Representa a placa de uma motocicleta.
  - `Money`: Representa valores monetários.
  - `RentalPeriod`: Representa o período de um aluguel.
  - `Cnpj`: Representa um CNPJ válido.
  - `DriverLicense`: Representa uma carteira de habilitação.

- **Enums**: Enumerações para valores fixos.
  - `RentalPlanType`: Tipos de plano de aluguel (7, 15, 30, 45, 50 dias).
  - `LicenseType`: Tipos de CNH (A, B, AB, etc.).

- **Exceções**: Exceções específicas do domínio.
  - `DomainException`: Exceção base para erros de domínio.

- **Eventos de Domínio**: Eventos que representam mudanças de estado.
  - `IDomainEvent`: Interface para eventos de domínio.
  - `MotorcycleCreatedEvent`: Evento de criação de motocicleta.

- **Interfaces de Repositório**: Contratos para acesso a dados.
  - `IBaseRepository<T>`: Contrato base para operações CRUD.
  - `IMotorcycleRepository`: Específico para motocicletas.
  - `IDeliveryPersonRepository`: Específico para entregadores.
  - `IRentalRepository`: Específico para aluguéis.

### Camada de Aplicação (Motorcycle.Application)

A camada de aplicação coordena os casos de uso da aplicação:

#### Componentes Principais

- **Serviços**: Implementam a lógica de aplicação.
  - `MotorcycleService`: Gerencia as motocicletas.
  - `DeliveryPersonService`: Gerencia os entregadores.
  - `RentalService`: Gerencia os aluguéis.
  - `AuthService`: Gerencia a autenticação.

- **DTOs**: Objetos de transferência de dados.
  - `MotorcycleDto`: Representa uma motocicleta na API.
  - `CreateMotorcycleDto`: Para criação de motocicletas.
  - `DeliveryPersonDto`: Representa um entregador na API.
  - `RentalDto`: Representa um aluguel na API.

- **Mapeamentos**: Conversão entre entidades e DTOs.
  - `MappingProfile`: Perfil de mapeamento usando AutoMapper.

- **Validações**: Validação de entrada de dados.
  - `CreateMotorcycleDtoValidator`: Valida dados para criação de motocicleta.
  - `CreateDeliveryPersonDtoValidator`: Valida dados para criação de entregador.
  - `CreateRentalDtoValidator`: Valida dados para criação de aluguel.

- **Interfaces**: Contratos para serviços da aplicação.
  - `IMotorcycleService`: Contrato para serviço de motocicletas.
  - `IDeliveryPersonService`: Contrato para serviço de entregadores.
  - `IRentalService`: Contrato para serviço de aluguéis.
  - `IAuthService`: Contrato para serviço de autenticação.

### Camada de Infraestrutura (Motorcycle.Infrastructure)

A camada de infraestrutura fornece implementações concretas para acesso a dados e serviços externos:

#### Componentes Principais

- **Contexto de Banco de Dados**: Configuração do Entity Framework Core.
  - `MotorcycleDbContext`: Contexto principal do EF Core.

- **Configurações de Entidades**: Mapeamento entre classes e tabelas.
  - `MotorcycleConfiguration`: Configuração para a entidade Motorcycle.
  - `DeliveryPersonConfiguration`: Configuração para a entidade DeliveryPerson.
  - `RentalConfiguration`: Configuração para a entidade Rental.

- **Repositórios**: Implementações de repositórios.
  - `BaseRepository<T>`: Implementação base de repositório.
  - `MotorcycleRepository`: Implementação específica para motocicletas.
  - `DeliveryPersonRepository`: Implementação específica para entregadores.
  - `RentalRepository`: Implementação específica para aluguéis.

- **Serviços de Infraestrutura**: Implementações de serviços externos.
  - `JwtTokenService`: Gera e valida tokens JWT.
  - `PasswordHashService`: Gerencia hash de senhas.
  - `EmailService`: Envio de e-mails.

- **Migrações**: Migrações do Entity Framework Core.

- **Mensageria**: Implementação de mensageria com RabbitMQ.
  - `RabbitMqEventPublisher`: Publicador de eventos.
  - `MotorcycleCreatedConsumer`: Consumidor de eventos de criação de motocicleta.

### Camada de API (Motorcycle.API)

A camada de API expõe os endpoints HTTP para consumo externo:

#### Componentes Principais

- **Controllers**: Controladores da API.
  - `MotorcyclesController`: Endpoints para motocicletas.
  - `DeliveryPersonsController`: Endpoints para entregadores.
  - `RentalsController`: Endpoints para aluguéis.
  - `AuthController`: Endpoints para autenticação.

- **Middlewares**: Componentes para processamento de requisições.
  - `ErrorHandlingMiddleware`: Middleware para tratamento de exceções.
  - `RequestLoggingMiddleware`: Middleware para log de requisições.

- **Filtros**: Filtros para processamento de ações.
  - `ValidateModelAttribute`: Valida o modelo de entrada.
  - `JwtAuthenticationAttribute`: Valida a autenticação JWT.

## Fluxos de Desenvolvimento

### Criação de Entidade

1. Defina a entidade na camada de domínio.
2. Crie value objects relacionados.
3. Adicione a interface de repositório.
4. Implemente o repositório na camada de infraestrutura.
5. Adicione a configuração do EF Core.
6. Crie os DTOs na camada de aplicação.
7. Implemente os serviços de aplicação.
8. Adicione o controller na camada de API.
9. Implemente testes unitários para cada camada.

### Exemplo: Implementação de Nova Entidade

```csharp
// 1. Definir a entidade na camada de domínio
// Domain/Entities/VehicleInspectionEntity.cs
public class VehicleInspectionEntity : Entity
{
    public Guid MotorcycleId { get; private set; }
    public Guid InspectorId { get; private set; }
    public DateTime InspectionDate { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public bool IsApproved { get; private set; }

    // Para EF Core
    private VehicleInspectionEntity() : base() { }

    private VehicleInspectionEntity(
        Guid motorcycleId,
        Guid inspectorId,
        DateTime inspectionDate,
        string notes,
        bool isApproved)
        : base()
    {
        MotorcycleId = motorcycleId;
        InspectorId = inspectorId;
        InspectionDate = inspectionDate;
        Notes = notes;
        IsApproved = isApproved;
    }

    public static VehicleInspectionEntity Create(
        Guid motorcycleId,
        Guid inspectorId,
        DateTime inspectionDate,
        string notes,
        bool isApproved)
    {
        // Validações...
        return new VehicleInspectionEntity(motorcycleId, inspectorId, inspectionDate, notes, isApproved);
    }
}

// 2. Adicionar interface de repositório
// Domain/Interfaces/Repositories/IVehicleInspectionRepository.cs
public interface IVehicleInspectionRepository : IBaseRepository<VehicleInspectionEntity>
{
    Task<IEnumerable<VehicleInspectionEntity>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
}

// 3. Implementar o repositório
// Infrastructure/Data/Repositories/VehicleInspectionRepository.cs
public class VehicleInspectionRepository : BaseRepository<VehicleInspectionEntity>, IVehicleInspectionRepository
{
    public VehicleInspectionRepository(MotorcycleDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<VehicleInspectionEntity>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.VehicleInspections
            .Where(vi => vi.MotorcycleId == motorcycleId)
            .ToListAsync(cancellationToken);
    }
}

// 4. Adicionar configuração do EF Core
// Infrastructure/Data/Configurations/VehicleInspectionConfiguration.cs
public class VehicleInspectionConfiguration : IEntityTypeConfiguration<VehicleInspectionEntity>
{
    public void Configure(EntityTypeBuilder<VehicleInspectionEntity> builder)
    {
        builder.ToTable("VehicleInspections");
        
        builder.HasKey(vi => vi.Id);
        
        builder.Property(vi => vi.InspectionDate)
            .IsRequired();
            
        builder.Property(vi => vi.Notes)
            .HasMaxLength(500);
            
        builder.Property(vi => vi.IsApproved)
            .IsRequired();
            
        builder.HasOne<MotorcycleEntity>()
            .WithMany()
            .HasForeignKey(vi => vi.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// 5. Adicionar DTOs
// Application/DTOs/VehicleInspection/VehicleInspectionDto.cs
public class VehicleInspectionDto
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public Guid InspectorId { get; set; }
    public DateTime InspectionDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}

// 6. Adicionar serviço de aplicação
// Application/Services/VehicleInspectionService.cs
public class VehicleInspectionService : IVehicleInspectionService
{
    // Implementação...
}
```

## Convenções de Código

### Nomenclatura

- **PascalCase**: Classes, interfaces, métodos, propriedades, enums.
- **camelCase**: Variáveis locais, parâmetros.
- **_camelCase**: Campos privados.

### Padrões de Código

- **Imutabilidade**: Value objects são imutáveis.
- **Encapsulamento**: Propriedades com setters privados.
- **Factory Methods**: Use métodos estáticos `Create` para instanciar entidades.
- **Validação**: Valide entrada no domínio.
- **DI**: Use injeção de dependência.

### SOLID

- **Princípio da Responsabilidade Única**: Classes com uma única responsabilidade.
- **Princípio do Aberto/Fechado**: Extensível sem modificação.
- **Princípio da Substituição de Liskov**: Subtipos devem ser substituíveis por seus tipos base.
- **Princípio da Segregação de Interface**: Interfaces específicas são melhores que uma interface geral.
- **Princípio da Inversão de Dependência**: Dependa de abstrações, não de implementações.

## Testes

O projeto segue a prática de TDD (Test-Driven Development) e contém testes para todas as camadas:

### Estrutura de Testes

- **Testes de Domínio**: Testam entidades, value objects e lógica de negócio.
- **Testes de Aplicação**: Testam serviços de aplicação.
- **Testes de Infraestrutura**: Testam repositórios e serviços externos.
- **Testes de API**: Testam endpoints HTTP.

### Ferramentas de Teste

- **xUnit**: Framework de testes.
- **Moq**: Framework de mock.
- **FluentAssertions**: Biblioteca para asserções legíveis.
- **EntityFrameworkCore.InMemory**: Banco de dados em memória para testes.

### Exemplo de Teste

```csharp
// Teste de Value Object
public class MoneyTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateMoney()
    {
        // Arrange
        var amount = 100m;
        var currency = "BRL";

        // Act
        var money = Money.Create(amount, currency);

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }
}

// Teste de Serviço com Mock
public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _mockRepository;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _mockRepository = new Mock<IMotorcycleRepository>();
        _service = new MotorcycleService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new MotorcycleEntity();
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}

### Cobertura de Testes

O projeto atualmente tem a seguinte cobertura de testes unitários:

**Estatísticas de Cobertura (Última atualização: 05/05/2025)**

| Camada           | Cobertura de Linhas    | Cobertura de Branches   |
|------------------|-----------------------|------------------------|
| Domain           | 10.2% (55/543)        | 6.2% (11/178)          |
| Application      | 17.4% (118/675)       | 0.9% (1/110)           |
| **Total**        | **14.2% (173/1218)**  | **4.2% (12/288)**       |


#### Avaliação por Componente

- **Validadores**: Excelente cobertura para validadores como `CreateMotorcycleValidator` (100%), `UpdateMotorcycleDtoValidator` (100%) e `ReturnMotorcycleDtoValidator` (100%).
- **DTOs**: Boa cobertura para DTOs como `CreateMotorcycleDto`, `UpdateMotorcycleDto`, `ReturnMotorcycleDto` (todos com 100%).
- **Value Objects**: Cobertura melhorada para value objects do domínio:
  - `Money`: Implementados testes para criação, comparação e operações básicas.
  - `RentalPeriod`: Implementados testes para criação, datas de início/fim e cálculos de períodos.
  - `LicensePlate`: Testes completos para validação e formatação.
  - `DriverLicense`: Pendente de implementação completa de testes.
- **Serviços**: Cobertura parcial para:
  - `RentalService`: Testes para criação, recuperação e gerenciamento de aluguéis.
  - `MotorcycleService`: Testes básicos implementados.
  - `DeliveryPersonService`: Testes básicos implementados.
- **Entidades**: Implementados testes iniciais para `RentalEntity` e algumas funções-chave.

#### Metas de Cobertura

- Curto prazo: Atingir 30% de cobertura geral.
- Médio prazo: Atingir 50% nas camadas Domain e Application.
- Longo prazo: Atingir 70% de cobertura geral, incluindo testes de API.

#### Monitoramento de Cobertura

A cobertura de testes é monitorada através da ferramenta Coverlet, integrada ao processo de build. Para gerar relatórios de cobertura localmente, execute:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

Para gerar um relatório HTML detalhado:

```bash
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

O relatório gerado pode ser visualizado abrindo o arquivo `coveragereport\index.html` em qualquer navegador.

Em sistemas Windows, se o comando de múltiplas etapas falhar com o operador '&&', execute os comandos separadamente:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Para verificar rapidamente as percentagens de cobertura no relatório HTML:

```bash
findstr /s "title=" coveragereport\index.html | findstr "%"
```

#### Estratégias para Aumentar a Cobertura

1. **Priorização por Complexidade**: Focar primeiro em testar componentes com lógica de negócios complexa.
2. **Testes de Integração**: Implementar testes que verifiquem a interação entre componentes.
3. **Mocking de Dependências**: Utilizar Moq para simular dependências externas.
4. **Value Objects e Entidades**: Aumentar a cobertura de testes para todos os value objects e entidades do domínio.
5. **Serviços de Aplicação**: Implementar testes abrangentes para todos os métodos dos serviços.
6. **Controllers**: Adicionar testes para os controllers da API, verificando respostas e status codes.

#### Classes Prioritárias para Testes

- **Domain Layer**:
  - `RentalEntity`: Lógica de negócios para gestão de aluguéis.
  - `MotorcycleEntity`: Validação e comportamentos de motocicletas.
  - Value Objects pendentes: `DriverLicense`, `Cnpj`

- **Application Layer**:
  - `RentalService`: Completar testes para processamento de aluguéis e cálculos.
  - `MotorcycleService`: Expandir testes para gerenciamento de motocicletas.
  - `DeliveryPersonService`: Expandir testes para gerenciamento de entregadores.

- **API Layer**:
  - `MotorcyclesController`: Implementar testes para todas as actions.
  - `RentalsController`: Implementar testes para todas as actions.
  - `DeliveryPersonsController`: Implementar testes para todas as actions.

#### Boas Práticas de Teste

1. **Nomenclatura Descritiva**: Use nomes que descrevam claramente o comportamento esperado.
   - Exemplo: `Should_ReturnError_When_PlateNumberIsInvalid`

2. **Padrão AAA (Arrange-Act-Assert)**:
   - Arrange: Configure as pré-condições e entradas para o teste.
   - Act: Execute o código que está sendo testado.
   - Assert: Verifique se o resultado é o esperado.

3. **Isolamento**: Cada teste deve ser independente e não depender do estado de outros testes.

4. **Teste Apenas Uma Coisa**: Cada método de teste deve verificar apenas um comportamento específico.

5. **Use Mock para Dependências Externas**: Isole o código de teste de dependências externas usando mocks.

## Documentação de Código

- Use comentários XML para documentar APIs públicas.
- Siga o padrão de documentação XML da Microsoft.
- Use comentários de linha para explicar código complexo.

```csharp
/// <summary>
/// Calcula o valor total do aluguel.
/// </summary>
/// <returns>O valor total em formato Money.</returns>
/// <exception cref="DomainException">Lançada quando a data de devolução não foi informada.</exception>
public Money CalculateTotalAmount()
{
    // Implementação...
}
```

## Branches e Fluxo de Trabalho Git

- **main**: Branch principal, sempre estável.
- **develop**: Branch de desenvolvimento, integração de features.
- **feature/\***: Branches para novas funcionalidades.
- **bugfix/\***: Branches para correção de bugs.
- **release/\***: Branches para preparação de releases.

### Fluxo de Trabalho

1. Crie uma branch a partir de develop.
2. Implemente a feature ou bugfix.
3. Execute os testes.
4. Faça o push da branch e abra um Pull Request.
5. Após aprovação, faça o merge para develop.
6. Periodicamente, faça o merge de develop para main como uma release.

## Configuração do Ambiente de Desenvolvimento

### Requisitos

- **Visual Studio 2022** ou **Visual Studio Code**
- **.NET 8.0 SDK**
- **SQL Server Developer Edition** ou **LocalDB**
- **Docker Desktop** (opcional)

### Configuração Inicial

1. Clone o repositório.
2. Restaure pacotes NuGet.
3. Configure a string de conexão no arquivo `appsettings.Development.json`.
4. Execute as migrações para criar o banco de dados local.
5. Execute a aplicação.

### Extensões Recomendadas (VS Code)

- **C# Dev Kit**
- **C# Extensions**
- **SQL Server (mssql)**
- **Docker**

## Solução de Problemas

### Erros Comuns no Desenvolvimento

1. **Erro de migração do Entity Framework**:
   - Execute `dotnet ef database drop --force` e aplique as migrações novamente.

2. **Problemas de dependência circular**:
   - Revise a arquitetura e aplique o princípio de inversão de dependência.

3. **Falhas nos testes unitários**:
   - Verifique se os mocks estão configurados corretamente.
   - Certifique-se de que as asserções estão corretas.

## Recursos Adicionais

- **Documentação .NET**: https://learn.microsoft.com/dotnet/
- **Entity Framework Core**: https://learn.microsoft.com/ef/core/
- **xUnit**: https://xunit.net/docs/getting-started/netcore/cmdline
- **Domain-Driven Design**: "Domain-Driven Design: Tackling Complexity in the Heart of Software" por Eric Evans
- **Clean Architecture**: "Clean Architecture: A Craftsman's Guide to Software Structure and Design" por Robert C. Martin 
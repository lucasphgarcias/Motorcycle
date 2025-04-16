using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Services;
using Motorcycle.Infrastructure.Storage.Configuration;

namespace Motorcycle.Infrastructure.Storage.Services;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioStorageService> _logger;
    private readonly string[] _validImageTypes = { "image/png", "image/bmp" };

    public MinioStorageService(IOptions<MinioSettings> settings, ILogger<MinioStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        // Inicialização do cliente MinIO
        var minioClientBuilder = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSSL);

        _minioClient = minioClientBuilder.Build();

        // Garantir que o bucket existe
        EnsureBucketExistsAsync().GetAwaiter().GetResult();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar se o tipo de arquivo é válido
            if (!IsValidImageType(contentType))
                throw new DomainException("Tipo de arquivo não permitido. Apenas PNG e BMP são aceitos.");

            // Criar argumentos de upload
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName)
                .WithContentType(contentType)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length);

            // Enviar o arquivo para o MinIO
            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            // Construir e retornar o caminho do arquivo
            return $"{_settings.BucketName}/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do arquivo {FileName} no MinIO", fileName);
            throw new DomainException($"Erro ao fazer upload do arquivo: {ex.Message}", ex);
        }
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            // Extrair nome do objeto do caminho completo
            var fileName = ExtractFileName(filePath);

            // Criar um MemoryStream para armazenar o conteúdo do arquivo
            var memoryStream = new MemoryStream();

            // Criar argumentos para obter o objeto
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            // Obter o objeto do MinIO
            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter o arquivo {FilePath} do MinIO", filePath);
            throw new DomainException($"Erro ao obter o arquivo: {ex.Message}", ex);
        }
    }

    public async Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            // Extrair nome do objeto do caminho completo
            var fileName = ExtractFileName(filePath);

            // Criar argumentos para remover o objeto
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName);

            // Remover o objeto do MinIO
            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir o arquivo {FilePath} do MinIO", filePath);
            throw new DomainException($"Erro ao excluir o arquivo: {ex.Message}", ex);
        }
    }

    public bool IsValidImageType(string contentType)
    {
        return _validImageTypes.Contains(contentType.ToLower());
    }

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            // Verificar se o bucket já existe
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_settings.BucketName);

            bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (!found)
            {
                // Se não existir, criar o bucket
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_settings.BucketName);

                await _minioClient.MakeBucketAsync(makeBucketArgs);
                
                _logger.LogInformation("Bucket {BucketName} criado com sucesso", _settings.BucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar/criar o bucket {BucketName} no MinIO", _settings.BucketName);
            throw new DomainException($"Erro ao inicializar o serviço de armazenamento: {ex.Message}", ex);
        }
    }

    private string ExtractFileName(string filePath)
    {
        // Se o caminho contiver o nome do bucket, remover essa parte
        if (filePath.StartsWith($"{_settings.BucketName}/"))
            return filePath.Substring(_settings.BucketName.Length + 1);
        
        return filePath;
    }
}
using Xunit;
using Moq;
using FluentAssertions;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.Handlers;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Services;

namespace BancoDigitalAna.ContaCorrente.Tests.Handlers;

public class CadastrarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICryptographyService> _mockCryptographyService;
    private readonly CadastrarContaHandler _handler;

    public CadastrarContaHandlerTests()
    {
        _mockRepository = new Mock<IContaCorrenteRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCryptographyService = new Mock<ICryptographyService>();
        
        _handler = new CadastrarContaHandler(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockCryptographyService.Object);
    }

    [Fact]
    public async Task Handle_ComCpfValido_DeveCriarContaComSucesso()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "12345678909", // CPF válido
            Nome = "João Silva",
            Senha = "SenhaForte123!"
        };

        _mockCryptographyService
            .Setup(c => c.Encrypt(It.IsAny<string>()))
            .Returns("cpf_criptografado");

        _mockCryptographyService
            .Setup(c => c.HashPassword(It.IsAny<string>()))
            .Returns("senha_hash");

        _mockRepository
            .Setup(r => r.ObterProximoNumeroContaAsync())
            .ReturnsAsync(100);

        _mockRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<BankMore.ContaCorrente.Domain.Entities.ContaCorrente>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NumeroContaCorrente.Should().Be(100);
        resultado.Nome.Should().Be("João Silva");
        resultado.Ativo.Should().BeTrue();
        resultado.Saldo.Should().Be(0);

        _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<BankMore.ContaCorrente.Domain.Entities.ContaCorrente>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComCpfInvalido_DeveLancarExcecao()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "12345678901", // CPF inválido
            Nome = "João Silva",
            Senha = "SenhaForte123!"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*INVALID_DOCUMENT*");
    }

    [Fact]
    public async Task Handle_ComCpfVazio_DeveLancarExcecao()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "",
            Nome = "João Silva",
            Senha = "SenhaForte123!"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [InlineData("52998224725")]  // CPF válido 1
    [InlineData("11144477735")]  // CPF válido 2
    [InlineData("12345678909")]  // CPF válido 3
    public async Task Handle_ComDiferentesCpfsValidos_DeveCriarConta(string cpfValido)
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = cpfValido,
            Nome = "Teste Usuario",
            Senha = "Senha123!"
        };

        _mockCryptographyService
            .Setup(c => c.Encrypt(It.IsAny<string>()))
            .Returns("cpf_criptografado");

        _mockCryptographyService
            .Setup(c => c.HashPassword(It.IsAny<string>()))
            .Returns("senha_hash");

        _mockRepository.Setup(r => r.ObterProximoNumeroContaAsync()).ReturnsAsync(1);
        _mockRepository.Setup(r => r.AdicionarAsync(It.IsAny<BankMore.ContaCorrente.Domain.Entities.ContaCorrente>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NumeroContaCorrente.Should().BeGreaterThan(0);
    }
}

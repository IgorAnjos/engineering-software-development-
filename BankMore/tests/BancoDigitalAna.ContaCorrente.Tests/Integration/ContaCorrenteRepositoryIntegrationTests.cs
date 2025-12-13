using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using BankMore.ContaCorrente.Domain.Entities;

namespace BancoDigitalAna.ContaCorrente.Tests.Integration;

public class ContaCorrenteRepositoryIntegrationTests : IDisposable
{
    private readonly ContaCorrenteDbContext _context;
    private readonly ContaCorrenteRepository _repository;

    public ContaCorrenteRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ContaCorrenteDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new ContaCorrenteDbContext(options);
        _repository = new ContaCorrenteRepository(_context);
    }

    [Fact]
    public async Task AdicionarAsync_DeveSalvarContaComSucesso()
    {
        // Arrange
        var conta = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente("12345678909", "João Silva", "senha123", "salt123");

        // Act
        await _repository.AdicionarAsync(conta);
        var resultado = await _repository.ObterPorIdAsync(conta.IdContaCorrente);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("João Silva");
        resultado.Cpf.Should().Be("12345678909");
        resultado.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task ObterPorNumeroAsync_DeveEncontrarContaPorNumero()
    {
        // Arrange
        var conta = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente("98765432100", "Maria Santos", "senha456", "salt456");
        await _repository.AdicionarAsync(conta);

        // Act
        var resultado = await _repository.ObterPorNumeroAsync(conta.Numero);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Numero.Should().Be(conta.Numero);
        resultado.Nome.Should().Be("Maria Santos");
    }

    [Fact]
    public async Task ObterPorCpfAsync_DeveEncontrarContaPorCpf()
    {
        // Arrange
        var cpf = "11144477735";
        var conta = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente(cpf, "Pedro Alves", "senha789", "salt789");
        await _repository.AdicionarAsync(conta);

        // Act
        var resultado = await _repository.ObterPorCpfAsync(cpf);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Cpf.Should().Be(cpf);
        resultado.Nome.Should().Be("Pedro Alves");
    }

    [Fact]
    public async Task AtualizarAsync_DeveModificarDadosDaConta()
    {
        // Arrange
        var conta = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente("52998224725", "Ana Costa", "senha000", "salt000");
        await _repository.AdicionarAsync(conta);

        // Act
        conta.Inativar();
        await _repository.AtualizarAsync(conta);
        var contaAtualizada = await _repository.ObterPorIdAsync(conta.IdContaCorrente);

        // Assert
        contaAtualizada.Should().NotBeNull();
        contaAtualizada!.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task ObterProximoNumeroContaAsync_DeveRetornarNumeroSequencial()
    {
        // Arrange
        var conta1 = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente("12345678909", "Usuario 1", "senha1", "salt1");
        var conta2 = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente("98765432100", "Usuario 2", "senha2", "salt2");
        
        await _repository.AdicionarAsync(conta1);
        await _repository.AdicionarAsync(conta2);

        // Act
        var proximoNumero = await _repository.ObterProximoNumeroContaAsync();

        // Assert
        proximoNumero.Should().BeGreaterThan(conta2.Numero);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Act
        var resultado = await _repository.ObterPorIdAsync(Guid.NewGuid().ToString());

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorNumeroAsync_ComNumeroInexistente_DeveRetornarNull()
    {
        // Act
        var resultado = await _repository.ObterPorNumeroAsync(99999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task AdicionarAsync_ComMultiplasContas_DeveFuncionar()
    {
        // Arrange & Act
        for (int i = 0; i < 10; i++)
        {
            var conta = new BankMore.ContaCorrente.Domain.Entities.ContaCorrente(
                $"1234567890{i}", 
                $"Usuario {i}", 
                $"senha{i}", 
                $"salt{i}"
            );
            await _repository.AdicionarAsync(conta);
        }

        // Assert
        var todasContas = await _context.ContasCorrentes.ToListAsync();
        todasContas.Should().HaveCount(10);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

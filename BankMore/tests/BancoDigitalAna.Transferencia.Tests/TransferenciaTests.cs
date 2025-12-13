using BankMore.Transferencia.Domain.Entities;

namespace BancoDigitalAna.Transferencia.Tests;

/// <summary>
/// Testes unitários para a entidade Transferencia
/// </summary>
public class TransferenciaTests
{
    [Fact]
    public void DeveCriarTransferenciaComDadosValidos()
    {
        // Arrange
        var idContaOrigem = "12345";
        var idContaDestino = "67890";
        var valor = 100.50m;

        // Act
        var transferencia = new BankMore.Transferencia.Domain.Entities.Transferencia(
            idContaOrigem,
            idContaDestino,
            valor
        );

        // Assert
        Assert.NotNull(transferencia);
        Assert.NotEmpty(transferencia.Id);
        Assert.Equal(idContaOrigem, transferencia.IdContaCorrenteOrigem);
        Assert.Equal(idContaDestino, transferencia.IdContaCorrenteDestino);
        Assert.Equal(100.50m, transferencia.Valor);
        Assert.NotEmpty(transferencia.DataMovimento);
    }

    [Fact]
    public void DeveArredondarValorParaDuasCasasDecimais()
    {
        // Arrange
        var idContaOrigem = "12345";
        var idContaDestino = "67890";
        var valor = 100.999m;

        // Act
        var transferencia = new BankMore.Transferencia.Domain.Entities.Transferencia(
            idContaOrigem,
            idContaDestino,
            valor
        );

        // Assert
        Assert.Equal(101.00m, transferencia.Valor);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoContaOrigemForNula()
    {
        // Arrange
        string? idContaOrigem = null;
        var idContaDestino = "67890";
        var valor = 100.50m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BankMore.Transferencia.Domain.Entities.Transferencia(
                idContaOrigem!,
                idContaDestino,
                valor
            )
        );
    }

    [Fact]
    public void DeveLancarExcecaoQuandoContaDestinoForNula()
    {
        // Arrange
        var idContaOrigem = "12345";
        string? idContaDestino = null;
        var valor = 100.50m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BankMore.Transferencia.Domain.Entities.Transferencia(
                idContaOrigem,
                idContaDestino!,
                valor
            )
        );
    }

    [Fact]
    public void DeveGerarIdUnicoParaCadaTransferencia()
    {
        // Arrange
        var idContaOrigem = "12345";
        var idContaDestino = "67890";
        var valor = 100.50m;

        // Act
        var transferencia1 = new BankMore.Transferencia.Domain.Entities.Transferencia(
            idContaOrigem,
            idContaDestino,
            valor
        );
        var transferencia2 = new BankMore.Transferencia.Domain.Entities.Transferencia(
            idContaOrigem,
            idContaDestino,
            valor
        );

        // Assert
        Assert.NotEqual(transferencia1.Id, transferencia2.Id);
    }
}

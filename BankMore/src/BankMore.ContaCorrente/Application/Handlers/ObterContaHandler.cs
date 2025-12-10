using MediatR;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Infrastructure.Services;

namespace BankMore.ContaCorrente.Application.Handlers;

public class ObterContaHandler : IRequestHandler<ObterContaQuery, ContaDto?>
{
    private readonly ContaCorrenteDbContext _context;
    private readonly ICryptographyService _cryptographyService;

    public ObterContaHandler(ContaCorrenteDbContext context, ICryptographyService cryptographyService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cryptographyService = cryptographyService ?? throw new ArgumentNullException(nameof(cryptographyService));
    }

    public async Task<ContaDto?> Handle(ObterContaQuery request, CancellationToken cancellationToken)
    {
        var conta = await _context.ContasCorrentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdContaCorrente == request.IdContaCorrente, cancellationToken);

        if (conta == null)
            return null;

        // Calcular saldo
        var saldo = await _context.Movimentos
            .Where(m => m.IdContaCorrente == request.IdContaCorrente)
            .SumAsync(m => m.TipoMovimento == 'C' ? m.Valor : -m.Valor, cancellationToken);

        // Descriptografar CPF antes de retornar
        var cpfDescriptografado = _cryptographyService.Decrypt(conta.Cpf);

        return new ContaDto
        {
            Id = conta.IdContaCorrente,
            NumeroContaCorrente = conta.Numero,
            Cpf = cpfDescriptografado,
            Nome = conta.Nome,
            Ativo = conta.Ativo,
            Saldo = saldo,
            DataCriacao = DateTime.UtcNow
        };
    }
}

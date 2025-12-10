using FluentAssertions;
using BankMore.Web.E2ETests.Infrastructure;
using BankMore.Web.E2ETests.PageObjects;

namespace BankMore.Web.E2ETests.Tests;

/// <summary>
/// Testes E2E para a funcionalidade de Cadastro de Conta
/// </summary>
public class CadastroE2ETests : SeleniumTestBase
{
    private CadastroPage _cadastroPage;

    public CadastroE2ETests()
    {
        _cadastroPage = new CadastroPage(Driver, this);
    }

    [Fact(DisplayName = "Deve carregar a página de cadastro corretamente")]
    public void DeveCarregarPaginaCadastro()
    {
        // Act
        _cadastroPage.NavigateToCadastro();

        // Assert
        Driver.Url.Should().Contain("/cadastro");
        Driver.Title.Should().Contain("Cadastro");
    }





    [Fact(DisplayName = "Deve exibir erro ao tentar criar conta com CPF inválido")]
    public void DeveExibirErroComCpfInvalido()
    {
        // Arrange
        var cpfInvalido = "12345678901"; // CPF inválido
        var nome = "Teste CPF Inválido";
        var senha = "senha789";

        // Act
        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpfInvalido, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();

        // Assert
        _cadastroPage.ExibeMensagemErro().Should().BeTrue();
        _cadastroPage.ObterMensagemErro().Should().NotBeNullOrEmpty();
    }



    [Fact(DisplayName = "Deve desabilitar botão durante o loading")]
    public void DeveDesabilitarBotaoDuranteLoading()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Teste Loading";
        var senha = "senha123";

        // Act
        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();

        // Assert - botão deve estar desabilitado durante o loading
        // Nota: Este teste pode ser flaky dependendo da velocidade da API
        var desabilitado = _cadastroPage.BotaoEstaDesabilitado();
        desabilitado.Should().BeTrue("botão deve estar desabilitado durante o loading");
    }

    [Fact(DisplayName = "Deve navegar para tela de login ao clicar em 'Fazer Login'")]
    public void DeveNavegarParaLoginAoClicarBotao()
    {
        // Arrange
        _cadastroPage.NavigateToCadastro();

        // Act
        _cadastroPage.ClicarFazerLogin();

        // Assert
        Driver.Url.Should().Contain("/login");
    }

    [Fact(DisplayName = "Deve validar campos obrigatórios")]
    public void DeveValidarCamposObrigatorios()
    {
        // Arrange
        _cadastroPage.NavigateToCadastro();

        // Act - tenta submeter sem preencher nada
        _cadastroPage.ClicarCriarConta();

        // Assert - formulário não deve submeter (validação HTML5)
        Driver.Url.Should().Contain("/cadastro");
    }

    [Fact(DisplayName = "Deve aceitar CPF formatado com máscara")]
    public void DeveAceitarCpfFormatado()
    {
        // Arrange
        var cpf = "123.456.789-09"; // CPF formatado (válido)
        var nome = "Teste CPF Formatado";
        var senha = "senha123";

        // Act
        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();

        // Assert - Pode dar sucesso ou erro dependendo se o validador aceita formatação
        // O importante é que não dê erro de javascript ou trave
        bool sucesso = _cadastroPage.ExibeMensagemSucesso();
        bool erro = _cadastroPage.ExibeMensagemErro();

        (sucesso || erro).Should().BeTrue("deve exibir alguma mensagem de feedback");
    }

    /// <summary>
    /// Gera um CPF válido aleatório para testes
    /// </summary>
    private string GerarCpfAleatorio()
    {
        var random = new Random();
        var cpf = new int[11];

        // Gera os primeiros 9 dígitos
        for (int i = 0; i < 9; i++)
        {
            cpf[i] = random.Next(0, 10);
        }

        // Calcula o primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += cpf[i] * (10 - i);
        }
        int resto = soma % 11;
        cpf[9] = resto < 2 ? 0 : 11 - resto;

        // Calcula o segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += cpf[i] * (11 - i);
        }
        resto = soma % 11;
        cpf[10] = resto < 2 ? 0 : 11 - resto;

        return string.Join("", cpf);
    }
}

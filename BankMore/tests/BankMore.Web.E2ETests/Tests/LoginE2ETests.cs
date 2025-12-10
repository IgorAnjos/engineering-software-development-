using FluentAssertions;
using BankMore.Web.E2ETests.Infrastructure;
using BankMore.Web.E2ETests.PageObjects;

namespace BankMore.Web.E2ETests.Tests;

/// <summary>
/// Testes E2E para a funcionalidade de Login
/// </summary>
public class LoginE2ETests : SeleniumTestBase
{
    private LoginPage _loginPage;
    private CadastroPage _cadastroPage;

    public LoginE2ETests()
    {
        _loginPage = new LoginPage(Driver, this);
        _cadastroPage = new CadastroPage(Driver, this);
    }

    [Fact(DisplayName = "Deve carregar a página de login corretamente")]
    public void DeveCarregarPaginaLogin()
    {
        // Act
        _loginPage.NavigateToLogin();

        // Assert
        Driver.Url.Should().Contain("/login");
        Driver.Title.Should().Contain("Login");
    }

    

   

    [Fact(DisplayName = "Deve exibir erro com credenciais inválidas")]
    public void DeveExibirErroComCredenciaisInvalidas()
    {
        // Arrange
        var contaInvalida = "99999999";
        var senhaInvalida = "senhaerrada";

        // Act
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(contaInvalida, senhaInvalida);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();

        // Assert
        _loginPage.ExibeMensagemErro().Should().BeTrue();
        _loginPage.ObterMensagemErro().Should().Contain("Credenciais inválidas");
        Driver.Url.Should().Contain("/login"); // Não deve redirecionar
    }

   

    [Fact(DisplayName = "Deve desabilitar botão durante o loading")]
    public void DeveDesabilitarBotaoDuranteLoading()
    {
        // Arrange
        var conta = "12345";
        var senha = "senha123";

        // Act
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(conta, senha);
        _loginPage.ClicarEntrar();

        // Assert
        var desabilitado = _loginPage.BotaoEstaDesabilitado();
        desabilitado.Should().BeTrue("botão deve estar desabilitado durante o loading");
    }

    [Fact(DisplayName = "Deve navegar para tela de cadastro ao clicar em 'Criar nova conta'")]
    public void DeveNavegarParaCadastroAoClicarBotao()
    {
        // Arrange
        _loginPage.NavigateToLogin();

        // Act
        _loginPage.ClicarCriarConta();

        // Assert
        Driver.Url.Should().Contain("/cadastro");
    }

    [Fact(DisplayName = "Deve validar campos obrigatórios")]
    public void DeveValidarCamposObrigatorios()
    {
        // Arrange
        _loginPage.NavigateToLogin();

        // Act - tenta submeter sem preencher nada
        _loginPage.ClicarEntrar();

        // Assert - formulário não deve submeter (validação HTML5)
        Driver.Url.Should().Contain("/login");
    }
     
}

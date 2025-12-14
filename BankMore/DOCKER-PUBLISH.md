# 🐳 Publicação de Imagens Docker - BankMore

## 📋 Visão Geral

O pipeline CI/CD do BankMore publica automaticamente as imagens Docker em **dois registries**:
- **GitHub Container Registry (GHCR)** - Público/Privado
- **Docker Hub** - Público

## 🔧 Configuração Inicial

### Passo 1: Criar Token do Docker Hub

1. Acesse [Docker Hub](https://hub.docker.com/)
2. Vá em **Account Settings** → **Security** → **New Access Token**
3. Nome: `GitHub Actions BankMore`
4. Permissões: `Read, Write, Delete`
5. Copie o token gerado

### Passo 2: Adicionar Secrets no GitHub

1. Vá no repositório: https://github.com/seu-usuario/engineering-software-development-
2. **Settings** → **Secrets and variables** → **Actions**
3. Clique em **New repository secret**
4. Adicione os seguintes secrets:

| Nome | Valor |
|------|-------|
| `DOCKERHUB_USERNAME` | Seu usuário do Docker Hub (ex: `igoranjos`) |
| `DOCKERHUB_TOKEN` | O token gerado no Passo 1 |

## 🚀 Como Funciona

### Publicação Automática

Quando você faz **push** para as branches `main` ou `develop`:

```bash
git add .
git commit -m "feat: nova funcionalidade"
git push origin main
```

O pipeline **automaticamente**:
1. ✅ Roda os testes
2. ✅ Faz build das imagens Docker
3. ✅ Publica no **GHCR** (`ghcr.io`)
4. ✅ Publica no **Docker Hub** (`docker.io`)

### Imagens Publicadas

#### GitHub Container Registry
```
ghcr.io/seu-usuario/bankmore-api-conta:latest
ghcr.io/seu-usuario/bankmore-api-conta:main
ghcr.io/seu-usuario/bankmore-api-transferencia:latest
ghcr.io/seu-usuario/bankmore-api-transferencia:main
ghcr.io/seu-usuario/bankmore-worker-tarifas:latest
ghcr.io/seu-usuario/bankmore-worker-tarifas:main
ghcr.io/seu-usuario/bankmore-web:latest
ghcr.io/seu-usuario/bankmore-web:main
```

#### Docker Hub
```
seu-usuario/bankmore-api-conta:latest
seu-usuario/bankmore-api-conta:main
seu-usuario/bankmore-api-transferencia:latest
seu-usuario/bankmore-api-transferencia:main
seu-usuario/bankmore-worker-tarifas:latest
seu-usuario/bankmore-worker-tarifas:main
seu-usuario/bankmore-web:latest
seu-usuario/bankmore-web:main
```

## 📦 Usar as Imagens Publicadas

### Atualizar docker-compose.yml

Depois que as imagens forem publicadas, você pode usar:

```yaml
services:
  api-conta:
    image: seu-usuario/bankmore-api-conta:latest
    # Remova a seção 'build'
    ports:
      - "5003:5003"
    environment:
      # ... suas variáveis de ambiente

  api-transferencia:
    image: seu-usuario/bankmore-api-transferencia:latest
    ports:
      - "5004:5004"
    environment:
      # ... suas variáveis de ambiente

  worker-tarifas:
    image: seu-usuario/bankmore-worker-tarifas:latest
    environment:
      # ... suas variáveis de ambiente

  web:
    image: seu-usuario/bankmore-web:latest
    ports:
      - "5000:80"
```

### Baixar e Executar

Qualquer pessoa pode baixar e executar:

```bash
# Baixar as imagens
docker pull seu-usuario/bankmore-api-conta:latest
docker pull seu-usuario/bankmore-api-transferencia:latest
docker pull seu-usuario/bankmore-worker-tarifas:latest
docker pull seu-usuario/bankmore-web:latest

# Executar com docker-compose
docker-compose up -d
```

## 🏷️ Tags Disponíveis

O pipeline cria automaticamente várias tags:

| Tag | Descrição | Exemplo |
|-----|-----------|---------|
| `latest` | Última versão da branch main | `bankmore-api-conta:latest` |
| `main` | Branch main | `bankmore-api-conta:main` |
| `develop` | Branch develop | `bankmore-api-conta:develop` |
| `main-abc123` | Commit SHA da main | `bankmore-api-conta:main-abc123` |

## 🔍 Verificar Publicação

### GitHub Container Registry
1. Vá em **Packages** no seu perfil do GitHub
2. Você verá todos os pacotes publicados

### Docker Hub
1. Acesse [Docker Hub](https://hub.docker.com/)
2. Vá em **Repositories**
3. Veja suas imagens publicadas

## 📊 Status do Build

Você pode acompanhar o build em:
- **Actions** → **CI/CD Pipeline - BankMore**

## 🔒 Segurança

- ✅ Tokens são armazenados como **secrets** (nunca expostos nos logs)
- ✅ Apenas pushs autorizados podem publicar
- ✅ Imagens são assinadas e verificáveis
- ✅ GHCR usa autenticação GitHub automática

## 🆘 Troubleshooting

### Erro: "secrets.DOCKERHUB_USERNAME not found"
- Verifique se os secrets foram adicionados corretamente no GitHub
- Nome dos secrets deve ser exatamente: `DOCKERHUB_USERNAME` e `DOCKERHUB_TOKEN`

### Erro: "unauthorized: authentication required"
- Token do Docker Hub pode estar expirado
- Recrie o token e atualize o secret

### Erro: "denied: installation not allowed"
- Verifique permissões do token no Docker Hub
- Token deve ter permissão de `Write`

## 🎯 Próximos Passos

1. ✅ Configure os secrets no GitHub
2. ✅ Faça um push para `main`
3. ✅ Aguarde o pipeline executar (~5-10 min)
4. ✅ Verifique as imagens no Docker Hub e GHCR
5. ✅ Use as imagens publicadas em produção

## 📝 Referências

- [Docker Hub](https://hub.docker.com/)
- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
- [GitHub Actions - Docker](https://docs.github.com/en/actions/publishing-packages/publishing-docker-images)

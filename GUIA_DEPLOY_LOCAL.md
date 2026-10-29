# Guia de deploy local — Nimbus Bank

Como colocar a aplicação para rodar na sua máquina, do zero. Tempo: ~10 minutos.

> **Importante:** este projeto é um ambiente de treino com falhas intencionais. Rode
> **só localmente**, isolado. Nunca exponha a porta na rede nem suba para um servidor público.

---

## 1. Pré-requisitos

- **.NET SDK 8.0 ou superior** (serve 8, 9 ou 10).
- Um terminal (PowerShell, Prompt, bash, zsh).
- Opcional: Visual Studio 2022, VS Code (C# Dev Kit) ou Rider.

### Instalar o .NET SDK
- **Windows:** `winget install Microsoft.DotNet.SDK.8` — ou o instalador em https://dotnet.microsoft.com/download
- **macOS:** `brew install --cask dotnet-sdk` — ou o instalador do site
- **Linux (Debian/Ubuntu):** `sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0`

Confirme:
```bash
dotnet --version
```

---

## 2. Obter o código

```bash
git clone https://github.com/GabrielNM12/vulnbank.git
cd vulnbank
```
(ou descompacte o .zip recebido e entre na pasta)

---

## 3. Restaurar e rodar

```bash
dotnet restore
dotnet run --project VulnBank
```

Na primeira execução o .NET baixa o pacote do SQLite, compila e cria o banco
`nimbus.db` já populado. Quando aparecer:

```
Now listening on: http://localhost:5080
Application started. Press Ctrl+C to shut down.
```

abra **http://localhost:5080**. Para parar: `Ctrl+C`.

As credenciais de acesso estão em `ACESSOS.md`.

---

## 4. Reiniciar os dados

O banco é um arquivo único. Para voltar ao estado inicial:

```bash
# pare o app (Ctrl+C)
rm VulnBank/nimbus.db        # Windows (PowerShell): Remove-Item VulnBank/nimbus.db
dotnet run --project VulnBank
```

O `nimbus.db` está no `.gitignore` e não vai para o repositório.

---

## 5. Problemas comuns

| Sintoma | Causa | Solução |
|---|---|---|
| `dotnet: command not found` | SDK fora do PATH | Reinstale o SDK e abra um terminal novo |
| `Address already in use` (5080) | Porta ocupada | Em `VulnBank/Properties/launchSettings.json` troque `5080` por `5090` |
| Abriu em `localhost:5000` | Rodou sem o perfil | Use `dotnet run --project VulnBank` |
| Erro ao transferir | Valor com vírgula | Use ponto (`500.00`) |
| Build estranho | Cache | `dotnet clean` e rode de novo |

---

## 6. Abrir no editor (opcional)

- **Visual Studio:** abra `VulnBank.sln` e aperte F5.
- **VS Code:** `code .` na pasta, instale o C# Dev Kit, Run → Start Debugging.
- **Rider:** abra `VulnBank.sln`.

---

*Nimbus Bank — ambiente de treino controlado, para rodar isolado. Uso educacional.*

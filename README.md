# Nimbus Bank

Aplicação web de uma fintech fictícia — login, conta, transferências, extrato e comprovantes — construída em **ASP.NET Core (C#) + SQLite**.

Este projeto é um **ambiente de treino de segurança ofensiva**. Ele contém **vulnerabilidades plantadas de propósito**, para serem encontradas, exploradas e corrigidas como exercício. As falhas específicas não são listadas aqui: descobri-las faz parte do exercício.

> ### ⚠️ Uso responsável
> Por conter falhas intencionais, rode esta aplicação **apenas localmente, em ambiente isolado**. **Não** publique em servidor público, VPS ou qualquer host acessível pela internet. Nenhum dado real deve ser usado. É uma instituição fictícia e não representa nenhuma empresa real.

---

## Como rodar

Pré-requisito: .NET SDK 8.0 ou superior. Passo a passo completo em [`GUIA_DEPLOY_LOCAL.md`](GUIA_DEPLOY_LOCAL.md).

```bash
dotnet run --project VulnBank
```

Depois abra **http://localhost:5080**.

As credenciais de acesso estão em [`ACESSOS.md`](ACESSOS.md).

Para reiniciar os dados, pare a aplicação, apague o arquivo `nimbus.db` e rode novamente.

---

## Estrutura

```
vulnbank/
├── VulnBank.sln
├── VulnBank/
│   ├── Program.cs
│   ├── Data/            # acesso ao banco (SQLite) e carga inicial
│   ├── Controllers/     # Home, Account, Admin, Api
│   ├── Models/          # User, Transaction
│   ├── Views/           # telas (Razor)
│   └── wwwroot/css/
├── README.md
├── ACESSOS.md           # credenciais de acesso
└── GUIA_DEPLOY_LOCAL.md # instalação e execução
```

---

*Projeto educacional de segurança de aplicações. Ambiente de treino controlado, para rodar isolado.*

# Acessos — Nimbus Bank (ambiente de treino)

Aplicação rodando em **http://localhost:5080**.

Use estas contas para explorar a aplicação:

| Usuário | Senha | Nome | Conta | Saldo inicial |
|---|---|---|---|---|
| `wiener` | `peter` | Peter Wiener | 40010001 | R$ 1.500,00 |
| `carlos` | `montoya` | Carlos Montoya | 40010002 | R$ 8.200,50 |

> Há também uma conta de suporte/administração no sistema. Faz parte do exercício
> descobrir como chegar até ela e o que é possível fazer a partir dela.

---

### Dicas de uso
- Para transferir, informe o **número da conta de destino** (ex.: `40010002`), o valor e uma descrição.
- Para reiniciar tudo ao estado inicial: pare a aplicação, apague `nimbus.db` e rode de novo.
- Valores usam **ponto** como separador decimal (ex.: `500.00`).

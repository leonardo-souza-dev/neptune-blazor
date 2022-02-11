﻿using Neptune.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neptune.Application
{
    public interface ITransacaoService
    {
        Task<List<Transacao>> ObterTodas();
        Task<Meses> ObterMeses_old(List<Conta> contas);
        Task<Meses2> ObterMeses(List<Conta> contas);
        Task<Transacao> AdicionarTransacao(Transacao transacao);
    }
}

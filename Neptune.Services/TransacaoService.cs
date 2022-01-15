﻿using Neptune.Domain;
using Neptune.Infra;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util;

namespace Neptune.Application
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IContaRepository _contaRepository;

        public TransacaoService(ITransacaoRepository transacaoRepository, IContaRepository contaRepository)
        {
            _transacaoRepository = transacaoRepository;
            _contaRepository = contaRepository;
        }

        public async Task<List<Transacao>> ObterTodas()
        {
            return await _transacaoRepository.ObterTodas();
        }

        public async Task<MesOld> ObterMes(DataMes mesTransacao, List<Conta> contas)
        {
            var transacoes = await _transacaoRepository.ObterMesContas(mesTransacao.Ano, mesTransacao.Mes, contas.Select(x => x.Id).ToList());

            var saldoUltimoDiaMesAnterior = await ObterSaldoUltimoDiaMesAnterior(mesTransacao, contas);

            var mes = new MesOld(mesTransacao, saldoUltimoDiaMesAnterior, transacoes);

            return mes;
        }

        public Transacao Criar(Transacao transacao)
        {
            return _transacaoRepository.Criar(transacao);
        }

        public Transacao Atualizar(Transacao transacao)
        {
            return _transacaoRepository.Atualizar(transacao);
        }

        public async Task<Meses> ObterMeses()
        {
            var todasTransacoes = await ObterTodas();
            var todasContas = await _contaRepository.ObterTodas();

            var meses = new Meses(todasTransacoes, todasContas);

            return meses;
        }

        private async Task<SaldoUltimoDiaMesAnterior> ObterSaldoUltimoDiaMesAnterior(DataMes mesTransacao, List<Conta> contas)
        {
            var primeiroDiaDoMes = new DateTime(mesTransacao.Ano, mesTransacao.Mes, 1);

            var todasTransacoes = await _transacaoRepository.ObterTodas();
            var todasTransacoesContas = todasTransacoes.Where(x => contas.Select(c => c.Id).Contains(x.ContaId)).ToList();
            var transacoesMesPassadoPraTras = todasTransacoesContas.Where(x => x.Data < primeiroDiaDoMes);


            var saldoUltimoDiaMesAnteriorContas = new List<SaldoUltimoDiaMesAnteriorConta>();

            contas.ForEach(conta =>
            {
                var somaMes = transacoesMesPassadoPraTras.Where(t => t.ContaId == conta.Id).Sum(x => x.Valor);
                var valor = conta.SaldoInicial - somaMes;

                saldoUltimoDiaMesAnteriorContas.Add(new SaldoUltimoDiaMesAnteriorConta(conta.Id, valor));
            });

            var saldoUltimoDiaMesAnterior = new SaldoUltimoDiaMesAnterior(saldoUltimoDiaMesAnteriorContas);

            return saldoUltimoDiaMesAnterior;
        }
    }
}
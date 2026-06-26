using System;
using System.Collections.Generic;
using System.Linq;
using Autoglass.PlataformaHUB.CrossCutting.Enums;
using Autoglass.PlataformaHUB.CrossCutting.Extensions;

namespace Autoglass.PlataformaHUB.Domain.Configuracoes
{
    /// <summary>
    /// Catálogo central com a definição de apuração de todos os serviços do relatório.
    /// Para adicionar/ajustar serviços (ex.: novos serviços de infraestrutura), basta
    /// alterar a lista <see cref="Todos"/> — a estrutura do relatório se ajusta automaticamente.
    /// </summary>
    public static class CatalogoServicos
    {
        /// <summary>Chave utilizada para apurar acessos de cada serviço.</summary>
        public const ChaveMetricaEnum ChaveAcessos = ChaveMetricaEnum.AcessosQuantidade;

        /// <summary>Chave utilizada para apurar a quantidade de aplicações de cada serviço.</summary>
        public const ChaveMetricaEnum ChaveAplicacoes = ChaveMetricaEnum.AplicacoesQuantidade;

        private static readonly IReadOnlyList<DefinicaoServico> _todos = ConstruirCatalogo()
            .OrderBy(d => d.Nome)
            .ToList();

        /// <summary>Todos os serviços do relatório, ordenados pelo nome.</summary>
        public static IReadOnlyList<DefinicaoServico> Todos => _todos;

        /// <summary>Serviços cujo total principal representa provisionamentos.</summary>
        public static IReadOnlyList<DefinicaoServico> ServicosProvisionamento =>
            _todos.Where(d => d.ClassificadoComoProvisionamento).ToList();

        /// <summary>Serviços de infraestrutura (compõem o KPI de provisionamentos de infraestrutura).</summary>
        public static IReadOnlyList<DefinicaoServico> ServicosInfraestrutura =>
            _todos.Where(d => d.ClassificadoComoInfraestrutura).ToList();

        private static IEnumerable<DefinicaoServico> ConstruirCatalogo()
        {
            // --- Integração ---
            yield return Criar(
                ContextoMetricaEnum.FilaSqs, CategoriaServicoEnum.Integracao,
                classificadoComoProvisionamento: true, classificadoComoInfraestrutura: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosStandardQuantidade, ChaveMetricaEnum.ProvisionamentosFifoQuantidade },
                        horasPorUnidade: 6m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("Standard", ChaveMetricaEnum.ProvisionamentosStandardQuantidade),
                            new EspecificacaoServico("FIFO", ChaveMetricaEnum.ProvisionamentosFifoQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.TopicoKafka, CategoriaServicoEnum.Integracao,
                classificadoComoProvisionamento: true, classificadoComoInfraestrutura: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosQuantidade },
                        horasPorUnidade: 7m)
                });

            yield return Criar(
                ContextoMetricaEnum.Mensageria, CategoriaServicoEnum.Integracao,
                indicadores: new[]
                {
                    DefinicaoIndicador.Aplicacoes(),
                    DefinicaoIndicador.Total(
                        "Mensagens enviadas",
                        new[]
                        {
                            ChaveMetricaEnum.MensagensEmailEnviadasQuantidade,
                            ChaveMetricaEnum.MensagensSmsEnviadasQuantidade,
                            ChaveMetricaEnum.MensagensSmsInterativaEnviadasQuantidade,
                            ChaveMetricaEnum.MensagensTeamsEnviadasQuantidade,
                            ChaveMetricaEnum.MensagensTelefoniaEnviadasQuantidade,
                            ChaveMetricaEnum.MensagensWhatsAppEnviadasQuantidade
                        },
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("E-mail", ChaveMetricaEnum.MensagensEmailEnviadasQuantidade),
                            new EspecificacaoServico("SMS", ChaveMetricaEnum.MensagensSmsEnviadasQuantidade),
                            new EspecificacaoServico("SMS Interativa", ChaveMetricaEnum.MensagensSmsInterativaEnviadasQuantidade),
                            new EspecificacaoServico("Teams", ChaveMetricaEnum.MensagensTeamsEnviadasQuantidade),
                            new EspecificacaoServico("Telefonia", ChaveMetricaEnum.MensagensTelefoniaEnviadasQuantidade),
                            new EspecificacaoServico("WhatsApp", ChaveMetricaEnum.MensagensWhatsAppEnviadasQuantidade)
                        })
                });

            // --- Utilitários ---
            yield return Criar(
                ContextoMetricaEnum.Feedback, CategoriaServicoEnum.Utilitarios,
                indicadores: new[]
                {
                    DefinicaoIndicador.Aplicacoes(),
                    DefinicaoIndicador.Total(
                        "Pesquisas criadas",
                        new[]
                        {
                            ChaveMetricaEnum.PesquisasCsatQuantidade,
                            ChaveMetricaEnum.PesquisasNpsQuantidade,
                            ChaveMetricaEnum.PesquisasBinarioQuantidade
                        },
                        horasPorUnidade: 9m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("CSAT", ChaveMetricaEnum.PesquisasCsatQuantidade),
                            new EspecificacaoServico("NPS", ChaveMetricaEnum.PesquisasNpsQuantidade),
                            new EspecificacaoServico("Binário", ChaveMetricaEnum.PesquisasBinarioQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.ArchIA, CategoriaServicoEnum.Utilitarios,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[]
                        {
                            ChaveMetricaEnum.DocumentosArqrefCadastroQuantidade,
                            ChaveMetricaEnum.DocumentosAdrCadastroQuantidade
                        },
                        horasPorUnidade: 5m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("ARQREF", ChaveMetricaEnum.DocumentosArqrefCadastroQuantidade),
                            new EspecificacaoServico("ADR", ChaveMetricaEnum.DocumentosAdrCadastroQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.Encurtador, CategoriaServicoEnum.Utilitarios,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Links encurtados",
                        new[]
                        {
                            ChaveMetricaEnum.LinksEncurtadosSimplesQuantidade,
                            ChaveMetricaEnum.LinksEncurtadosPersonalizadosQuantidade
                        })
                });

            // --- Segurança ---
            yield return Criar(
                ContextoMetricaEnum.Cognito, CategoriaServicoEnum.Seguranca,
                classificadoComoProvisionamento: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[]
                        {
                            ChaveMetricaEnum.ProvisionamentosUrlLoginQuantidade,
                            ChaveMetricaEnum.ProvisionamentosAppClientQuantidade,
                            ChaveMetricaEnum.ProvisionamentosUserPoolQuantidade
                        },
                        horasPorUnidade: 4m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("URL", ChaveMetricaEnum.ProvisionamentosUrlLoginQuantidade),
                            new EspecificacaoServico("App Client", ChaveMetricaEnum.ProvisionamentosAppClientQuantidade),
                            new EspecificacaoServico("User Pool", ChaveMetricaEnum.ProvisionamentosUserPoolQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.Bitbucket, CategoriaServicoEnum.Seguranca,
                classificadoComoProvisionamento: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosQuantidade },
                        horasPorUnidade: 2.5m)
                });

            yield return Criar(
                ContextoMetricaEnum.Auditoria, CategoriaServicoEnum.Seguranca,
                indicadores: new[]
                {
                    DefinicaoIndicador.Aplicacoes(),
                    DefinicaoIndicador.Total("Eventos", new[] { ChaveMetricaEnum.EventosQuantidade })
                });

            // Claims não possui detalhes.
            yield return Criar(
                ContextoMetricaEnum.Claims, CategoriaServicoEnum.Seguranca,
                indicadores: Array.Empty<DefinicaoIndicador>());

            // --- Dev ---
            yield return Criar(
                ContextoMetricaEnum.FeatureFlag, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true, classificadoComoInfraestrutura: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosQuantidade },
                        horasPorUnidade: 5m)
                });

            yield return Criar(
                ContextoMetricaEnum.SolutionFrontend, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[]
                        {
                            ChaveMetricaEnum.ProvisionamentosIonicQuantidade,
                            ChaveMetricaEnum.ProvisionamentosDsInternoQuantidade,
                            ChaveMetricaEnum.ProvisionamentosDsExternoQuantidade
                        },
                        horasPorUnidade: 10.5m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("Ionic", ChaveMetricaEnum.ProvisionamentosIonicQuantidade),
                            new EspecificacaoServico("DS Interno", ChaveMetricaEnum.ProvisionamentosDsInternoQuantidade),
                            new EspecificacaoServico("DS Externo", ChaveMetricaEnum.ProvisionamentosDsExternoQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.SolutionBackend, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[]
                        {
                            ChaveMetricaEnum.ProvisionamentosKubernetesQuantidade,
                            ChaveMetricaEnum.ProvisionamentosSwarmQuantidade,
                            ChaveMetricaEnum.ProvisionamentosFargateQuantidade
                        },
                        horasPorUnidade: 12m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("Kubernetes", ChaveMetricaEnum.ProvisionamentosKubernetesQuantidade),
                            new EspecificacaoServico("Swarm", ChaveMetricaEnum.ProvisionamentosSwarmQuantidade),
                            new EspecificacaoServico("Fargate", ChaveMetricaEnum.ProvisionamentosFargateQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.BucketS3, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true, classificadoComoInfraestrutura: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosQuantidade },
                        horasPorUnidade: 4.5m)
                });

            yield return Criar(
                ContextoMetricaEnum.Liquibase, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true, classificadoComoInfraestrutura: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosQuantidade },
                        horasPorUnidade: 5.5m)
                });

            yield return Criar(
                ContextoMetricaEnum.Biblioteca, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[]
                        {
                            ChaveMetricaEnum.ProvisionamentosAngularQuantidade,
                            ChaveMetricaEnum.ProvisionamentosDotnetQuantidade
                        },
                        horasPorUnidade: 8m,
                        especificacoes: new[]
                        {
                            new EspecificacaoServico("Angular", ChaveMetricaEnum.ProvisionamentosAngularQuantidade),
                            new EspecificacaoServico("Dotnet", ChaveMetricaEnum.ProvisionamentosDotnetQuantidade)
                        })
                });

            yield return Criar(
                ContextoMetricaEnum.SonarQube, CategoriaServicoEnum.Dev,
                classificadoComoProvisionamento: true, classificadoComoInfraestrutura: true,
                indicadores: new[]
                {
                    DefinicaoIndicador.Total(
                        "Provisionamentos no período",
                        new[] { ChaveMetricaEnum.ProvisionamentosQuantidade },
                        horasPorUnidade: 5.5m)
                });
        }

        private static DefinicaoServico Criar(
            ContextoMetricaEnum contexto,
            CategoriaServicoEnum categoria,
            IReadOnlyList<DefinicaoIndicador> indicadores,
            bool classificadoComoProvisionamento = false,
            bool classificadoComoInfraestrutura = false)
        {
            return new DefinicaoServico
            {
                Contexto = contexto,
                Nome = contexto.ObterDescricao(),
                Categoria = categoria,
                ClassificadoComoProvisionamento = classificadoComoProvisionamento,
                ClassificadoComoInfraestrutura = classificadoComoInfraestrutura,
                Indicadores = indicadores
            };
        }
    }
}

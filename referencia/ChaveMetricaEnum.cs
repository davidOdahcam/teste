using System.ComponentModel;

namespace Autoglass.PlataformaHUB.CrossCutting.Enums
{
    public enum ChaveMetricaEnum
    {
        [Description("Aplicacoes:Quantidade")]
        AplicacoesQuantidade = 1,

        // ArchIA
        [Description("Documentos:Arqref:Cadastro:Quantidade")]
        DocumentosArqrefCadastroQuantidade = 2,

        [Description("Documentos:Adr:Cadastro:Quantidade")]
        DocumentosAdrCadastroQuantidade = 3,

        // Auditoria
        [Description("Eventos:Quantidade")]
        EventosQuantidade = 4,

        // Biblioteca
        [Description("Provisionamentos:Angular:Quantidade")]
        ProvisionamentosAngularQuantidade = 5,

        [Description("Provisionamentos:Dotnet:Quantidade")]
        ProvisionamentosDotnetQuantidade = 6,

        // Clarity
        [Description("Sessoes:Quantidade")]
        SessoesQuantidade = 7,

        // Cognito
        [Description("Provisionamentos:AppClient:Quantidade")]
        ProvisionamentosAppClientQuantidade = 8,

        [Description("Provisionamentos:UserPool:Quantidade")]
        ProvisionamentosUserPoolQuantidade = 9,

        [Description("Provisionamentos:UrlLogin:Quantidade")]
        ProvisionamentosUrlLoginQuantidade = 10,

        // Encurtador
        [Description("LinksEncurtados:Simples:Quantidade")]
        LinksEncurtadosSimplesQuantidade = 11,

        [Description("LinksEncurtados:Personalizados:Quantidade")]
        LinksEncurtadosPersonalizadosQuantidade = 12,

        // Feedback
        [Description("Pesquisas:Csat:Quantidade")]
        PesquisasCsatQuantidade = 13,

        [Description("Pesquisas:Nps:Quantidade")]
        PesquisasNpsQuantidade = 14,

        [Description("Pesquisas:Binario:Quantidade")]
        PesquisasBinarioQuantidade = 15,

        // Fila SQS
        [Description("Provisionamentos:Standard:Quantidade")]
        ProvisionamentosStandardQuantidade = 16,

        [Description("Provisionamentos:Fifo:Quantidade")]
        ProvisionamentosFifoQuantidade = 17,

        // Mensageria
        [Description("Mensagens:Email:Enviadas:Quantidade")]
        MensagensEmailEnviadasQuantidade = 18,

        [Description("Mensagens:Sms:Enviadas:Quantidade")]
        MensagensSmsEnviadasQuantidade = 19,

        [Description("Mensagens:SmsInterativa:Enviadas:Quantidade")]
        MensagensSmsInterativaEnviadasQuantidade = 20,

        [Description("Mensagens:Teams:Enviadas:Quantidade")]
        MensagensTeamsEnviadasQuantidade = 21,

        [Description("Mensagens:Telefonia:Enviadas:Quantidade")]
        MensagensTelefoniaEnviadasQuantidade = 22,

        [Description("Mensagens:WhatsApp:Enviadas:Quantidade")]
        MensagensWhatsAppEnviadasQuantidade = 23,

        // Bitbucket, BucketS3, FeatureFlag, Liquibase, SonarQube, TopicoKafka
        [Description("Provisionamentos:Quantidade")]
        ProvisionamentosQuantidade = 24,

        // Solution Backend
        [Description("Provisionamentos:Kubernetes:Quantidade")]
        ProvisionamentosKubernetesQuantidade = 25,

        [Description("Provisionamentos:Swarm:Quantidade")]
        ProvisionamentosSwarmQuantidade = 26,

        [Description("Provisionamentos:Fargate:Quantidade")]
        ProvisionamentosFargateQuantidade = 27,

        // Solution Frontend
        [Description("Provisionamentos:Ionic:Quantidade")]
        ProvisionamentosIonicQuantidade = 28,

        [Description("Provisionamentos:DsInterno:Quantidade")]
        ProvisionamentosDsInternoQuantidade = 29,

        [Description("Provisionamentos:DsExterno:Quantidade")]
        ProvisionamentosDsExternoQuantidade = 30,

        // Acessos (por contexto/serviço) - necessário para os KPIs e gráficos de acesso do relatório
        [Description("Acessos:Quantidade")]
        AcessosQuantidade = 31,
 
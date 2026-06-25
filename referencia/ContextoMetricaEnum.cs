namespace Autoglass.PlataformaHUB.CrossCutting.Enums
{
    public enum ContextoMetricaEnum
    {
        [Description("ArchIA")]
        ArchIA = 1,

        [Description("Auditoria")]
        Auditoria = 2,

        [Description("Solution Backend")]
        SolutionBackend = 3,

        [Description("Biblioteca")]
        Biblioteca = 4,

        [Description("Bitbucket")]
        Bitbucket = 5,

        [Description("Bucket S3")]
        BucketS3 = 6,

        [Description("Claims")]
        Claims = 7,

        [Description("Cognito")]
        Cognito = 8,

        [Description("Encurtador")]
        Encurtador = 9,

        [Description("Feature Flag")]
        FeatureFlag = 10,

        [Description("Feedback")]
        Feedback = 11,

        [Description("Fila SQS")]
        FilaSqs = 12,

        [Description("Solution Frontend")]
        SolutionFrontend = 13,

        [Description("Liquibase")]
        Liquibase = 14,

        [Description("Mensageria")]
        Mensageria = 15,

        [Description("SonarQube")]
        SonarQube = 16,

        [Description("Tópico Kafka")]
        TopicoKafka = 17,

        [Description("Vault")]
        Vault = 18
    }
}
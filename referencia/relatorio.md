## ABA VISÃO GERAL

### KPIs

**KPI 1**
- Nome: Acessos à plataforma
- Exemplo: 476
- Explicação: Total de acessos no período selecionado

**KPI 2**
- Nome: Serviços acessados no período
- Exemplo: 100% 
- Explicação: Seviços que receberam qualquer tipo de acesso no período / quantidade total de serviços

**KPI 3**
- Nome: Horas economizadas
- Exemplo: 214
- Explicação: cada tipo de serviço tem um peso em horas

**KPI 4**
- Nome: Provisionamentos de infraestrutura
- Exemplo: 600
- Explicação: somente provisionamentos como Fila SQS, Kafka, S3, Feature flag, liquibase e sonarqube - quero poder adicionar mais depois

### Graficos

**Acessos por período**
- Explicação: Quantidade de acessos por data, considerando o período do relatório

**Serviços em destaque - Mais acessados**
- Explicação: Quantidade de acessos por serviço, considerando o período do relatório, ordem decrescente

**Serviços em destaque - Maior economia de horas**
- Explicação: Economia em horas por serviço, considerando o período do relatório, ordem decrescente

## ABA VISÃO SERVIÇOS

### Tabela (lista ordenada pelo nome)

**FilaSqs**
- Acessos: Quantidade de acessos no período
- Categoria: Integração
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período
    - Horas economizadas no processo: quantidade de provisionamentos * 6 horas
    - Especificação: quantidade por tipo (Standard/FIFO) no período

**TopicoKafka**
- Acessos: Quantidade de acessos no período
- Categoria: Integração
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período
    - Horas economizadas no processo: quantidade de provisionamentos * 7 horas

**Mensageria**
- Acessos: Quantidade de acessos no período
- Categoria: Integração
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Aplicacoes: quantidade de aplicações registrada mais recente, não leva em consideração o período
    - Mensagens enviadas: soma dos registros de mensagens enviadas no período
    - Atividade no período: Quantidade de mensagens enviadas por data no período
    - Especificação: quantidade de Mensagens enviadas por tipo no período

**Feeback**
- Acessos: Quantidade de acessos no período
- Categoria: Utilitários
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Aplicacoes: quantidade de aplicações registrada mais recente, não leva em consideração o período
    - Pesquisas criadas: soma dos registros de quantidade de pesquisas csat, nps e binario no período
    - Atividade no período: Quantidade de pesquisas por data
    - Economia: quantidade de pesquisas criadas * 9 horas
    - Quantidade de pesquisa criada no período por tipo

**Cognito**
- Acessos: Quantidade de acessos no período
- Categoria: Segurança
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 4 horas
    - Especificação: quantidade por tipo (URL/App Client/User Pool) no período

**FeatureFlag**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 5 horas

**ArchIA**
- Acessos: Quantidade de acessos no período
- Categoria: Utilitários
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 5 horas
    - Especificação: quantidade por tipo (ARQREF/ADR) no período

**SolutionFrontend**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 10.5 horas
    - Especificação: quantidade por tipo no período

**Bitbucket**
- Acessos: Quantidade de acessos no período
- Categoria: Segurança
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 2.5 horas

**SolutionBackend**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 12 horas
    - Especificação: quantidade por tipo no período

**BucketS3**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 4.5 horas

**Liquibase**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 5.5 horas

**Biblioteca**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 8 horas
    - Especificação: quantidade por tipo no período

**Claims**
- Acessos: Quantidade de acessos no período
- Categoria: Segurança
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes: Não tem detalhes

**SonarQube**
- Acessos: Quantidade de acessos no período
- Categoria: Dev
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Provisionamentos no período: soma dos registros de provisionamento no período
    - Atividade no período: provisionamentos por data no período 
    - Horas economizadas no processo: quantidade de provisionamentos * 5.5 horas

**Auditorias**
- Acessos: Quantidade de acessos no período
- Categoria: Segurança
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Aplicacoes: quantidade de aplicações registrada mais recente, não leva em consideração o período
    - Eventos: soma dos registros de eventos no período
    - Atividade no período: Quantidade de eventos por data no período

**Encurtador**
- Acessos: Quantidade de acessos no período
- Categoria: Utilitários
- Último uso: última data com registro de acesso, não deve considerar o período
- Detalhes:
    - Links encurtados: soma de links simples e personalizados no período
    - Atividade no período: Quantidade de links encurtados por data no período

## ABA EMPRESAS

### Gráficos

**Consumo de serviços de infraestrutura**
- Quantidade de provisionamentos de serviços de infraestrutura por empresa no período

**Provisionamentos por período**
- Quantidade de provisionamentos por data para cada empresa no período

### Tabela

**Provisionamentos por empresa**
- Quantidade de provisionamentos por serviço, por empresa
 
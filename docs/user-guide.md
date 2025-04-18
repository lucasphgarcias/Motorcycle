# Guia do Usuário - Sistema de Gestão de Aluguel de Motocicletas

Este guia fornece instruções detalhadas sobre como utilizar o Sistema de Gestão de Aluguel de Motocicletas, direcionado aos administradores e operadores do sistema.

## Índice

1. [Visão Geral do Sistema](#visão-geral-do-sistema)
2. [Primeiros Passos](#primeiros-passos)
3. [Gestão de Motocicletas](#gestão-de-motocicletas)
4. [Gestão de Entregadores](#gestão-de-entregadores)
5. [Gestão de Aluguéis](#gestão-de-aluguéis)
6. [Relatórios](#relatórios)
7. [Configurações do Sistema](#configurações-do-sistema)
8. [Solução de Problemas](#solução-de-problemas)

## Visão Geral do Sistema

O Sistema de Gestão de Aluguel de Motocicletas permite o controle completo da frota de motocicletas, cadastro de entregadores e gestão de aluguéis. A plataforma foi projetada para facilitar o gerenciamento diário da operação, proporcionando uma visão clara do status de cada motocicleta, histórico de aluguéis e informações detalhadas sobre os entregadores.

### Principais Funcionalidades

- Cadastro e gestão de motocicletas
- Cadastro e gestão de entregadores
- Criação e acompanhamento de aluguéis
- Processamento de devoluções
- Geração de relatórios
- Controle financeiro de recebimentos

## Primeiros Passos

### Requisitos do Sistema

Para acessar o sistema, você precisará de:

- Navegador web atualizado (Chrome, Firefox, Edge ou Safari)
- Conexão com a internet
- Credenciais de acesso fornecidas pelo administrador do sistema

### Acessando o Sistema

1. Abra seu navegador e acesse: `https://app.motorcyclerental.com`
2. Na tela de login, insira seu e-mail e senha
3. Clique no botão "Entrar"

![Tela de Login](../images/login_screen.png)

### Interface Principal

Após o login, você terá acesso ao painel principal (dashboard) que apresenta:

- Resumo da quantidade de motocicletas disponíveis/alugadas
- Aluguéis ativos
- Aluguéis que vencem nos próximos 7 dias
- Gráfico de desempenho financeiro
- Acesso rápido às principais funcionalidades

![Dashboard](../images/dashboard.png)

### Menu de Navegação

O menu lateral permite navegar entre as diferentes seções do sistema:

- **Dashboard**: Visão geral do sistema
- **Motocicletas**: Gestão da frota
- **Entregadores**: Cadastro e consulta de entregadores
- **Aluguéis**: Gerenciamento de contratos
- **Relatórios**: Geração de relatórios diversos
- **Configurações**: Ajustes do sistema
- **Minha Conta**: Gerenciamento do seu perfil

## Gestão de Motocicletas

### Visualizando Motocicletas

1. No menu lateral, clique em "Motocicletas"
2. A lista de motocicletas será exibida com as seguintes informações:
   - Modelo
   - Placa
   - Ano
   - Status (Disponível, Alugada, Manutenção)
   - Taxa diária de aluguel

![Lista de Motocicletas](../images/motorcycles_list.png)

### Filtros e Busca

Para encontrar motocicletas específicas:

1. Use o campo de busca no topo da lista para procurar por modelo ou placa
2. Utilize os filtros para refinar por status, ano ou faixa de valor

### Cadastrando Nova Motocicleta

1. Na tela de listagem de motocicletas, clique no botão "+ Nova Motocicleta"
2. Preencha os campos do formulário:
   - Modelo
   - Ano
   - Placa
   - Valor da diária
   - Informações adicionais (opcionais)
3. Clique em "Salvar"

![Cadastro de Motocicleta](../images/motorcycle_form.png)

### Detalhes da Motocicleta

Para visualizar informações detalhadas de uma motocicleta:

1. Clique no modelo da motocicleta na lista
2. A tela de detalhes mostrará:
   - Informações básicas da motocicleta
   - Histórico de aluguéis
   - Histórico de manutenções
   - Documentação

### Editando Motocicleta

1. Na tela de detalhes da motocicleta, clique no botão "Editar"
2. Atualize os campos necessários
3. Clique em "Salvar"

### Alterando Status da Motocicleta

1. Na tela de detalhes, clique no menu suspenso de status
2. Selecione o novo status (Disponível, Manutenção)
3. Se necessário, adicione uma observação
4. Clique em "Confirmar"

### Registrando Manutenção

1. Na tela de detalhes, clique na aba "Manutenções"
2. Clique no botão "Registrar Manutenção"
3. Preencha o formulário com:
   - Data de início
   - Descrição do serviço
   - Valor
   - Fornecedor
4. Clique em "Salvar"

## Gestão de Entregadores

### Visualizando Entregadores

1. No menu lateral, clique em "Entregadores"
2. A lista de entregadores será exibida com:
   - Nome
   - CNPJ
   - Contato
   - Status
   - Motocicleta atual (se houver aluguel ativo)

![Lista de Entregadores](../images/delivery_persons_list.png)

### Cadastrando Novo Entregador

1. Na tela de listagem de entregadores, clique no botão "+ Novo Entregador"
2. Preencha o formulário com:
   - Nome completo
   - CNPJ
   - E-mail
   - Telefone
   - Informações da CNH (número, categoria, data de validade)
   - Endereço
3. Clique em "Salvar"

![Cadastro de Entregador](../images/delivery_person_form.png)

### Detalhes do Entregador

1. Clique no nome do entregador na lista
2. A tela de detalhes mostrará:
   - Informações pessoais
   - Documentação
   - Histórico de aluguéis
   - Ocorrências (se houver)

### Editando Entregador

1. Na tela de detalhes do entregador, clique no botão "Editar"
2. Atualize os campos necessários
3. Clique em "Salvar"

### Verificando Documentação

1. Na tela de detalhes, acesse a aba "Documentos"
2. Visualize os documentos anexados (CNH, Comprovante de Endereço, etc.)
3. Para adicionar um novo documento, clique em "Adicionar Documento"
4. Selecione o tipo de documento, faça o upload do arquivo e clique em "Salvar"

## Gestão de Aluguéis

### Visualizando Aluguéis

1. No menu lateral, clique em "Aluguéis"
2. A lista de aluguéis será exibida com:
   - Número do contrato
   - Entregador
   - Motocicleta
   - Data de início
   - Data de término prevista
   - Status (Ativo, Concluído, Cancelado)

![Lista de Aluguéis](../images/rentals_list.png)

### Criando Novo Aluguel

1. Na tela de listagem de aluguéis, clique no botão "+ Novo Aluguel"
2. Selecione o entregador (ou cadastre um novo)
3. Selecione a motocicleta disponível
4. Escolha o plano de aluguel:
   - 7 dias
   - 15 dias
   - 30 dias
   - 45 dias
   - 90 dias
5. Defina a data de início
6. Revise os valores calculados automaticamente
7. Clique em "Criar Aluguel"

![Criação de Aluguel](../images/rental_form.png)

### Detalhes do Aluguel

1. Clique no número do contrato na lista
2. A tela de detalhes mostrará:
   - Informações do contrato
   - Dados do entregador
   - Dados da motocicleta
   - Valores e pagamentos
   - Ocorrências (se houver)

### Registrando Devolução

1. Na tela de detalhes do aluguel, clique no botão "Registrar Devolução"
2. Preencha a data de devolução
3. Registre o estado da motocicleta:
   - Selecione as condições (Normal, Danos Leves, Danos Graves)
   - Adicione fotos, se necessário
   - Registre o hodômetro
4. O sistema calculará automaticamente valores adicionais (se houver antecipação ou atraso)
5. Clique em "Confirmar Devolução"

![Registro de Devolução](../images/return_form.png)

### Cancelando Aluguel

1. Na tela de detalhes do aluguel, clique no menu de opções (três pontos)
2. Selecione "Cancelar Aluguel"
3. Informe o motivo do cancelamento
4. Clique em "Confirmar"

## Relatórios

### Tipos de Relatórios Disponíveis

- **Relatório de Frota**: Status atual de todas as motocicletas
- **Relatório de Aluguéis**: Resumo de aluguéis por período
- **Relatório Financeiro**: Receitas, despesas e lucro
- **Relatório de Entregadores**: Histórico e atividade de entregadores
- **Relatório de Manutenções**: Histórico de manutenções e custos

### Gerando um Relatório

1. No menu lateral, clique em "Relatórios"
2. Selecione o tipo de relatório desejado
3. Defina os filtros (período, status, entregador, etc.)
4. Clique em "Gerar Relatório"
5. O relatório será exibido na tela e você poderá:
   - Exportar como PDF
   - Exportar como Excel
   - Imprimir diretamente
   - Enviar por e-mail

![Geração de Relatórios](../images/report_generation.png)

## Configurações do Sistema

### Perfil de Usuário

Para gerenciar seu perfil:

1. Clique no seu nome no canto superior direito
2. Selecione "Minha Conta"
3. Você poderá:
   - Atualizar informações pessoais
   - Alterar senha
   - Configurar preferências de notificação

### Configurações Gerais

Se você possui privilégios de administrador:

1. No menu lateral, clique em "Configurações"
2. Você terá acesso a:
   - **Usuários**: Gerenciamento de usuários do sistema
   - **Planos de Aluguel**: Configuração de planos e valores
   - **Parâmetros**: Ajustes gerais do sistema
   - **Backup**: Opções de backup de dados
   - **Logs**: Registro de atividades do sistema

### Gerenciando Usuários

1. Em Configurações, acesse "Usuários"
2. Você poderá:
   - Visualizar todos os usuários
   - Criar novos usuários
   - Editar permissões
   - Desativar/reativar usuários

![Gerenciamento de Usuários](../images/users_management.png)

### Configurando Planos de Aluguel

1. Em Configurações, acesse "Planos de Aluguel"
2. Você poderá:
   - Ajustar valores diários para cada plano
   - Criar novos planos personalizados
   - Definir regras de cobrança para devolução antecipada/tardia

## Solução de Problemas

### Problemas Comuns e Soluções

| Problema | Possível Solução |
|----------|------------------|
| Não consigo fazer login | Verifique se o e-mail e senha estão corretos. Use a opção "Esqueci minha senha" para redefinir sua senha. |
| A página não carrega | Verifique sua conexão com a internet. Limpe o cache do navegador ou tente outro navegador. |
| Não encontro uma motocicleta | Verifique se os filtros estão ativos. Tente buscar pelo número da placa exato. |
| Erro ao criar um aluguel | Verifique se a motocicleta selecionada está realmente disponível e se todos os campos obrigatórios foram preenchidos. |
| O relatório não é gerado | Tente reduzir o período selecionado. Verifique se há muitos filtros aplicados. |

### Contato de Suporte

Para problemas técnicos ou dúvidas sobre o sistema:

- **E-mail**: suporte@motorcyclerental.com
- **Telefone**: (11) 1234-5678
- **Chat**: Disponível no ícone no canto inferior direito da tela
- **Horário de atendimento**: Segunda a Sexta, das 8h às 18h

## Atalhos de Teclado

Para usuários avançados, os seguintes atalhos de teclado estão disponíveis:

- **Alt+M**: Ir para Motocicletas
- **Alt+E**: Ir para Entregadores
- **Alt+A**: Ir para Aluguéis
- **Alt+R**: Ir para Relatórios
- **Alt+N**: Criar Novo (item da seção atual)
- **Alt+B**: Voltar à tela anterior
- **Alt+S**: Salvar formulário atual
- **Ctrl+F**: Busca rápida

---

© 2023 Sistema de Gestão de Aluguel de Motocicletas. Todos os direitos reservados. 
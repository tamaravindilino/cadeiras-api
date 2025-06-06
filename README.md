## Sistema de Cadastro de Cadeiras de Dentista

* Objetivo

Desenvolver uma api de gerenciamento de cadeiras de dentista utilizando a plataforma .NET. O aplicativo deve permitir a criação, leitura, atualização e exclusão de cadeiras. 


* Funcionalidades

- Cadastro de cadeiras
- Alocação automática de cadeiras proporcional ao tempo solicitado
- Verificação de disponibilidade no intervalo informado
- Distribuição cíclica entre cadeiras disponíveis


* Stack utilizada

- C#
- ASP.NET Core 8
- MySQL


# ** Como rodar o projeto localmente

1. Clone do git 
  git clone https://github.com/tamaravindilino/cadeiras-api.git


2. Configure sua conexão com o MySQL no appsetting.json

3. Execute o projeto

  dotnet ef migrations add Inicial
  dotnet ef database update
  dotnet run

  https://localhost:7144/swagger/index.html
  https://localhost:5144/swagger/index.html
# 🚀 DotNetJenkinsPipeline

Este é um projeto de exemplo em .NET criado com o objetivo de testar e validar pipelines de CI/CD locais, utilizando o Jenkins rodando em container Docker e fazendo deploy em ambiente IIS no Windows, através de um agente Jenkins.

---

## 🎯 Objetivo

- Validar um fluxo de build e deploy on-premise, sem depender de serviços em nuvem  
- Usar Jenkins em container Docker para configurar jobs e pipelines  
- Simular um cenário realista de automação de entrega para aplicações .NET  
- Fazer o deploy automaticamente em ambiente IIS local quando houver alteração na branch `main`  

---

## 🛠 Tecnologias utilizadas

- ASP.NET  
- Docker (para o Jenkins)  
- Jenkins (em container)  
- Git  
- C#  
- Domain-Driven Design (DDD)  
- Clean Code  

---

## 📦 O que o projeto contém

- Uma API .NET com um Value Object de CPF, implementando validação, imutabilidade e boas práticas de DDD  
- Um pipeline de CI/CD configurado no Jenkins, com agente Windows  
- Publicação automatizada no IIS local via Web Deploy (`msdeploy.exe`)  

---

## 📁 Estrutura do Projeto

```groovy
src/
└── services/
    └── DotNetJenkinsPipeline/
        ├── Controllers/
        │   └── CpfController.cs
        ├── Domain/
        │   └── ValueObjects/
        │       └── Cpf.cs
        ├── Properties/
        │   └── PublishProfiles/
        │       ├── FolderProfile.pubxml
        │       └── JenkinsProfile.pubxml
        ├── appsettings.json
        ├── Dockerfile
        ├── Program.cs
        └── Startup.cs
```

---

⚙️ Jenkins Pipeline

O Jenkins está configurado para executar um pipeline que realiza:
- Checkout do código da branch main
- Build da solução
- Publicação com dotnet build usando o profile JenkinsProfile.pubxml
- Deploy no IIS com Web Deploy
- Reinício do serviço IIS (w3svc)

---

📄 Jenkinsfile usado

```groovy
pipeline {
    agent { label 'windows' }
    environment {
        dotnet = 'C:\\Program Files\\dotnet\\dotnet.exe'
    }
    stages {
        stage('Checkout Stage') {
            steps {
                git credentialsId: 'cred-github-andre', url: 'https://github.com/AndreBorgees/DotNetJenkinsPipeline.git', branch: 'main'
            }
        }
        stage('Build Stage') {
            steps {
                bat 'dotnet build %WORKSPACE%\\DotNetJenkinsPipeline.sln --configuration Release'
            }
        }
        stage("Release Stage") {
            steps {
                bat 'dotnet build %WORKSPACE%\\DotNetJenkinsPipeline.sln /p:PublishProfile="%WORKSPACE%\\src\\services\\DotNetJenkinsPipeline\\Properties\\PublishProfiles\\JenkinsProfile.pubxml" /p:Platform="Any CPU" /p:DeployOnBuild=true /m'
            }
        }
        stage('Deploy Stage') {
            steps {
                bat 'net stop "w3svc"'
                bat '"C:\\Program Files (x86)\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe" -verb:sync -source:package="%WORKSPACE%\\src\\services\\DotNetJenkinsPipeline\\bin\\Debug\\net5.0\\DotNetJenkinsPipeline.zip" -dest:auto -setParam:"IIS Web Application Name"="DotNetJenkinsPipeline" -skip:objectName=filePath,absolutePath=".\\\\PackageTmp\\\\Web.config$" -enableRule:DoNotDelete -allowUntrusted=true'
                bat 'net start "w3svc"'
            }
        }
    }
}
```
---

🚀 Como usar

✅ Pré-requisitos
- .NET SDK instalado no agente Windows
- Docker instalado para rodar o Jenkins localmente
- IIS habilitado e funcionando na máquina do agente
- Web Deploy instalado (msdeploy.exe)
- Jenkins Agent configurado no Windows com label windows

📦 Passo a passo
1. Clonar o projeto
```groovy
git clone https://github.com/AndreBorgees/DotNetJenkinsPipeline.git
```

2. Rodar o Jenkins com Docker
```groovy
docker run -d -p 8080:8080 -p 50000:50000 --name jenkins \
  -v jenkins_home:/var/jenkins_home \
  jenkins/jenkins:lts
```
Acesse o Jenkins em: http://localhost:8080

3. Configurar o agente no Windows
- No Jenkins, vá em Manage Jenkins > Nodes e crie um novo node chamado windows
- Baixe o agent.jar no link fornecido pelo Jenkins
- Execute o agente com o comando (substitua SEU_SECRET pelo token gerado):
```groovy
java -jar agent.jar -jnlpUrl http://localhost:8080/computer/windows/slave-agent.jnlp -secret SEU_SECRET
```

4. Criar o pipeline no Jenkins
- Crie um pipeline no Jenkins e configure o repositório Git:
```groovy
URL: https://github.com/AndreBorgees/DotNetJenkinsPipeline.git
Branch: main
```
O Jenkins irá detectar mudanças e executar o pipeline automaticamente.

5. Após um push na branch main:
- O build será feito
- A aplicação será publicada
- O deploy será realizado automaticamente no IIS

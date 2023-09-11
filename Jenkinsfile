#!groovy

def kaniko_backend_build(String proj_name, Integer index) {
  def lowercased = proj_name.toLowerCase()
  container("kaniko${index}") {
    stage("${proj_name} image build") {
      sh """
        /kaniko/executor  --dockerfile ContainerConfigs/Backend/Dockerfile \
                          --context . \
                          --destination harbor.dev.kub.core.dm.am/staging/${lowercased}:${env.GIT_COMMIT} \
                          --cache \
                          --cache-repo harbor.dev.kub.core.dm.am/staging/${lowercased}-cache \
                          --build-arg PROJECT_NAME=${proj_name}
      """
    }
  } 
}

podTemplate(containers: [
  containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/sdk:6.0', alwaysPullImage: true, command: 'sleep', args: 'infinity'),
  containerTemplate(name: 'nodejs', image: 'node:current', alwaysPullImage: true, command: 'sleep', args: 'infinity', envVars: [containerEnvVar(key: 'NODE_OPTIONS', value: '--openssl-legacy-provider')]),
  containerTemplate(name: 'bun', image: 'oven/bun:latest', alwaysPullImage: true, command: 'sleep', args: 'infinity'),
  containerTemplate(name: 'kaniko0', image: 'gcr.io/kaniko-project/executor:debug', alwaysPullImage: true, command: 'sleep', args: 'infinity'),
  containerTemplate(name: 'kaniko1', image: 'gcr.io/kaniko-project/executor:debug', alwaysPullImage: true, command: 'sleep', args: 'infinity'),
  containerTemplate(name: 'kaniko2', image: 'gcr.io/kaniko-project/executor:debug', alwaysPullImage: true, command: 'sleep', args: 'infinity'),
  containerTemplate(name: 'kaniko3', image: 'gcr.io/kaniko-project/executor:debug', alwaysPullImage: true, command: 'sleep', args: 'infinity')
]) {
  node(POD_LABEL) {
    properties([disableConcurrentBuilds(abortPrevious: true)])
    
    stage('Checkout') {
      def scmVars = checkout scm
      fileOperations([
        folderCreateOperation(folderPath:'publish')
      ])
      env.GIT_COMMIT = scmVars.GIT_COMMIT
    }
        
    parallel(dotnet: {
      container('dotnet') {
        stage('DotNet Build') {
          try {
            sh "dotnet build DM/DM.sln --nologo -logger:/srv/msbuildlogger/MSBuildJenkins.dll"
          }
          finally {
            recordIssues tool: issues(pattern: 'issues.json.log'), enabledForFailure: true, qualityGates: [[threshold: 1, type: 'TOTAL_ERROR', unstable: false], [threshold: 1, type: 'NEW_NORMAL', unstable: true]], publishAllIssues: true
          }
        }
        stage('DotNet Test') {
          warnError('Tests failed!') {
            sh 'dotnet test --no-restore --no-build --nologo --logger trx --results-directory UnitTestResults DM/DM.sln & wait'
          }
          sh '/srv/tools/trx2junit UnitTestResults/*.trx'
          recordIssues tool: junitParser(pattern: 'UnitTestResults/*.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
          junit testResults: 'UnitTestResults/*.xml', allowEmptyResults: true
        }
        stage('DotNet Publish') {
          sh "dotnet publish DM/Web/DM.Web.API --nologo --output publish/DM.Web.API --no-build"
          sh "dotnet publish DM/Services/DM.Services.Mail.Sender.Consumer --nologo --output publish/DM.Services.Mail.Sender.Consumer --no-build"
          sh "dotnet publish DM/Services/DM.Services.Search.Consumer --nologo --output publish/DM.Services.Search.Consumer --no-build"
          sh "dotnet publish DM/Services/DM.Services.Notifications.Consumer --nologo --output publish/DM.Services.Notifications.Consumer --no-build"
        }
      }
    }, typescript: {
      dir("DM/Web/DM.Web.Next") {
        container('bun') {
          stage('VueJs Update Packages') {
            sh 'bun update -f --no-save'
          }
        }
        container('nodejs') {
          stage('VueJs Build') {
            sh 'yarn build'
          }
          stage('VueJS Lint') {
            warnError('Lint failed!') {
              sh 'yarn lint --format checkstyle --output-file eslintreport.xml'
            }
            recordIssues tool: esLint(pattern: 'eslintreport.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
          }
        }
      }
      stage('VueJS Publish') {
        fileOperations([
          folderCopyOperation(sourceFolderPath:'DM/Web/DM.Web.Next/dist', destinationFolderPath: 'publish/DM.Web.Next')
        ])
      }
    }, failFast: true)
    
    
    
    def proj_names = [
      "DM.Web.API",
      "DM.Services.Mail.Sender.Consumer",
      "DM.Services.Search.Consumer",
      "DM.Services.Notifications.Consumer"]
      
    def stepsForParallel = [:]
    proj_names.eachWithIndex { proj, index ->
      stepsForParallel[proj] = {         
          kaniko_backend_build(proj, index)
      }
    }
    stepsForParallel['failFast'] = true
    
    parallel(stepsForParallel)
    
    stage('Zip Artifacts') {
      zip zipFile: 'buildresult.zip', dir: 'publish'
      archiveArtifacts artifacts: 'buildresult.zip', fingerprint: true
    }
  }
}

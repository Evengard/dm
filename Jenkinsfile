#!groovy

podTemplate(containers: [
	containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/sdk:6.0', alwaysPullImage: true, command: 'sleep', args: 'infinity'),
	containerTemplate(name: 'nodejs', image: 'node:current', alwaysPullImage: true, command: 'sleep', args: 'infinity', envVars: [containerEnvVar(key: 'NODE_OPTIONS', value: '--openssl-legacy-provider')]),
]) {
	node(POD_LABEL) {
		properties([disableConcurrentBuilds(abortPrevious: true)])
		
		stage('Checkout') {
			checkout scm
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
			container('nodejs') {
				dir("DM/Web/DM.Web.Next") {
					stage('VueJs Build') {
						sh 'yarn install --network-timeout 300000'
						sh 'yarn upgrade --latest --network-timeout 300000'
						sh 'yarn build'
					}
					stage('VueJS Lint') {
						warnError('Lint failed!') {
							sh 'yarn lint --format checkstyle --output-file eslintreport.xml'
						}
						recordIssues tool: esLint(pattern: 'eslintreport.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
					}
				}
				stage('VueJS Publish') {
					sh 'mkdir -p publish/DM.Web.Next'
					sh 'cp -r DM/Web/DM.Web.Next/dist/. publish/DM.Web.Next'
				}
			}
		}, failFast: true)
		
		stage('Zip Artifacts') {
			zip zipFile: 'buildresult.zip', dir: 'publish'
			archiveArtifacts artifacts: 'buildresult.zip', fingerprint: true
		}
	}
}

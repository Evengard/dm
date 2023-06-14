#!groovy

podTemplate(containers: [
	containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/sdk:6.0', alwaysPullImage: true, command: '/srv/tools/tini', args: '-g -- sleep infinity', envVars: [containerEnvVar(key: 'UseSharedCompilation', value: 'false'), containerEnvVar(key: 'DISABLEOUTOFPROCTASKHOST', value: 'true')], ttyEnabled: true),
	containerTemplate(name: 'nodejs', image: 'node:current', alwaysPullImage: true, command: '/srv/tools/tini', args: '-g -- sleep infinity', envVars: [containerEnvVar(key: 'NODE_OPTIONS', value: '--openssl-legacy-provider')]),
]) {
	node(POD_LABEL) {
		stage('Checkout') {
			checkout scm
		}
        
		parallel(dotnet: {
			container('dotnet') {
				stage('DotNet Build') {
					try {
                        dotnetBuild project: 'DM/DM.sln', option: '-logger:/srv/msbuildlogger/MSBuildJenkins.dll', nologo: true, shutDownBuildServers: true, properties: ['UseSharedCompilation': 'false']
					}
					finally {
						recordIssues tool: issues(pattern: 'issues.json.log'), enabledForFailure: true, qualityGates: [[threshold: 1, type: 'TOTAL_ERROR', unstable: false], [threshold: 1, type: 'NEW_NORMAL', unstable: true]], publishAllIssues: true
					}
				}
				stage('DotNet Test') {
                    //sh 'apt-get update && apt-get install -y tini || true'
					warnError('Tests failed!') {
                        //sh 'tini -s -- dotnet test --no-restore --no-build --nologo --logger trx --results-directory UnitTestResults DM/DM.sln'
                        dotnetTest project: 'DM/DM.sln', logger:'trx', resultsDirectory: 'UnitTestResults', noBuild: true, noRestore: true, nologo: true, shutDownBuildServers: true, properties: ['UseSharedCompilation': 'false']
					}
					sh '/srv/tools/trx2junit UnitTestResults/*.trx'
					recordIssues tool: junitParser(pattern: 'UnitTestResults/*.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
					junit testResults: 'UnitTestResults/*.xml', allowEmptyResults: true
				}
				stage('DotNet Publish') {
					dotnetPublish project: 'DM/Web/DM.Web.API', noBuild: true, outputDirectory: 'publish/DM.Web.API', nologo: true, shutDownBuildServers: true
					dotnetPublish project: 'DM/Services/DM.Services.Mail.Sender.Consumer', noBuild: true, outputDirectory: 'publish/DM.Services.Mail.Sender.Consumer', nologo: true, shutDownBuildServers: true
					dotnetPublish project: 'DM/Services/DM.Services.Search.Consumer', noBuild: true, outputDirectory: 'publish/DM.Services.Search.Consumer', nologo: true, shutDownBuildServers: true
					dotnetPublish project: 'DM/Services/DM.Services.Notifications.Consumer', noBuild: true, outputDirectory: 'publish/DM.Services.Notifications.Consumer', nologo: true, shutDownBuildServers: true
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
					fileOperations([
						folderCopyOperation(sourceFolderPath:'DM/Web/DM.Web.Next/dist', destinationFolderPath: 'publish/DM.Web.Next')
					])
				}
			}
		}, failFast: true)
		
		stage('Zip Artifacts') {
			zip zipFile: 'buildresult.zip', dir: 'publish', archive: true
			//archiveArtifacts artifacts: 'buildresult.zip', fingerprint: true
		}
	}
}

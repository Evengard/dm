#!groovy

node {
	stage('Checkout') {
		checkout scm
	}
	docker.image('mcr.microsoft.com/dotnet/sdk:6.0').inside('-v /srv/msbuildlogger:/msbuildlogger -v /srv/tools:/tools') {
		withEnv(['HOME=/tmp/jenkins']) {
			stage('DotNet Build') {
				try {
					dotnetBuild project: 'DM/DM.sln', option: '-logger:/msbuildlogger/MSBuildJenkins.dll', nologo: true
				}
				finally {
					recordIssues tool: issues(pattern: 'issues.json.log'), enabledForFailure: true, qualityGates: [[threshold: 1, type: 'TOTAL_ERROR', unstable: false], [threshold: 1, type: 'NEW_NORMAL', unstable: true]], publishAllIssues: true
				}
			}
			stage('DotNet Test') {
				warnError('Tests failed!') {
					dotnetTest project: 'DM/DM.sln', logger:'trx', resultsDirectory: 'UnitTestResults', noBuild: true, noRestore: true, nologo: true
				}
				sh '/tools/trx2junit UnitTestResults/*.trx'
				recordIssues tool: junitParser(pattern: 'UnitTestResults/*.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
				junit testResults: 'UnitTestResults/*.xml', allowEmptyResults: true
			}
		}
	}
}

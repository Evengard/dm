#!groovy

podTemplate(containers: [
	containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/sdk:6.0', command: 'sleep', args: 'infinity', envVars: [containerEnvVar(key: 'MSBUILDDISABLENODEREUSE', value: '1')]),
	containerTemplate(name: 'nodejs', image: 'node:current', command: 'sleep', args: 'infinity', envVars: [containerEnvVar(key: 'NODE_OPTIONS', value: '--openssl-legacy-provider')]),
],
volumes: [
	hostPathVolume(mountPath: '/srv', hostPath: '/srv'),
]) {
	node(POD_LABEL) {
		stage('Checkout') {
			checkout scm
		}
		parallel(dotnet: {
			container('dotnet') {
				stage('DotNet Build') {
					try {
						dotnetBuild project: 'DM/DM.sln', option: '-logger:/srv/msbuildlogger/MSBuildJenkins.dll', properties: ['UseRazorBuildServer': 'false', 'UseSharedCompilation': 'false'], nologo: true
					}
					finally {
						recordIssues tool: issues(pattern: 'issues.json.log'), enabledForFailure: true, qualityGates: [[threshold: 1, type: 'TOTAL_ERROR', unstable: false], [threshold: 1, type: 'NEW_NORMAL', unstable: true]], publishAllIssues: true
					}
				}
				stage('DotNet Test') {
					warnError('Tests failed!') {
						dotnetTest project: 'DM/DM.sln', logger:'trx', resultsDirectory: 'UnitTestResults', properties: ['UseRazorBuildServer': 'false', 'UseSharedCompilation': 'false'], noBuild: true, noRestore: true, nologo: true, verbosity: 'quiet'
					}
					sh '/srv/tools/trx2junit UnitTestResults/*.trx'
					recordIssues tool: junitParser(pattern: 'UnitTestResults/*.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
					junit testResults: 'UnitTestResults/*.xml', allowEmptyResults: true
				}
			}
		}, typescript: {
			container('nodejs') {
				stage('VueJs Build') {
					//try {
						dir("DM/Web/DM.Web.Modern") {
							sh 'yarn install --network-timeout 300000'
							sh 'yarn build'
						}
					//}
					//finally {
						//recordIssues tool: esLint(), enabledForFailure: true, qualityGates: [[threshold: 1, type: 'TOTAL_ERROR', unstable: false], [threshold: 1, type: 'NEW_NORMAL', unstable: true]], publishAllIssues: true
					//}
				}
			}
		})
	}
}

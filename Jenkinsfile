#!groovy

podTemplate(containers: [
	containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/sdk:6.0', alwaysPullImage: true, command: 'sleep', args: 'infinity', envVars: [containerEnvVar(key: 'MSBUILDDISABLENODEREUSE', value: '1')]),
	containerTemplate(name: 'nodejs', image: 'node:current', alwaysPullImage: true, command: 'sleep', args: 'infinity', envVars: [containerEnvVar(key: 'NODE_OPTIONS', value: '--openssl-legacy-provider')]),
]) {
	node(POD_LABEL) {
		stage('Checkout') {
			checkout scm
		}
		parallel(dotnet: {
			container('dotnet') {
				stage('DotNet Build') {
					try {
						dotnetBuild project: 'DM/DM.sln', option: '-logger:/srv/msbuildlogger/MSBuildJenkins.dll', properties: ['UseRazorBuildServer': 'false', 'UseSharedCompilation': 'false'], nologo: true, shutDownBuildServers: true
					}
					finally {
						recordIssues tool: issues(pattern: 'issues.json.log'), enabledForFailure: true, qualityGates: [[threshold: 1, type: 'TOTAL_ERROR', unstable: false], [threshold: 1, type: 'NEW_NORMAL', unstable: true]], publishAllIssues: true
					}
				}
				stage('DotNet Test') {
					warnError('Tests failed!') {
						dotnetTest project: 'DM/DM.sln', logger:'trx', resultsDirectory: 'UnitTestResults', properties: ['UseRazorBuildServer': 'false', 'UseSharedCompilation': 'false', 'ParallelizeTestCollections': 'false', 'ParallelizeAssembly': 'false', 'MaxParallelThreads': '1'], noBuild: true, noRestore: true, nologo: true, verbosity: 'quiet', shutDownBuildServers: true
					}
					sh '/srv/tools/trx2junit UnitTestResults/*.trx'
					recordIssues tool: junitParser(pattern: 'UnitTestResults/*.xml'), qualityGates: [[threshold: 1, type: 'TOTAL', unstable: true]], publishAllIssues: true
					junit testResults: 'UnitTestResults/*.xml', allowEmptyResults: true
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
			}
		})
	}
}

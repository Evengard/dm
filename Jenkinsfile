#!groovy

pipeline {
	agent none
	stages {
		stage('DotNet Build') {
			agent {
				docker {
					image 'mcr.microsoft.com/dotnet/sdk:6.0'
					args '-v /srv/msbuildlogger:/msbuildlogger'
				}
			}
			environment {
				HOME = "/tmp/jenkins"
			}
			steps {
				dotnetBuild project: 'DM/DM.sln', option: '-logger:/msbuildlogger/MSBuildJenkins.dll'
				recordIssues tool: issues(pattern: 'issues.json.log')
			}
		}
	}
}

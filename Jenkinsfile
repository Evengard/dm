#!groovy

pipeline {
	agent none
	stages {
		stage('DotNet Build') {
			agent {
				docker {
					image 'mcr.microsoft.com/dotnet/sdk:6.0'
				}
			}
			environment {
				HOME = "/tmp/jenkins"
			}
			steps {
				sh 'dotnet build DM/DM.sln'
			}
		}
	}
}

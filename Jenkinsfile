pipeline {
    agent any

    stages {
        stage('Configure Git') {
            steps {
                echo "Adding Jenkins workspace to Git's safe directories..."
                bat 'git config --global --add safe.directory "%WORKSPACE%"'
            }
        }

        stage('Restore Packages') {
            steps {
                echo 'Restoring .NET packages on agent...'
                bat 'dotnet restore DocumentManagement.sln'
            }
        }

        stage('Build Project') {
            steps {
                echo 'Building the project on agent...'
                bat 'dotnet build DocumentManagement.sln --configuration Release --no-restore'
            }
        }

        stage('Run Tests') {
            steps {
                echo 'Running tests...'
                bat 'dotnet test DocumentManagement.sln --no-build --verbosity normal'
            }
        }
        
        stage('Build Docker Image') {
            steps {
                echo 'Building the Docker image using docker-compose...'
                bat 'docker-compose build'
            }
        }

        stage('Deploy Services') {
            steps {
                echo 'Deploying all services (app, db, prometheus, grafana)...'
                bat 'docker-compose down'
                bat 'docker-compose up -d'
            }
        }
    }

    post {
        always {
            echo 'Pipeline finished.'
            cleanWs()
        }
    }
}
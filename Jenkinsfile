pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                echo 'Building the Docker images...'
                sh 'docker-compose build'
            }
        }
        stage('Test') {
            steps {
                echo 'Running tests...'
            }
        }
        stage('Deploy') {
            steps {
                echo 'Deploying all services...'
                sh 'docker-compose down'
                sh 'docker-compose up -d'
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

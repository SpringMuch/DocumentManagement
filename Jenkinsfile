pipeline {
    agent any

    stages {
        stage('Build Docker Image') {
            steps {
                echo 'Building all services defined in docker-compose...'
                sh 'docker-compose build'
            }
        }
        stage('Deploy Services') {
            steps {
                echo 'Starting all services...'
                sh 'docker-compose down'
                sh 'docker-compose up -d'
            }
        }
    }
}

pipeline {
    agent any


    options {
        skipDefaultCheckout()
    }

    stages {
        // Stage 1: Checkout code một cách thủ công
        stage('Checkout') {
            steps {
                echo 'Cleaning workspace and checking out code manually...'
                // Xóa sạch thư mục làm việc cũ để đảm bảo không còn file rác
                cleanWs() 
                
                // Chạy lệnh git clone thủ công
                git branch: 'master', url: 'https://github.com/SpringMuch/DocumentManagement.git'
            }
        }

        // Các stage tiếp theo sẽ chạy như bình thường
        stage('Build & Deploy') {
            steps {
                echo 'Building and deploying services...'
                sh 'docker-compose down'
                sh 'docker-compose build'
                sh 'docker-compose up -d'
            }
        }
    }
    
    post {
        always {
            echo 'Pipeline finished.'
        }
    }
}
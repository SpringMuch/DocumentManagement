pipeline {
    // Agent này cần có cả .NET SDK (để chạy dotnet build/test) 
    // và Docker/Docker-Compose (để build và deploy).
    // Container Jenkins chúng ta đã cấu hình đáp ứng được điều này.
    agent any

    stages {
        // === Giai đoạn Build & Test theo cách của thầy ===

        stage('Restore Packages') {
            steps {
                echo 'Restoring .NET packages on agent...'
                // Sử dụng 'bat' vì Jenkins đang chạy trên Windows
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

        // === Giai đoạn Đóng gói và Triển khai theo cách của bạn ===
        
        stage('Build Docker Image') {
            steps {
                echo 'Building the Docker image using docker-compose...'
                // Docker sẽ tận dụng cache từ các bước build trước nên sẽ rất nhanh
                bat 'docker-compose build'
            }
        }

        stage('Deploy Services') {
            steps {
                echo 'Deploying all services (app, db, prometheus, grafana)...'
                // Dừng các container cũ và khởi động lại với image mới
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
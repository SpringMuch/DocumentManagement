# Giai đoạn 1: Build ứng dụng
# Sử dụng image .NET SDK để có đầy đủ công cụ build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép file .sln và các file .csproj để tận dụng cache của Docker
# Giúp quá trình restore package nhanh hơn ở những lần build sau
COPY ["DocumentManagement.sln", "."]
COPY ["DocumentManagement/DocumentManagement.csproj", "DocumentManagement/"]
RUN dotnet restore "DocumentManagement/DocumentManagement.csproj"

# Sao chép toàn bộ mã nguồn còn lại
COPY . .
WORKDIR "/src/DocumentManagement"
# Build và publish ứng dụng ở chế độ Release
RUN dotnet publish "DocumentManagement.csproj" -c Release -o /app/publish

# Giai đoạn 2: Tạo image để chạy ứng dụng
# Sử dụng image ASP.NET Runtime gọn nhẹ hơn
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Sao chép kết quả đã publish từ giai đoạn build
COPY --from=build /app/publish .

# Định nghĩa lệnh sẽ được chạy khi container khởi động
ENTRYPOINT ["dotnet", "DocumentManagement.dll"]
# Nama Proyek
FileUploadSvc

Project test dotnet dengan case API upload file.
## Requirement
- .net core 8
- Mysql database, bisa juga sqlserver dengan menambahkan library koneksinya

## List API
[AUTH]
POST
/api/Auth/register

POST
/api/Auth/login



[FILE]
POST
/api/File/upload

POST
/api/File/upload-chunk

GET
/api/File/list

GET
/api/File/download/{id}


Detailnya bisa diligat di swaggerUI

http://localhost:5052/swagger/index.html

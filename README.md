

Tapi untuk kasus di atas, chache tidak terlalu berfungsi, karena key datanya selalu unik dari increment number. 


8. Project Code sebagaimana dibawah ini.


# Nama Proyek
FileUploadSvc

Project test dotnet dengan case API upload file.
## Requirement
- .net core 8
- Mysql database, bisa juga sqlserver dengan menambahkan library koneksinya

## List API
[AUTH]
```
POST
/api/Auth/register

POST
/api/Auth/login
```


[FILE]
```
POST
/api/File/upload (untuk upload file small dan normal size)

POST
/api/File/upload-chunk (untuk upload file big size)
```
disini ada return value untuk menunjukkan file sukses di upload baik untuk kondisi partial chunk dan kondisi file complete.

```
GET
/api/File/list

GET
/api/File/download/{id}
```

Detailnya bisa dilihat di swaggerUI

http://localhost:5052/swagger/index.html

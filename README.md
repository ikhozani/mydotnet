#Jawaban Pertanyaan


1. Saya kira code tersebut dapat dipersingkat sebagai berikut agar lebih mudah dibaca

if (application != null && application.protected != null) 
{

	return application.protected.shieldLastRun;
 
}

2. Untuk melengkapi code tersebut, kita perlu membuat class atau struct sebagai return value dari GetInfo() tersebut

public class ApplicationInfo
{

    public string Path { get; set; }
    public string Name { get; set; }

}

public ApplicationInfo GetInfo()
{
    return new ApplicationInfo
    {
        Path = "C:/apps/",
        Name = "Shield.exe"
    };
}




3. Untuk memordifikasi dengan private members, class bisa diubah menjadi:

public class Laptop
{
    private string _os; // Private field

    public string Os
    {
        get { return _os; }
        private set { _os = value; } // Private setter to restrict modification
    }

    public Laptop(string os)
    {
        Os = os;
    }

    // Optionally, you can provide methods to change the OS if needed
    public void UpdateOs(string newOs)
    {
        Os = newOs;
    }
}





4. Kode yang diberikan berpotensi menyebabkan memory leak karena objek myList terus berkembang tanpa batas dalam loop tak berujung. Ini dapat menghabiskan memori sistem seiring waktu karena List<Product> terus menyimpan referensi ke setiap instance Product yang dibuat. Solusinya dapat dilakukan dengan membersihkan list secara berkala atau dengan menghentikan loop while(true).


5. Kode yang diberikan berpotensi menyebabkan memory leak akibat penanganan event. Jika subscriber event tidak berhenti berlangganan dari suatu event dengan benar, maka publisher event akan tetap menyimpan referensi ke subscriber tersebut, sehingga mencegah garbage collector untuk mengambil kembali memori yang digunakan oleh subscriber, meskipun subscriber tersebut sudah tidak berada dalam lingkup (scope)


6. Code program tersebut bisa menyebabkan memory leak dan alokasi memory yang besar hingga insufficient memory pada system. Hal ini disebabkan adanya looping forever dimana setip loop mengalokasinya anak node yang di keep di root not. dan setial child node mengalokasikan 1000 cucu node dan selalu di keep refrenence nya.Jadi memory akan banyak teralokasi sementara garbage collector sulit bekerja karena reference dari anak dan cucu node masih di keep terus.


7. Chache tidak terlalu berfungsi di sini, karena key datanya selalu unik. Jadi memang sulit untuk di cache.


8. Project Code sebagaimana dibawah ini.


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
/api/File/upload (untuk upload file small dan normal size)

POST
/api/File/upload-chunk (untuk upload file big size)

GET
/api/File/list

GET
/api/File/download/{id}


Detailnya bisa diligat di swaggerUI

http://localhost:5052/swagger/index.html

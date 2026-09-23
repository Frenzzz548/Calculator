## Identitas

| Keterangan | Data |
|---|---|
| **Nama** | Bagus Cahya Saputra |
| **NRP** | 5025241067 |
| **Kelas** | PBKK-C |

# Calculator App

Calculator App adalah aplikasi kalkulator desktop sederhana yang dibuat menggunakan C# dan Windows Forms. Aplikasi ini menyediakan operasi aritmatika dasar dan beberapa fungsi matematika tambahan dalam satu tampilan.

## Fitur

- Penjumlahan, pengurangan, perkalian, dan pembagian.
- Operasi berantai, misalnya `10 + 20 + 5`.
- Input angka desimal.
- Mengubah tanda angka menjadi positif atau negatif.
- Persentase.
- Akar kuadrat.
- Kuadrat angka.
- Faktorial untuk bilangan bulat non-negatif.
- Sinus, cosinus, dan tangen menggunakan satuan derajat.
- Tombol backspace untuk menghapus satu digit terakhir.
- Tombol clear untuk mengatur ulang kalkulator.
- Riwayat perhitungan sampai 50 entri.
- Dukungan input dari keyboard.
- Pesan error untuk input atau operasi yang tidak valid.

## Tampilan

Tampilan aplikasi terdiri dari:

- Judul aplikasi.
- Label ekspresi yang sedang dikerjakan.
- Display hasil kalkulator.
- Tombol angka dan operasi.
- Tombol fungsi matematika.
- Daftar riwayat perhitungan.

Display dibuat read-only. Input dilakukan melalui tombol aplikasi atau keyboard.

## Operasi yang Tersedia

### Operasi dasar

Operator yang tersedia adalah:

- `+` untuk penjumlahan.
- `-` untuk pengurangan.
- `*` atau tombol perkalian untuk perkalian.
- `/` atau tombol pembagian untuk pembagian.
- `=` atau Enter untuk menghitung hasil.

Jika operator ditekan dua kali berturut-turut, operator sebelumnya akan diganti dengan operator baru. Operasi dasar juga dapat dilanjutkan setelah hasil sebelumnya ditampilkan.

### Persentase

Perilaku tombol persen bergantung pada kondisi kalkulator:

- Jika tidak ada operator yang sedang aktif, angka dibagi 100. Contoh: `150%` menjadi `1.5`.
- Jika sedang berada dalam operasi, persentase dihitung dari angka pertama. Contoh: `200 + 50%` diperlakukan sebagai `200 + 100`.

### Fungsi matematika

- Akar kuadrat hanya menerima angka yang tidak negatif.
- Kuadrat menghitung nilai angka dikalikan dengan dirinya sendiri.
- Faktorial hanya menerima bilangan bulat non-negatif.
- Sinus, cosinus, dan tangen menerima input dalam derajat, bukan radian.

## Shortcut Keyboard

| Tombol | Fungsi |
| --- | --- |
| `0` sampai `9` | Memasukkan angka |
| `+` | Penjumlahan |
| `-` | Pengurangan |
| `*` | Perkalian |
| `/` | Pembagian |
| `.` atau `,` | Angka desimal |
| `Enter` atau `=` | Menghitung hasil |
| `Backspace` | Menghapus satu digit |
| `Escape` | Clear atau reset kalkulator |

## Penanganan Error

Aplikasi menampilkan dialog error apabila terjadi kondisi seperti:

- Pembagian dengan angka nol.
- Hasil terlalu besar untuk ditampilkan.
- Akar kuadrat dari angka negatif.
- Faktorial dari angka negatif atau angka desimal.
- Kegagalan saat menghitung faktorial.

## Struktur Kode

```text
CalculatorApp/
├── CalculatorApp.slnx
├── CalculatorApp/
│   ├── CalculatorApp.csproj
│   ├── CalculatorService.cs
│   ├── Form1.cs
│   ├── Form1.Designer.cs
│   ├── Form1.resx
│   └── Program.cs
└── README.md
```

### `CalculatorService.cs`

Berisi logika utama kalkulator, termasuk state angka, operator, ekspresi, hasil, riwayat, dan validasi. Service ini menggunakan event untuk memberi tahu form ketika display, ekspresi, riwayat, atau pesan error berubah.

### `Form1.cs`

Mengatur interaksi Windows Forms. File ini menerima klik tombol, menghubungkan event dari `CalculatorService` ke tampilan, serta menangani input keyboard.

### `Form1.Designer.cs`

Berisi kode hasil Windows Forms Designer untuk membuat kontrol dan layout form.

### `Program.cs`

Menjadi entry point aplikasi Windows Forms.

## Teknologi

- C#
- .NET 10
- Windows Forms
- Target framework: `net10.0-windows`

## Menjalankan Aplikasi

### Persyaratan

- Windows.
- .NET 10 SDK.

### Melalui terminal

Masuk ke folder project, lalu jalankan:

```powershell
dotnet run --project .\CalculatorApp\CalculatorApp.csproj
```

### Build project

Untuk melakukan build tanpa menjalankan aplikasi:

```powershell
dotnet build .\CalculatorApp\CalculatorApp\CalculatorApp.csproj
```

Atau buka file `CalculatorApp.slnx` menggunakan Visual Studio atau Visual Studio Code dengan ekstensi C# yang sesuai, kemudian jalankan project Windows Forms.

## Catatan Implementasi

- Angka diproses menggunakan tipe `double`.
- Panjang input angka dibatasi sampai 16 digit.
- Hasil ditampilkan menggunakan format angka umum dengan budaya invariant.
- Riwayat baru ditambahkan di bagian paling atas daftar.
- Ketika jumlah riwayat lebih dari 50, entri paling lama akan dihapus.
- Ekspresi yang terlalu panjang pada label akan dipotong dan hanya bagian akhirnya yang ditampilkan.

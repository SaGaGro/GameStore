# 🎮 GameStore

ระบบขายเกม Digital ออนไลน์ ที่เชื่อม **Publisher** กับ **Customer** โดยมี **Admin** คอยดูแลระบบ
พัฒนาด้วย ASP.NET Core MVC + MySQL

## สารบัญ

- [ฟีเจอร์หลัก](#ฟีเจอร์หลัก)
- [Roles และสิทธิ์](#roles-และสิทธิ์)
- [ระบบโปรโมชั่น](#ระบบโปรโมชั่น)
- [Tech Stack](#tech-stack)
- [โครงสร้างโปรเจกต์](#โครงสร้างโปรเจกต์)
- [เริ่มต้นใช้งาน](#เริ่มต้นใช้งาน)
- [บัญชีทดสอบ](#บัญชีทดสอบ)
- [เอกสารเพิ่มเติม](#เอกสารเพิ่มเติม)
- [แนวทางการทำงานร่วมกันในทีม](#แนวทางการทำงานร่วมกันในทีม)
- [ข้อจำกัดที่ทราบ](#ข้อจำกัดที่ทราบ)

## ฟีเจอร์หลัก

| Role | ความสามารถ |
|------|-----------|
| Customer | ค้นหา/กรองเกม (ชื่อ, หมวดหมู่, ราคา, คะแนน), ซื้อเกม, ใช้แต้มลดราคาตอน Checkout, Wishlist (เพิ่มทั้งหมดลงตะกร้าได้), รีวิวเกมที่ซื้อแล้ว (1 ครั้ง/เกม), ดูประวัติแต้ม, ยื่นขอเป็น Publisher |
| Publisher | เพิ่มเกมพร้อมอัปโหลดรูปปก (รอ Admin อนุมัติก่อนเผยแพร่), แก้ไข/ลบเกมของตัวเอง, สร้างโปรโมชั่น, ดูรายได้ ยอดขาย คะแนนเฉลี่ย และรีวิว |
| Admin | อนุมัติ/ปฏิเสธเกมที่ Publisher ส่งมา, จัดการ Campaign ระดับแพลตฟอร์ม, อนุมัติ Publisher Request |
| SuperAdmin | ทำได้ทุกอย่างที่ Admin ทำได้ + จัดการ User ทุกคน, ระงับบัญชี, เปลี่ยน Role |

## Roles และสิทธิ์

| RoleId | Role | หน้าที่ |
|:------:|------|--------|
| 1 | SuperAdmin | จัดการทุกอย่างในระบบ |
| 2 | Admin | ดูแลระบบเบื้องต้น อนุมัติเกม ดูรายงาน จัดการโปรโมชั่น |
| 3 | Publisher | เจ้าของเกม/ค่ายเกม |
| 4 | Customer | ผู้ใช้ทั่วไป |

## ระบบโปรโมชั่น

| ประเภท | รายละเอียด |
|--------|-----------|
| FestivalSale | Publisher ตั้งส่วนลด % และช่วงเวลา ราคาลดอัตโนมัติในช่วงนั้น |
| FreeGame | แจกฟรีตามช่วงเวลา ผู้ใช้กด Claim เพื่อเพิ่มเกมเข้า Library |
| Early Bird | Pre-order เกมที่ยังไม่วางจำหน่าย (ReleaseDate ในอนาคต) และมี DiscountPercent จะแสดงราคาพิเศษ |
| Points | ซื้อทุก ฿10 ได้ 1 แต้ม, 1 แต้ม = ฿0.50 ใช้หักราคาตอน Checkout |
| SeriesDiscount | มีเกมใน Series เดียวกันอยู่ใน Library แล้ว ได้ส่วนลด 30% สำหรับภาคอื่น |
| Campaign | Admin สร้าง Campaign (เช่น Summer Sale, Songkran Festival) เลือกเกมที่ร่วมรายการ ส่วนลดใช้อัตโนมัติตามช่วงเวลา |

## Tech Stack

- **Backend:** ASP.NET Core MVC (C#, .NET 10), แยกเป็น Areas ตาม Role (`Admin`, `Publisher`, `Customer`)
- **Database:** MySQL + EF Core 9 (Pomelo provider)
- **Auth:** Session-based (เก็บ `UserId`, `RoleId`, `DisplayName`)
- **Frontend:** Razor Views + Bootstrap
- **File storage:** รูปปกเกมเก็บที่ `GameStore/wwwroot/uploads/games/`

## โครงสร้างโปรเจกต์

```
GameStore/                     <- repo root
├── DB.sql                     # Schema + seed data ของฐานข้อมูล MySQL
├── Requirement.txt            # รายละเอียด requirement ของระบบ
├── GameStore_DFD.drawio       # Data Flow Diagram
├── GameStore_Documentation.docx
└── GameStore/                 # ASP.NET Core project
    ├── Areas/
    │   ├── Admin/             # Admin + SuperAdmin
    │   ├── Customer/
    │   └── Publisher/
    ├── Controllers/           # Account (login/register), Home
    ├── Models/DB/             # EF Core entities + DbContext (Csi402dbContext)
    ├── ViewModels/
    ├── Views/                 # Shared views
    ├── wwwroot/               # Static files + uploads
    ├── appsettings.json       # Config (ไม่มีรหัสผ่านจริง)
    └── Program.cs
```

## เริ่มต้นใช้งาน

### สิ่งที่ต้องมี

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- MySQL Server 8+ (โปรเจกต์ถูกพัฒนาด้วย MySQL 9.x)
- Git

### 1. Clone

```bash
git clone <repository-url>
cd GameStore
```

### 2. สร้างฐานข้อมูล

```sql
CREATE DATABASE csi402db CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE csi402db;
SOURCE DB.sql;
```

หรือใช้ MySQL Workbench เปิดไฟล์ `DB.sql` แล้วรันบนฐานข้อมูล `csi402db`

### 3. ตั้งค่า Connection String (สำคัญ)

`appsettings.json` ใส่ค่า placeholder (`CHANGE_ME`) ไว้เท่านั้น **ห้าม commit รหัสผ่านจริง**
ให้เก็บค่าจริงของแต่ละคนด้วย [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) ซึ่งเก็บนอกโฟลเดอร์โปรเจกต์:

```bash
cd GameStore
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=csi402db;user=root;password=<รหัสผ่าน MySQL ของคุณ>;SslMode=None;AllowPublicKeyRetrieval=True;"
```

> User Secrets ใช้ได้เมื่อรันใน Development environment (ค่าเริ่มต้นของ `dotnet run`)
> บน Production ให้ใช้ Environment Variable `ConnectionStrings__DefaultConnection` แทน

### 4. รันโปรเจกต์

```bash
cd GameStore
dotnet run --launch-profile https
```

เปิดเบราว์เซอร์ที่ https://localhost:7214 (หรือ http://localhost:5126)

## บัญชีทดสอบ

Seed data ใน `DB.sql` สร้างบัญชีต่อไปนี้ (ทุกบัญชีใช้รหัสผ่าน `password123`) **สำหรับ Development เท่านั้น**

| Username | Role |
|----------|------|
| `superadmin` | SuperAdmin |
| `admin01` | Admin |
| `pixelstudio`, `stormgames` | Publisher |
| `gamer_th99`, `proplayer_th`, `casualgamer`, `nightowl`, `dragonslayer` | Customer |

## เอกสารเพิ่มเติม

- [Requirement.txt](Requirement.txt) — ข้อกำหนดระบบโดยละเอียด
- [GameStore_Documentation.docx](GameStore_Documentation.docx) — เอกสารประกอบโปรเจกต์
- [GameStore_DFD.drawio](GameStore_DFD.drawio) — Data Flow Diagram (เปิดด้วย [diagrams.net](https://app.diagrams.net) หรือ VS Code extension Draw.io)

## แนวทางการทำงานร่วมกันในทีม

1. **อย่า push ตรงเข้า `main`** — แตก branch ใหม่จาก `main` ทุกครั้ง
   ตั้งชื่อ `feature/<ชื่อฟีเจอร์>`, `fix/<ชื่อบั๊ก>`, `docs/<หัวข้อ>`
2. Commit message สั้น ชัดเจน ขึ้นต้นด้วยประเภท เช่น `feat: add wishlist add-all`, `fix: points calculation at checkout`
3. เปิด **Pull Request** เข้า `main` ให้เพื่อนอย่างน้อย 1 คน review ก่อน merge
4. ก่อนเปิด PR ให้ตรวจว่า `dotnet build` ผ่าน
5. **ห้าม commit** รหัสผ่าน, connection string จริง, ไฟล์ใน `bin/` `obj/` หรือรูปที่ผู้ใช้อัปโหลด (มี `.gitignore` ดูแลให้แล้ว)
6. ถ้าแก้ schema ให้อัปเดต `DB.sql` และ `Models/DB/` ใน PR เดียวกัน และแจ้งทีมให้รัน script ใหม่

## ข้อจำกัดที่ทราบ

- รหัสผ่านถูกเก็บและเทียบเป็น **plain text** (`AccountController`) — ต้องเปลี่ยนเป็น hash (เช่น `PasswordHasher<T>` หรือ BCrypt) ก่อนใช้งานจริง
- ยังไม่มี Automated Tests
- มีโค้ดตัวอย่างจากแล็บค้างอยู่ (เช่น `Labstudent`, `Lab8`–`Lab10` views) ที่ควรลบเมื่อไม่ใช้แล้ว
- Controller บางตัวใน Admin (`Banner`, `Category`, `Report`) ยังเป็น stub

## License

ยังไม่ได้กำหนด License — ตกลงกันในทีมแล้วเพิ่มไฟล์ `LICENSE` ตามต้องการ

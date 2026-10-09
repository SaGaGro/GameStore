-- ==========================================
-- 1. สร้างตาราง Master Data / Lookup Tables
-- ==========================================

-- 1.1 ตารางเก็บ Role
CREATE TABLE Roles
(
    RoleId INT AUTO_INCREMENT NOT NULL,
    RoleName NVARCHAR(50) NOT NULL,      
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (RoleId)
);

-- 1.2 ตารางเก็บสถานะของเกม
CREATE TABLE GameStatuses
(
    StatusId INT NOT NULL,               
    StatusName NVARCHAR(50) NOT NULL,    
    Description NVARCHAR(255),           
    
    PRIMARY KEY (StatusId)
);

-- 1.3 ตารางเก็บแฟรนไชส์เกม 
CREATE TABLE GameSeries
(
    SeriesId INT AUTO_INCREMENT NOT NULL,
    SeriesName NVARCHAR(100) NOT NULL,
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (SeriesId)
);

-- ==========================================
-- 2. สร้างตารางหลัก (Users, Games, Campaigns)
-- ==========================================

-- 2.1 ตารางเก็บข้อมูลผู้ใช้งาน
CREATE TABLE Users
(
    UserId NVARCHAR(50) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255),
    Email NVARCHAR(100),
    RoleId INT NOT NULL,
    WalletPoints DECIMAL(18,2) DEFAULT 0,
    DisplayName NVARCHAR(100),
    ProfileImage NVARCHAR(500),
    Phone NVARCHAR(20),
    Country NVARCHAR(50),
    IsActive BOOLEAN DEFAULT TRUE,

    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- 2.2 ตารางเก็บข้อมูลเกม
CREATE TABLE Games
(
    GameId INT AUTO_INCREMENT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description TEXT,
    BasePrice DECIMAL(18,2) NOT NULL,
    CoverImage NVARCHAR(255),
    ReleaseDate DATETIME,
    Genre NVARCHAR(100),
    DiscountPercent DECIMAL(5,2) DEFAULT 0,     -- % ส่วนลดปัจจุบัน (อาจมาจาก Campaign)
    IsFree BOOLEAN DEFAULT FALSE,               -- เกมฟรีถาวร
    HasTrial BOOLEAN DEFAULT FALSE,             -- มีเวอร์ชั่นทดลองเล่น
    TrialHours INT,                             -- จำนวนชั่วโมงที่ทดลองได้
    Rating DECIMAL(3,2) DEFAULT 0,             -- คะแนนเฉลี่ย (คำนวณจาก Reviews)
    ReviewCount INT DEFAULT 0,                  -- จำนวน review ทั้งหมด
    SoldCount INT DEFAULT 0,                    -- จำนวนที่ขายได้
    IsApproved BOOLEAN DEFAULT FALSE,           -- Admin อนุมัติแล้ว

    StatusId INT NOT NULL DEFAULT 0,
    SeriesId INT,
    PublisherId NVARCHAR(50),

    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (GameId),
    FOREIGN KEY (StatusId) REFERENCES GameStatuses(StatusId),
    FOREIGN KEY (SeriesId) REFERENCES GameSeries(SeriesId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

-- 2.3 ตารางจัดแคมเปญหลัก 
CREATE TABLE Campaigns
(
    CampaignId INT AUTO_INCREMENT NOT NULL,
    CampaignName NVARCHAR(100) NOT NULL,
    DiscountPercentage DECIMAL(5,2) NOT NULL, 
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,             
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (CampaignId)
);

-- ==========================================
-- 3. สร้างตารางเชื่อมโยง (Mapping / Transactions)
-- ==========================================

-- 3.1 ตารางเชื่อมแคมเปญกับเกม
CREATE TABLE CampaignGames
(
    CampaignGameId INT AUTO_INCREMENT NOT NULL,
    CampaignId INT NOT NULL,
    GameId INT NOT NULL,
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (CampaignGameId),
    FOREIGN KEY (CampaignId) REFERENCES Campaigns(CampaignId),
    FOREIGN KEY (GameId) REFERENCES Games(GameId),
    
    CONSTRAINT UQ_Campaign_Game UNIQUE (CampaignId, GameId) 
);

-- 3.2 ตาราง Wishlist / Pre-register 
CREATE TABLE Wishlists
(
    WishlistId INT AUTO_INCREMENT NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    GameId INT NOT NULL,
    IsPreRegistered BOOLEAN DEFAULT FALSE,      
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (WishlistId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (GameId) REFERENCES Games(GameId),
    
    CONSTRAINT UQ_Wishlist_User_Game UNIQUE (UserId, GameId) 
);

-- 3.3 ตารางคำสั่งซื้อหลัก
CREATE TABLE Orders
(
    OrderId INT AUTO_INCREMENT NOT NULL,
    CustomerId NVARCHAR(50) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,       
    PointsUsed DECIMAL(18,2) DEFAULT 0,        
    FinalAmount DECIMAL(18,2) NOT NULL,       
    PointsEarned DECIMAL(18,2) DEFAULT 0,      
    OrderStatus NVARCHAR(20) DEFAULT 'Pending', 
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (OrderId),
    FOREIGN KEY (CustomerId) REFERENCES Users(UserId)
);

-- 3.4 ตารางรายละเอียดในคำสั่งซื้อ 
CREATE TABLE OrderItems
(
    OrderItemId INT AUTO_INCREMENT NOT NULL,
    OrderId INT NOT NULL,
    GameId INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,         
    DiscountAmount DECIMAL(18,2) DEFAULT 0,    
    NetPrice DECIMAL(18,2) NOT NULL,          
    DiscountNote NVARCHAR(100),               
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (OrderItemId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (GameId) REFERENCES Games(GameId)
);

-- 3.5 ตารางคลังเกมของลูกค้า
CREATE TABLE UserLibraries
(
    LibraryId INT AUTO_INCREMENT NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    GameId INT NOT NULL,
    
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (LibraryId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (GameId) REFERENCES Games(GameId),
    
    CONSTRAINT UQ_UserLibrary_User_Game UNIQUE (UserId, GameId) 
);

-- 3.6 ตารางรีวิวเกม
CREATE TABLE Reviews
(
    ReviewId INT AUTO_INCREMENT NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    GameId INT NOT NULL,
    Rating INT NOT NULL,                        -- คะแนน 1-5
    Comment TEXT,

    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (ReviewId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (GameId) REFERENCES Games(GameId),

    CONSTRAINT UQ_Review_User_Game UNIQUE (UserId, GameId) -- 1 user รีวิวได้ 1 ครั้งต่อเกม
);

-- 3.7 ตารางโปรโมชั่นของ Publisher
CREATE TABLE Promotions
(
    PromotionId INT AUTO_INCREMENT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description TEXT,
    Type NVARCHAR(50) NOT NULL,                 -- 'FestivalSale', 'FreeGame', 'TrialDiscount', 'SeriesDiscount'
    DiscountPercent DECIMAL(5,2) NOT NULL,
    GameId INT NOT NULL,
    PublisherId NVARCHAR(50),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,

    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (PromotionId),
    FOREIGN KEY (GameId) REFERENCES Games(GameId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

-- 3.8 ตารางคำขอสมัคร Publisher
CREATE TABLE PublisherRequests
(
    RequestId INT AUTO_INCREMENT NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    CompanyName NVARCHAR(200) NOT NULL,
    Description TEXT NOT NULL,
    ContactInfo NVARCHAR(500) NOT NULL,
    LogoImage NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Pending',      -- 'Pending', 'Approved', 'Rejected'
    RejectReason NVARCHAR(500),
    ReviewedBy NVARCHAR(50),
    ReviewedAt DATETIME,

    RequestedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (RequestId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ReviewedBy) REFERENCES Users(UserId)
);

-- 3.9 ตารางประวัติแต้ม
CREATE TABLE PointHistories
(
    PointHistoryId INT AUTO_INCREMENT NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    Type NVARCHAR(20) NOT NULL,                 -- 'Earned', 'Used', 'Expired'
    Points INT NOT NULL,                        -- บวก = ได้รับ, ลบ = ใช้/หมดอายุ
    Description NVARCHAR(500),
    ExpiresAt DATETIME,

    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (PointHistoryId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- ==========================================
-- 4. INSERT ข้อมูลเริ่มต้น (Master Data)
-- ==========================================

INSERT INTO Roles (RoleName) 
VALUES 
('SuperAdmin'), 
('Admin'), 
('Publisher'), 
('Customer');

INSERT INTO GameStatuses (StatusId, StatusName, Description)
VALUES
(0, 'Coming Soon', 'เกมยังไม่วางจำหน่าย (รองรับการกด Pre-register)'),
(1, 'Available', 'เกมวางจำหน่ายปกติ'),
(2, 'Free Claim', 'เปิดให้กดรับฟรีตามช่วงเวลาโปรโมชั่น (Promo 2)');

-- ==========================================
-- 5. SEED DATA
-- ==========================================

-- 5.1 GameSeries
INSERT INTO GameSeries (SeriesName) VALUES
('Cyber Warriors'),
('Dragon Quest'),
('Indie Originals');

-- 5.2 Users
-- Password: 'password123' (plain text สำหรับ dev — ในระบบจริงต้อง hash)
INSERT INTO Users (UserId, Username, PasswordHash, Email, RoleId, WalletPoints, DisplayName, Phone, Country, IsActive) VALUES
('u-superadmin-001', 'superadmin',   'password123', 'superadmin@gamestore.com',  1, 0.00,    'Super Admin',     '099-000-0001', 'TH', TRUE),
('u-admin-001',      'admin01',      'password123', 'admin01@gamestore.com',      2, 0.00,    'Admin One',       '099-000-0002', 'TH', TRUE),
('u-pub-001',        'pixelstudio',  'password123', 'contact@pixelstudio.com',    3, 0.00,    'Pixel Studio',    '081-234-5678', 'TH', TRUE),
('u-pub-002',        'stormgames',   'password123', 'hello@stormgames.com',        3, 0.00,    'Storm Games',     '082-345-6789', 'TH', TRUE),
('u-cust-001',       'gamer_th99',   'password123', 'gamer99@email.com',           4, 95.00,   'GamerTH_99',      '089-123-4567', 'TH', TRUE),
('u-cust-002',       'proplayer_th', 'password123', 'proplayer@email.com',         4, 200.00,  'ProPlayer_TH',    '088-234-5678', 'TH', TRUE),
('u-cust-003',       'casualgamer',  'password123', 'casual@email.com',            4, 30.00,   'CasualGamer',     '087-345-6789', 'TH', TRUE),
('u-cust-004',       'nightowl',     'password123', 'nightowl@email.com',          4, 0.00,    'NightOwl',        '086-456-7890', 'TH', TRUE),
('u-cust-005',       'dragonslayer', 'password123', 'dragon@email.com',            4, 150.00,  'DragonSlayer',    '085-567-8901', 'TH', TRUE);

-- 5.3 Games
INSERT INTO Games (Title, Description, BasePrice, CoverImage, ReleaseDate, Genre, DiscountPercent, IsFree, HasTrial, TrialHours, Rating, ReviewCount, SoldCount, IsApproved, StatusId, SeriesId, PublisherId) VALUES
('Cyber Warriors 2077',  'เกม Action RPG สุดมันส์ในโลกอนาคต ต่อสู้กับ AI และองค์กรมืดในเมืองไซเบอร์พังค์',            1790.00, 'https://placehold.co/400x250/0f3460/e94560?text=Cyber+Warriors',  '2024-03-15 00:00:00', 'Action RPG',  20.00, FALSE, FALSE, NULL, 4.50, 3, 15420, TRUE,  1, 1, 'u-pub-001'),
('Shadow Realms',        'ผจญภัยในโลกแฟนตาซีมืด ค้นหาความจริงที่ซ่อนอยู่ในอาณาจักรเงา',                               1290.00, 'https://placehold.co/400x250/0f3460/e94560?text=Shadow+Realms',   '2024-06-01 00:00:00', 'Adventure',   0.00,  FALSE, FALSE, NULL, 4.20, 0,  8730, TRUE,  1, NULL, 'u-pub-001'),
('Speed Racer X',        'เกมแข่งรถความเร็วสูง ลู่วิ่งสุดอันตรายทั่วโลก',                                              890.00,  'https://placehold.co/400x250/0f3460/e94560?text=Speed+Racer',     '2024-01-20 00:00:00', 'Racing',      0.00,  FALSE, FALSE, NULL, 3.90, 0,  5210, TRUE,  1, NULL, 'u-pub-002'),
('Galaxy Defense',       'เกม Strategy วางแผนป้องกันกาแล็กซี่จากการรุกรานของมนุษย์ต่างดาว รับฟรีได้เลย!',              0.00,    'https://placehold.co/400x250/0f3460/27ae60?text=Galaxy+Defense',   '2023-11-10 00:00:00', 'Strategy',    0.00,  TRUE,  FALSE, NULL, 4.00, 0, 32100, TRUE,  2, NULL, 'u-pub-002'),
('Dragon Quest Online',  'MMORPG สุดอลังการ ผจญภัยกับผู้เล่นทั่วโลกในโลกแห่งมังกร',                                   1590.00, 'https://placehold.co/400x250/0f3460/e94560?text=Dragon+Quest',    '2023-09-05 00:00:00', 'MMORPG',      0.00,  FALSE, TRUE,  5,    4.70, 2, 22340, TRUE,  1, 2, 'u-pub-001'),
('Pixel Dungeon',        'Roguelike สุดท้าทาย สำรวจดันเจี้ยนสุ่มที่ไม่มีวันสิ้นสุด',                                   390.00,  'https://placehold.co/400x250/0f3460/e94560?text=Pixel+Dungeon',   '2024-08-22 00:00:00', 'Roguelike',   50.00, FALSE, FALSE, NULL, 4.10, 1, 11200, TRUE,  1, 3, 'u-pub-001'),
('Battle Arena',         'MOBA ฟรี! รวมทีม 5v5 ต่อสู้เพื่อความเป็นใหญ่ในสนามรบ',                                      0.00,    'https://placehold.co/400x250/0f3460/27ae60?text=Battle+Arena',    '2023-07-01 00:00:00', 'MOBA',        0.00,  TRUE,  FALSE, NULL, 4.30, 0, 45600, TRUE,  2, NULL, 'u-pub-002'),
('Farm Life Story',      'เกม Simulation ผ่อนคลาย ปลูกผัก เลี้ยงสัตว์ สร้างฟาร์มในฝัน',                               590.00,  'https://placehold.co/400x250/0f3460/e94560?text=Farm+Life',       '2024-04-10 00:00:00', 'Simulation',  0.00,  FALSE, FALSE, NULL, 4.40, 1,  9870, TRUE,  1, NULL, 'u-pub-002'),
('Zombie Survival',      'เอาชีวิตรอดในโลกที่เต็มไปด้วยซอมบี้ รวบรวมทรัพยากรและสร้างฐานที่มั่น',                     990.00,  'https://placehold.co/400x250/0f3460/e94560?text=Zombie+Survival', '2024-02-14 00:00:00', 'Horror',      30.00, FALSE, FALSE, NULL, 3.80, 0,  6540, TRUE,  1, NULL, 'u-pub-002'),
('Sky Kingdom',          'ผจญภัยในอาณาจักรลอยฟ้า ค้นหาโบราณวัตถุที่สูญหายไปนับพันปี',                                 1490.00, 'https://placehold.co/400x250/0f3460/e94560?text=Sky+Kingdom',     '2024-05-30 00:00:00', 'Adventure',   0.00,  FALSE, FALSE, NULL, 4.60, 0, 18900, TRUE,  1, NULL, 'u-pub-001'),
('Ninja Clash',          'เกม Fighting ระบบต่อสู้ลึก ฝึกฝนท่าไม้ตายและเอาชนะคู่ต่อสู้',                                790.00,  'https://placehold.co/400x250/0f3460/e94560?text=Ninja+Clash',     '2024-07-18 00:00:00', 'Fighting',    0.00,  FALSE, TRUE,  3,    4.00, 0,  7650, TRUE,  1, NULL, 'u-pub-002'),
('Ocean Explorer',       'สำรวจโลกใต้ทะเลลึกที่ไม่มีใครเคยเห็น ค้นพบสิ่งมีชีวิตแปลกประหลาด',                         690.00,  'https://placehold.co/400x250/0f3460/e94560?text=Ocean+Explorer',  '2024-09-01 00:00:00', 'Simulation',  0.00,  FALSE, FALSE, NULL, 4.20, 0,  4320, TRUE,  1, NULL, 'u-pub-001'),
('Mystic Legends',       'RPG สุดมหากาพย์ ตามรอยตำนานโบราณในโลกแฟนตาซีที่กว้างใหญ่',                                  1290.00, 'https://placehold.co/400x250/0f3460/f39c12?text=Mystic+Legends',  '2026-06-01 00:00:00', 'RPG',         0.00,  FALSE, FALSE, NULL, 0.00, 0,     0, FALSE, 0, NULL, 'u-pub-001'),
('Cyber Warriors: Zero', 'ภาคก่อนของ Cyber Warriors 2077 เรื่องราวที่ไม่เคยถูกเล่าถึง',                                990.00,  'https://placehold.co/400x250/0f3460/e94560?text=CW+Zero',         '2026-12-01 00:00:00', 'Action RPG',  0.00,  FALSE, FALSE, NULL, 0.00, 0,     0, FALSE, 0, 1, 'u-pub-001');

-- 5.4 Campaigns
INSERT INTO Campaigns (CampaignName, DiscountPercentage, StartDate, EndDate, IsActive) VALUES
('Summer Sale 2026',  30.00, '2026-03-16 00:00:00', '2026-03-31 23:59:59', TRUE),
('Songkran Festival', 50.00, '2026-04-10 00:00:00', '2026-04-17 23:59:59', TRUE);

-- 5.5 CampaignGames
INSERT INTO CampaignGames (CampaignId, GameId) VALUES
(1, 1), -- Summer Sale → Cyber Warriors 2077
(1, 2), -- Summer Sale → Shadow Realms
(1, 9), -- Summer Sale → Zombie Survival
(2, 5), -- Songkran   → Dragon Quest Online
(2, 6); -- Songkran   → Pixel Dungeon

-- 5.6 Wishlists
INSERT INTO Wishlists (UserId, GameId, IsPreRegistered) VALUES
('u-cust-001', 2,  FALSE), -- gamer_th99    → Shadow Realms
('u-cust-001', 5,  FALSE), -- gamer_th99    → Dragon Quest Online
('u-cust-001', 10, FALSE), -- gamer_th99    → Sky Kingdom
('u-cust-002', 1,  FALSE), -- proplayer_th  → Cyber Warriors 2077
('u-cust-002', 13, TRUE),  -- proplayer_th  → Mystic Legends (Pre-register)
('u-cust-003', 13, TRUE),  -- casualgamer   → Mystic Legends (Pre-register)
('u-cust-005', 14, TRUE);  -- dragonslayer  → Cyber Warriors: Zero (Pre-register)

-- 5.7 Orders
INSERT INTO Orders (CustomerId, TotalAmount, PointsUsed, FinalAmount, PointsEarned, OrderStatus) VALUES
('u-cust-001', 1790.00, 0.00,   1790.00, 90.00, 'Completed'),
('u-cust-001',  590.00, 0.00,    590.00, 30.00, 'Completed'),
('u-cust-001',  890.00, 50.00,   840.00, 42.00, 'Completed'),
('u-cust-002', 1590.00, 0.00,   1590.00, 80.00, 'Completed'),
('u-cust-003', 1290.00, 0.00,   1290.00, 65.00, 'Completed'),
('u-cust-005', 1790.00, 100.00, 1690.00, 85.00, 'Completed');

-- 5.8 OrderItems
INSERT INTO OrderItems (OrderId, GameId, UnitPrice, DiscountAmount, NetPrice, DiscountNote) VALUES
(1, 1,  1790.00, 0.00,   1790.00, NULL),
(2, 8,   590.00, 0.00,    590.00, NULL),
(3, 3,   890.00, 50.00,   840.00, 'ใช้แต้มลดราคา'),
(4, 5,  1590.00, 0.00,   1590.00, NULL),
(5, 2,  1290.00, 0.00,   1290.00, NULL),
(6, 1,  1790.00, 358.00, 1432.00, 'Summer Sale 20%');

-- 5.9 UserLibraries
INSERT INTO UserLibraries (UserId, GameId) VALUES
('u-cust-001', 1),  -- gamer_th99   → Cyber Warriors 2077
('u-cust-001', 4),  -- gamer_th99   → Galaxy Defense (Free Claim)
('u-cust-001', 7),  -- gamer_th99   → Battle Arena (Free)
('u-cust-001', 8),  -- gamer_th99   → Farm Life Story
('u-cust-002', 5),  -- proplayer_th → Dragon Quest Online
('u-cust-002', 4),  -- proplayer_th → Galaxy Defense
('u-cust-002', 7),  -- proplayer_th → Battle Arena
('u-cust-003', 2),  -- casualgamer  → Shadow Realms
('u-cust-005', 1),  -- dragonslayer → Cyber Warriors 2077
('u-cust-005', 5);  -- dragonslayer → Dragon Quest Online

-- 5.10 Reviews
INSERT INTO Reviews (UserId, GameId, Rating, Comment) VALUES
('u-cust-001', 1, 5, 'เกมสนุกมาก กราฟิกสวย ระบบการเล่นลึก เล่นได้เป็นร้อยชั่วโมงไม่เบื่อ แนะนำเลย!'),
('u-cust-002', 5, 4, 'โดยรวมดี แต่บางจุดยังมี bug อยู่ รอ patch อัปเดตอยู่ครับ'),
('u-cust-003', 6, 5, 'คุ้มค่ากับราคามากๆ เนื้อเรื่องดี เพลงประกอบเพราะ ชอบมาก!'),
('u-cust-004', 8, 3, 'เล่นได้เรื่อยๆ แต่ content ยังน้อยไป รอ DLC เพิ่ม'),
('u-cust-005', 5, 5, 'สุดยอดเกม MMORPG ที่ดีที่สุดที่เคยเล่น!');

-- 5.11 Promotions
INSERT INTO Promotions (Title, Description, Type, DiscountPercent, GameId, PublisherId, StartDate, EndDate, IsActive) VALUES
('Summer Sale - Cyber Warriors',  'ลดพิเศษ 20% ฉลองฤดูร้อน',          'FestivalSale',  20.00, 1, 'u-pub-001', '2026-03-16 00:00:00', '2026-03-31 23:59:59', TRUE),
('Pixel Dungeon ลด 50%',          'ลดครึ่งราคา จำกัดเวลา',              'FestivalSale',  50.00, 6, 'u-pub-001', '2026-03-11 00:00:00', '2026-03-24 23:59:59', TRUE),
('Dragon Quest Trial Discount',   'ผู้เล่นที่ทดลองเล่นแล้วลด 15%',     'TrialDiscount', 15.00, 5, 'u-pub-001', '2026-02-19 00:00:00', '2026-03-20 23:59:59', FALSE),
('Galaxy Defense Free Claim',     'รับเกมฟรีตลอดเดือนมีนาคม',           'FreeGame',     100.00, 4, 'u-pub-002', '2026-03-01 00:00:00', '2026-03-31 23:59:59', TRUE),
('Battle Arena Free',             'เกมฟรีถาวร ไม่มีวันหมดอายุ',          'FreeGame',     100.00, 7, 'u-pub-002', '2023-07-01 00:00:00', '2099-12-31 23:59:59', TRUE);

-- 5.12 PublisherRequests
INSERT INTO PublisherRequests (UserId, CompanyName, Description, ContactInfo, Status, ReviewedBy, ReviewedAt) VALUES
('u-cust-001', 'Awesome Studio', 'ค่ายเกม Indie จากประเทศไทย เน้นเกม RPG สำหรับผู้เล่นชาวไทย', 'awesome@studio.com', 'Pending',  NULL,          NULL),
('u-cust-002', 'Speed Games',   'ค่ายเกมแข่งรถ มีประสบการณ์ด้านเกม Racing มากกว่า 5 ปี',       'speed@games.com',   'Pending',  NULL,          NULL),
('u-cust-003', 'Cozy Dev',      'ทีมพัฒนาเกม Simulation และ Casual เน้นกลุ่มผู้เล่นหน้าใหม่',   'cozy@dev.com',      'Approved', 'u-admin-001', '2026-01-15 10:30:00');

-- 5.13 PointHistories
INSERT INTO PointHistories (UserId, Type, Points, Description, ExpiresAt) VALUES
('u-cust-001', 'Earned',  90,  'ซื้อเกม Cyber Warriors 2077 (฿1,790)',    '2026-09-18 23:59:59'),
('u-cust-001', 'Earned',  30,  'ซื้อเกม Farm Life Story (฿590)',           '2026-09-28 23:59:59'),
('u-cust-001', 'Used',   -50,  'ใช้แต้มลดราคาซื้อ Speed Racer X',         NULL),
('u-cust-001', 'Earned',  42,  'ซื้อเกม Speed Racer X (฿890)',             '2026-10-08 23:59:59'),
('u-cust-001', 'Expired', -17, 'แต้มหมดอายุ',                              NULL),
('u-cust-002', 'Earned',  80,  'ซื้อเกม Dragon Quest Online (฿1,590)',     '2026-09-20 23:59:59'),
('u-cust-002', 'Earned',  120, 'โบนัสสมาชิกใหม่',                          '2026-12-31 23:59:59'),
('u-cust-003', 'Earned',  65,  'ซื้อเกม Shadow Realms (฿1,290)',           '2026-09-25 23:59:59'),
('u-cust-003', 'Used',   -35,  'ใช้แต้มลดราคา',                            NULL),
('u-cust-005', 'Earned',  85,  'ซื้อเกม Cyber Warriors 2077 (฿1,690)',    '2026-09-21 23:59:59'),
('u-cust-005', 'Earned',  80,  'ซื้อเกม Dragon Quest Online (฿1,590)',     '2026-09-21 23:59:59'),
('u-cust-005', 'Used',   -15,  'ใช้แต้มลดราคา',                            NULL);
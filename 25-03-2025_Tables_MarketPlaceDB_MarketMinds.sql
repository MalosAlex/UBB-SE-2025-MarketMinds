USE MarketPlace;
GO

CREATE TABLE Users (
    id INT PRIMARY KEY IDENTITY(1,1),
    email NVARCHAR(255) NOT NULL,
    username NVARCHAR(50) NOT NULL,
    userType INT NOT NULL,
	balance FLOAT NOT NULL,
	rating FLOAT NULL,
    passwordHash NVARCHAR(255) NOT NULL,
    CONSTRAINT UQ_Users_Email UNIQUE (email),
    CONSTRAINT UQ_Users_Username UNIQUE (username)
);

CREATE TABLE Reviews (
	id INT PRIMARY KEY IDENTITY(1,1),
	reviewer_id INT NOT NULL,
	seller_id INT NOT NULL,
	description NVARCHAR(500) NULL,
	rating FLOAT NOT NULL,
	CONSTRAINT FK_Reviews_ReviewerUsers FOREIGN KEY (reviewer_id) REFERENCES Users(id),
	CONSTRAINT FK_Reviews_SellerUsers FOREIGN KEY (seller_id) REFERENCES Users(id),
);

CREATE TABLE ReviewsPictures(
	id INT PRIMARY KEY IDENTITY(1,1),
	url NVARCHAR(256) NOT NULL,
	review_id INT NOT NULL,
	CONSTRAINT FK_Image_Reviews FOREIGN KEY (review_id) REFERENCES Reviews(id)
);

CREATE TABLE ProductConditions (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(100) NOT NULL,
    description NVARCHAR(500) NULL,
	CONSTRAINT UQ_ProductConditions_Title UNIQUE (title),
);

CREATE TABLE ProductCategories (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(100) NOT NULL,
    description NVARCHAR(500) NULL,
	CONSTRAINT UQ_ProductCategories_Title UNIQUE (title),
);

CREATE TABLE ProductTags (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(100) NOT NULL,
	CONSTRAINT UQ_ProductTags_Title UNIQUE (title),
);

----- PRODUCTS ------
-- BUY PRODUCTS --

CREATE TABLE BuyProducts (
	id INT PRIMARY KEY IDENTITY(1,1),
    	title NVARCHAR(100) NOT NULL,
	description NVARCHAR(3000) NULL,
	seller_id INT NOT NULL,
	condition_id INT NOT NULL,
	category_id INT NOT NULL,
    	price FLOAT NOT NULL,
	CONSTRAINT FK_Products_Users FOREIGN KEY (seller_id) REFERENCES Users(id),
	CONSTRAINT FK_Products_ProductConditions FOREIGN KEY (condition_id) REFERENCES ProductConditions(id),
	CONSTRAINT FK_Products_ProductCategories FOREIGN KEY (category_id) REFERENCES ProductCategories(id),
);

CREATE TABLE BuyProductImages (
	id INT PRIMARY KEY IDENTITY(1,1),
	url NVARCHAR(256) NOT NULL,
	product_id INT NOT NULL,
	CONSTRAINT FK_Image_BuyProductsProduct FOREIGN KEY (product_id) REFERENCES BuyProducts(id)
);

CREATE TABLE BuyProductProductTags (
	id INT PRIMARY KEY IDENTITY(1,1),
	product_id INT NOT NULL,
	tag_id INT NOT NULL,
	CONSTRAINT FK_BuyProductProductTags_BuyProducts FOREIGN KEY (product_id) REFERENCES BuyProducts(id),
	CONSTRAINT FK_BuyProductProductTags_ProductTags FOREIGN KEY (tag_id) REFERENCES ProductTags(id)
);

-- BORROW PRODUCTS --
CREATE TABLE BorrowProducts (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(100) NOT NULL,
    description NVARCHAR(3000) NULL,
    seller_id INT NOT NULL,
    condition_id INT NOT NULL,
    category_id INT NOT NULL,
    time_limit DATE NOT NULL,
    start_date DATE NULL,
    end_date DATE NULL,
    daily_rate FLOAT NOT NULL,
    is_borrowed BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_BorrowProducts_Users FOREIGN KEY (seller_id) REFERENCES Users(id),
    CONSTRAINT FK_BorrowProducts_ProductConditions FOREIGN KEY (condition_id) REFERENCES ProductConditions(id),
    CONSTRAINT FK_BorrowProducts_ProductCategories FOREIGN KEY (category_id) REFERENCES ProductCategories(id)
)

CREATE TABLE BorrowProductImages (
	id INT PRIMARY KEY IDENTITY(1,1),
	url NVARCHAR(256) NOT NULL,
	product_id INT NOT NULL,
	CONSTRAINT FK_Image_BorrowProduct FOREIGN KEY (product_id) REFERENCES BorrowProducts(id)
);

CREATE TABLE BorrowProductProductTags (
	id INT PRIMARY KEY IDENTITY(1,1),
	product_id INT NOT NULL,
	tag_id INT NOT NULL,
	CONSTRAINT FK_BorrowProductProductTags_BorrowProducts FOREIGN KEY (product_id) REFERENCES BorrowProducts(id),
	CONSTRAINT FK_BorrowProductProductTags_ProductTags FOREIGN KEY (tag_id) REFERENCES ProductTags(id)
);

-- AUCTION PRODUCTS --
CREATE TABLE AuctionProducts (
	id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(100) NOT NULL,
	description NVARCHAR(3000) NULL,
	seller_id INT NOT NULL,
	condition_id INT NOT NULL,
	category_id INT NOT NULL,
	start_datetime DATETIME NOT NULL,
	end_datetime DATETIME NOT NULL,
	starting_price FLOAT NOT NULL,
	current_price FLOAT NOT NULL,
	CONSTRAINT FK_AuctionProducts_Seller FOREIGN KEY (seller_id) REFERENCES Users(id),
	CONSTRAINT FK_AuctionProducts_ProductConditions FOREIGN KEY (condition_id) REFERENCES ProductConditions(id),
	CONSTRAINT FK_AuctionProducts_ProductCategories FOREIGN KEY (category_id) REFERENCES ProductCategories(id),
);

CREATE TABLE AuctionProductsImages (
	id INT PRIMARY KEY IDENTITY(1,1),
	url NVARCHAR(256) NOT NULL,
	product_id INT NOT NULL,
	CONSTRAINT FK_Image_AuctionProduct FOREIGN KEY (product_id) REFERENCES AuctionProducts(id)
);

CREATE TABLE AuctionProductProductTags (
	id INT PRIMARY KEY IDENTITY(1,1),
	product_id INT NOT NULL,
	tag_id INT NOT NULL,
	CONSTRAINT FK_AuctionProductProductTags_AuctionProducts FOREIGN KEY (product_id) REFERENCES AuctionProducts(id),
	CONSTRAINT FK_AuctionProductProductTags_ProductTags FOREIGN KEY (tag_id) REFERENCES ProductTags(id)
);

CREATE TABLE Bids (
	id INT PRIMARY KEY IDENTITY(1,1),
	bidder_id INT NOT NULL,
	product_id INT NOT NULL,
	price FLOAT NOT NULL,
	timestamp DATETIME NOT NULL,
	CONSTRAINT FK_Bids_Users FOREIGN KEY (bidder_id) REFERENCES Users(id),
	CONSTRAINT FK_Bids_AuctionProducts FOREIGN KEY (product_id) REFERENCES AuctionProducts(id)
);

----- BASKET ------
CREATE TABLE Baskets (
	id INT PRIMARY KEY IDENTITY(1,1),
	buyer_id INT NOT NULL,
	CONSTRAINT FK_Baskets_Users FOREIGN KEY (buyer_id) REFERENCES Users(id)
);

CREATE TABLE BasketItemsBuyProducts (
	id INT PRIMARY KEY IDENTITY(1,1),
	basket_id INT NOT NULL,
	product_id INT NOT NULL,
	quantity INT NOT NULL DEFAULT(1),
	price FLOAT NOT NULL,
	CONSTRAINT FK_BasketItems_Basket FOREIGN KEY (basket_id) REFERENCES Baskets(id),
	CONSTRAINT FK_BasketItems_Product FOREIGN KEY (product_id) REFERENCES BuyProducts(id),
);

------- DROPS -------
-- Drop Basket-related tables first
DROP TABLE IF EXISTS BasketItemsBuyProducts;
DROP TABLE IF EXISTS Baskets;

-- Drop Bids table (dependent on Users and AuctionProducts)
DROP TABLE IF EXISTS Bids;

-- Drop Auction-related tables
DROP TABLE IF EXISTS AuctionProductProductTags;
DROP TABLE IF EXISTS AuctionProductsImages;
DROP TABLE IF EXISTS AuctionProducts;

-- Drop Borrow-related tables
DROP TABLE IF EXISTS BorrowProductProductTags;
DROP TABLE IF EXISTS BorrowProductImages;
DROP TABLE IF EXISTS BorrowProducts;

-- Drop Buy-related tables
DROP TABLE IF EXISTS BuyProductProductTags;
DROP TABLE IF EXISTS BuyProductImages;
DROP TABLE IF EXISTS BuyProducts;

-- Drop Reviews table (dependent on Users)
DROP TABLE IF EXISTS ReviewsPictures
DROP TABLE IF EXISTS Reviews;

-- Drop product category, condition, and tag-related tables
DROP TABLE IF EXISTS ProductTags;
DROP TABLE IF EXISTS ProductCategories;
DROP TABLE IF EXISTS ProductConditions;

-- Drop Users table last since many tables reference it
DROP TABLE IF EXISTS .Users;







-- SQL QUERRIES FOR INSERTING DATA


-- ADD USERS FIRST
INSERT INTO Users (email, username, userType, balance, rating, passwordHash)
VALUES 
('alice@example.com', 'alice123', 1, 500.00, 4.5, 'hashedpassword1'),
('bob@example.com', 'bob321', 2, 1000.00, 4.8, 'hashedpassword2');

--ADD PRODUCT CONDITIONS, CATEGORIES AND TAGS
INSERT INTO ProductConditions (title, description)
VALUES 
('New', 'Brand new item, unopened'),
('Used', 'Item has been previously used');

INSERT INTO ProductCategories (title, description)
VALUES 
('Electronics', 'Devices like phones, laptops, etc.'),
('Furniture', 'Chairs, tables, beds, etc.');

INSERT INTO ProductTags (title)
VALUES 
('Tech'),
('Home'),
('Vintage');




-- INSERT QUERRIES FOR AUCTION PRODUCTS

INSERT INTO AuctionProducts (title, description, seller_id, condition_id, category_id,    start_datetime, end_datetime, starting_price, current_price)
VALUES 
('Used iPhone 12', 'Still works great. Minor scratches.', 1, 2, 1, '2025-03-25 10:00:00', '2025-03-30 10:00:00', 300.00, 300.00) , 	
('Gaming Laptop', 'High-end gaming laptop with RTX 3070.', 2, 1, 1, '2025-03-26 09:00:00', '2025-03-29 09:00:00', 800.00, 800.00),
('Antique Clock', 'Old collectible wall clock.', 1, 2, 2, '2025-03-24 15:00:00', '2025-03-28 15:00:00', 100.00, 100.00),
('Smartwatch', 'Waterproof smartwatch with GPS.', 2, 1, 1, '2025-03-27 14:00:00', '2025-03-31 14:00:00', 120.00, 120.00),
('Bluetooth Speaker', 'Compact speaker with deep bass.', 1, 1, 1, '2025-03-26 11:00:00', '2025-03-29 11:00:00', 40.00, 40.00);

INSERT INTO AuctionProductsImages (url, product_id)
VALUES 
('https://i.imgur.com/XBpDHa7.jpeg', 1),
('https://i.imgur.com/u9j0U5Y.jpeg', 1),
('https://i.imgur.com/YYXgjHM.jpeg' , 2),
('https://i.imgur.com/Yq7jIzr.jpeg' , 3),
('https://i.imgur.com/4XWqyj5.jpeg' , 3),
('https://i.imgur.com/ZC5UaQZ.jpeg' , 4),
('https://i.imgur.com/grOzWu8.jpeg' , 5),
('https://i.imgur.com/srDfm59.jpeg' , 5);


INSERT INTO AuctionProductProductTags (product_id, tag_id)
VALUES 
(1, 1),
(1, 3),
(2, 1),
(3, 3),
(4 , 1),
(4 , 2),
(5 , 1),
(5 , 2);

INSERT INTO Bids (bidder_id, product_id, price, timestamp)
VALUES 
(2, 1, 350.00, '2025-03-25 12:30:00');




-- INSERT QUERRIES FOR BORROW PRODUCTS

INSERT INTO BorrowProducts (title, description, seller_id, condition_id, category_id, time_limit, start_date, end_date, daily_rate, is_borrowed) 
VALUES 
('Professional Camera', 'DSLR camera perfect for events.',1, 1, 1,'2025-03-30', '2025-03-25', '2025-03-30',12.00, 1),
('Toolbox', 'Complete DIY toolkit for home repairs.',2, 2, 2,'2025-03-30', '2025-03-24', '2025-03-28',4.00, 1);



INSERT INTO BorrowProductImages (url, product_id)
VALUES 
('https://i.imgur.com/yVSgkw1.jpeg', 1),
('https://i.imgur.com/gMaRVZN.jpeg', 1),
('https://i.imgur.com/OTDwYjh.jpeg' , 2),
('https://i.imgur.com/WL5yCjH.jpeg' , 2);

INSERT INTO BorrowProductProductTags (product_id, tag_id)
VALUES 
(1, 2),
(1, 1),
(2, 2);


-- INSERT QUERRIES FOR BUY PRODUCTS

INSERT INTO BuyProducts (title, description, seller_id, condition_id, category_id, price
) VALUES 
('Wireless Headphones', 'Brand new noise-cancelling headphones with long battery life.',2, 1, 1, 120.00),
 ('Wooden Table', 'Solid oak wood dining table.', 1, 2, 2, 250.00),
 ('Gaming Mouse', 'RGB, programmable buttons.', 2, 1, 1, 45.00);

INSERT INTO BuyProductImages (url, product_id)
VALUES 
('https://i.imgur.com/LhlIBMt.jpeg', 1),
('https://i.imgur.com/GmWI7GM.jpeg' , 1),
('https://i.imgur.com/oTYyO1O.jpeg' , 2),
('https://i.imgur.com/DQbBuGr.jpeg' , 2),
('https://i.imgur.com/oDwPDES.jpeg' , 3);

INSERT INTO BuyProductProductTags (product_id, tag_id)
VALUES 
(1, 1),
(2, 2),
(2, 3),
(3, 1);


-- INSERT QUERIES FOR REVIEWS
INSERT INTO Reviews (reviewer_id, seller_id, description, rating)
VALUES 
(1, 2, 'Great seller! The product was exactly as described. My son loves it!', 5.0),
(2, 1, 'Fast shipping and great communication.', 4.5),
(1, 2, 'It said that this was a football, not a rugby ball or whatever. Damn you americans and your football/soccer thing. Football is football, a BALL', 1.0),
(2, 1, 'Excellent service, will buy again.', 5.0);

-- INSERT QUERIES FOR REVIEW IMAGES
INSERT INTO ReviewsPictures (url, review_id)
VALUES 
('https://i.imgur.com/Wr7FSB0.jpeg', 2),
('https://i.imgur.com/yOMoqoS.jpeg', 1),
('https://i.imgur.com/4ikPbty.jpeg', 1),
('https://i.imgur.com/vG2ozCz.jpeg', 3),
('https://i.imgur.com/r3q6HKj.jpeg', 4);

UPDATE BorrowProducts
SET is_borrowed = 0,
    start_date = '2025-03-27',
    time_limit='2025-04-20';

-- Bids
ALTER TABLE Bids DROP CONSTRAINT FK_Bids_AuctionProducts;
ALTER TABLE Bids
ADD CONSTRAINT FK_Bids_AuctionProducts
FOREIGN KEY (product_id) REFERENCES AuctionProducts(id) ON DELETE CASCADE;

-- AuctionProductProductTags
ALTER TABLE AuctionProductProductTags DROP CONSTRAINT FK_AuctionProductProductTags_AuctionProducts;
ALTER TABLE AuctionProductProductTags
ADD CONSTRAINT FK_AuctionProductProductTags_AuctionProducts
FOREIGN KEY (product_id) REFERENCES AuctionProducts(id) ON DELETE CASCADE;

-- AuctionProductsImages
ALTER TABLE AuctionProductsImages DROP CONSTRAINT FK_Image_AuctionProduct;
ALTER TABLE AuctionProductsImages
ADD CONSTRAINT FK_Image_AuctionProduct
FOREIGN KEY (product_id) REFERENCES AuctionProducts(id) ON DELETE CASCADE;
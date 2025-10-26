#!/bin/bash
set -e

mongosh -u root -p root --authenticationDatabase admin <<EOF
use CatalogDatabase

db.CatalogBrand.insertMany([
  { Id: 1, Name: "Azure" },
  { Id: 2, Name: ".NET" },
  { Id: 3, Name: "Visual Studio" },
  { Id: 4, Name: "SQL Server" },
  { Id: 5, Name: "Other" }
])

db.CatalogType.insertMany([
  { Id: 1, Name: "Mug" },
  { Id: 2, Name: "T-Shirt" },
  { Id: 3, Name: "Sheet" },
  { Id: 4, Name: "USB Memory Stick" }
])

db.CatalogItem.insertMany([
  { Id: 1, Name: ".NET Bot Black Sweatshirt", Description: ".NET Bot Black Sweatshirt", Price: 19.5, PictureUri: "/images/products/1.png", CatalogTypeId: 2, CatalogBrandId: 2 },
  { Id: 2, Name: ".NET Black & White Mug", Description: ".NET Black & White Mug", Price: 8.5, PictureUri: "/images/products/2.png", CatalogTypeId: 1, CatalogBrandId: 2 },
  { Id: 3, Name: "Prism White T-Shirt", Description: "Prism White T-Shirt", Price: 12, PictureUri: "/images/products/3.png", CatalogTypeId: 2, CatalogBrandId: 5 },
  { Id: 4, Name: ".NET Foundation Sweatshirt", Description: ".NET Foundation Sweatshirt", Price: 12, PictureUri: "/images/products/4.png", CatalogTypeId: 2, CatalogBrandId: 2 },
  { Id: 5, Name: "Roslyn Red Sheet", Description: "Roslyn Red Sheet", Price: 8.5, PictureUri: "/images/products/5.png", CatalogTypeId: 3, CatalogBrandId: 5 },
  { Id: 6, Name: ".NET Blue Sweatshirt", Description: ".NET Blue Sweatshirt", Price: 12, PictureUri: "/images/products/6.png", CatalogTypeId: 2, CatalogBrandId: 2 },
  { Id: 7, Name: "Roslyn Red T-Shirt", Description: "Roslyn Red T-Shirt", Price: 12, PictureUri: "/images/products/7.png", CatalogTypeId: 2, CatalogBrandId: 5 },
  { Id: 8, Name: "Kudu Purple Sweatshirt", Description: "Kudu Purple Sweatshirt", Price: 8.5, PictureUri: "/images/products/8.png", CatalogTypeId: 2, CatalogBrandId: 5 },
  { Id: 9, Name: "Cup<T> White Mug", Description: "Cup<T> White Mug", Price: 12, PictureUri: "/images/products/9.png", CatalogTypeId: 1, CatalogBrandId: 5 },
  { Id: 10, Name: ".NET Foundation Sheet", Description: ".NET Foundation Sheet", Price: 12, PictureUri: "/images/products/10.png", CatalogTypeId: 3, CatalogBrandId: 2 },
  { Id: 11, Name: "Cup<T> Sheet", Description: "Cup<T> Sheet", Price: 8.5, PictureUri: "/images/products/11.png", CatalogTypeId: 3, CatalogBrandId: 2 },
  { Id: 12, Name: "Prism White TShirt", Description: "Prism White TShirt", Price: 12, PictureUri: "/images/products/12.png", CatalogTypeId: 2, CatalogBrandId: 5 }
])

print('Database initialized successfully')
EOF
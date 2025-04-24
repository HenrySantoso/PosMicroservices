using Play.Transaction.Service.Dtos;
using Play.Transaction.Service.Entities;

namespace Play.Transaction.Service
{
    public static class Exstensions
    {
        public static SaleItemsDto AsDto(this SaleItems saleItems)
        {
            return new SaleItemsDto(
                saleItems.Id,
                saleItems.ProductId,
                saleItems.SaleId,
                saleItems.Quantity,
                saleItems.Price
            );
        }

        public static SaleItemsProductDto AsProductDto(this SaleItems saleItems, string productName)
        {
            return new SaleItemsProductDto(
                saleItems.Id,
                saleItems.ProductId,
                saleItems.SaleId,
                productName,
                saleItems.Price,
                saleItems.Quantity
            );
        }

        public static UpdateProductStockDto AsUpdateProductStockDto(
            this SaleItems saleItems,
            int stockQuantity
        )
        {
            return new UpdateProductStockDto(saleItems.ProductId, stockQuantity);
        }

        //Sales
        public static SalesDto AsDto(this Sales sales)
        {
            return new SalesDto(sales.Id, sales.CustomerId, sales.SaleDate, sales.TotalAmount);
        }

        public static SaleByIdDto AsSaleByIdDto(this Sales sales, string customerName)
        {
            return new SaleByIdDto(
                sales.Id,
                sales.CustomerId,
                customerName,
                sales.SaleDate,
                sales.TotalAmount
            );
        }

        public static SaleItemDetailDto AsItemDetailDto(
            this SaleItems saleItems,
            string productName
        )
        {
            return new SaleItemDetailDto(
                saleItems.ProductId,
                productName,
                saleItems.Quantity,
                saleItems.Price
            );
        }

        public static SaleDetailDto AsSaleDetailDto(
            this Sales sales,
            string customerName,
            string productName,
            List<SaleItemDetailDto> saleItemDetails
        )
        {
            return new SaleDetailDto(
                sales.Id,
                sales.CustomerId,
                customerName,
                sales.SaleDate,
                sales.TotalAmount,
                saleItemDetails // Menggunakan list SaleItemDetailDto
            );
        }

        public static List<SaleItemDetailDto> AsDetailDtoList(
            this List<SaleItems> saleItems,
            List<ProductDto> products
        )
        {
            return saleItems
                .Select(item =>
                    item.AsItemDetailDto(
                        products.FirstOrDefault(p => p.Id == item.ProductId)?.ProductName
                            ?? "Unknown"
                    )
                )
                .ToList();
        }
    }
}

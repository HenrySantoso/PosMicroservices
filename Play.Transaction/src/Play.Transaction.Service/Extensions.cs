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

        public static SalesDto AsDto(this Sales sales)
        {
            return new SalesDto(sales.Id, sales.CustomerId, sales.SaleDate, sales.TotalAmount);
        }
    }
}

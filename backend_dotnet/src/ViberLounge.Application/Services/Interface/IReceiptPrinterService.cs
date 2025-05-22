using ViberLounge.Application.DTOs.Sale;

namespace ViberLounge.Application.Interfaces
{
    public interface IReceiptPrinterService
    {
        void PrintReceipt(SaleResponseDto sale);
    }
}
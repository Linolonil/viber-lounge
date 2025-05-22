using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ViberLounge.Application.DTOs.Sale;
using ViberLounge.Application.Interfaces;
using ViberLounge.Infrastructure.Logging;

namespace ViberLounge.Application.Services
{
    public class ReceiptPrinterService : IReceiptPrinterService
    {
        private readonly IPrinter _printer;
        private readonly EPSON _emitter;
        private readonly ILoggerService _logger;

        public ReceiptPrinterService(string printerType, string printerAddress, ILoggerService logger)
        {
            _logger = logger;
            _emitter = new EPSON();
            
            try
            {
                // Configurar a impressora de acordo com o tipo
                _printer = printerType.ToLower() switch
                {
                    "network" => new NetworkPrinter(new NetworkPrinterSettings() { ConnectionString = printerAddress }),
                    "serial" => new SerialPrinter(printerAddress, 9600),
                    "usb" => new SerialPrinter(printerAddress, 9600), // Para USB em Linux
                    "bluetooth" => new SerialPrinter(printerAddress, 9600),
                    _ => throw new ArgumentException($"Tipo de impressora não suportado: {printerType}")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(new Exception(ex.Message),"Erro ao inicializar impressora: {ex.Message}");
                throw;
            }
        }

        public void PrintReceipt(SaleResponseDto sale)
        {
            try
            {
                _logger.LogInformation($"Imprimindo recibo para venda ID: {sale.Id}");
                
                // var commands = new List<byte[]>
                // {
                //     _emitter.Initialize(),
                //     _emitter. SetAlign(ESCPOS_NET.Emitters.AlignmentPositions.Center),
                //     _emitter.Bold(true),
                //     _emitter.PrintLine("VIBER LOUNGE"),
                //     _emitter.Bold(false),
                //     _emitter.NewLine(),
                //     _emitter.SetAlign(ESCPOS_NET.Emitters.AlignmentPositions.Left),
                //     _emitter.PrintLine($"Venda #: {sale.Id}"),
                //     _emitter.NewLine(),
                //     _emitter.PrintLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}"),
                //     _emitter.NewLine(),
                //     _emitter.PrintLine("-------------------------------"),
                //     _emitter.NewLine(),
                //     _emitter.PrintLine("ITENS"),
                //     _emitter.NewLine()
                // };

                // Adicionar cada item da venda
                // if (sale.Items != null)
                // {
                //     foreach (var item in sale.Items)
                //     {
                //         commands.Add(_emitter.PrintLine($"{item.Quantity}x {item.ProductName} - R$ {item.Subtotal:F2}"));
                //         commands.Add(_emitter.NewLine());
                //     }
                // }

                // commands.AddRange(new List<byte[]>
                // {
                //     _emitter.PrintLine("-------------------------------"),
                //     _emitter.NewLine(),
                //     _emitter.Bold(true),
                //     _emitter.PrintLine($"TOTAL: R$ {sale.TotalPrice:F2}"),
                //     _emitter.Bold(false),
                //     _emitter.NewLine(),
                //     _emitter.NewLine(),
                //     _emitter.SetAlign(ESCPOS_NET.Emitters.AlignmentPositions.Center),
                //     _emitter.PrintLine("Obrigado por sua compra!"),
                //     _emitter.NewLine(),
                //     _emitter.NewLine(),
                //     _emitter.CutPaper()
                // });

                // _printer.Write(commands.ToArray());
                // _logger.LogInformation("Recibo impresso com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(new Exception(ex.Message), "Erro ao imprimir recibo!!");
                throw;
            }
        }
    }
}
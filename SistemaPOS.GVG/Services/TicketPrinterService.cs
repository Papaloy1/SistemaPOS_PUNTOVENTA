using SistemaPOS.Desktop.Models;
using System;
using System.Drawing;
using System.Drawing.Printing;
// Asegúrate de importar los namespaces de tus modelos
// using SistemaPOS.GVG.Models; 

namespace SistemaPOS.GVG.Services
{
    public class TicketPrinterService
    {
        // Almacenamos la venta en memoria durante el proceso de impresión
        private VentaDTO _ventaActual;

        /// <summary>
        /// Inicia el proceso de impresión enviando el documento a la cola del sistema.
        /// </summary>
        /// <param name="venta">El objeto de la venta confirmada.</param>
        /// <param name="nombreImpresora">Nombre exacto de la impresora en Windows.</param>
        public void ImprimirTicket(VentaDTO venta, string nombreImpresora = "Microsoft Print to PDF")
        {
            _ventaActual = venta;

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = nombreImpresora;

            // Suscribimos el evento que se encarga de "dibujar" el ticket
            printDocument.PrintPage += new PrintPageEventHandler(DibujarTicket);

            try
            {
                printDocument.Print();
            }
            catch (Exception ex)
            {
                // Aquí deberías registrar el error (Log) o lanzar una excepción personalizada
                throw new Exception($"Error al comunicar con la impresora: {ex.Message}");
            }
        }

        /// <summary>
        /// Método subyacente que diseña el layout del ticket utilizando GDI+.
        /// </summary>
        private void DibujarTicket(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Definición de fuentes tipográficas para impresoras térmicas
            Font fontRegular = new Font("Courier New", 10);
            Font fontBold = new Font("Courier New", 10, FontStyle.Bold);
            Font fontMini = new Font("Courier New", 8);

            int y = 10; // Posición vertical inicial
            int margenIzquierdo = 10;
            int anchoTicket = 280; // Ajustable según el papel (58mm/80mm)

            // --- ENCABEZADO ---
            // Centramos el texto matemáticamente calculando el ancho del string
            string nombreNegocio = "PINTURAS EL GORDO'S";
            SizeF tamanoTexto = graphics.MeasureString(nombreNegocio, fontBold);
            float posXCentrado = (anchoTicket - tamanoTexto.Width) / 2;

            graphics.DrawString(nombreNegocio, fontBold, Brushes.Black, posXCentrado, y);
            y += 20;

            graphics.DrawString("Comprobante de Venta", fontRegular, Brushes.Black, margenIzquierdo, y);
            y += 20;
            graphics.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", fontRegular, Brushes.Black, margenIzquierdo, y);
            y += 20;
            graphics.DrawString($"Folio Venta: {_ventaActual.Id}", fontBold, Brushes.Black, margenIzquierdo, y);
            y += 25;

            graphics.DrawString("---------------------------------", fontRegular, Brushes.Black, margenIzquierdo, y);
            y += 15;

            // --- DETALLES DE PRODUCTOS ---
            graphics.DrawString("CANT  DESCRIPCION          TOTAL", fontBold, Brushes.Black, margenIzquierdo, y);
            y += 20;

            // Iteramos sobre el detalle (Asegúrate de que tu DTO tenga una colección DetalleVenta)
            if (_ventaActual.Detalles != null)
            {
                foreach (var item in _ventaActual.Detalles)
                {
                    // Formateamos para que cuadre en las columnas usando PadRight/PadLeft
                    string lineaItem = $"{item.Cantidad.ToString().PadRight(5)} {item.NombreProducto.PadRight(18).Substring(0, 18)} {item.Subtotal.ToString("C2").PadLeft(8)}";
                    graphics.DrawString(lineaItem, fontRegular, Brushes.Black, margenIzquierdo, y);
                    y += 15;
                }
            }

            y += 10;
            graphics.DrawString("---------------------------------", fontRegular, Brushes.Black, margenIzquierdo, y);
            y += 20;

            // --- TOTALES ---
            string textoTotal = $"TOTAL: {_ventaActual.Total.ToString("C2")}";
            SizeF tamanoTotal = graphics.MeasureString(textoTotal, fontBold);
            graphics.DrawString(textoTotal, fontBold, Brushes.Black, anchoTicket - tamanoTotal.Width, y);
            y += 30;

            // --- PIE DE PÁGINA ---
            string agradecimiento = "¡Gracias por su preferencia!";
            SizeF tamanoAgradecimiento = graphics.MeasureString(agradecimiento, fontMini);
            graphics.DrawString(agradecimiento, fontMini, Brushes.Black, (anchoTicket - tamanoAgradecimiento.Width) / 2, y);
        }
    }
}   
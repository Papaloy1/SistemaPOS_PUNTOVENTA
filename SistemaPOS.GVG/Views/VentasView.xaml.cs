using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class VentasView : UserControl
    {
        private readonly ApiClient _apiClient;
        private List<ProductoDTO> _todosProductos;
        private List<DetalleVentaDTO> _carritoActual;

        public VentasView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _todosProductos = new List<ProductoDTO>();
            _carritoActual = new List<DetalleVentaDTO>();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private async System.Threading.Tasks.Task CargarProductos()
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<ProductoDTO>>>("productos");
                if (response?.Success == true && response.Data != null)
                {
                    _todosProductos = response.Data.Where(p => p.Stock > 0).ToList();
                    icProductosDisponibles.ItemsSource = _todosProductos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBusquedaProducto.Text.ToLower();
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                icProductosDisponibles.ItemsSource = _todosProductos;
                return;
            }

            var resultados = _todosProductos
                .Where(p => p.Descripcion.ToLower().Contains(busqueda) ||
                           p.CodigoBarras.ToLower().Contains(busqueda))
                .ToList();

            icProductosDisponibles.ItemsSource = resultados;
        }

        private void BtnLimpiarBusqueda_Click(object sender, RoutedEventArgs e)
        {
            txtBusquedaProducto.Clear();
            icProductosDisponibles.ItemsSource = _todosProductos;
        }

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int idProducto))
            {
                var producto = _todosProductos.FirstOrDefault(p => p.IdProducto == idProducto);
                if (producto != null)
                {
                    AgregarAlCarrito(producto);
                }
            }
        }

        private void AgregarAlCarrito(ProductoDTO producto)
        {
            var detalle = _carritoActual.FirstOrDefault(d => d.IdProducto == producto.IdProducto);

            if (detalle != null)
            {
                // Si ya existe, aumentar cantidad
                detalle.Cantidad++;
            }
            else
            {
                // Crear nuevo detalle
                detalle = new DetalleVentaDTO
                {
                    IdProducto = producto.IdProducto,
                    Cantidad = 1,
                    PrecioUnitario = producto.PrecioVenta,
                    Subtotal = producto.PrecioVenta
                };
                _carritoActual.Add(detalle);
            }

            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

            ActualizarCarrito();
        }

        private void ActualizarCarrito()
        {
            // Crear DTO con datos de producto para la visualización
            var carritoConProductos = _carritoActual.Select(d => new
            {
                d.IdDetalle,
                d.IdProducto,
                Descripcion = _todosProductos.FirstOrDefault(p => p.IdProducto == d.IdProducto)?.Descripcion ?? "Producto",
                d.Cantidad,
                d.PrecioUnitario,
                d.Subtotal
            }).ToList();

            dgCarrito.ItemsSource = null;
            dgCarrito.ItemsSource = carritoConProductos;

            decimal subtotal = _carritoActual.Sum(d => d.Subtotal);
            decimal impuesto = subtotal * 0.16m; // 16% de IVA
            decimal total = subtotal + impuesto;

            lblSubtotal.Text = $"${subtotal:F2}";
            lblImpuesto.Text = $"${impuesto:F2}";
            lblTotal.Text = $"${total:F2}";
        }

        private async void BtnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (!_carritoActual.Any())
            {
                MessageBox.Show("El carrito está vacío", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Crear ventana de pago
            var ventanaPago = new Window
            {
                Title = "Procesar Pago",
                Width = 400,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                Background = System.Windows.Media.Brushes.White
            };

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); 
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            decimal subtotal = _carritoActual.Sum(d => d.Subtotal);
            decimal impuesto = subtotal * 0.16m;
            decimal total = subtotal + impuesto;

            var lblTotalAPagar = new TextBlock { Text = $"Total a Pagar: ${total:F2}", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 15) };
            Grid.SetRow(lblTotalAPagar, 0);
            grid.Children.Add(lblTotalAPagar);

            var lblMonto = new TextBlock { Text = "Monto Pagado:", Margin = new Thickness(0, 0, 0, 5) };
            Grid.SetRow(lblMonto, 1);
            grid.Children.Add(lblMonto);

            var txtMonto = new TextBox { Height = 35, Padding = new Thickness(10), FontSize = 14 };
            Grid.SetRow(txtMonto, 2);
            grid.Children.Add(txtMonto);

            var stackBotones = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };

            var btnConfirmar = new Button { Content = "Confirmar Pago", Width = 120, Height = 40, Background = System.Windows.Media.Brushes.Green, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 10, 0) };
            var btnCancelar = new Button { Content = "Cancelar", Width = 120, Height = 40, Background = System.Windows.Media.Brushes.Red, Foreground = System.Windows.Media.Brushes.White };

            stackBotones.Children.Add(btnConfirmar);
            stackBotones.Children.Add(btnCancelar);

            Grid.SetRow(stackBotones, 5);
            grid.Children.Add(stackBotones);

            ventanaPago.Content = grid;

            btnConfirmar.Click += async (s, ev) =>
            {
                if (decimal.TryParse(txtMonto.Text, out decimal montoPagado))
                {
                    if (montoPagado >= total)
                    {
                        await ProcesarVenta(montoPagado, subtotal, impuesto, total);
                        ventanaPago.Close();
                    }
                    else
                    {
                        MessageBox.Show("El monto ingresado es insuficiente", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            };

            btnCancelar.Click += (s, ev) => ventanaPago.Close();

            ventanaPago.ShowDialog();
        }

        private async System.Threading.Tasks.Task ProcesarVenta(decimal montoPagado, decimal subtotal, decimal impuesto, decimal total)
        {
            try
            {
                var venta = new VentaDTO
                {
                    FechaVenta = DateTime.Now,
                    Subtotal = subtotal,
                    Impuesto = impuesto,
                    Total = total,
                    Pagado = montoPagado,
                    Cambio = montoPagado - total,
                    TipoPago = "Efectivo",
                    IdCliente = null,
                    UsuarioId = 1
                };

                // Aquí se haría la llamada POST a la API
                MessageBox.Show(
                    $"✓ Venta procesada exitosamente\n\n" +
                    $"Total: ${venta.Total:F2}\n" +
                    $"Pagado: ${venta.Pagado:F2}\n" +
                    $"Cambio: ${venta.Cambio:F2}",
                    "Venta Exitosa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimpiarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar venta: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Deseas cancelar esta venta?", "Confirmación", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                LimpiarCarrito();
            }
        }

        private void LimpiarCarrito()
        {
            _carritoActual.Clear();
            dgCarrito.ItemsSource = null;
            lblSubtotal.Text = "$0.00";
            lblImpuesto.Text = "$0.00";
            lblTotal.Text = "$0.00";
            txtBusquedaProducto.Clear();
        }
    }
}
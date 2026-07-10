using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class CajaView : UserControl
    {
        private readonly ApiClient _apiClient;
        private List<CajaDTO> _todasCajas;

        public CajaView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _todasCajas = new List<CajaDTO>();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarCajas();
        }

        private async System.Threading.Tasks.Task CargarCajas()
        {
            try
            {
                lblEstado.Text = "⏳ Cargando cajas...";

                var response = await _apiClient.GetAsync<ApiResponse<List<CajaDTO>>>("cajas");

                if (response?.Success == true && response.Data != null)
                {
                    _todasCajas = response.Data;
                    dgCajas.ItemsSource = _todasCajas;

                    var cajaActiva = response.Data.Find(c => c.Estado == "Abierta");
                    if (cajaActiva != null)
                    {
                        lblEstado.Text = $"✅ Caja Activa: {cajaActiva.NombreCaja}";
                        lblSaldoInicial.Text = $"${cajaActiva.SaldoInicial:F2}";
                        lblTotalVentas.Text = $"${cajaActiva.TotalVentas:F2}";
                        lblSaldoActual.Text = $"${(cajaActiva.SaldoInicial + cajaActiva.TotalVentas):F2}";
                        btnAbrirCaja.IsEnabled = false;
                        btnCerrarCaja.IsEnabled = true;
                    }
                    else
                    {
                        lblEstado.Text = "⚠️ No hay caja abierta";
                        btnAbrirCaja.IsEnabled = true;
                        btnCerrarCaja.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = $"❌ Error al cargar cajas";
                MessageBox.Show($"Error: {ex.Message}", "Error de Conexión",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAbrirCaja_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new Window
            {
                Title = "Abrir Nueva Caja",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                Background = System.Windows.Media.Brushes.White
            };

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var lblNombre = new TextBlock { Text = "Nombre Caja:", Margin = new Thickness(0, 0, 0, 5) };
            Grid.SetRow(lblNombre, 0);
            grid.Children.Add(lblNombre);

            var txtNombre = new TextBox { Height = 35, Padding = new Thickness(10), FontSize = 14 };
            Grid.SetRow(txtNombre, 1);
            grid.Children.Add(txtNombre);

            var lblMonto = new TextBlock { Text = "Saldo Inicial:", Margin = new Thickness(0, 15, 0, 5) };
            Grid.SetRow(lblMonto, 2);
            grid.Children.Add(lblMonto);

            var txtMonto = new TextBox { Height = 35, Padding = new Thickness(10), FontSize = 14 };
            Grid.SetRow(txtMonto, 3);
            grid.Children.Add(txtMonto);

            var stackBotones = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };

            var btnConfirmar = new Button { Content = "Abrir Caja", Width = 120, Height = 40, Background = System.Windows.Media.Brushes.Green, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 10, 0) };
            var btnCancelar = new Button { Content = "Cancelar", Width = 120, Height = 40, Background = System.Windows.Media.Brushes.Red, Foreground = System.Windows.Media.Brushes.White };

            stackBotones.Children.Add(btnConfirmar);
            stackBotones.Children.Add(btnCancelar);

            Grid.SetRow(stackBotones, 6);
            grid.Children.Add(stackBotones);

            ventana.Content = grid;

            btnConfirmar.Click += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Ingresa el nombre de la caja", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtMonto.Text, out decimal saldoInicial))
                {
                    MessageBox.Show("Ingresa un monto válido", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show($"Caja '{txtNombre.Text}' abierta con saldo inicial de ${saldoInicial:F2}",
                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                ventana.Close();
                await CargarCajas();
            };

            btnCancelar.Click += (s, ev) => ventana.Close();

            ventana.ShowDialog();
        }

        private void BtnCerrarCaja_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Funcionalidad de cerrar caja próximamente.",
                "Información",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}

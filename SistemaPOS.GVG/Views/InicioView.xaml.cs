using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class InicioView : UserControl
    {
        private readonly ApiClient _apiClient;

        public InicioView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarDatos();
        }

        private async System.Threading.Tasks.Task CargarDatos()
        {
            try
            {
                // Cargar productos
                var prodResponse = await _apiClient.GetAsync<ApiResponse<List<ProductoDTO>>>("productos");
                if (prodResponse?.Success == true && prodResponse.Data != null)
                {
                    lblTotalProductos.Text = prodResponse.Data.Count.ToString();
                }

                // Cargar clientes
                var clientResponse = await _apiClient.GetAsync<ApiResponse<List<ClienteDTO>>>("clientes");
                if (clientResponse?.Success == true && clientResponse.Data != null)
                {
                    lblTotalClientes.Text = clientResponse.Data.Count.ToString();
                }

                // Cargar caja activa
                try
                {
                    var cajaResponse = await _apiClient.GetAsync<ApiResponse<CajaDTO>>("cajas/activa");
                    if (cajaResponse?.Success == true && cajaResponse.Data != null)
                    {
                        lblEstadoCaja.Text = "ABIERTA";
                        lblEstadoCaja.Foreground = System.Windows.Media.Brushes.Green;
                    }
                }
                catch
                {
                    lblEstadoCaja.Text = "CERRADA";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

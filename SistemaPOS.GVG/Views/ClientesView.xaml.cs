using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class ClientesView : UserControl
    {
        private readonly ApiClient _apiClient;
        private List<ClienteDTO> _todosClientes;

        public ClientesView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _todosClientes = new List<ClienteDTO>();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarClientes();
        }

        private async System.Threading.Tasks.Task CargarClientes()
        {
            try
            {
                lblClienteCount.Text = "⏳ Cargando clientes...";
                dgClientes.ItemsSource = null;

                var response = await _apiClient.GetAsync<ApiResponse<List<ClienteDTO>>>("clientes");

                if (response?.Success == true && response.Data != null)
                {
                    _todosClientes = response.Data;
                    dgClientes.ItemsSource = _todosClientes;
                    lblClienteCount.Text = $"✅ Se encontraron {_todosClientes.Count} clientes";
                }
                else
                {
                    lblClienteCount.Text = $"⚠️ {response?.Message ?? "Sin clientes disponibles"}";
                }
            }
            catch (Exception ex)
            {
                lblClienteCount.Text = $"❌ Error al cargar clientes";
                MessageBox.Show($"Error: {ex.Message}", "Error de Conexión",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBusqueda.Text.ToLower();

            if (string.IsNullOrWhiteSpace(busqueda))
            {
                dgClientes.ItemsSource = _todosClientes;
                lblClienteCount.Text = $"✅ Se encontraron {_todosClientes.Count} clientes";
                return;
            }

            var resultados = _todosClientes
                .Where(c => c.Nombre.ToLower().Contains(busqueda) ||
                           c.Telefono?.Contains(busqueda) == true ||
                           c.Correo?.ToLower().Contains(busqueda) == true)
                .ToList();

            dgClientes.ItemsSource = resultados;
            lblClienteCount.Text = $"🔍 Se encontraron {resultados.Count} clientes";
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            txtBusqueda.Clear();
            _ = CargarClientes();
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Funcionalidad de crear nuevo cliente próximamente.",
                "Información",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}

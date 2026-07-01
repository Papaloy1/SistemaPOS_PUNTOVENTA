using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class ProductosView : UserControl
    {
        private readonly ApiClient _apiClient;
        private List<ProductoDTO> _todosProductos;

        public ProductosView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _todosProductos = new List<ProductoDTO>();
        }

        // Se ejecuta automáticamente al cargar la vista
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        // Cargar productos desde la API
        private async System.Threading.Tasks.Task CargarProductos()
        {
            try
            {
                // Mostrar estado de carga
                lblProductoCount.Text = "⏳ Cargando productos...";
                icProductos.ItemsSource = null;

                // Llamada a la API
                var response = await _apiClient.GetAsync<ApiResponse<List<ProductoDTO>>>("productos");

                if (response?.Success == true && response.Data != null)
                {
                    _todosProductos = response.Data;

                    // Mostrar productos en la UI
                    icProductos.ItemsSource = _todosProductos;

                    // Actualizar contador
                    lblProductoCount.Text = $"✅ Se encontraron {_todosProductos.Count} productos";
                }
                else
                {
                    lblProductoCount.Text = $"⚠️ {response?.Message ?? "Sin productos disponibles"}";
                    icProductos.ItemsSource = new List<ProductoDTO>();
                }
            }
            catch (Exception ex)
            {
                lblProductoCount.Text = $"❌ Error al cargar productos";
                MessageBox.Show(
                    $"Error al consultar el inventario:\n{ex.Message}",
                    "Fallo de Conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Buscar productos
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBusqueda.Text.ToLower();

            if (string.IsNullOrWhiteSpace(busqueda))
            {
                icProductos.ItemsSource = _todosProductos;
                lblProductoCount.Text = $"✅ Se encontraron {_todosProductos.Count} productos";
                return;
            }

            // Filtrar por código o descripción
            var resultados = _todosProductos
                .Where(p => p.CodigoBarras.ToLower().Contains(busqueda) ||
                            p.Descripcion.ToLower().Contains(busqueda))
                .ToList();

            icProductos.ItemsSource = resultados;
            lblProductoCount.Text = $"🔍 Se encontraron {resultados.Count} productos";
        }

        // Actualizar lista
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            txtBusqueda.Clear();
            _ = CargarProductos();
        }

        // Agregar nuevo producto
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Funcionalidad de crear nuevo producto próximamente.",
                "Información",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}

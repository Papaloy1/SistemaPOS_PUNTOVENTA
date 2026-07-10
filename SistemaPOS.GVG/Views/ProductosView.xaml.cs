using SistemaPOS.Desktop.Models;
using SistemaPOS.Desktop.Services;
using SistemaPOS.GVG.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            // Crear la ventana de diálogo
            var dialogWindow = new NuevoProductoDialog();

            // Mostrar como ventana modal
            if (dialogWindow.ShowDialog() == true)
            {
                // Si se guardó exitosamente, recargar la tabla
                _ = CargarProductos();
            }
        }

        // Editar producto
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var producto = button?.Tag as ProductoDTO;

            if (producto == null)
            {
                MessageBox.Show("No se pudo cargar el producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Abrir diálogo de edición pasando el producto
            var dialogWindow = new NuevoProductoDialog(producto);  // Usa NuevoProductoDialog reutilizable

            if (dialogWindow.ShowDialog() == true)
            {
                // Si se guardó exitosamente, recargar la tabla
                _ = CargarProductos();
            }
        }

        // Eliminar producto
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var producto = button?.Tag as ProductoDTO;

            if (producto == null)
            {
                MessageBox.Show("No se pudo cargar el producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirmar eliminación
            var resultado = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el producto:\n\n{producto.Descripcion}?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                _ = EliminarProducto(producto.IdProducto);
            }
        }

        // Eliminar producto desde la API
        private async System.Threading.Tasks.Task EliminarProducto(int idProducto)
        {
            try
            {
                var success = await _apiClient.DeleteAsync("productos", idProducto);

                if (success)
                {
                    MessageBox.Show(
                        "Producto eliminado correctamente",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Recargar la lista
                    _ = CargarProductos();
                }
                else
                {
                    MessageBox.Show(
                        "Error al eliminar el producto",
                        "Fallo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al eliminar el producto:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.GVG.Views
{
    public partial class NuevoProductoDialog : Window
    {
        private ProductoDTO _productoEdicion;

        public NuevoProductoDialog()
        {
            InitializeComponent();
            _productoEdicion = null;
        }

        public NuevoProductoDialog(ProductoDTO producto)
        {
            InitializeComponent();
            _productoEdicion = producto;
            CargarDatos(producto);
        }

        private void CargarDatos(ProductoDTO producto)
        {
            // Cargar los datos del producto en los campos de edición
            txtCodigoBarras.Text = producto.CodigoBarras;
            txtDescripcion.Text = producto.Descripcion;
            txtPrecioCosto.Text = producto.PrecioCosto.ToString();
            txtPrecioVenta.Text = producto.PrecioVenta.ToString();

            // Si existe un campo de categoría, también cargarlo
            if (!string.IsNullOrEmpty(producto.Categoria))
            {
                foreach (ComboBoxItem item in cmbCategoria.Items)
                {
                    if (item.Content.ToString() == producto.Categoria)
                    {
                        cmbCategoria.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validaciones básicas de campos vacíos
            if (string.IsNullOrWhiteSpace(txtCodigoBarras.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                cmbCategoria.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete los campos obligatorios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validación de conversiones numéricas (precios)
            if (!decimal.TryParse(txtPrecioCosto.Text, out decimal precioCosto) ||
                !decimal.TryParse(txtPrecioVenta.Text, out decimal precioVenta))
            {
                MessageBox.Show("Los precios deben ser valores numéricos válidos.", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 3. Crear el objeto con la estructura que espera la API
                var nuevoProducto = new
                {
                    CodigoBarras = txtCodigoBarras.Text,
                    Descripcion = txtDescripcion.Text,
                    Categoria = ((ComboBoxItem)cmbCategoria.SelectedItem).Content.ToString(),
                    PrecioCosto = precioCosto,
                    PrecioVenta = precioVenta
                };

                string json = JsonSerializer.Serialize(nuevoProducto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Recuerda verificar el puerto de tu API local
                    client.BaseAddress = new Uri("https://localhost:XXXX/");

                    // 4. Ejecutar la petición POST hacia el endpoint de Productos
                    HttpResponseMessage response = await client.PostAsync("api/productos", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Producto registrado correctamente en el catálogo.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true; // Cierra la ventana indicando éxito
                        this.Close();
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al guardar el producto: {errorResponse}", "Error del Servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fallo de conexión: {ex.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
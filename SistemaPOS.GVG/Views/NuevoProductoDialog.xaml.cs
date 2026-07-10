using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using SistemaPOS.Desktop.Models;
using SistemaPOS.Desktop.Services;

namespace SistemaPOS.Desktop.Views.Dialogs
{
    public partial class NuevoProductoDialog : Window
    {
        private readonly ApiClient _apiClient;
        private ProductoDTO _productoEnEdicion;  // Guardar el producto en edición
        public ProductoDTO ProductoCreado { get; set; }

        // Constructor para crear nuevo producto
        public NuevoProductoDialog()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _productoEnEdicion = null;
            this.Title = "➕ Nuevo Producto";
        }

        // Constructor para editar producto existente
        public NuevoProductoDialog(ProductoDTO producto)
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _productoEnEdicion = producto;
            this.Title = "✏️ Editar Producto";
            CargarProductoEnFormulario(producto);
        }

        // Cargar datos del producto en el formulario
        private void CargarProductoEnFormulario(ProductoDTO producto)
        {
            TxtCodigoBarras.Text = producto.CodigoBarras;
            TxtDescripcion.Text = producto.Descripcion;
            TxtCategoria.Text = producto.Categoria;
            TxtAcabado.Text = producto.Acabado;
            TxtTamanio.Text = producto.Tamanio;
            TxtStock.Text = producto.Stock.ToString();
            TxtPrecioCosto.Text = producto.PrecioCosto.ToString();
            TxtPrecioVenta.Text = producto.PrecioVenta.ToString();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(TxtCodigoBarras.Text) ||
                    string.IsNullOrWhiteSpace(TxtDescripcion.Text) ||
                    string.IsNullOrWhiteSpace(TxtPrecioVenta.Text))
                {
                    MessageBox.Show("Por favor complete todos los campos obligatorios.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Crear/actualizar objeto del producto
                var producto = new ProductoDTO
                {
                    IdProducto = _productoEnEdicion?.IdProducto ?? 0,  // Usar ID si está editando
                    CodigoBarras = TxtCodigoBarras.Text.Trim(),
                    Descripcion = TxtDescripcion.Text.Trim(),
                    Categoria = TxtCategoria.Text.Trim(),
                    Acabado = TxtAcabado.Text.Trim(),
                    Tamanio = TxtTamanio.Text.Trim(),
                    Stock = decimal.TryParse(TxtStock.Text, out var stock) ? stock : 0,
                    PrecioCosto = decimal.TryParse(TxtPrecioCosto.Text, out var costo) ? costo : 0,
                    PrecioVenta = decimal.Parse(TxtPrecioVenta.Text)
                };

                if (_productoEnEdicion == null)
                {
                    // Crear nuevo producto
                    var productoCreado = await _apiClient.PostAsync<ProductoDTO>("Productos", producto);

                    MessageBox.Show($"Producto '{productoCreado.Descripcion}' creado exitosamente.",
                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    ProductoCreado = productoCreado;
                }
                else
                {
                    // Actualizar producto existente
                    var productoActualizado = await _apiClient.PutAsync<ProductoDTO>("Productos", _productoEnEdicion.IdProducto, producto);

                    MessageBox.Show($"Producto '{productoActualizado.Descripcion}' actualizado exitosamente.",
                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    ProductoCreado = productoActualizado;
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error al guardar producto",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Validar solo números y decimales
        private void NumeroInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecimalInput(e.Text);
        }

        private bool IsDecimalInput(string text)
        {
            return decimal.TryParse(text, out _) || text == "." || text == ",";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class InventarioView : UserControl
    {
        private readonly ApiClient _apiClient;

        public InventarioView()
        {
            InitializeComponent();
            _apiClient = new ApiClient(); // Instanciamos tu cliente HTTP
        }

        // Se ejecuta automáticamente al cargar la vista en pantalla
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        // Método para cargar productos desde la API
        private async System.Threading.Tasks.Task CargarProductos()
        {
            try
            {
                // Llamada GET a la API: http://localhost:5000/api/productos
                var listaProductos = await _apiClient.GetAsync<List<ProductoDTO>>("Productos");

                // Mapeo de los datos a la tabla visual
                dgInventario.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de red al consultar el inventario: {ex.Message}", "Fallo de conexión",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}
using System.Windows;
using SistemaPOS.Desktop.Views; // Importamos la carpeta de vistas

namespace SistemaPOS.Desktop
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();

            // Cargar la vista de inicio por defecto (si la creas)
            // AreaTrabajoMain.Content = new InicioView();
        }

        // Evento de navegación: Módulo de Inventario
        private void BtnMenuInventario_Click(object sender, RoutedEventArgs e)
        {
            // Limpia el panel e inyecta la interfaz de inventario
            AreaTrabajoMain.Content = new InventarioView();
        }

        // Evento de navegación: Módulo de Ventas
        private void BtnMenuVentas_Click(object sender, RoutedEventArgs e)
        {
            // Limpia el panel e inyecta la interfaz de punto de venta
            AreaTrabajoMain.Content = new VentasView();
        }
    }
}
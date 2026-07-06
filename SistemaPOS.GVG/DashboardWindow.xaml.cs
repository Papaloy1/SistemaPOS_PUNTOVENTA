using System.Windows;
using SistemaPOS.Desktop.Views;

namespace SistemaPOS.Desktop
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
        }

        // Navegación al Inicio
        private void BtnMenuInicio_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new InicioView();
        }

        // Navegación al módulo de Ventas
        private void btnMenuVentas_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new VentasView();
        }

        // Navegación al módulo de Inventario
        private void BtnMenuInventario_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new ProductosView();
        }

        // Navegación al módulo de Clientes
        private void BtnMenuClientes_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new ClientesView();
        }

        // Navegación al módulo de Caja
        private void BtnMenuCaja_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new CajaView();
        }
    }
}
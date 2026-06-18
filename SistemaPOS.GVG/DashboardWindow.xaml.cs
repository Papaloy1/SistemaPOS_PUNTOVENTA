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

        // Navegación al módulo de Inventario
        private void BtnMenuInventario_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new InventarioView();
        }

        // Navegación al módulo de Ventas
        private void btnMenuVentas_Click(object sender, RoutedEventArgs e)
        {
            AreaTrabajoMain.Content = new VentasView();
        }
    }
}
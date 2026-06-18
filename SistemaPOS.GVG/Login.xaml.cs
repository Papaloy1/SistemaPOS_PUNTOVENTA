using System;
using System.Windows;

namespace SistemaPOS.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            // Recolección de datos
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Password.Trim(); // Propiedad exclusiva de PasswordBox

            // 1. Prevención de errores básica
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, ingrese su usuario y contraseña.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // 2. Simulación de validación
            if (usuario == "admin" && password == "1234")
            {
                // Opcional: Puedes quitar el MessageBox si prefieres que entre directo
                MessageBox.Show($"Bienvenido al sistema, {usuario}.",
                                "Acceso Concedido",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // Instanciamos el Dashboard que acabamos de crear
                DashboardWindow dashboard = new DashboardWindow();

                // Mostramos la nueva ventana en pantalla
                dashboard.Show();

                // Cerramos la ventana de Login actual para liberar memoria
                this.Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas. Verifique e intente de nuevo.",
                                "Error de Acceso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }
    }
}


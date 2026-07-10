using SistemaPOS.GVG;
using SistemaPOS.Desktop;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace SistemaPOS.GVG.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Capturar datos de la interfaz
            string usuario = txtUsername.Text; // Verifica que el nombre del TextBox coincida
            string password = txtPassword.Password; // Verifica que sea un PasswordBox

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor ingrese su usuario y contraseña.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 2. Preparar el objeto JSON para enviar a la API
                var loginData = new { Username = usuario, password = password};
                string json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 3. Configurar el cliente HTTP
                using (var client = new HttpClient())
                {
                    // IMPORTANTE: Cambia el puerto "XXXX" por el puerto real en el que corre tu API (ej. 7000 o 5001)
                    client.BaseAddress = new Uri("https://localhost:7269");

                    // 4. Ejecutar la petición POST
                    HttpResponseMessage response = await client.PostAsync("api/auth/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leemos el Token y el Rol devueltos por la API
                        string respuestaJson = await response.Content.ReadAsStringAsync();

                        // Si la validación es correcta, abrimos el Dashboard
                        DashboardWindow dashboard = new DashboardWindow();
                        dashboard.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Credenciales incorrectas. Acceso denegado.", "Error de Autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("No se pudo conectar con el servidor de base de datos. Verifique que la API esté en ejecución.\n\nDetalle: " + ex.Message, "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
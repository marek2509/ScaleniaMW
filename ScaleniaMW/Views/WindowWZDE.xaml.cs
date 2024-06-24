using ScaleniaMW.Entities;
using ScaleniaMW.Helpers;
using ScaleniaMW.Services;
using System.Linq;
using System.Windows;

namespace ScaleniaMW.Views
{
    /// <summary>
    /// Logika interakcji dla klasy WindowWZDE.xaml
    /// </summary>
    public partial class WindowWZDE : Window
    {
        MainDbContext dbContext;
        Dzialki_NService _dzialki_NService;
        public WindowWZDE()
        {
            InitializeComponent();
            dbContext = new MainDbContext(ConnectionHelper.GetConnectionString());

            _dzialki_NService =  new Dzialki_NService(dbContext);


            tbxWZDE.Text = string.Join(", ", _dzialki_NService.GetAll().Select(x => x.IDD));
        }
    }
}
using System.Windows.Controls;
using System.Collections.Generic;

namespace InertialScroller
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            List<string> items = new List<string>();

            for (int iCount = 0; iCount < 50; iCount++)
                items.Add("Signup now for the spring bootcamp. Check with the front desk today!" + " ( " + iCount.ToString() + " )");

            itemsControl.ItemsSource = items;
        }
    }
}

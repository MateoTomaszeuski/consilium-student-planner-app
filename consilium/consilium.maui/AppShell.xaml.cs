namespace Consilium.Maui;
public partial class AppShell : Shell {
    public AppShell() {
        InitializeComponent();

        Loaded += (_, _) =>
        {
#if IOS || MACCATALYST
        // run after the shell items are initialized
        RemoveTabIcons();
#endif
        };
    }

#if IOS || MACCATALYST
private void RemoveTabIcons()
{
    foreach (var item in Items)
    {
        if (item is TabBar tabBar)
        {
            foreach (var section in tabBar.Items)
            {
                if (section is ShellSection shellSection)
                {
                    foreach (var content in shellSection.Items)
                    {
                        content.Icon = null;
                    }
                }
            }
        }
    }
}
#endif
}
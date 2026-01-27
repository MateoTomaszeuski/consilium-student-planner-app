#if MACCATALYST || IOS
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Consilium.Maui {
    public sealed class ShellRendererWithoutBugs : ShellRenderer {
        private sealed class ShellTabBarAppearanceTrackerWithoutBugs : ShellTabBarAppearanceTracker {
            public override void UpdateLayout(UITabBarController controller) {
                foreach (UITabBarItem tabbarItem in controller.TabBar.Items) {
                    if (tabbarItem.Image is UIImage image) {
                        CGSize size = new(30, 30);
                        UIGraphics.BeginImageContextWithOptions(size, false, 0);
                        image.Draw(new CGRect(new CGPoint(0, 0), size));
                        UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                        UIGraphics.EndImageContext();
                        tabbarItem.Image = resizedImage;
                    }
                }
            }
        }

        protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
            => new ShellTabBarAppearanceTrackerWithoutBugs();
    }
}
#endif
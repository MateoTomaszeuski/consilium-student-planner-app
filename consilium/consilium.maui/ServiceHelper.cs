using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consilium.Maui;

public static class ServiceHelper {
    public static T GetService<T>() where T : notnull =>
        Current.GetService<T>() ?? throw new InvalidOperationException($"Service {typeof(T)} not registered.");

    private static IServiceProvider Current =>
        ((App)Application.Current).Services
        ?? throw new InvalidOperationException("MauiContext not available yet.");
}
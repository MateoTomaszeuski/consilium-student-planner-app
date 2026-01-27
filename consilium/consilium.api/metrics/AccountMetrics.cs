using System.Diagnostics.Metrics;

namespace Consilium.API.Metrics;

public class AccountMetrics {
    private readonly UpDownCounter<int> loginCounter;

    public AccountMetrics(IMeterFactory meterFactory) {
        var meter = meterFactory.Create("Consilium.Accounts");
        loginCounter = meter.CreateUpDownCounter<int>("consilium.accounts.logins");
    }

    public void AddedAccount() {
        loginCounter.Add(1);
    }

    public void RemovedAccount() {
        loginCounter.Add(-1);
    }
}
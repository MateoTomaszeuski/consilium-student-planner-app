using System.Diagnostics.Metrics;

namespace Consilium.API.Metrics;

public class NewFeatureMerics {

    private readonly Counter<long> _nonIntegratedViewClicks;

    public NewFeatureMerics(IMeterFactory meterFactory) {
        var meter = meterFactory.Create("Consilium.NewFeature");

        _nonIntegratedViewClicks = meter.CreateCounter<long>(
            name: "consilium.newfeature.nonintegrated.view.clicks",
            description: "Number of clicks on the non‑integrated view");
    }

    public void NonIntegratedViewClicked() {
        _nonIntegratedViewClicks.Add(1);
    }

}